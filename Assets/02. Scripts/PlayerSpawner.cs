using UnityEngine;
using Photon.Pun;

public class PlayerSpawner : MonoBehaviourPunCallbacks
{
    public static PlayerSpawner Instance { get; private set; }

    [SerializeField] private Transform[] _spawnPoints;
    public Transform[] SpawnPoints => _spawnPoints;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    // Room 입장 완료 후 스폰
    public override void OnJoinedRoom()
    {
        SpawnPlayer();
    }

    private void SpawnPlayer()
    {
        string prefabName = PhotonRoomManager.Instance.SelectedCharacterType == ECharacterType.Male
            ? "MalePlayer"
            : "FemalePlayer";

        int randomIndex = Random.Range(0, _spawnPoints.Length);
        PhotonNetwork.Instantiate(prefabName, _spawnPoints[randomIndex].position, Quaternion.identity);
    }
}
