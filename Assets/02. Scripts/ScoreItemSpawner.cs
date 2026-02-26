using Photon.Pun;
using UnityEngine;

public class ScoreItemSpawner : MonoBehaviourPun
{
    public static ScoreItemSpawner Instance { get; private set; }

    [SerializeField] private string _scoreItemPrefabName = "ScoreItem";

    [Header("Death Drop")]
    [SerializeField] private int _minCount = 3;
    [SerializeField] private int _maxCount = 5;
    [SerializeField] private float _upForce = 5f;
    [SerializeField] private float _spreadForce = 3f;

    [Header("Periodic Drop")]
    [SerializeField] private float _dropHeight = 15f;
    [SerializeField] private float _dropRadius = 5f;
    [SerializeField] private int _dropMinCount = 1;
    [SerializeField] private int _dropMaxCount = 3;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }

    public void RequestSpawnDeathDrop(Vector3 deathPosition)
    {
        photonView.RPC(nameof(RPC_SpawnDeathDropItems), RpcTarget.MasterClient,
            deathPosition);
    }

    [PunRPC]
    private void RPC_SpawnDeathDropItems(Vector3 deathPosition)
    {
        Vector3 spawnPos = deathPosition + Vector3.up * 1f;
        int count = Random.Range(_minCount, _maxCount + 1);

        var items = SpawnItems(spawnPos, count);
        items.ApplyRadialForce(_upForce, _spreadForce);
    }

    public void RequestPeriodicDrop(Vector3 playerPosition)
    {
        photonView.RPC(nameof(RPC_SpawnPeriodicDropItems), RpcTarget.MasterClient,
            playerPosition);
    }

    [PunRPC]
    private void RPC_SpawnPeriodicDropItems(Vector3 playerPosition)
    {
        int count = Random.Range(_dropMinCount, _dropMaxCount + 1);

        for (int i = 0; i < count; i++)
        {
            Vector2 randomCircle = Random.insideUnitCircle * _dropRadius;
            Vector3 spawnPos = playerPosition + new Vector3(randomCircle.x, _dropHeight, randomCircle.y);

            PhotonNetwork.InstantiateRoomObject(_scoreItemPrefabName, spawnPos, Quaternion.identity);
        }
    }

    private GameObject[] SpawnItems(Vector3 position, int count)
    {
        var items = new GameObject[count];
        for (int i = 0; i < count; i++)
        {
            items[i] = PhotonNetwork.InstantiateRoomObject(
                _scoreItemPrefabName, position, Quaternion.identity);
        }
        return items;
    }
}
