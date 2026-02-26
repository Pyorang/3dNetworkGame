using System.Collections;
using Photon.Pun;
using UnityEngine;

public class ScoreItem : MonoBehaviourPun
{
    [SerializeField] private int _scoreValue = 100;
    [SerializeField] private float _lifetime = 30f;

    private Rigidbody _rb;
    [SerializeField] private float _rotateSpeed = 180f;
    private bool _isCollected = false;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _rb.constraints = RigidbodyConstraints.FreezeRotation;
    }

    private void Start()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            StartCoroutine(DestroyAfterLifetime());
        }
    }

    private IEnumerator DestroyAfterLifetime()
    {
        yield return new WaitForSeconds(_lifetime);
        if (gameObject != null)
            PhotonNetwork.Destroy(gameObject);
    }

    private void Update()
    {
        transform.Rotate(0f, _rotateSpeed * Time.deltaTime, 0f);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Map"))
        {
            _rb.isKinematic = true;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (_isCollected) return;

        PlayerController player = other.GetComponentInParent<PlayerController>();
        if (player == null) return;
        if (!player.PhotonView.IsMine) return;
        if (player.IsDead) return;

        _isCollected = true;
        player.AddScore(_scoreValue);
        photonView.RPC(nameof(RPC_Collect), RpcTarget.MasterClient);
    }

    [PunRPC]
    private void RPC_Collect()
    {
        if (!PhotonNetwork.IsMasterClient) return;
        PhotonNetwork.Destroy(gameObject);
    }
}
