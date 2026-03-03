using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;

public class ScoreManager : MonoBehaviourPunCallbacks
{
    public static ScoreManager Instance { get; private set; }

    private const string SCORE_KEY = "Score";

    public event System.Action OnRankingChanged;
    public event System.Action<int> OnLocalScoreChanged;

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
        PhotonNetwork.LocalPlayer.SetCustomProperties(
            new Hashtable { { SCORE_KEY, 0 } }
        );
    }

    public void UpdateScore(int score)
    {
        PhotonNetwork.LocalPlayer.SetCustomProperties(
            new Hashtable { { SCORE_KEY, score } }
        );
        OnLocalScoreChanged?.Invoke(score);
    }

    public int GetLocalScore()
    {
        return GetScore(PhotonNetwork.LocalPlayer);
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
    {
        if (changedProps.ContainsKey(SCORE_KEY))
        {
            OnRankingChanged?.Invoke();
        }
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        OnRankingChanged?.Invoke();
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        OnRankingChanged?.Invoke();
    }

    public List<(string name, int score)> GetRanking()
    {
        return PhotonNetwork.PlayerList
            .Select(p => (p.NickName, GetScore(p)))
            .OrderByDescending(p => p.Item2)
            .ToList();
    }

    private int GetScore(Player player)
    {
        if (player.CustomProperties.TryGetValue(SCORE_KEY, out object score))
            return (int)score;
        return 0;
    }
}
