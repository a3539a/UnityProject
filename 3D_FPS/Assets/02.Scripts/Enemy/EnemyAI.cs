using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    // �����¸� ��Ÿ���� ���� ������(enum) ���� ����
    public enum State
    {
        PATROL, TRACE, ATTACK, DIE
    }

    public State state = State.PATROL; // �ʱ���� ����

    Transform playerTr;
    Transform enemyTr;

    public float attackDist = 5f; // ���� �����Ÿ�
    public float traceDist = 8f; // ���� �����Ÿ�
    public bool isDie = false; // ���� ��� ���� �Ǵ� ����

    WaitForSeconds ws; // �ڷ�ƾ���� ����� �����ð� ����

    MoveAgent moveAgent; // ���� �̵��� �����ϴ� MoveAgent ��ũ��Ʈ ��������
    EnemyFire enemyFire;
    EnemyFOV enemyFOV;

    Animator anim;

    // �ִϸ����� ��Ʈ�ѷ��� �Ķ���͸� �ؽ�ȭ �Ͽ�
    // ���� �����ϴ� �κ�
    readonly int hashMove = Animator.StringToHash("IsWalk");
    readonly int hashSpeed = Animator.StringToHash("Speed");
    
    readonly int hashDie = Animator.StringToHash("Die");
    readonly int hashDieIdx = Animator.StringToHash("DieIdx");

    // �ִϸ��̼� �پ缺�� ���Ͽ� ��Ƽ�ö��̾� ���� �Ķ����
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

        // WaitForSeconds(�����ð�) �̹Ƿ� �����Ӱ� �����ϸ� ��
        ws = new WaitForSeconds(0.3f);

        // Cycle Offset �� �����ϰ� 0 ~ 1
        anim.SetFloat(hashOffset, Random.Range(0f, 1f));
        // Speed �� �����ϰ� 1 ~ 1.4
        anim.SetFloat(hashWalkSpeed, Random.Range(1f, 1.4f));
    }

    private void OnEnable()
    {
        StartCoroutine(CheckState());
        StartCoroutine(Action());

        // Damage ��ũ��Ʈ�� �̺�Ʈ �Լ����ٰ�
        // �ش� ��ũ��Ʈ�� OnPlayerDie �Լ��� ����
        // �� ���� +=�� ���ϱⰡ �ƴ϶� �̺�Ʈ�� �Լ� ������ �ǹ�
        // ���������� -= �� ���
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
            // Distance �Լ��� �÷��̾�� ���� �Ÿ��� ���
            float dist = Vector3.Distance(playerTr.position, enemyTr.position);
            // �� �Ʒ� ������ �Ÿ� ��� �ڵ��̸� Distance ���� ������ �̹��ϰ� ����
            //float dist = (playerTr.position - enemyTr.position).sqrMagnitude;

            if(dist <= attackDist)
            {
                // ���� ��Ÿ��� �÷��̾ ���̴��� üũ
                if (enemyFOV.IsViewPlayer())
                {
                    state = State.ATTACK;
                }
                else
                {
                    state = State.TRACE;
                }
            }
            // ���� �ݰ� �� �þ߰� ������ üũ
            else if (enemyFOV.IsTracePlayer())
            {
                state = State.TRACE;
            }
            else
            {
                state = State.PATROL;
            }

            // 0.3 �ʸ��� ���� ���¸� üũ�ϵ��� ����
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
                    // �� ����� �±׸� �������־��
                    // ���� �Ŵ������� ���� �ľ��� �� ���ܵ�
                    this.gameObject.tag = "Untagged";

                    isDie = true;
                    enemyFire.isFire = false;
                    moveAgent.Stop();
                    anim.SetTrigger(hashDie);
                    // 3���� Die �ִϸ��̼� �� �ϳ��� �����ϱ� ���ؼ�
                    // DieIdx �Ķ���Ϳ� 0 ~ 2 �� �� ������ ���� ����
                    anim.SetInteger(hashDieIdx, Random.Range(0, 3));
                    // ���� �װ��� �ݶ��̴� ��Ȱ��ȭ�Ͽ� ���ʿ��� �浹 ����
                    GetComponent<CapsuleCollider>().enabled = false;

                    break;
            }
        }
    }

    void Update()
    {
        // MoveAgent ���� ������� speed ������Ƽ�� ���� ���� ����
        // �̵� �ӵ� ��ȭ�� '�ǽð�'���� üũ �Ͽ� �ִϸ������� ���� Ʈ�� ����
        anim.SetFloat(hashSpeed, moveAgent.speed);
    }

    public void OnPlayerDie()
    {
        moveAgent.Stop(); // ������̼� ��� ��Ȱ��ȭ
        enemyFire.isFire = false;
        StopAllCoroutines(); // ��� �ڷ�ƾ �Լ� ����

        anim.SetTrigger(hashPlayerDie);
    }
}
