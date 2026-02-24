using UnityEngine;

[RequireComponent(typeof(Animator), typeof(PlayerController))]
public class PlayerAttackAbility : PlayerAbility
{
    [Header("공격 설정")]
    [Tooltip("공격 순서 (Animator의 ComboIndex 값)")]
    public int[] ComboOrder;

    private Animator _animator;
    private int _currentCombo = 0;
    private bool _isAttacking = false;
    public bool IsAttacking => _isAttacking;
    private bool _canNextAttack = false;
    private static readonly int Attack = Animator.StringToHash("Attack");
    private static readonly int ComboIndex = Animator.StringToHash("ComboIndex");

    protected override void Awake()
    {
        base.Awake();
        _animator = GetComponent<Animator>();
    }

    private void Update()
    {
        if (!_owner.PhotonView.IsMine) return;

        if (Input.GetMouseButtonDown(0))
        {
            TryAttack();
        }
    }

    private void TryAttack()
    {
        if (!_isAttacking)
        {
            if (_owner.Stat.Stamina < _owner.Stat.AttackStaminaRequired) return;

            _owner.Stat.Stamina -= _owner.Stat.AttackStaminaCost;
            _isAttacking = true;
            _currentCombo = 0;
            _animator.SetInteger(ComboIndex, ComboOrder[_currentCombo]);
            _animator.SetTrigger(Attack);
        }
        else if (_canNextAttack)
        {
            if (_owner.Stat.Stamina < _owner.Stat.AttackStaminaRequired) return;

            _owner.Stat.Stamina -= _owner.Stat.AttackStaminaCost;
            _canNextAttack = false;
            _currentCombo++;

            if (_currentCombo >= ComboOrder.Length)
            {
                _currentCombo = 0;
            }

            _animator.SetInteger(ComboIndex, ComboOrder[_currentCombo]);
            _animator.SetTrigger(Attack);
        }
    }

    public void OnCanNextAttack()
    {
        _canNextAttack = true;
    }

    public void OnAttackEnd()
    {
        _isAttacking = false;
        _canNextAttack = false;
        _currentCombo = 0;
    }
}
