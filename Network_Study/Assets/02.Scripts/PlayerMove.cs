using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class PlayerMove : MonoBehaviourPun, IPunObservable
{
    public float moveSpeed = 3f;
    public float rotSpeed = 200f;
    public GameObject cameraRig;
    public Transform myCharacter;
    public Animator anim;

    [Header("캐릭터 UI")]
    public Text nameText;

    [Header("동기화 관련")]
    Vector3 setPos;
    Quaternion setRot;
    float dirSpeed = 0;

    void Start()
    {
        // 사용자의 오브젝트일 때만 카메라 활성화
        cameraRig.SetActive(photonView.IsMine);

        // 각 접속자의 닉네임을 출력
        // 포톤 접속할 때 지정한 닉네임을 가져옴
        nameText.text = photonView.Owner.NickName;

        // 자신의 이름은 녹색, 다른사람의 이름은 빨간색
        if (photonView.IsMine)
        {
            nameText.color = Color.green;
        }
        else
        {
            nameText.color = Color.red;
        }
    }

    void Update()
    {
        Move();
        Rotate();
    }

    private void Move()
    {
        if (photonView.IsMine)
        {
            // 왼손 썸스틱의 방향값을 가져와 캐릭터의 이동방향을 정해준다.
            //Vector2 stickPos = ARAVRInput.Get(ARAVRInput.LHand, ARAVRInput.Controller.LTouch);
            //Vector2 stickPos = OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick, OVRInput.Controller.LTouch);

            //Vector3 dir = new Vector3(stickPos.x, 0, stickPos.y);

            float h = Input.GetAxisRaw("Horizontal");
            float v = Input.GetAxisRaw("Vertical");

            Vector3 dir = new Vector3(h, 0, v);

            dir.Normalize();

            // 캐릭터의 이동방향 벡터를 카메라가 바라보는 방향을 정면으로 하도록 변경
            dir = cameraRig.transform.TransformDirection(dir);
            transform.position += dir * moveSpeed * Time.deltaTime;

            // 만일, 왼손 썸스틱을 기울이면 그 방향으로 캐릭터를 회전
            float magnitude = dir.magnitude;

            if (magnitude > 0)
            {
                myCharacter.rotation = Quaternion.LookRotation(dir);
            }

            // 애니메이터의 블랜드 트리 변수에 벡터의 크기를 전달
            anim.SetFloat("Speed", magnitude);
        }
        else
        {
            // 전체 오브젝트의 위치값과 캐릭터의 회전값을
            // 서버에서 전달받는 값으로 동기화한다.
            transform.position = Vector3.Lerp(transform.position, setPos, Time.deltaTime * 20f) ;
            myCharacter.rotation = Quaternion.Lerp(myCharacter.rotation, setRot, Time.deltaTime * 20f);

            // 서버에서 전달받은 값으로 애니메이터 파라미터 값을 동기화
            anim.SetFloat("Speed", dirSpeed);
        }
    }

    private void Rotate()
    {
        if (photonView.IsMine)
        {
            // 오른손의 방향 값에서 좌우 기울기를 누적
            float rotH = OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick, OVRInput.Controller.RTouch).x;

            // cameraRig 오브젝트를 회전
            cameraRig.transform.eulerAngles += new Vector3(0, rotH, 0) * rotSpeed * Time.deltaTime;
        }
    }

    // 데이터 동기화를 위한 데이터 전송 및 수신 기능
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        // 만일, 데이터를 전송하는 상황일 때
        if (stream.IsWriting)
        {
            stream.SendNext(transform.position); // 위치
            stream.SendNext(myCharacter.rotation); // 회전
            stream.SendNext(anim.GetFloat("Speed")); // float
        }
        // 수신 할 때
        // 송신 데이터 순서 대로 받는다
        else if (stream.IsWriting)
        {
            // 순서대로이긴 한데 타입을 지정 해줘야한다.
            setPos = (Vector3)stream.ReceiveNext(); // 위치
            setRot = (Quaternion)stream.ReceiveNext(); // 회전
            dirSpeed = (float)stream.ReceiveNext(); // float
        }
    }
}
