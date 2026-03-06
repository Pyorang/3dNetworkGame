using UnityEngine;
using UnityEngine.AI;
using Photon.Pun;

[RequireComponent(typeof(NavMeshAgent), typeof(Animator), typeof(PhotonView))]
public class BearController : MonoBehaviourPun, IPunObservable, IDamageable
{
    [Header("References")]
    public BearStat Stat;
    public Collider HitCollider;

    [HideInInspector] public Animator Animator;
    [HideInInspector] public NavMeshAgent Agent;
    [HideInInspector] public BearStateMachine StateMachine;

    // 애니메이터 해시
    public static readonly int HashMoveSpeed = Animator.StringToHash("MoveSpeed");
    public static readonly int HashHit = Animator.StringToHash("Hit");
    public static readonly int HashDie = Animator.StringToHash("Die");
    public static readonly int HashIsRunning = Animator.StringToHash("IsRunning");
    public static readonly int HashIsAttacking = Animator.StringToHash("IsAttacking");

    // 스폰 & 타겟
    public Vector3 SpawnPosition { get; private set; }
    public Transform LockedTarget { get; private set; }

    [SerializeField] private bool _isDead;
    [SerializeField] private BearStateType _currentStateType;
    private BearStateType _syncedStateType;

    private void Awake()
    {
        Animator = GetComponent<Animator>();
        Agent = GetComponent<NavMeshAgent>();
        SpawnPosition = transform.position;

        InitializeStateMachine();

        if (!PhotonNetwork.IsMasterClient)
        {
            Agent.enabled = false;
        }
    }

    private void InitializeStateMachine()
    {
        StateMachine = new BearStateMachine();
        StateMachine.AddState(BearStateType.Idle, new BearIdleState(this));
        StateMachine.AddState(BearStateType.Patrol, new BearPatrolState(this));
        StateMachine.AddState(BearStateType.Chase, new BearChaseState(this));
        StateMachine.AddState(BearStateType.Attack, new BearAttackState(this));
        StateMachine.AddState(BearStateType.Return, new BearReturnState(this));
        StateMachine.AddState(BearStateType.Hit, new BearHitState(this));
        StateMachine.AddState(BearStateType.Dead, new BearDeadState(this));
        StateMachine.Initialize(BearStateType.Idle);

        StateMachine.OnStateChanged += () => _currentStateType = StateMachine.CurrentStateType;
    }

    private void Update()
    {
        if (_isDead) return;

        if (PhotonNetwork.IsMasterClient)
        {
            StateMachine.Update();
            UpdateAnimatorBools();
        }
        else
        {
            SyncAnimationFromState();
        }
    }

    private void UpdateAnimatorBools()
    {
        BearStateType state = StateMachine.CurrentStateType;

        bool isRunning = state == BearStateType.Chase || state == BearStateType.Return;
        bool isAttacking = state == BearStateType.Attack;

        Animator.SetBool(HashIsRunning, isRunning);
        Animator.SetBool(HashIsAttacking, isAttacking);
    }

    private void UpdateAnimatorBools(BearStateType state)
    {
        bool isRunning = state == BearStateType.Chase || state == BearStateType.Return;
        bool isAttacking = state == BearStateType.Attack;

        Animator.SetBool(HashIsRunning, isRunning);
        Animator.SetBool(HashIsAttacking, isAttacking);
    }

    // === 리모트 애니메이션 동기화 ===
    private void SyncAnimationFromState()
    {
        if (_syncedStateType != _currentStateType)
        {
            _currentStateType = _syncedStateType;
            UpdateAnimatorBools(_syncedStateType);
        }
    }

    // === 감지 (MasterClient 전용) ===
    public bool TryDetectPlayer()
    {
        if (LockedTarget != null) return true;

        Collider[] hits = Physics.OverlapSphere(transform.position, Stat.DetectRange);
        foreach (var hit in hits)
        {
            if (hit.CompareTag("Player"))
            {
                PlayerController pc = hit.GetComponentInParent<PlayerController>();
                if (pc != null && !pc.IsDead)
                {
                    Transform cameraRoot = pc.transform.Find("CameraRoot");
                    LockedTarget = cameraRoot != null ? cameraRoot : pc.transform;
                    return true;
                }
            }
        }
        return false;
    }

    public void ClearTarget()
    {
        LockedTarget = null;
    }

    // === 순찰 (MasterClient 전용) ===
    public Vector3 GetRandomPatrolPoint()
    {
        for (int i = 0; i < 10; i++)
        {
            Vector2 randomCircle = Random.insideUnitCircle * Stat.PatrolRadius;
            Vector3 randomPoint = SpawnPosition + new Vector3(randomCircle.x, 0f, randomCircle.y);

            if (NavMesh.SamplePosition(randomPoint, out NavMeshHit navHit, 2f, NavMesh.AllAreas))
            {
                return navHit.position;
            }
        }
        return SpawnPosition;
    }



    // === 공격 데미지 (애니메이션 이벤트에서 호출) ===
    public void OnAttackHit()
    {
        if (!PhotonNetwork.IsMasterClient) return;
        ApplyDamageToTarget();
    }

    // === Attack 애니메이션 종료 이벤트 (마지막 프레임에서 호출) ===
    public void OnAttackEnd()
    {
        if (!PhotonNetwork.IsMasterClient) return;
        if (StateMachine.CurrentStateType == BearStateType.Attack)
        {
            (StateMachine.CurrentState as BearAttackState)?.OnAttackAnimFinished();
        }
    }



    // === Dead 애니메이션 종료 이벤트 (마지막 프레임에서 호출) ===
    public void OnDeadEnd()
    {
        if (!PhotonNetwork.IsMasterClient) return;

        BearSpawner.Instance.RequestRespawn();
        PhotonNetwork.Destroy(gameObject);
    }

    private void ApplyDamageToTarget()
    {
        if (LockedTarget == null) return;

        float dist = Vector3.Distance(transform.position, LockedTarget.position);
        if (dist > Stat.AttackRange * 1.5f) return;

        PlayerController target = LockedTarget.GetComponentInParent<PlayerController>();
        if (target != null && !target.IsDead)
        {
            target.PhotonView.RPC(
                nameof(PlayerController.TakeDamage),
                target.PhotonView.Owner,
                Stat.AttackPower,
                "Bear"
            );
        }
    }

    // === 데미지 수신 (MasterClient에서 처리) ===
    [PunRPC]
    public void TakeDamage(float damage, string killerName)
    {
        if (!PhotonNetwork.IsMasterClient) return;
        if (_isDead) return;

        Stat.HP -= damage;

        if (Stat.HP <= 0f)
        {
            _isDead = true;
            photonView.RPC(nameof(RPC_Die), RpcTarget.All);
        }
        else
        {
            photonView.RPC(nameof(RPC_Hit), RpcTarget.All);
        }
    }

    [PunRPC]
    private void RPC_Hit()
    {
        if (_isDead) return;

        if (PhotonNetwork.IsMasterClient)
        {
            StateMachine.ChangeState(BearStateType.Hit);
        }
        else
        {
            Animator.SetTrigger(HashHit);
        }
    }

    [PunRPC]
    private void RPC_Die()
    {
        _isDead = true;
        Animator.ResetTrigger(HashHit);

        if (PhotonNetwork.IsMasterClient)
        {
            StateMachine.ChangeState(BearStateType.Dead);
        }
        else
        {
            Animator.SetTrigger(HashDie);
            if (HitCollider != null)
                HitCollider.enabled = false;
        }
    }

    // === Photon 동기화 ===
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(Stat.HP);
            stream.SendNext((int)StateMachine.CurrentStateType);
            stream.SendNext(_isDead);
        }
        else
        {
            Stat.HP = (float)stream.ReceiveNext();
            _syncedStateType = (BearStateType)(int)stream.ReceiveNext();
            _isDead = (bool)stream.ReceiveNext();
        }
    }

    // === 기즈모 ===
    private void OnDrawGizmosSelected()
    {
        Vector3 pos = Application.isPlaying ? SpawnPosition : transform.position;

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(pos, Stat != null ? Stat.PatrolRadius : 6f);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, Stat != null ? Stat.DetectRange : 8f);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, Stat != null ? Stat.AttackRange : 2f);
    }
}
