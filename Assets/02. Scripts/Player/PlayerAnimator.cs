using UnityEngine;

[RequireComponent(typeof(Animator), typeof(PlayerController))]
public class PlayerAnimator : PlayerAbility
{
    private Animator _animator;
    private static readonly int MoveSpeed = Animator.StringToHash("MoveSpeed");

    protected override void Awake()
    {
        base.Awake();
        _animator = GetComponent<Animator>();
    }

    private void Update()
    {
        if (!_owner.PhotonView.IsMine) return;

        _animator.SetFloat(MoveSpeed, _owner.GetAbility<PlayerMoveAbility>().CurrentSpeed, 0.1f, Time.deltaTime);
    }
}
