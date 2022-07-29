using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    // 적상태를 나타내기 위한 열거형(enum) 변수 정의
    public enum State
    {
        PATROL, TRACE, ATTACK, DIE
    }

    public State state = State.PATROL; // 초기상태 지정

    Transform playerTr;
    Transform enemyTr;

    public float attackDist = 5f; // 공격 사정거리
    public float traceDist = 8f; // 추적 사정거리
    public bool isDie = false; // 적의 사망 유무 판단 변수

    WaitForSeconds ws; // 코루틴에서 사용할 지연시간 변수

    MoveAgent moveAgent; // 적의 이동을 제어하는 MoveAgent 스크립트 가져오기
    EnemyFire enemyFire;
    EnemyFOV enemyFOV;

    Animator anim;

    // 애니메이터 컨트롤러의 파라미터를 해시화 하여
    // 성능 개성하는 부분
    readonly int hashMove = Animator.StringToHash("IsWalk");
    readonly int hashSpeed = Animator.StringToHash("Speed");
    
    readonly int hashDie = Animator.StringToHash("Die");
    readonly int hashDieIdx = Animator.StringToHash("DieIdx");

    // 애니메이션 다양성을 위하여 멀티플라이어 적용 파라미터
    readonly int hashOffset = Animator.StringToHash("Offset");
    readonly int hashWalkSpeed = Animator.StringToHash("WalkSpeed");

    readonly int hashPlayerDie = Animator.StringToHash("PlayerDie");

    void Awake()
    {
        var player = GameObject.FindGameObjectWithTag("PLAYER");
        if (player != null)
        {
            playerTr = player.GetComponent<Transform>();
        }
        enemyTr = GetComponent<Transform>();
        moveAgent = GetComponent<MoveAgent>();
        enemyFire = GetComponent<EnemyFire>();
        enemyFOV = GetComponent<EnemyFOV>();
        anim = GetComponent<Animator>();

        // WaitForSeconds(지연시간) 이므로 자유롭게 설정하면 됨
        ws = new WaitForSeconds(0.3f);

        // Cycle Offset 값 랜덤하게 0 ~ 1
        anim.SetFloat(hashOffset, Random.Range(0f, 1f));
        // Speed 값 랜덤하게 1 ~ 1.4
        anim.SetFloat(hashWalkSpeed, Random.Range(1f, 1.4f));
    }

    private void OnEnable()
    {
        StartCoroutine(CheckState());
        StartCoroutine(Action());

        // Damage 스크립트의 이벤트 함수에다가
        // 해당 스크립트의 OnPlayerDie 함수를 연결
        // 이 때에 +=는 더하기가 아니라 이벤트에 함수 연결을 의미
        // 연결해제는 -= 를 사용
        Damage.OnPlayerDieEvent += this.OnPlayerDie;
    }

    private void OnDisable()
    {
        Damage.OnPlayerDieEvent -= this.OnPlayerDie;
    }

    IEnumerator CheckState()
    {
        yield return new WaitForSeconds(1f);

        while (!isDie)
        {
            if (state == State.DIE)
            {
                yield break;
            }
            // Distance 함수로 플레이어와 적의 거리를 계산
            float dist = Vector3.Distance(playerTr.position, enemyTr.position);
            // 위 아래 동일한 거리 계산 코드이며 Distance 보다 성능은 미묘하게 좋음
            //float dist = (playerTr.position - enemyTr.position).sqrMagnitude;

            if(dist <= attackDist)
            {
                // 공격 사거리라도 플레이어가 보이는지 체크
                if (enemyFOV.IsViewPlayer())
                {
                    state = State.ATTACK;
                }
                else
                {
                    state = State.TRACE;
                }
            }
            // 추적 반경 및 시야각 안인지 체크
            else if (enemyFOV.IsTracePlayer())
            {
                state = State.TRACE;
            }
            else
            {
                state = State.PATROL;
            }

            // 0.3 초마다 적의 상태를 체크하도록 구현
            yield return ws;
        }
    }

    IEnumerator Action()
    {
        while (!isDie)
        {
            yield return ws;

            switch (state)
            {
                case State.PATROL:
                    enemyFire.isFire = false;
                    anim.SetBool(hashMove, true);
                    moveAgent.patrolling = true;
                    break;
                case State.ATTACK:
                    anim.SetBool(hashMove, false);
                    moveAgent.Stop();

                    if(enemyFire.isFire == false)
                        enemyFire.isFire = true;

                    break;
                case State.TRACE:
                    enemyFire.isFire = false;
                    anim.SetBool(hashMove, true);
                    moveAgent.traceTarget = playerTr.position;
                    break;
                case State.DIE:
                    // 적 사망후 태그를 변경해주어야
                    // 게임 매니저에서 수량 파악할 때 제외됨
                    this.gameObject.tag = "Untagged";

                    isDie = true;
                    enemyFire.isFire = false;
                    moveAgent.Stop();
                    anim.SetTrigger(hashDie);
                    // 3가지 Die 애니메이션 중 하나를 선택하기 위해서
                    // DieIdx 파라미터에 0 ~ 2 값 중 랜덤한 값을 설정
                    anim.SetInteger(hashDieIdx, Random.Range(0, 3));
                    // 적이 죽고나서 콜라이더 비활성화하여 불필요한 충돌 방지
                    GetComponent<CapsuleCollider>().enabled = false;

                    break;
            }
        }
    }

    void Update()
    {
        // MoveAgent 에서 만들었던 speed 프로퍼티의 값을 전달 해줌
        // 이동 속도 변화를 '실시간'으로 체크 하여 애니메이터의 블렌드 트리 제어
        anim.SetFloat(hashSpeed, moveAgent.speed);
    }

    public void OnPlayerDie()
    {
        moveAgent.Stop(); // 내비게이션 기능 비활성화
        enemyFire.isFire = false;
        StopAllCoroutines(); // 모든 코루틴 함수 정지

        anim.SetTrigger(hashPlayerDie);
    }
}
