using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class DroneAI : MonoBehaviour
{
    enum DroneState
    {
        IDLE, 
        MOVE, 
        ATTACK, 
        DAMAGE, 
        DIE
    }

    DroneState state = DroneState.IDLE; // 초기 상태 설정

    // private 지만 인스펙터에 노출
    [SerializeField]
    int hp = 3; // 드론의 체력

    [Header("대기 상태 관련")]
    public float idleDelayTime = 2f; // 대기 상태의 지속 시간
    float currTime; // 경과 시간

    [Header("이동 상태 관련")]
    public float moveSpeed = 1f; // 이동 속도
    Transform tower; // 타워 위치
    NavMeshAgent agent; // 길 찾기를 수행할 네브메쉬에이전트

    [Header("공격 상태 관련")]
    public float attackRange = 3f; // 공격 범위
    public float attackDelayTime = 2f; // 공격 딜레이

    [Header("죽음 상태 관련")]
    // 폭발 효과
    Transform explosion;
    ParticleSystem expEffect;
    AudioSource expAudio;

    void Start()
    {
        tower = GameObject.Find("Tower").transform; // 하이어라키에 타워 오브젝트 찾기
        agent = GetComponent<NavMeshAgent>(); // NavMeshAgent 컴포넌트 가져오기
        agent.enabled = false; // 꺼놓기
        agent.speed = moveSpeed; // 속도 설정

        explosion = GameObject.Find("Explosion").transform; // 하이어라키에 Explosion 오브젝트 찾기
        expEffect = explosion.GetComponent<ParticleSystem>();
        expAudio = explosion.GetComponent<AudioSource>();

    }

    void Update()
    {
        switch (state)
        {
            case DroneState.IDLE:
                Idle();
                break;
            case DroneState.MOVE:
                Move();
                break;
            case DroneState.ATTACK:
                Attack();
                break;
            case DroneState.DAMAGE:
                //Damage();
                break;
            case DroneState.DIE:
                Die();
                break;
        }

        Debug.Log("currState : " + state);
    }

    // 일정시간 동안 기다렸다가 상태를 공격으로 전환
    void Idle()
    {
        // 1) 시간이 흘러야한다.
        currTime += Time.deltaTime;
        // 2) 만약 경과 시간이 대기시간을 초과했다면
        if (currTime > idleDelayTime)
        {
            // 3) 상태를 이동으로 전환
            state = DroneState.MOVE;
            agent.enabled = true; // agent 활성화
        }
    }
     // 타워를 향해 이동
    void Move()
    {
        // 네비게이션할 목적지 설정
        agent.SetDestination(tower.position);

        // 공격 범위 안에 들어오면 공격상태로 전환
        if(Vector3.Distance(transform.position, tower.position) < attackRange)
        {
            state = DroneState.ATTACK;
            agent.enabled = false; // agent의 동작 정지
        }
    }

    void Attack()
    {
        // 1) 시간이 흐른다
        currTime += Time.deltaTime ;
        // 2) 경과 시간이 공격 지연 시간을 초과 하면
        if(currTime > attackDelayTime)
        {
            // 3) 공격 -> Tower의 HP 를호출해 데미지 처리
            Tower.instance.HP--;
            // 4) 경과 시간 초기화
            currTime = 0;
        }
    }

    IEnumerator Damage()
    {
        // 1) 길찾기 정지
        agent.enabled = false ;
        // 2) 자식 객체의 MeshRenderer 에서 재질 얻어 오기
        Material mat = GetComponentInChildren<MeshRenderer>().material;
        // 3) 원래 색을 저장
        Color originColor = mat.color;
        // 4) 재질의 색 변경
        mat.color = Color.red;
        // 5) 0.1초 기다리기
        yield return new WaitForSeconds(0.1f);
        // 6) 재질의 색을 원래대로
        mat.color = originColor;
        // 7) 상태를 idle로 전환
        state = DroneState.IDLE;
        // 8) 경과 시간 초기화
        currTime = 0;

    }

    void Die()
    {

    }

    public void OnDamageProcess()
    {
        // 체력을 감소시키고 죽지 않았다면 상태를 데미지로 전환 해주자
        // 1) 체력 감소
        hp--;
        // 2) 만약 죽지 않았다면
        if (hp <= 0)
        {
            // 폭발효과의 위치 지정
            explosion.position = transform.position;
            // 이펙트 재생
            expEffect.Play();
            // 이펙트 사운드
            expAudio.Play();
            // 드론 없애기
            gameObject.GetComponent<CapsuleCollider>().enabled = false;
            Destroy(gameObject, 0.5f);

            return;
        }
        else
        {
            // 3) 상태를 데미지로 전환
            state = DroneState.DAMAGE;
            // 코루틴 호출
            StopAllCoroutines();
            StartCoroutine(Damage());
        }
    }
}
