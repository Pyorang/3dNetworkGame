using Photon.Pun;
using UnityEngine;

public class ScoreItemSpawner : MonoBehaviour
{
    [SerializeField] private string _scoreItemPrefabName = "ScoreItem";
    [SerializeField] private int _minCount = 3;
    [SerializeField] private int _maxCount = 5;
    [SerializeField] private float _upForce = 5f;
    [SerializeField] private float _spreadForce = 3f;

    private void OnEnable()
    {
        PlayerController.OnPlayerKilled += OnPlayerKilled;
    }

    private void OnDisable()
    {
        PlayerController.OnPlayerKilled -= OnPlayerKilled;
    }

    private void OnPlayerKilled(string killer, string victim)
    {
        if (!PhotonNetwork.IsMasterClient) return;

        PlayerController[] players = FindObjectsOfType<PlayerController>();
        PlayerController deadPlayer = null;
        foreach (var p in players)
        {
            if (p.PhotonView.Owner.NickName == victim)
            {
                deadPlayer = p;
                break;
            }
        }
        if (deadPlayer == null) return;

        Vector3 spawnPos = deadPlayer.transform.position + Vector3.up * 1f;
        int count = Random.Range(_minCount, _maxCount + 1);

        for (int i = 0; i < count; i++)
        {
            GameObject item = PhotonNetwork.Instantiate(_scoreItemPrefabName, spawnPos, Quaternion.identity);
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
