using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LobbyUI : MonoBehaviourPunCallbacks
{
    [Header("캐릭터 오브젝트")]
    [SerializeField] private GameObject _maleCharacter;
    [SerializeField] private GameObject _femaleCharacter;

    [Header("버튼")]
    [SerializeField] private Button _maleButton;
    [SerializeField] private Button _femaleButton;
    [SerializeField] private Button _makeRoomButton;

    [SerializeField] private TMP_InputField _nickNameField;
    [SerializeField] private TMP_InputField _roomNameInputField;

    [Header("방 목록")]
    [SerializeField] private Transform _roomListContent;
    [SerializeField] private RoomItem _roomItemPrefab;

    private ECharacterType _currentCharacterType = ECharacterType.Male;
    private Dictionary<string, RoomInfo> _roomCache = new Dictionary<string, RoomInfo>();

    public ECharacterType CurrentCharacterType => _currentCharacterType;

    private void Start()
    {
        _makeRoomButton.onClick.AddListener(MakeRoom);

        // 초기 상태: 남자 활성화, 여자 비활성화
        _maleCharacter.SetActive(true);
        _femaleCharacter.SetActive(false);

        // 초기 버튼 상태: 현재 선택된 남자 버튼 비활성화
        _maleButton.interactable = false;
        _femaleButton.interactable = true;

        _maleButton.onClick.AddListener(OnMaleButtonClicked);
        _femaleButton.onClick.AddListener(OnFemaleButtonClicked);
    }

    private void OnMaleButtonClicked()
    {
        _currentCharacterType = ECharacterType.Male;

        _maleCharacter.SetActive(true);
        _femaleCharacter.SetActive(false);

        _maleButton.interactable = false;
        _femaleButton.interactable = true;
    }

    private void OnFemaleButtonClicked()
    {
        _currentCharacterType = ECharacterType.Female;

        _maleCharacter.SetActive(false);
        _femaleCharacter.SetActive(true);

        _maleButton.interactable = true;
        _femaleButton.interactable = false;
    }

    // 방 목록 갱신 콜백
    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        // 변경된 것만 캐시에 반영
        foreach (RoomInfo info in roomList)
        {
            if (info.RemovedFromList)
                _roomCache.Remove(info.Name);
            else
                _roomCache[info.Name] = info;
        }

        // 캐시 기반으로 전체 목록 다시 그리기
        foreach (Transform child in _roomListContent)
            Destroy(child.gameObject);

        foreach (RoomInfo info in _roomCache.Values)
        {
            RoomItem item = Instantiate(_roomItemPrefab, _roomListContent);
            item.UpdateRoomInfo(info);
        }
    }

    private void MakeRoom()
    {
        string nickName = _nickNameField.text;
        string roomName = _roomNameInputField.text;

        if (string.IsNullOrEmpty(nickName) || string.IsNullOrEmpty(roomName))
        {
            Debug.LogWarning("닉네임과 방 이름을 입력해주세요.");
            return;
        }

        PhotonNetwork.NickName = nickName;

        // 성별 정보와 방 정보를 PhotonRoomManager에 전달
        PhotonRoomManager.Instance.SetCharacterType(_currentCharacterType);
        PhotonRoomManager.Instance.SetPendingRoom(roomName, nickName, isCreate: true);

        // 씬 이동 먼저
        PhotonNetwork.LoadLevel("Game");
    }
}
