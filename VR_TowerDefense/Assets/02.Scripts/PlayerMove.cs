using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    // 이동속도 
    public float speed;
    // CharacterController 컴포넌트
    CharacterController cc;

    [Header("낙하관련")]
    // 중력 가속도의 크기
    public float gravity = -9.8f;
    float yVelocity = 0;

    [Header("점프관련")]
    // 점프 파워
    public float jumpPower = 5f;

    void Start()
    {
        cc = GetComponent<CharacterController>();
    }

    void Update()
    {
        // 사용자의 입력을 받는다.
        float h = ARAVRInput.GetAxis("Horizontal");
        float v = ARAVRInput.GetAxis("Vertical");

        // 입력받은 방향을 만든다.
        Vector3 dir = new Vector3(h, 0, v);

        // 카메라가 바라보는 방향으로 입력값 변환
        dir = Camera.main.transform.TransformDirection(dir);

        // 중력을 적용한 수직방향 추가 v = v0 + at;
        yVelocity += gravity * Time.deltaTime;
        dir.y = yVelocity;

        // 땅에 있을 때는 중력속도를 0으로 초기화
        if (cc.isGrounded)
        {
            yVelocity = 0;
        }

        // 사용자가 오른쪽 컨트롤러 Two 버튼 누르면 y축 속도에 jumpPower를 할당
        if (ARAVRInput.GetDown(ARAVRInput.Button.Two, ARAVRInput.Controller.RTouch))
        {
            yVelocity = jumpPower;
        }

        // 이동한다
        // Move(방향 * 속도)
        cc.Move(dir * speed * Time.deltaTime);
    }
}
