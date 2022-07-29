using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    Vector3 m_Movement;
    Quaternion m_Rotation = Quaternion.identity;
    Rigidbody m_rb;

    Animator anim;

    AudioSource m_AudioSource;

    // �÷��̾� ����ȸ�� �ӵ�
    public float turnSpeed;

    void Awake()
    {
        anim = GetComponent<Animator>();
        m_rb = GetComponent<Rigidbody>();
        m_AudioSource = GetComponent<AudioSource>();
    }

    void FixedUpdate()
    {
        // �÷��̾� ������
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        m_Movement.Set(h, 0f, v);
        m_Movement.Normalize();

        // ���� ���ϱ� Mathf.Approximately(h, 0f) -> h�� ���� 0�� ����ϸ� �� �ƴϸ� ���� ��ȯ
        bool hasHInput = !Mathf.Approximately(h, 0f);
        bool hasVInput = !Mathf.Approximately(v, 0f);

        bool isWalking = hasHInput || hasVInput; // �ᱹ �÷��̾ �̵��� ���

        // �ִϸ��̼� ���
        anim.SetBool("isWalking", isWalking);

        // ����� ���
        if (isWalking)
        {
            if (!m_AudioSource.isPlaying)
            {
                m_AudioSource.Play();
            }
        }
        else
        {
            m_AudioSource.Stop();
        }

        // Vector3.RotateTowards(���� ȸ����, ��ǥ ȸ����, ������ ��ȭ��, ũ���� ��ȭ��)
        Vector3 desireForward = Vector3.RotateTowards(transform.forward, m_Movement, turnSpeed * Time.deltaTime, 0f);
        m_Rotation = Quaternion.LookRotation(desireForward);

    }

    private void OnAnimatorMove()
    {
        // ������ٵ��� �������� �ִϸ������� �����ǰ� �Ȱ��� ������
        m_rb.MovePosition(m_rb.position + (m_Movement * anim.deltaPosition.magnitude));
        // ���� ������
        m_rb.MoveRotation(m_Rotation);
    }
}
