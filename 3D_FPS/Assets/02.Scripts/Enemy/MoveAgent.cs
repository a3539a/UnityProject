using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

// 해당 속성은 typeof에 지정된 컴포넌트가 반드시 필요하며
// 없을 경우 동작하지 않으며 에러 표시해줌
[RequireComponent(typeof(NavMeshAgent))]
public class MoveAgent : MonoBehaviour
{
    // 순찰지점을 저장하기 위한 LIst
    public List<Transform> wayPoints;
    public int nextIdx; // 다음 순찰지점의 인덱스

    NavMeshAgent agent;
    Transform enemyTr;

    // 프로퍼티 작성
    readonly float patrolSpeed = 1.5f;
    readonly float traceSpeed = 4.0f;
    float damping = 1f;


    bool _patrolling; // 순찰 여부 판단 함수
    public bool patrolling
    {
        get { return _patrolling; } // 값을 가져다 사용할 때
        set  // 값을 가져와서 저장할 때
        { 
            // 외부에서 전달된 값은 value를 통하여 내부변수에 저장됨
            _patrolling = value;
            if (_patrolling)
            {
                agent.speed = patrolSpeed;
                damping = 1f;
                MoveWayPoint();
            }
        }
    }

    Vector3 _traceTarget;
    public Vector3 traceTarget
    {
        get { return _traceTarget; }
        set 
        { 
            _traceTarget = value; 
            agent.speed = traceSpeed;
            damping = 7f;
            // 대상 추적 함수 호출
            TraceTarget(_traceTarget);
        }
    }

    public float speed
    {
        get { return agent.velocity.magnitude; }
    }

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        enemyTr = GetComponent<Transform>();
        // 목적지에 다가갈수록 감속하는 옵션
        agent.autoBraking = false; // 자동 브레이크 비활성화
        agent.updateRotation = false; // 자동 회전 기능 비활성화
        agent.speed = patrolSpeed;

        // var : 가변 자료형
        // 뒤에 대입되는(저장하는) 값에 따라 형태가 바뀜 (메타몽!)
        var group = GameObject.Find("WayPointGroup");
        if (group != null)
        {
            // WayPointGroup 오브젝트의 자식 중
            // Transform 컴포넌트를 지닌 오브젝트 전부 가져오기
            group.GetComponentsInChildren<Transform>(wayPoints);
            // 0번째 인덱스 요소 삭제(부모 오브젝트)
            wayPoints.RemoveAt(0);
        }

        // 다음 순찰지점을 0 ~ (순찰지점의 갯수 - 1) 중 랜덤한 곳 지정
        nextIdx = Random.Range(0, wayPoints.Count);

        // 순찰지점 탐색하는 함수 호출
        //MoveWayPoint();
        patrolling = true;
    }

    void MoveWayPoint()
    {
        // isPathStale 메소드는 AI가 경로 계산중인 상태를 말함
        // 계산 중일 때는 true, 아닐때는 false
        if (agent.isPathStale)
        {
            return;
        }

        // 경로 계산이 끝나고 나면 다음 목적지를 지정
        agent.destination = wayPoints[nextIdx].position;
        // 브레이크 해제해서 이동 시작
        agent.isStopped = false;
    }

    void TraceTarget(Vector3 pos) // 플레이어 추적에 사용되는 함수
    {
        if (agent.isPathStale)
        {
            return;
        }

        agent.destination = pos; // 추적 대상 설정
        agent.isStopped = false;
    }

    public void Stop()
    {
        agent.isStopped = true;
        agent.velocity = Vector3.zero; // 바로 멈추기 위해
        // 순찰모드 정지
        _patrolling = false;
    }

    void Update()
    {
        if (!agent.isStopped) // 적이 이동중이라면
        {
            // 진행항 방향 벡터를 쿼터니언 각으로 변환
            // 즉, 목적지 방향을 유니티에서 사용하는 쿼터니언으로 변환
            Quaternion rot = Quaternion.LookRotation(agent.desiredVelocity);

            // Slerp 구면 선형보간은 3번째 파라미터인 시간값이 커질수록 예민하게 반응
            enemyTr.rotation = Quaternion.Slerp(enemyTr.rotation, rot, Time.deltaTime * damping);
        }

        // 순찰모드가 아닐 경우에는 아래에 있는
        // 목적지 탐색하는 로직 수행 안함
        if (!_patrolling)
        {
            return;
        }

        // 속도가 0.04 이상이면서, 남은 거리가 0.5 이하인 경우
        // 즉, 이동중인데 남은거리가 얼마 안남았으면
        // 다음 목적지로 변경하고자 함
        if (agent.velocity.sqrMagnitude >= (0.2f * 0.2f) && agent.remainingDistance <= 0.5f)
        {
            //nextIdx++;
            //nextIdx = nextIdx % wayPoints.Count;

            nextIdx = Random.Range(0, wayPoints.Count);

            // 다음 목적지 설정 끝난 후 이동 함수 실행
            MoveWayPoint();
        }
    }
}
