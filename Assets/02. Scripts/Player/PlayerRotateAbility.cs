using UnityEngine;

[RequireComponent(typeof(PlayerStats))]
public class PlayerRotateAbility : MonoBehaviour
{
    public Transform CameraRoot;

    private PlayerStats _stats;
    private float _mx;
    private float _my;

    private void Awake()
    {
        _stats = GetComponent<PlayerStats>();
    }

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update()
    {
        _mx += Input.GetAxis("Mouse X") * _stats.RotationSpeed * Time.deltaTime;
        _my -= Input.GetAxis("Mouse Y") * _stats.RotationSpeed * Time.deltaTime;

        _my = Mathf.Clamp(_my, -90f, 90f);

        transform.eulerAngles = new Vector3(0f, _mx, 0f);
        CameraRoot.localRotation = Quaternion.Euler(_my, 0f, 0f);
    }
}
