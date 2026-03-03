using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;

public class BearSpawner : MonoBehaviourPunCallbacks
{
    public static BearSpawner Instance { get; private set; }

    [Header("Spawn Settings")]
    [SerializeField] private string _bearPrefabName = "Bear";
    [SerializeField] private Transform[] _spawnPoints;
    [SerializeField] private int _bearCount = 1;
    [SerializeField] private float _respawnDelay = 10f;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public override void OnJoinedRoom()
    {
        if (!PhotonNetwork.IsMasterClient) return;
        SpawnBears();
    }

    private void SpawnBears()
    {
        for (int i = 0; i < _bearCount; i++)
        {
            SpawnBear();
        }
    }

    private void SpawnBear()
    {
        Transform point = _spawnPoints[Random.Range(0, _spawnPoints.Length)];
        PhotonNetwork.InstantiateRoomObject(
            _bearPrefabName,
            point.position,
            point.rotation
        );
    }

    public void RequestRespawn()
    {
        if (!PhotonNetwork.IsMasterClient) return;
        StartCoroutine(RespawnCoroutine());
    }

    private IEnumerator RespawnCoroutine()
    {
        yield return new WaitForSeconds(_respawnDelay);
        SpawnBear();
    }
}
