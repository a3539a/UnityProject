using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCam : MonoBehaviour
{
    public Transform targetTr;
    Transform camTr; // 카메라 자기자신의 트랜스폼

    // 바로 밑줄에 위치한 변수의 조절 범위 제한
    // 인스펙터 창에서 조절가능한 슬라이드로 변경됨
    // 카메라와 대상과의 거리
    [Range(2f, 20f)]
    public float distance = 10f;
    
    // y축 높이
    [Range(0f, 10f)]
    public float height = 2f;

    [Range(0f, 2f)] // 모델의 키를 2미터로 상정
    public float targetOffset = 1f;

    public float damping = 0.3f; // 반응속도

    Vector3 velocity = Vector3.zero;

    [Header("벽 만났을 때")]
    public float heightAboveWall = 7f; // 조정 높이
    public float colliderRadius = 1.8f; // 충돌체 반지름
    public float overDamping = 5f; // 반응 속도
    float originHeight; // 벽에서 벗어났을 때 원복할 위치

    [Header("박스(기타) 만났을 때")]
    public float heightAboveObstacle = 12f;
    public float castOffset = 1f;

    void Start()
    {
        camTr = GetComponent<Transform>();
        originHeight = height;
    }

    private void Update()
    {
        // 콜라이더가 colliderRadius 반경 내에 감지되면
        if (Physics.CheckSphere(camTr.position, colliderRadius))
        {
            // 선형보간 함수를 활용하여 카메라의 높이를 부드럽게 상승
            // 직선운동이므로 SLerp 가 아니라 Lerp 쓰는것도 무방
            height = Mathf.Lerp(height, heightAboveWall, Time.deltaTime * overDamping);
        }
        else // 충돌체가 없으면 원래 높이로 복구
        {
            height = Mathf.Lerp(height, originHeight, Time.deltaTime * overDamping);
        }

        // 플레이어가 장애물에 가려졌는지 판단하기 위한 레이가 조준할 위치
        Vector3 castTarget = targetTr.position + (targetTr.up * castOffset);
        // 레이의 조준 방향 설정
        Vector3 castDir = (castTarget - camTr.position).normalized;

        RaycastHit hit;

        if (Physics.Raycast(camTr.position, castDir, out hit, Mathf.Infinity))
        {
            // 레이가 부딪힌것이 플레이어가 아니라면 즉, 장애물이라면
            if (!hit.collider.CompareTag("PLAYER"))
            {
                height = Mathf.Lerp(height, heightAboveObstacle, Time.deltaTime * overDamping);
            }
            else
            {
                height = Mathf.Lerp(height, originHeight, Time.deltaTime * overDamping);
            }
        }
    }

    void LateUpdate()
    {
        // 추적해야 할 대상의 뒤쪽으로 distance 만큼 이동
        // 높이를 height 만큼 이동
        Vector3 pos = targetTr.position + (-targetTr.forward * distance) + (Vector3.up * height);

        // 카메라 이동을 Slerp 함수로 부드럽게 변경
        // Slerp(출발위치, 목적지, 소요시간)
        // 출발지 부터 목적지까지로 도착하기위한 소요시간
        //camTr.position = Vector3.Slerp(camTr.position, pos, damping * Time.deltaTime);

        camTr.position = Vector3.SmoothDamp(camTr.position, // 출발
            pos, // 도착지
            ref velocity, // 현재 속도
            damping); // 도달시간

        // Camera 를 피벗좌표를 향해 이동
        // Camera 가 피벗위치를 비추도록
        camTr.LookAt(targetTr.position + (targetTr.up * targetOffset));
    }

}
