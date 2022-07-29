using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

// �ش� �Ӽ��� typeof�� ������ ������Ʈ�� �ݵ�� �ʿ��ϸ�
// ���� ��� �������� ������ ���� ǥ������
[RequireComponent(typeof(NavMeshAgent))]
public class MoveAgent : MonoBehaviour
{
    // ���������� �����ϱ� ���� LIst
    public List<Transform> wayPoints;
    public int nextIdx; // ���� ���������� �ε���

    NavMeshAgent agent;
    Transform enemyTr;

    // ������Ƽ �ۼ�
    readonly float patrolSpeed = 1.5f;
    readonly float traceSpeed = 4.0f;
    float damping = 1f;


    bool _patrolling; // ���� ���� �Ǵ� �Լ�
    public bool patrolling
    {
        get { return _patrolling; } // ���� ������ ����� ��
        set  // ���� �����ͼ� ������ ��
        { 
            // �ܺο��� ���޵� ���� value�� ���Ͽ� ���κ����� �����
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
            // ��� ���� �Լ� ȣ��
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
        // �������� �ٰ������� �����ϴ� �ɼ�
        agent.autoBraking = false; // �ڵ� �극��ũ ��Ȱ��ȭ
        agent.updateRotation = false; // �ڵ� ȸ�� ��� ��Ȱ��ȭ
        agent.speed = patrolSpeed;

        // var : ���� �ڷ���
        // �ڿ� ���ԵǴ�(�����ϴ�) ���� ���� ���°� �ٲ� (��Ÿ��!)
        var group = GameObject.Find("WayPointGroup");
        if (group != null)
        {
            // WayPointGroup ������Ʈ�� �ڽ� ��
            // Transform ������Ʈ�� ���� ������Ʈ ���� ��������
            group.GetComponentsInChildren<Transform>(wayPoints);
            // 0��° �ε��� ��� ����(�θ� ������Ʈ)
            wayPoints.RemoveAt(0);
        }

        // ���� ���������� 0 ~ (���������� ���� - 1) �� ������ �� ����
        nextIdx = Random.Range(0, wayPoints.Count);

        // �������� Ž���ϴ� �Լ� ȣ��
        //MoveWayPoint();
        patrolling = true;
    }

    void MoveWayPoint()
    {
        // isPathStale �޼ҵ�� AI�� ��� ������� ���¸� ����
        // ��� ���� ���� true, �ƴҶ��� false
        if (agent.isPathStale)
        {
            return;
        }

        // ��� ����� ������ ���� ���� �������� ����
        agent.destination = wayPoints[nextIdx].position;
        // �극��ũ �����ؼ� �̵� ����
        agent.isStopped = false;
    }

    void TraceTarget(Vector3 pos) // �÷��̾� ������ ���Ǵ� �Լ�
    {
        if (agent.isPathStale)
        {
            return;
        }

        agent.destination = pos; // ���� ��� ����
        agent.isStopped = false;
    }

    public void Stop()
    {
        agent.isStopped = true;
        agent.velocity = Vector3.zero; // �ٷ� ���߱� ����
        // ������� ����
        _patrolling = false;
    }

    void Update()
    {
        if (!agent.isStopped) // ���� �̵����̶��
        {
            // ������ ���� ���͸� ���ʹϾ� ������ ��ȯ
            // ��, ������ ������ ����Ƽ���� ����ϴ� ���ʹϾ����� ��ȯ
            Quaternion rot = Quaternion.LookRotation(agent.desiredVelocity);

            // Slerp ���� ���������� 3��° �Ķ������ �ð����� Ŀ������ �����ϰ� ����
            enemyTr.rotation = Quaternion.Slerp(enemyTr.rotation, rot, Time.deltaTime * damping);
        }

        // ������尡 �ƴ� ��쿡�� �Ʒ��� �ִ�
        // ������ Ž���ϴ� ���� ���� ����
        if (!_patrolling)
        {
            return;
        }

        // �ӵ��� 0.04 �̻��̸鼭, ���� �Ÿ��� 0.5 ������ ���
        // ��, �̵����ε� �����Ÿ��� �� �ȳ�������
        // ���� �������� �����ϰ��� ��
        if (agent.velocity.sqrMagnitude >= (0.2f * 0.2f) && agent.remainingDistance <= 0.5f)
        {
            //nextIdx++;
            //nextIdx = nextIdx % wayPoints.Count;

            nextIdx = Random.Range(0, wayPoints.Count);

            // ���� ������ ���� ���� �� �̵� �Լ� ����
            MoveWayPoint();
        }
    }
}
