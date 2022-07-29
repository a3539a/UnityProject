using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grab : MonoBehaviour
{
    // 필요 속성 : 물체를 잡고 있는지 여부, 잡고있는 물체, 잡을 물체의 종류, 잡을수 있는 거리
    // 불체크
    bool isGrabbing = false;
    // 잡은 물체
    GameObject grabbledObj;
    // 잡은 물체의 종류
    public LayerMask grabbledLayer;
    // 잡을 수 있는 거리
    public float grabRange = 0.2f;

    [Header("Obj Throw")]
    // 물체 던지기 변수
    // 이전 위치
    Vector3 prevPos;
    // 던질 힘
    public float throwPower = 10;
    // 이전 회전
    Quaternion prevRot;
    // 회전력
    public float rotPower = 5;

    [Header("원거리 물체 잡기")]
    // 원거리에서 물체를 잡는 기능 활성화 여부
    public bool isRemoteGrab = true;
    // 원거리에서 물체를 잡을 수 있는 거리
    public float remoteGrabDistance = 20;

    private void Update()
    {
        // 물체 잡기 
        // 1. 물체를 잡지 않고 있을 경우
        if (!isGrabbing)
        {
            // 잡기 시도
            TryGrab();
        }
        else
        {
            // 물체 놓기
            TryUnGrab();
        }
    }

    void TryGrab()
    {
        if (ARAVRInput.GetDown(ARAVRInput.Button.HandTrigger, ARAVRInput.Controller.RTouch))
        {
            // 원거리 그랩 활성화 되면
            if (isRemoteGrab)
            {
                Ray ray = new Ray(ARAVRInput.RHandPosition, ARAVRInput.RHandDirection);
                RaycastHit hitInfo;

                // SphereCast(레이, 반지름, 정보, 거리, 검출 레이어)
                if (Physics.SphereCast(ray, 0.5f, out hitInfo, remoteGrabDistance, grabbledLayer))
                {
                    // 잡은 상태
                    isGrabbing = true;
                    // 잡은놈 저장
                    grabbledObj = hitInfo.transform.gameObject;
                    // 빨려 들어오는 동작 구현
                    StartCoroutine(GrabbingAnimation());
                }
                return;
            }

            // 트리거를 당기는 순간 오른쪽 컨트롤러의 위치에서
            // 오버랩스피어를 통하여 그랩범위 만큼의 콜라이더 생성
            // 해당 범위 내에서 grabbledLayer에 해당되는 모든 오브젝트를 가지고옴
            Collider[] hitObj = Physics.OverlapSphere(ARAVRInput.RHandPosition, grabRange, grabbledLayer);

            // 가장 가까운 물체 인덱스
            int closest = 0;

            // 손과 가장 가까운 물체 선택
            for (int i = 0; i < hitObj.Length; i++)
            {
                // 손과 가장 가까운 물체와의 거리
                Vector3 closestPos = hitObj[closest].transform.position;
                float closestDistance = Vector3.Distance(closestPos, ARAVRInput.RHandPosition);

                // 다음 물체와 손의 거리
                Vector3 nextPos = hitObj[i].transform.position;
                float nextDistance = Vector3.Distance(nextPos, ARAVRInput.RHandPosition);

                // 다음 물체와의 거리가 더 가깝다면
                if (nextDistance < closestDistance)
                {
                    // 인덱스 교체
                    closest = i;
                }
            }

            // 검출된 물체가 있는 경우
            if (hitObj.Length > 0)
            {
                // 잡은 상태로 전환
                isGrabbing = true;
                // 잡은 물체에 대한 기억
                grabbledObj = hitObj[closest].gameObject;
                // 잡은 물체를 손의 자식으로 등록
                grabbledObj.transform.parent = ARAVRInput.RHand;
                // 잡은 오브젝트는 물리엔진 효과 받지 않도록 해줌
                grabbledObj.GetComponent<Rigidbody>().isKinematic = true;

                // 던지기 전 초기 위치 저장
                prevPos = ARAVRInput.RHandPosition;
                // 던지기 전 초기 회전 저장
                prevRot = ARAVRInput.RHand.rotation;
            }
        }
    }

    void TryUnGrab()
    {
        // 던질 방향
        Vector3 throwDir = ARAVRInput.RHandPosition - prevPos;
        // 위치 기억
        prevPos = ARAVRInput.RHandPosition;

        // 쿼터니언 공식
        // angle1 = Q1, angle2 = Q2
        // angle1 + angle2 = Q1 * Q2
        // -angle2 = Quaternion.Inverse(Q2)
        // angle2 - angle1 = Q2 * Quaternion.Inverse(Q1)
        // 회전방향 = current - previous 의 차로 구함 (- 는 Inverse 기억)
        Quaternion deltaRotation = ARAVRInput.RHand.rotation * Quaternion.Inverse(prevRot);
        // 이전 회전 저장
        prevRot = ARAVRInput.RHand.rotation;

        if (ARAVRInput.GetUp(ARAVRInput.Button.HandTrigger, ARAVRInput.Controller.RTouch))
        {
            // 잡지 않은 상태로 전환
            isGrabbing = false;
            // 물리 기능 활성화
            grabbledObj.GetComponent<Rigidbody>().isKinematic = false;
            // 자식 오브젝트 없애기
            grabbledObj.transform.parent = null;
            // 던지기
            grabbledObj.GetComponent<Rigidbody>().velocity = throwDir * throwPower;

            // 각 속도 = (1/dt) * d0(특정 축 기준 변위 각도)
            float angle;
            Vector3 axis;

            deltaRotation.ToAngleAxis(out angle, out axis);
            Vector3 anglurVelocity = (1.0f / Time.deltaTime) * angle * axis;
            grabbledObj.GetComponent<Rigidbody>().angularVelocity = anglurVelocity;

            // 잡은 물체가 없도록 설정
            grabbledObj = null;
        }
    }

    IEnumerator GrabbingAnimation()
    {
        // 물리 기능 정지
        grabbledObj.GetComponent<Rigidbody>().isKinematic = true;
        prevPos = ARAVRInput.RHandPosition; // 초기 위치
        prevRot = ARAVRInput.RHand.rotation; // 초기 회전

        Vector3 startLocation = grabbledObj.transform.position;
        Vector3 targetLocation = ARAVRInput.RHandPosition + ARAVRInput.RHandDirection * 0.1f;

        float currTime = 0;
        float finishTime = 0.2f;

        // 시간 경과율
        float elapsedRate = currTime / finishTime;

        while (elapsedRate < 1)
        {
            currTime += Time.deltaTime;
            elapsedRate = currTime / finishTime;
            grabbledObj.transform.position = Vector3.Lerp(startLocation, targetLocation, elapsedRate);

            yield return null;
        }

        // 잡은 물체를 손의 자식으로 등록
        grabbledObj.transform.position = targetLocation;
        grabbledObj.transform.parent = ARAVRInput.RHand;
    }

}
