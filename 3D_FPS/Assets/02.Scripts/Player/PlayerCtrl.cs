using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCtrl : MonoBehaviour
{
    Transform tr;
    Animation anim;

    public float moveSpeed = 10f;
    public float turnSpeed = 150f;

    private void Start()
    {
        // �ش� ������Ʈ�� ������Ʈ �Ӽ��� ����ϱ� ���� ȣ��
        tr = GetComponent<Transform>();
        anim = GetComponent<Animation>();

        // �ִϸ޼��� �����ϰ��ִ� Ŭ�� ��
        // �ش� �̸��� �ִϸ��̼� ���
        anim.Play("Idle");
    }

    private void Update()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        float rx = Input.GetAxis("Mouse X"); // ���콺�� X��ǥ �޾ƿ���

        // ���� �¿� �̵� ���� ���� ���
        // ������ 1�ε� �� ���ϳ�?
        Vector3 moveDir = (Vector3.forward * v) + (Vector3.right * h);

        // Translate(���� * �ӷ� * Time.deltatime)
        // normalized�� ���ؼ� ������ ����ȭ
        // Time.deltaTime �� �̿��ؼ� ������ �յ�ȭ
        tr.Translate(moveDir.normalized * moveSpeed * Time.deltaTime);

        // ���콺 ������ ���� ȸ��
        tr.Rotate(Vector3.up * rx * turnSpeed * Time.deltaTime);

        PlayerAnim(h, v);
    }

    void PlayerAnim(float _h, float _v)
    {
        if (_v >= 0.1f) // ��
        {
            // CrossFade("�ִϸ��̼� �̸�", ����Ÿ�� : ��ȯ�ð�);
            // �ִϸ��̼��� �ڿ������� Fade ��� �ǵ��� �ϴ� �����Լ�
            anim.CrossFade("RunF", 0.25f);
        }
        else if (_v <= -0.1f) // ��
        {
            anim.CrossFade("RunB", 0.25f);
        }
        else if (_h >= 0.1f)
        {
            anim.CrossFade("RunR", 0.25f);
        }
        else if (_h <= -0.1f)
        {
            anim.CrossFade("RunL", 0.25f);
        }
        else
        {
            anim.CrossFade("Idle", 0.25f);
        }
    }
}
