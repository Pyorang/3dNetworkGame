using UnityEngine;

[RequireComponent(typeof(CharacterController), typeof(PlayerController))]
public class PlayerMoveAbility : PlayerAbility
{
    private const float GRAVITY = -9.81f;
    private CharacterController _characterController;
    public float CurrentSpeed { get; private set; }
    private float _verticalVelocity = 0f;

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

        _characterController.Move(direction * _owner.Stat.MoveSpeed * Time.deltaTime);
    }
}
