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

    // 플레이어 방향회전 속도
    public float turnSpeed;

    void Awake()
    {
        anim = GetComponent<Animator>();
        m_rb = GetComponent<Rigidbody>();
        m_AudioSource = GetComponent<AudioSource>();
    }

    void FixedUpdate()
    {
        // 플레이어 움직임
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        m_Movement.Set(h, 0f, v);
        m_Movement.Normalize();

        // 방향 구하기 Mathf.Approximately(h, 0f) -> h의 값과 0이 비슷하면 참 아니면 거짓 반환
        bool hasHInput = !Mathf.Approximately(h, 0f);
        bool hasVInput = !Mathf.Approximately(v, 0f);

        bool isWalking = hasHInput || hasVInput; // 결국 플레이어가 이동할 경우

        // 애니메이션 출력
        anim.SetBool("isWalking", isWalking);

        // 오디오 출력
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

        // Vector3.RotateTowards(현재 회전값, 목표 회전값, 각도의 변화량, 크기의 변화량)
        Vector3 desireForward = Vector3.RotateTowards(transform.forward, m_Movement, turnSpeed * Time.deltaTime, 0f);
        m_Rotation = Quaternion.LookRotation(desireForward);

    }

    private void OnAnimatorMove()
    {
        // 리지드바디의 포지션을 애니메이터의 포지션과 똑같이 맞춰춤
        m_rb.MovePosition(m_rb.position + (m_Movement * anim.deltaPosition.magnitude));
        // 방향 맞춰줌
        m_rb.MoveRotation(m_Rotation);
    }
}
