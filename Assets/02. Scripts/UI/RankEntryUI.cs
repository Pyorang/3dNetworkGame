using TMPro;
using UnityEngine;

public class RankEntryUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _nameText;
    [SerializeField] private TextMeshProUGUI _scoreText;

    public void UpdateEntry(string playerName, int score)
    {
        _nameText.text = playerName;
        _scoreText.text = score.ToString("N0");
    }

    public void Clear()
    {
        _nameText.text = "";
        _scoreText.text = "";
    }
}
