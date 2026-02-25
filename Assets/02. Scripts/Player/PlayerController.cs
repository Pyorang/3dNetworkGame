using System;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class PlayerController : MonoBehaviour, IPunObservable, IDamageable
{
    public PhotonView PhotonView;
    public PlayerStat Stat;
    public bool IsDead { get; private set; } = false;

    [SerializeField] private float _respawnDelay = 5f;

    private Dictionary<Type, PlayerAbility> _abilitiesCache = new();
    private Animator _animator;
    private CharacterController _characterController;
    private static readonly int DieTrigger = Animator.StringToHash("Die");

    private void Awake()
    {
        PhotonView = GetComponent<PhotonView>();
        _animator = GetComponent<Animator>();
        _characterController = GetComponent<CharacterController>();
    }

    #region 데미지 / 사망

    [PunRPC]
    public void TakeDamage(float damage)
    {
        if (IsDead) return;

        Stat.HP -= damage;
        if (Stat.HP <= 0f) Die();
    }

    private void Die()
    {
        if (IsDead) return;

        IsDead = true;
        _animator.SetTrigger(DieTrigger);
        PhotonView.RPC(nameof(SyncDeath), RpcTarget.Others);
        StartCoroutine(RespawnCoroutine());
    }

    [PunRPC]
    private void SyncDeath()
    {
        IsDead = true;
        _animator.SetTrigger(DieTrigger);
    }

    #endregion

    #region 리스폰

    private System.Collections.IEnumerator RespawnCoroutine()
    {
        yield return new WaitForSeconds(_respawnDelay);

        Vector3 respawnPos = GetRandomSpawnPosition();
        Teleport(respawnPos);
        ResetStats();

        IsDead = false;
        _animator.SetTrigger("Idle");

        PhotonView.RPC(nameof(SyncRespawn), RpcTarget.Others, respawnPos.x, respawnPos.y, respawnPos.z);
    }

    [PunRPC]
    private void SyncRespawn(float x, float y, float z)
    {
        Teleport(new Vector3(x, y, z));
        ResetStats();

        IsDead = false;
        _animator.SetTrigger("Idle");
    }

    private Vector3 GetRandomSpawnPosition()
    {
        Transform[] spawnpoints = PhotonServerManager.SpawnpointsStatic;
        int randomIndex = UnityEngine.Random.Range(0, spawnpoints.Length);
        return spawnpoints[randomIndex].position;
    }

    private void Teleport(Vector3 position)
    {
        _characterController.enabled = false;
        transform.position = position;
        _characterController.enabled = true;
    }

    private void ResetStats()
    {
        Stat.HP = Stat.MaxHp;
        Stat.Stamina = Stat.MaxStamina;
    }

    #endregion

    #region 유틸리티

    public T GetAbility<T>() where T : PlayerAbility
    {
        var type = typeof(T);
        if (_abilitiesCache.TryGetValue(type, out PlayerAbility ability))
            return ability as T;

        ability = GetComponent<T>();
        if (ability != null)
        {
            _abilitiesCache[ability.GetType()] = ability;
            return ability as T;
        }

        throw new Exception($"어빌리티 {type.Name}을 {gameObject.name}에서 찾을 수 없습니다.");
    }

    #endregion

    #region 네트워크 동기화

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(Stat.HP);
            stream.SendNext(Stat.Stamina);
        }
        else if (stream.IsReading)
        {
            Stat.HP = (float)stream.ReceiveNext();
            Stat.Stamina = (float)stream.ReceiveNext();
        }
    }

    #endregion
}
