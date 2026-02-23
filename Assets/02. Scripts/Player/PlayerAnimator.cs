using UnityEngine;

[RequireComponent(typeof(Animator), typeof(PlayerMoveAbility))]
public class PlayerAnimator : MonoBehaviour
{
    private Animator _animator;
    private PlayerMoveAbility _moveAbility;
    private static readonly int MoveSpeed = Animator.StringToHash("MoveSpeed");

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _moveAbility = GetComponent<PlayerMoveAbility>();
    }

    private void Update()
    {
        _animator.SetFloat(MoveSpeed, _moveAbility.CurrentSpeed, 0.1f, Time.deltaTime);
    }
}
