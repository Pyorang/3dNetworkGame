using UnityEngine;

[RequireComponent(typeof(CharacterController), typeof(PlayerController))]
public class PlayerMoveAbility : PlayerAbility
{
    private const float GRAVITY = -9.81f;

    private CharacterController _characterController;
    public float CurrentSpeed { get; private set; }
    public bool IsSprinting { get; private set; }

    private float _verticalVelocity = 0f;
    private float _staminaRegenTimer = 0f;

    [Header("낙하 사망")]
    [SerializeField] private float _lethalFallDistance = 10f;
    private float _highestY;
    private bool _wasGrounded;

    protected override void Awake()
    {
        base.Awake();
        _characterController = GetComponent<CharacterController>();
    }

    private void Start()
    {
        _highestY = transform.position.y;
    }

    private void Update()
    {
        if (!_owner.PhotonView.IsMine || _owner.IsDead) return;

        Vector3 direction = GetMoveDirection();
        HandleSprint();
        HandleJumpAndGravity();

        direction.y = _verticalVelocity;
        float speedMultiplier = IsSprinting ? _owner.Stat.SprintSpeedMultiplier : 1f;
        _characterController.Move(direction * _owner.Stat.MoveSpeed * speedMultiplier * Time.deltaTime);

        CheckFallDamage();
    }

    #region 이동

    private Vector3 GetMoveDirection()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        if (_owner.GetAbility<PlayerAttackAbility>().IsAttacking)
        {
            h = 0f;
            v = 0f;
        }

        CurrentSpeed = new Vector2(h, v).magnitude;
        return (transform.forward * v + transform.right * h).normalized;
    }

    private void HandleJumpAndGravity()
    {
        if (_characterController.isGrounded)
        {
            _verticalVelocity = 0f;

            if (Input.GetKey(KeyCode.Space) && _owner.Stat.Stamina >= _owner.Stat.JumpStaminaRequired)
            {
                _owner.Stat.Stamina -= _owner.Stat.JumpStaminaCost;
                _verticalVelocity = _owner.Stat.JumpPower;
            }
        }
        else
        {
            _verticalVelocity += GRAVITY * Time.deltaTime;
        }
    }

    #endregion

    #region 스태미나

    private void HandleSprint()
    {
        bool isMoving = CurrentSpeed > 0.1f;
        bool wantsToSprint = Input.GetKey(KeyCode.LeftShift) && isMoving;
        IsSprinting = wantsToSprint && _owner.Stat.Stamina > 0f;

        if (IsSprinting)
        {
            _owner.Stat.Stamina -= _owner.Stat.StaminaDrainRate * Time.deltaTime;
            _staminaRegenTimer = _owner.Stat.StaminaRegenDelay;
        }
        else
        {
            _staminaRegenTimer -= Time.deltaTime;
            if (_staminaRegenTimer <= 0f)
            {
                _owner.Stat.Stamina += _owner.Stat.StaminaRegenRate * Time.deltaTime;
            }
        }
    }

    #endregion

    #region 낙하 판정

    private void CheckFallDamage()
    {
        bool isGrounded = _characterController.isGrounded;

        if (isGrounded)
        {
            if (!_wasGrounded)
            {
                float fallDistance = _highestY - transform.position.y;
                if (fallDistance >= _lethalFallDistance)
                {
                    _owner.TakeDamage(_owner.Stat.HP);
                }
            }
            _highestY = transform.position.y;
        }
        else
        {
            if (transform.position.y > _highestY)
                _highestY = transform.position.y;
        }

        _wasGrounded = isGrounded;
    }

    #endregion
}
