using Photon.Pun;
using UnityEngine;

public class ScoreItemSpawner : MonoBehaviourPun
{
    public static ScoreItemSpawner Instance { get; private set; }

    [SerializeField] private string _scoreItemPrefabName = "ScoreItem";
    [SerializeField] private int _minCount = 3;
    [SerializeField] private int _maxCount = 5;
    [SerializeField] private float _upForce = 5f;
    [SerializeField] private float _spreadForce = 3f;

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

    /// <summary>
    /// 죽은 플레이어가 호출. 마스터 클라이언트에게 RPC로 스폰 요청.
    /// </summary>
    public void RequestSpawnItems(Vector3 deathPosition)
    {
        photonView.RPC(nameof(RPC_SpawnItems), RpcTarget.MasterClient,
            deathPosition);
    }

    [PunRPC]
    private void RPC_SpawnItems(Vector3 deathPosition)
    {
        Vector3 spawnPos = deathPosition + Vector3.up * 1f;
        int count = Random.Range(_minCount, _maxCount + 1);

        for (int i = 0; i < count; i++)
        {
            GameObject item = PhotonNetwork.InstantiateRoomObject(
                _scoreItemPrefabName, spawnPos, Quaternion.identity);
            Rigidbody rb = item.GetComponent<Rigidbody>();
            if (rb != null)
            {
                float angle = (360f / count) * i * Mathf.Deg2Rad;
                Vector3 dir = new Vector3(Mathf.Cos(angle), 0f, Mathf.Sin(angle));
                Vector3 force = Vector3.up * _upForce + dir * _spreadForce;
                rb.AddForce(force, ForceMode.Impulse);
            }
        }
    }
}
