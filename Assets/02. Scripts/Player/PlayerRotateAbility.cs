using Unity.Cinemachine;
using UnityEngine;

[RequireComponent(typeof(PlayerController))]
public class PlayerRotateAbility : PlayerAbility
{
    public Transform CameraRoot;

    private float _mx;
    private float _my;

    private void Start()
    {
        if (!_owner.PhotonView.IsMine) return;
        Cursor.lockState = CursorLockMode.Locked;

        CinemachineCamera vcam = GameObject.Find("FollowCamera").GetComponent<CinemachineCamera>();
        vcam.Follow = CameraRoot.transform;
    }

    private void Update()
    {
        if (!_owner.PhotonView.IsMine || _owner.IsDead) return;

        _mx += Input.GetAxis("Mouse X") * _owner.Stat.RotationSpeed * Time.deltaTime;
        _my -= Input.GetAxis("Mouse Y") * _owner.Stat.RotationSpeed * Time.deltaTime;

        _my = Mathf.Clamp(_my, -90f, 90f);

        transform.eulerAngles = new Vector3(0f, _mx, 0f);
        CameraRoot.localRotation = Quaternion.Euler(_my, 0f, 0f);
    }
}
