using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RoomItem : MonoBehaviour
{
    [SerializeField] private TMP_Text _roomNameText;
    [SerializeField] private TMP_Text _masterNameText;
    [SerializeField] private TMP_Text _playerCountText;
    [SerializeField] private Button _joinButton;

    private string _roomName;

    private void Start()
    {
        _joinButton.onClick.AddListener(OnClickJoinRoom);
    }

    public void UpdateRoomInfo(RoomInfo info)
    {
        _roomName = info.Name;
        _roomNameText.text = info.Name;
        _playerCountText.text = $"{info.PlayerCount}/{info.MaxPlayers}";

        // CustomProperties에서 방장 이름 가져오기
        if (info.CustomProperties.TryGetValue("MasterName", out object masterName))
            _masterNameText.text = masterName.ToString();
        else
            _masterNameText.text = "Unknown";
    }

    private void OnClickJoinRoom()
    {
        PhotonRoomManager.Instance.SetCharacterType(ECharacterType.Male);
        PhotonRoomManager.Instance.SetPendingRoom(_roomName, "", isCreate: false);
        PhotonNetwork.LoadLevel("Game");
    }
}
