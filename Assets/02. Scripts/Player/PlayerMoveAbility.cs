using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMoveAbility : MonoBehaviour
{
    public float MoveSpeed = 7f;

    public float JumpForce = 2.5f;

    private const float GRAVITY = -9.81f;
    private CharacterController _characterController;
    private PlayerAttackAbility _plyaerAttackAbility;
    public float CurrentSpeed { get; private set; }
    private float _verticalVelocity = 0f;

    private void Awake()
    {
        _characterController = GetComponent<CharacterController>();
        _plyaerAttackAbility = GetComponent<PlayerAttackAbility>();
    }

    private void Update()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        if(_plyaerAttackAbility.IsAttacking)
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
                _verticalVelocity = JumpForce;
            }
        }
        else
        {
            _verticalVelocity += GRAVITY * Time.deltaTime;
        }

        direction.y = _verticalVelocity;

        _characterController.Move(direction * MoveSpeed * Time.deltaTime);
    }
}

