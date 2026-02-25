using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using System;

public class PhotonRoomManager : MonoBehaviourPunCallbacks
{
    public static PhotonRoomManager Instance { get; private set; }

    private Room _room;
    public Room Room => _room;

    public Transform[] Spawnpoints;

    public event Action OnDataChanged;              // 룸 정보가 변경될 때마다 호출되는 이벤트

    public event Action<Player> OnPlayerEnter;      // 룸에 새로운 플레이어가 입장할 때 호출되는 이벤트
    public event Action<Player> OnPlayerLeft;       // 룸에서 플레이어가 나갈 때 호출되는 이벤트

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    // 방 입장에 성공하면 자동으로 호출되는 콜백 함수
    public override void OnJoinedRoom()
    {
       _room = PhotonNetwork.CurrentRoom;

        OnDataChanged?.Invoke();

        int randomIndex = UnityEngine.Random.Range(0, Spawnpoints.Length);
        PhotonNetwork.Instantiate("Player", Spawnpoints[randomIndex].position, Quaternion.identity);
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        OnDataChanged?.Invoke();
        OnPlayerEnter?.Invoke(newPlayer);
    }

    public override void OnPlayerLeftRoom(Player newPlayer)
    {
        OnDataChanged?.Invoke();
        OnPlayerLeft?.Invoke(newPlayer);
    }
}
