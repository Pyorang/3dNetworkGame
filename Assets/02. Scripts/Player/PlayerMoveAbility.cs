using UnityEngine;

[RequireComponent(typeof(CharacterController), typeof(PlayerStats))]
public class PlayerMoveAbility : MonoBehaviour
{
    private const float GRAVITY = -9.81f;
    private CharacterController _characterController;
    private PlayerAttackAbility _plyaerAttackAbility;
    private PlayerStats _stats;
    public float CurrentSpeed { get; private set; }
    private float _verticalVelocity = 0f;

    private void Awake()
    {
        _characterController = GetComponent<CharacterController>();
        _plyaerAttackAbility = GetComponent<PlayerAttackAbility>();
        _stats = GetComponent<PlayerStats>();
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
                _verticalVelocity = _stats.JumpForce;
            }
        }
        else
        {
            _verticalVelocity += GRAVITY * Time.deltaTime;
        }

        direction.y = _verticalVelocity;

        _characterController.Move(direction * _stats.MoveSpeed * Time.deltaTime);
    }
}

