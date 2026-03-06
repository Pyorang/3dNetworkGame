using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;
using System;

public class PhotonRoomManager : MonoBehaviourPunCallbacks
{
    public static PhotonRoomManager Instance { get; private set; }

    private Room _room;
    public Room Room => _room;

    public ECharacterType SelectedCharacterType { get; private set; }

    // 씬 이동 후 처리할 방 정보
    private string _pendingRoomName;
    private string _pendingMasterName;
    private bool _pendingIsCreate;
    private bool _hasPendingRoom;
    private bool _isGameSceneLoaded;

    public event Action OnDataChanged;
    public event Action<Player> OnPlayerEnter;
    public event Action<Player> OnPlayerLeft;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public override void OnEnable()
    {
        base.OnEnable();
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    public override void OnDisable()
    {
        base.OnDisable();
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    public void SetCharacterType(ECharacterType type)
    {
        SelectedCharacterType = type;
    }

    // 씬 이동 전 방 정보 저장
    public void SetPendingRoom(string roomName, string masterName, bool isCreate)
    {
        _pendingRoomName = roomName;
        _pendingMasterName = masterName;
        _pendingIsCreate = isCreate;
        _hasPendingRoom = true;
        _isGameSceneLoaded = false;
    }

    // Game 씬 로드 완료 후 방 생성 or 입장 시도
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name != "Game") return;
        if (!_hasPendingRoom) return;

        _isGameSceneLoaded = true;

        // 연결 준비됐으면 바로 실행
        if (PhotonNetwork.IsConnectedAndReady)
            ExecutePendingRoom();
        // 아니면 OnJoinedLobby / OnConnectedToMaster 콜백에서 실행
    }

    private void ExecutePendingRoom()
    {
        if (!_hasPendingRoom) return;
        if (!_isGameSceneLoaded) return;

        _hasPendingRoom = false;

        if (_pendingIsCreate)
        {
            RoomOptions roomOptions = new RoomOptions();
            roomOptions.MaxPlayers = 20;
            roomOptions.IsVisible = true;
            roomOptions.IsOpen = true;
            roomOptions.CustomRoomProperties = new ExitGames.Client.Photon.Hashtable()
            {
                { "MasterName", _pendingMasterName }
            };
            roomOptions.CustomRoomPropertiesForLobby = new string[] { "MasterName" };

            Debug.Log($"[PhotonRoomManager] CreateRoom 시도: {_pendingRoomName}");
            PhotonNetwork.CreateRoom(_pendingRoomName, roomOptions);
        }
        else
        {
            Debug.Log($"[PhotonRoomManager] JoinRoom 시도: {_pendingRoomName}");
            PhotonNetwork.JoinRoom(_pendingRoomName);
        }
    }

    // 연결이 늦게 됐을 때를 대비해 콜백에서도 실행 시도
    public override void OnConnectedToMaster()
    {
        if (_hasPendingRoom && _isGameSceneLoaded)
            PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
        if (_hasPendingRoom && _isGameSceneLoaded)
            ExecutePendingRoom();
    }

    public override void OnJoinedRoom()
    {
        _room = PhotonNetwork.CurrentRoom;
        Debug.Log($"[PhotonRoomManager] OnJoinedRoom - room={_room.Name}, OnDataChanged 구독자 수={OnDataChanged?.GetInvocationList()?.Length ?? 0}");
        OnDataChanged?.Invoke();
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.Log($"[PhotonRoomManager] OnPlayerEnteredRoom - {newPlayer.NickName}, OnPlayerEnter 구독자 수={OnPlayerEnter?.GetInvocationList()?.Length ?? 0}");
        OnDataChanged?.Invoke();
        OnPlayerEnter?.Invoke(newPlayer);
    }

    public override void OnPlayerLeftRoom(Player newPlayer)
    {
        OnDataChanged?.Invoke();
        OnPlayerLeft?.Invoke(newPlayer);
    }

    // === 안전한 구독 메서드 (구독 시 현재 상태 즉시 전달) ===
    public void RegisterDataChanged(Action callback)
    {
        OnDataChanged += callback;
        Debug.Log($"[PhotonRoomManager] RegisterDataChanged - room null? {_room == null}");
        if (_room != null)
            callback.Invoke();
    }

    public void RegisterPlayerEnter(Action<Player> callback)
    {
        OnPlayerEnter += callback;
    }

    public void RegisterPlayerLeft(Action<Player> callback)
    {
        OnPlayerLeft += callback;
    }
}
