using UnityEngine;

public class RankingUI : MonoBehaviour
{
    [SerializeField] private RankEntryUI[] _rankEntries;

    private void OnEnable()
    {
        if (ScoreManager.Instance != null)
            ScoreManager.Instance.OnRankingChanged += UpdateRanking;
    }

    private void Start()
    {
        if (ScoreManager.Instance != null)
        {
            ScoreManager.Instance.OnRankingChanged -= UpdateRanking;
            ScoreManager.Instance.OnRankingChanged += UpdateRanking;
            UpdateRanking();
        }
    }

    private void OnDisable()
    {
        if (ScoreManager.Instance != null)
            ScoreManager.Instance.OnRankingChanged -= UpdateRanking;
    }

    private void UpdateRanking()
    {
        var ranking = ScoreManager.Instance.GetRanking();

        for (int i = 0; i < _rankEntries.Length; i++)
        {
            if (i < ranking.Count)
                _rankEntries[i].UpdateEntry(ranking[i].name, ranking[i].score);
            else
                _rankEntries[i].Clear();
        }
    }
}
