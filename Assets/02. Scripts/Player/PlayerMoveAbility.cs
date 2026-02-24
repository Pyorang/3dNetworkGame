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

    protected override void Awake()
    {
        base.Awake();
        _characterController = GetComponent<CharacterController>();
    }

    private void Update()
    {
        if (!_owner.PhotonView.IsMine) return;

        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        if (_owner.GetAbility<PlayerAttackAbility>().IsAttacking)
        {
            h = 0f;
            v = 0f;
        }

        Vector3 direction = (transform.forward * v + transform.right * h).normalized;
        CurrentSpeed = new Vector2(h, v).magnitude;

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

        float speedMultiplier = IsSprinting ? _owner.Stat.SprintSpeedMultiplier : 1f;

        if (_characterController.isGrounded)
        {
            _verticalVelocity = 0f;

            if (Input.GetKey(KeyCode.Space))
            {
                _verticalVelocity = _owner.Stat.JumpPower;
            }
        }
        else
        {
            _verticalVelocity += GRAVITY * Time.deltaTime;
        }

        direction.y = _verticalVelocity;

        _characterController.Move(direction * _owner.Stat.MoveSpeed * speedMultiplier * Time.deltaTime);
    }
}
