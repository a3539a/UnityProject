using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyFOV : MonoBehaviour
{
    public float viewRange = 15f; // 적의 추적 사정거리 범위

    [Range(0f, 360f)]
    public float viewAngle = 120f; // 시야각

    Transform enemyTr;
    Transform playerTr;
    int playerLayer;
    int obstacleLayer;
    int layerMask;

    private void Start()
    {
        enemyTr = GetComponent<Transform>();
        playerTr = GameObject.FindGameObjectWithTag("PLAYER").transform;

        playerLayer = LayerMask.NameToLayer("PLAYER");
        obstacleLayer = LayerMask.NameToLayer("OBSTACLE");

        // 레이어를 or 연산하여 다중 레이어로 병합
        layerMask = 1 << playerLayer | 1 << obstacleLayer;
    }

    // 시야각 안에 있는 점의 좌표(x, y, z)값을 계산하는 함수
    public Vector3 CiclePoint(float angle)
    {
        // 로컬좌표로 사용하기 위해서 적 캐릭터의 y회전축 값을 더함
        angle += transform.eulerAngles.y;
        return new Vector3(Mathf.Sin(angle * Mathf.Deg2Rad), 0, Mathf.Cos(angle * Mathf.Deg2Rad));
    }

    // 플레이어를 추적할 수 있는지 판단하는 함수
    public bool IsTracePlayer()
    {
        bool isTrace = false;
        // 시야범위 안에 플레이어 존재하는지 체크
        Collider[] colls = Physics.OverlapSphere(enemyTr.position, viewRange, 1 << playerLayer);

        // 시야에 플레이어가 존재 한다면
        if (colls.Length == 1)
        {
            // 적이 플레이어를 보는 방향
            Vector3 dir = (playerTr.position - enemyTr.position).normalized;
            // 적 캐릭터의 시야에 존재하는지 판단
            if (Vector3.Angle(enemyTr.forward, dir) < viewAngle * 0.5f)
            {
                isTrace = true;
            }
        }
        return isTrace;
    }

    public bool IsViewPlayer()
    {
        bool isView = false;

        RaycastHit hit; // 레이캐스트 시 충돌 정보 저장하기 위한 변수
        // RaycastHit2D 와는 다르게 미리 변수를 선언해두고 사용해야함

        Vector3 dir = (playerTr.position - enemyTr.position).normalized;

        if(Physics.Raycast(enemyTr.position, // 레이 발사 위치
                           dir,  // 발사 방향
                           out hit, // 충돌 정보 출력 변수
                           viewRange, // 레이 사거리
                           layerMask)) // 감지 레이어
        {
            // 장애물과 플레이어가 감지 될 수 있음.
            // 떄문에 그 중에서 플레이어일 경우에만 true 값 전달
            isView = (hit.collider.CompareTag("PLAYER"));
        }
        return isView;
    }
}
