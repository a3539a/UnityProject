using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing         ;

public class Teleport : MonoBehaviour
{
    // 텔레포트를 표시할 UI
    public Transform teleportCircleUI;
    // 선을 그릴 라인 렌더러
    LineRenderer lr;

    Vector3 originScale = Vector3.one * 0.02f;

    [Header("포스트 프로세싱 관련")]
    public bool isWarp = false;
    public float warpTime = 0.1f;
    public PostProcessVolume post;

    void Start()
    {
        // 시작 할 때 비활성화
        teleportCircleUI.gameObject.SetActive(false);
        lr = GetComponent<LineRenderer>();
    }

    void Update()
    {
        // 왼쪽 컨트롤러 버튼 One 누를 때
        if (ARAVRInput.GetDown(ARAVRInput.Button.One, ARAVRInput.Controller.LTouch))
        {
            // 라인렌더러 활성화
            lr.enabled = true;
        }
        // 왼쪽 컨트롤러 버튼 One 땠을 때
        else if(ARAVRInput.GetUp(ARAVRInput.Button.One, ARAVRInput.Controller.LTouch))
        {
            // 라인렌더러 비활성화
            lr.enabled = false;

            if (teleportCircleUI.gameObject.activeSelf)
            {

                if (!isWarp)
                {
                    GetComponent<CharacterController>().enabled = false;
                    // 텔레포트 UI 위치로 포지션값 변경
                    transform.position = teleportCircleUI.position + Vector3.up;
                    GetComponent<CharacterController>().enabled = true;
                }
                else
                {
                    // 워프 기능을 사용 할때는 Warp 코루틴 호출
                    StartCoroutine(Warp());
                }
            }

            // UI 끄기
            teleportCircleUI.gameObject.SetActive(false);
        }
        // 왼쪽 컨트롤러 버튼 One 누르고 있으면
        else if (ARAVRInput.Get(ARAVRInput.Button.One, ARAVRInput.Controller.LTouch))
        {
            // 왼쪽 컨트롤러를 기준으로 Ray를 만든다.
            Ray ray = new Ray(ARAVRInput.LHandPosition, ARAVRInput.LHandDirection);
            RaycastHit hitInfo;

            // TERRAIN 레이어 검출
            int layer = 1 << LayerMask.NameToLayer("TERRAIN");

            if (Physics.Raycast(ray, out hitInfo, 200, layer))
            {
                // 부딪힌 지점에 텔레포트 UI 표시
                // ray 가 부딪힌 지점에 라인그리기
                lr.SetPosition(0, ray.origin);
                lr.SetPosition(1, hitInfo.point);
                
                // ray가 부딪힌 지점에 텔레포트UI 표시
                teleportCircleUI.gameObject.SetActive(true);
                teleportCircleUI.position = hitInfo.point;

                // 텔레포트 UI가 위로 누워 있도록 방향 설정
                teleportCircleUI.forward = hitInfo.normal;

                // 텔레포트 UI의 크기가 거리에 따라 보정되게
                teleportCircleUI.localScale = originScale * Mathf.Max(1, hitInfo.distance);
            }
            else
            {
                // Ray 충돌이 없으면 선만 그려지도록 처리
                lr.SetPosition(0, ray.origin);
                lr.SetPosition(1, ray.origin + ARAVRInput.LHandDirection * 200);

                // 텔레포트 UI는 화면에서 없앰
                teleportCircleUI.gameObject.SetActive(true);
            }
        }
    }

    IEnumerator Warp()
    {
        // 워프 느낌을 표현할 모션 블러
        MotionBlur blur;
        // 워프 시작점 기억
        Vector3 pos = transform.position;
        // 목적지
        Vector3 targetPos = teleportCircleUI.position + Vector3.up;
        // 워프경과시간
        float currTime = 0;
        // 포스트 프로세싱에서 사용 중인 프로파일에서 모션블러 얻어 오기
        post.profile.TryGetSettings<MotionBlur>(out blur);
        // 워프 시작 전 블러 켜기
        blur.active = true;
        GetComponent<CharacterController>().enabled = false;

        while(currTime < warpTime)
        {
            // 경과 시간 흐르게 하기
            currTime += Time.deltaTime;
            // 워프의 시작점에서 도착점에 도착하기 위해 워프시간 동안 이동
            transform.position = Vector3.Lerp(pos, targetPos, currTime / warpTime);
            // 코루틴 대기
            yield return null;
        }

        // 텔레포트 UI 위치로 순간 이동
        transform.position = teleportCircleUI.position + Vector3.up;
        // 캐릭터 컨트롤러 다시 켜기
        GetComponent<CharacterController>().enabled = true;
        // 포스트 효과 끄기
        blur.active = false;
    }
}
