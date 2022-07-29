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

    DroneState state = DroneState.IDLE; // �ʱ� ���� ����

    // private ���� �ν����Ϳ� ����
    [SerializeField]
    int hp = 3; // ����� ü��

    [Header("��� ���� ����")]
    public float idleDelayTime = 2f; // ��� ������ ���� �ð�
    float currTime; // ��� �ð�

    [Header("�̵� ���� ����")]
    public float moveSpeed = 1f; // �̵� �ӵ�
    Transform tower; // Ÿ�� ��ġ
    NavMeshAgent agent; // �� ã�⸦ ������ �׺�޽�������Ʈ

    [Header("���� ���� ����")]
    public float attackRange = 3f; // ���� ����
    public float attackDelayTime = 2f; // ���� ������

    [Header("���� ���� ����")]
    // ���� ȿ��
    Transform explosion;
    ParticleSystem expEffect;
    AudioSource expAudio;

    void Start()
    {
        tower = GameObject.Find("Tower").transform; // ���̾��Ű�� Ÿ�� ������Ʈ ã��
        agent = GetComponent<NavMeshAgent>(); // NavMeshAgent ������Ʈ ��������
        agent.enabled = false; // ������
        agent.speed = moveSpeed; // �ӵ� ����

        explosion = GameObject.Find("Explosion").transform; // ���̾��Ű�� Explosion ������Ʈ ã��
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

    // �����ð� ���� ��ٷȴٰ� ���¸� �������� ��ȯ
    void Idle()
    {
        // 1) �ð��� �귯���Ѵ�.
        currTime += Time.deltaTime;
        // 2) ���� ��� �ð��� ���ð��� �ʰ��ߴٸ�
        if (currTime > idleDelayTime)
        {
            // 3) ���¸� �̵����� ��ȯ
            state = DroneState.MOVE;
            agent.enabled = true; // agent Ȱ��ȭ
        }
    }
     // Ÿ���� ���� �̵�
    void Move()
    {
        // �׺���̼��� ������ ����
        agent.SetDestination(tower.position);

        // ���� ���� �ȿ� ������ ���ݻ��·� ��ȯ
        if(Vector3.Distance(transform.position, tower.position) < attackRange)
        {
            state = DroneState.ATTACK;
            agent.enabled = false; // agent�� ���� ����
        }
    }

    void Attack()
    {
        // 1) �ð��� �帥��
        currTime += Time.deltaTime ;
        // 2) ��� �ð��� ���� ���� �ð��� �ʰ� �ϸ�
        if(currTime > attackDelayTime)
        {
            // 3) ���� -> Tower�� HP ��ȣ���� ������ ó��
            Tower.instance.HP--;
            // 4) ��� �ð� �ʱ�ȭ
            currTime = 0;
        }
    }

    IEnumerator Damage()
    {
        // 1) ��ã�� ����
        agent.enabled = false ;
        // 2) �ڽ� ��ü�� MeshRenderer ���� ���� ��� ����
        Material mat = GetComponentInChildren<MeshRenderer>().material;
        // 3) ���� ���� ����
        Color originColor = mat.color;
        // 4) ������ �� ����
        mat.color = Color.red;
        // 5) 0.1�� ��ٸ���
        yield return new WaitForSeconds(0.1f);
        // 6) ������ ���� �������
        mat.color = originColor;
        // 7) ���¸� idle�� ��ȯ
        state = DroneState.IDLE;
        // 8) ��� �ð� �ʱ�ȭ
        currTime = 0;

    }

    void Die()
    {

    }

    public void OnDamageProcess()
    {
        // ü���� ���ҽ�Ű�� ���� �ʾҴٸ� ���¸� �������� ��ȯ ������
        // 1) ü�� ����
        hp--;
        // 2) ���� ���� �ʾҴٸ�
        if (hp <= 0)
        {
            // ����ȿ���� ��ġ ����
            explosion.position = transform.position;
            // ����Ʈ ���
            expEffect.Play();
            // ����Ʈ ����
            expAudio.Play();
            // ��� ���ֱ�
            gameObject.GetComponent<CapsuleCollider>().enabled = false;
            Destroy(gameObject, 0.5f);

            return;
        }
        else
        {
            // 3) ���¸� �������� ��ȯ
            state = DroneState.DAMAGE;
            // �ڷ�ƾ ȣ��
            StopAllCoroutines();
            StartCoroutine(Damage());
        }
    }
}
