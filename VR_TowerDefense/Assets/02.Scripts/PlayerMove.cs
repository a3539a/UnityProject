using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    // �̵��ӵ� 
    public float speed;
    // CharacterController ������Ʈ
    CharacterController cc;

    [Header("���ϰ���")]
    // �߷� ���ӵ��� ũ��
    public float gravity = -9.8f;
    float yVelocity = 0;

    [Header("��������")]
    // ���� �Ŀ�
    public float jumpPower = 5f;

    void Start()
    {
        cc = GetComponent<CharacterController>();
    }

    void Update()
    {
        // ������� �Է��� �޴´�.
        float h = ARAVRInput.GetAxis("Horizontal");
        float v = ARAVRInput.GetAxis("Vertical");

        // �Է¹��� ������ �����.
        Vector3 dir = new Vector3(h, 0, v);

        // ī�޶� �ٶ󺸴� �������� �Է°� ��ȯ
        dir = Camera.main.transform.TransformDirection(dir);

        // �߷��� ������ �������� �߰� v = v0 + at;
        yVelocity += gravity * Time.deltaTime;
        dir.y = yVelocity;

        // ���� ���� ���� �߷¼ӵ��� 0���� �ʱ�ȭ
        if (cc.isGrounded)
        {
            yVelocity = 0;
        }

        // ����ڰ� ������ ��Ʈ�ѷ� Two ��ư ������ y�� �ӵ��� jumpPower�� �Ҵ�
        if (ARAVRInput.GetDown(ARAVRInput.Button.Two, ARAVRInput.Controller.RTouch))
        {
            yVelocity = jumpPower;
        }

        // �̵��Ѵ�
        // Move(���� * �ӵ�)
        cc.Move(dir * speed * Time.deltaTime);
    }
}
