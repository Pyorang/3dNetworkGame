using TMPro;
using UnityEngine;

public class UI_Score : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _scoreText;

    private void OnEnable()
    {
        if (ScoreManager.Instance != null)
            ScoreManager.Instance.OnLocalScoreChanged += OnScoreChanged;
    }

    private void Start()
    {
        if (ScoreManager.Instance != null)
        {
            ScoreManager.Instance.OnLocalScoreChanged -= OnScoreChanged;
            ScoreManager.Instance.OnLocalScoreChanged += OnScoreChanged;
        }
    }

    private void OnDisable()
    {
        if (ScoreManager.Instance != null)
            ScoreManager.Instance.OnLocalScoreChanged -= OnScoreChanged;
    }

    private void OnScoreChanged(int score)
    {
        _scoreText.text = score.ToString("N0");
    }
}
