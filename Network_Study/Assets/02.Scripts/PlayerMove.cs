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

    [Header("ĳ���� UI")]
    public Text nameText;

    [Header("����ȭ ����")]
    Vector3 setPos;
    Quaternion setRot;
    float dirSpeed = 0;

    void Start()
    {
        // ������� ������Ʈ�� ���� ī�޶� Ȱ��ȭ
        cameraRig.SetActive(photonView.IsMine);

        // �� �������� �г����� ���
        // ���� ������ �� ������ �г����� ������
        nameText.text = photonView.Owner.NickName;

        // �ڽ��� �̸��� ���, �ٸ������ �̸��� ������
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
            // �޼� �潺ƽ�� ���Ⱚ�� ������ ĳ������ �̵������� �����ش�.
            //Vector2 stickPos = ARAVRInput.Get(ARAVRInput.LHand, ARAVRInput.Controller.LTouch);
            //Vector2 stickPos = OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick, OVRInput.Controller.LTouch);

            //Vector3 dir = new Vector3(stickPos.x, 0, stickPos.y);

            float h = Input.GetAxisRaw("Horizontal");
            float v = Input.GetAxisRaw("Vertical");

            Vector3 dir = new Vector3(h, 0, v);

            dir.Normalize();

            // ĳ������ �̵����� ���͸� ī�޶� �ٶ󺸴� ������ �������� �ϵ��� ����
            dir = cameraRig.transform.TransformDirection(dir);
            transform.position += dir * moveSpeed * Time.deltaTime;

            // ����, �޼� �潺ƽ�� ����̸� �� �������� ĳ���͸� ȸ��
            float magnitude = dir.magnitude;

            if (magnitude > 0)
            {
                myCharacter.rotation = Quaternion.LookRotation(dir);
            }

            // �ִϸ������� ���� Ʈ�� ������ ������ ũ�⸦ ����
            anim.SetFloat("Speed", magnitude);
        }
        else
        {
            // ��ü ������Ʈ�� ��ġ���� ĳ������ ȸ������
            // �������� ���޹޴� ������ ����ȭ�Ѵ�.
            transform.position = Vector3.Lerp(transform.position, setPos, Time.deltaTime * 20f) ;
            myCharacter.rotation = Quaternion.Lerp(myCharacter.rotation, setRot, Time.deltaTime * 20f);

            // �������� ���޹��� ������ �ִϸ����� �Ķ���� ���� ����ȭ
            anim.SetFloat("Speed", dirSpeed);
        }
    }

    private void Rotate()
    {
        if (photonView.IsMine)
        {
            // �������� ���� ������ �¿� ���⸦ ����
            float rotH = OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick, OVRInput.Controller.RTouch).x;

            // cameraRig ������Ʈ�� ȸ��
            cameraRig.transform.eulerAngles += new Vector3(0, rotH, 0) * rotSpeed * Time.deltaTime;
        }
    }

    // ������ ����ȭ�� ���� ������ ���� �� ���� ���
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        // ����, �����͸� �����ϴ� ��Ȳ�� ��
        if (stream.IsWriting)
        {
            stream.SendNext(transform.position); // ��ġ
            stream.SendNext(myCharacter.rotation); // ȸ��
            stream.SendNext(anim.GetFloat("Speed")); // float
        }
        // ���� �� ��
        // �۽� ������ ���� ��� �޴´�
        else if (stream.IsWriting)
        {
            // ��������̱� �ѵ� Ÿ���� ���� ������Ѵ�.
            setPos = (Vector3)stream.ReceiveNext(); // ��ġ
            setRot = (Quaternion)stream.ReceiveNext(); // ȸ��
            dirSpeed = (float)stream.ReceiveNext(); // float
        }
    }
}
