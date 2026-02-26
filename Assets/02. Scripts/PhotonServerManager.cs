using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;

public class PhotonServerManager : MonoBehaviourPunCallbacks
{
    // MonoBehaviour                     : Unity의 다양한 '이벤트' 콜백 함수를 오버라이드할 수 있다. (Awake, Start, Update ..
    // MonoBehaviourPunCallbacks         : Pun의 다양한 '서버 이벤트' 콜백 함수를 오버라이드할 수 있다.
    // - 서버 접속에 성공/실패했다.
    // - 내가 방 입장에 성공/실패했다.
    // - 누군가가 내 방에 들어왔다. 등등등..

    private string _version = "0.0.1";
    private string _nickName = "Pyorang";

    private void Start()
    {
        _nickName += $"_{UnityEngine.Random.Range(100, 000)}";

        PhotonNetwork.GameVersion = _version;
        PhotonNetwork.NickName = _nickName;

        PhotonNetwork.SendRate = 30; // 얼마나 자주 데이터를 송수신할 것인가.. (실제 송수신)
        PhotonNetwork.SerializationRate = 10; // 얼마나 자주 데이터를 직렬화 할 것인지. (송수신 준비)

        // 방장이 로드한 씬 게임에 다른 유저들도 똑같이 그 씬을 로드하도록 동기화해준다.
        // 방장 (마스터 클라이언트) : 방을 만든 '소유자' (방에는 하나의 마스터 클라이언트가 존재)
        // 방장이 씬을 로드하면, 다른 유저들도 똑같은 씬을 로드하도록 해준다.
        PhotonNetwork.AutomaticallySyncScene = true;

        // 위에 설정한 값들을 이용해서 서버로 접속 시도
        PhotonNetwork.ConnectUsingSettings();
    }

    // 포톤 서버에 접속이 성공하면 호출되는 콜백 함수
    public override void OnConnected()
    {
        Debug.Log("네임서버 접속 성공!");
        Debug.Log(PhotonNetwork.CloudRegion);
    }

    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinLobby();
    }

    // 로비 입장에 성공하면 자동으로 호출되는 콜백 함수
    public override void OnJoinedLobby()
    {
        Debug.Log("로비 접속 완료");
        Debug.Log(PhotonNetwork.InLobby);

        PhotonNetwork.JoinRandomRoom();
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log($"방 입장 실패했습니다: {returnCode} - {message}");

        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = 20;
        roomOptions.IsVisible = true;
        roomOptions.IsOpen = true;

        PhotonNetwork.CreateRoom("test", roomOptions);
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        Debug.Log($"방 입장 실패했습니다: {returnCode} - {message}");
    }
}
