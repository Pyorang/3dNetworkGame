using Photon.Realtime;
using TMPro;
using UnityEngine;

public class UI_RoomLog : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _logText;

    private void Start()
    {
        _logText.text = "Room Entered.\n";

        PhotonRoomManager.Instance.OnPlayerEnter += OnPlayerEnter;
        PhotonRoomManager.Instance.OnPlayerLeft += OnPlayerLeft;
        PlayerController.OnPlayerKilled += OnPlayerKilled;
    }

    private void OnDestroy()
    {
        PlayerController.OnPlayerKilled -= OnPlayerKilled;
    }

    private void OnPlayerEnter(Player newPlayer)
    {
        _logText.text += $"{newPlayer.NickName} has entered the room.\n";
    }

    private void OnPlayerLeft(Player player)
    {
        _logText.text += $"{player.NickName} has left the room.\n";
    }

    private void OnPlayerKilled(string killer, string victim)
    {
        _logText.text += $"{killer} killed {victim}.\n";
    }
}
