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
        // 해당 오브젝트의 컴포넌트 속성을 사용하기 위해 호출
        tr = GetComponent<Transform>();
        anim = GetComponent<Animation>();

        // 애니메션이 보유하고있는 클립 중
        // 해당 이름의 애니메이션 재생
        anim.Play("Idle");
    }

    private void Update()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        float rx = Input.GetAxis("Mouse X"); // 마우스의 X좌표 받아오기

        // 전후 좌우 이동 방향 벡터 계산
        // 어차피 1인데 왜 곱하냐?
        Vector3 moveDir = (Vector3.forward * v) + (Vector3.right * h);

        // Translate(방향 * 속력 * Time.deltatime)
        // normalized를 통해서 벡터의 정규화
        // Time.deltaTime 을 이용해서 프레임 균등화
        tr.Translate(moveDir.normalized * moveSpeed * Time.deltaTime);

        // 마우스 움직임 따라 회전
        tr.Rotate(Vector3.up * rx * turnSpeed * Time.deltaTime);

        PlayerAnim(h, v);
    }

    void PlayerAnim(float _h, float _v)
    {
        if (_v >= 0.1f) // 앞
        {
            // CrossFade("애니메이션 이름", 엔드타임 : 전환시간);
            // 애니메이션이 자연스럽게 Fade 재생 되도록 하는 내장함수
            anim.CrossFade("RunF", 0.25f);
        }
        else if (_v <= -0.1f) // 뒤
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
