using TMPro;
using UnityEngine;

public class UI_Score : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _scoreText;

    private void OnEnable()
    {
        PlayerController.OnScoreChanged += OnScoreChanged;
    }

    private void OnDisable()
    {
        PlayerController.OnScoreChanged -= OnScoreChanged;
    }

    private void OnScoreChanged(int score)
    {
        _scoreText.text = score.ToString("N0");
    }
}
