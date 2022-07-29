using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class GazePointerCtrl : MonoBehaviour
{
    public Video360Play vp360;

    public Transform uiCanvas; // 일정시간동안 시선이 머무는 것을 보여주기 위한 UI
    public Image gazeImg; // VideoPlayer를 제어하기 위한 네임스페이스

    Vector3 defaultScale; // UI 기본스케일을 저장해두기 위한 값
    public float uiScaleVal = 1f; // UI 카메라가 1m일 때 배율

    bool isHitObj; // 시선(레이)가 닿았는지
    GameObject prevHitObj; // 이전 시선이 머물렀던 오브젝트
    GameObject currHitObj; // 현재 시선이 머무는 오브젝트
    float currGazeTime = 0;
    public float gazeChargeTime = 3f;


    void Start()
    {
        defaultScale = uiCanvas.localScale; // 오브젝트가 갖는 기본 스케일 값
        currGazeTime = 0;
    }

    void Update()
    {
        // 캔버스 오브젝트의 스케일을 거리에 따라 조절한다.
        // 1) 카메라를 기준으로 전방 방향의 좌표를 구한다.
        Vector3 dir = transform.TransformPoint(Vector3.forward);
        // 2) 카메라를 기준으로 전방의 레이를 설정한다.
        Ray ray = new Ray(transform.position, dir);

        RaycastHit hitInfo; // 히트된 오브젝트의 정보를 담는다.

        // 3) 레이에 부딪힌 경우에는 거리값을 이용해 uiCanvas의 크기를 조절한다.
        if(Physics.Raycast(ray, out hitInfo))
        {
            uiCanvas.localScale = defaultScale * uiScaleVal * hitInfo.distance;
            uiCanvas.position = transform.forward * hitInfo.distance;

            if (hitInfo.transform.CompareTag("GazeObj"))
            {
                isHitObj = true;
            }
            currHitObj = hitInfo.transform.gameObject;
        }
        // 4) 아무것도 부딪히지 않으면 기본 스케일 값으로 uiCanvas의 크기를 조절한다.
        else
        {
            uiCanvas.localScale = defaultScale * uiScaleVal;
            uiCanvas.position = transform.position + dir;
        }
        // 5) uiCanvas가 항상 카메라 오브젝트를 바라보게 한다.
        uiCanvas.forward = transform.forward * -1;

        if (isHitObj)
        {
            if (currHitObj == prevHitObj)
            {
                currGazeTime += Time.deltaTime;
            }
            else
            {
                prevHitObj = currHitObj;
            }

            // hit 된 오브젝트가 VideoPlayer 컴포넌트를 가졌는지 확인
            // CheckVideoFrame() 에서 확인 후 재생
            HitObjChecker(currHitObj, true);
        }
        else // 시선이 벗어났거나 GazeObj가 아니라면 시간을 초기화
        {
            if (prevHitObj != null)
            {
                HitObjChecker(prevHitObj, false);
                prevHitObj = null;
            }
            currGazeTime = 0;
        }

        // Mathf.Clamp(범위를 정하고자 하는 값, 최소값, 최대값)
        currGazeTime = Mathf.Clamp(currGazeTime, 0, gazeChargeTime);
        gazeImg.fillAmount = currGazeTime / gazeChargeTime;

        isHitObj = false;
        currHitObj = null;
    }

    // 히트 된 오브젝트 타입별로 작동방식을 구분해준다.
    void HitObjChecker(GameObject hitObj, bool isActive)
    {
        // 충돌난 오브젝트에 Video Player 컴포넌트가 존재하는지 판단
        if (hitObj.GetComponent<VideoPlayer>())
        {
            if (isActive)
            {
                // CheckVideoFrame 함수에 true 매개변수 삽입
                hitObj.GetComponent<VideoFrame>().CheckVideoFrame(true);
            }
            else
            {
                hitObj.GetComponent<VideoFrame>().CheckVideoFrame(false);
            }
        }

        if (currGazeTime / gazeChargeTime >= 1)
        {
            // Contains : 해당 문자열이 포함 되어 있으면 true
            if (hitObj.name.Contains("Right"))
            {
                vp360.SwapVideoClip(true); // 다음 영상
            }
            else if (hitObj.name.Contains("Left"))
            {
                vp360.SwapVideoClip(false); // 이전 영상
            }
            else
            {
                // GetSiblingIndex : 자식 오브젝트의 하이어라키 순위를 얻어옴
                vp360.SetVideoPlay(hitObj.transform.GetSiblingIndex());
            }
            currGazeTime = 0;
        }
    }
}
