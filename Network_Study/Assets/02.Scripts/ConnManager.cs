using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// Photon API �� ����ϱ� ���� ���� �����̽�
using Photon.Pun;
using Photon.Realtime;

// ��Ʈ��ũ ó�� Ŭ����
public class ConnManager : MonoBehaviourPunCallbacks
{
    private void Start()
    {
        PhotonNetwork.GameVersion = "0.1";

        // ���ӿ��� ����� ������� �̸��� �������� ���Ѵ�.
        int num = Random.Range(0, 1000);
        PhotonNetwork.NickName = "Player_" + num;

        // ���ӿ� �����ϸ� ������ Ŭ���̾�Ʈ�� ������ ���� �ڵ����� ����ȭ
        PhotonNetwork.AutomaticallySyncScene = true;

        // ���� ����
        // ConnectUsingSettings �Լ��� ȣ�� �Ǹ� ���� �ϰ� ��
        // ConnectUsingSettings(��������)
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        // ������ ������ ���ӵǰ� �� �� �κ�� ���� �õ�
        PhotonNetwork.JoinLobby(TypedLobby.Default);
    }

    public override void OnJoinedLobby()
    {
        Debug.Log("Lobby Joined");
        RoomOptions ro = new RoomOptions() { IsVisible = true,
                                            IsOpen = true,
                                            MaxPlayers = 8};
        // JoinOrCreateRoom(����, ��ɼ�, �κ�����)
        PhotonNetwork.JoinOrCreateRoom("NetTest", ro, TypedLobby.Default);
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("Room Joined");

        // �ݰ� 2m �̳��� Player �������� ����
        Vector2 originPos = Random.insideUnitCircle * 2f;
        PhotonNetwork.Instantiate("Player", new Vector3(originPos.x, 0, originPos.y), Quaternion.identity);
    }
}
