using UnityEngine;

public class PlayerMoveAbility : MonoBehaviour
{
    public float MoveSpeed = 7f;

    public float JumpForce = 2.5f;

    private const float GRAVITY = -9.81f;
    private CharacterController _characterController;
    private float _verticalVelocity = 0f;

    private void Awake()
    {
        _characterController = GetComponent<CharacterController>();
    }

    private void Update()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        Vector3 direction = new Vector3(v, 0, h).normalized;

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

