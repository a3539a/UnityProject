using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// Photon API 를 사용하기 위한 네임 스페이스
using Photon.Pun;
using Photon.Realtime;

// 네트워크 처리 클래스
public class ConnManager : MonoBehaviourPunCallbacks
{
    private void Start()
    {
        PhotonNetwork.GameVersion = "0.1";

        // 게임에서 사용할 사용자의 이름을 랜덤으로 정한다.
        int num = Random.Range(0, 1000);
        PhotonNetwork.NickName = "Player_" + num;

        // 게임에 참여하면 마스터 클라이언트가 구성한 씬에 자동으로 동기화
        PhotonNetwork.AutomaticallySyncScene = true;

        // 서버 접속
        // ConnectUsingSettings 함수가 호출 되면 접속 하게 됨
        // ConnectUsingSettings(버전정보)
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        // 마스터 서버에 접속되고 난 뒤 로비로 접속 시도
        PhotonNetwork.JoinLobby(TypedLobby.Default);
    }

    public override void OnJoinedLobby()
    {
        Debug.Log("Lobby Joined");
        RoomOptions ro = new RoomOptions() { IsVisible = true,
                                            IsOpen = true,
                                            MaxPlayers = 8};
        // JoinOrCreateRoom(방제, 방옵션, 로비종류)
        PhotonNetwork.JoinOrCreateRoom("NetTest", ro, TypedLobby.Default);
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("Room Joined");

        // 반경 2m 이내에 Player 프리팹을 생성
        Vector2 originPos = Random.insideUnitCircle * 2f;
        PhotonNetwork.Instantiate("Player", new Vector3(originPos.x, 0, originPos.y), Quaternion.identity);
    }
}
