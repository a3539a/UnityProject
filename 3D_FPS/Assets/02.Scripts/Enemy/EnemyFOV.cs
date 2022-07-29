using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyFOV : MonoBehaviour
{
    public float viewRange = 15f; // ���� ���� �����Ÿ� ����

    [Range(0f, 360f)]
    public float viewAngle = 120f; // �þ߰�

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

        // ���̾ or �����Ͽ� ���� ���̾�� ����
        layerMask = 1 << playerLayer | 1 << obstacleLayer;
    }

    // �þ߰� �ȿ� �ִ� ���� ��ǥ(x, y, z)���� ����ϴ� �Լ�
    public Vector3 CiclePoint(float angle)
    {
        // ������ǥ�� ����ϱ� ���ؼ� �� ĳ������ yȸ���� ���� ����
        angle += transform.eulerAngles.y;
        return new Vector3(Mathf.Sin(angle * Mathf.Deg2Rad), 0, Mathf.Cos(angle * Mathf.Deg2Rad));
    }

    // �÷��̾ ������ �� �ִ��� �Ǵ��ϴ� �Լ�
    public bool IsTracePlayer()
    {
        bool isTrace = false;
        // �þ߹��� �ȿ� �÷��̾� �����ϴ��� üũ
        Collider[] colls = Physics.OverlapSphere(enemyTr.position, viewRange, 1 << playerLayer);

        // �þ߿� �÷��̾ ���� �Ѵٸ�
        if (colls.Length == 1)
        {
            // ���� �÷��̾ ���� ����
            Vector3 dir = (playerTr.position - enemyTr.position).normalized;
            // �� ĳ������ �þ߿� �����ϴ��� �Ǵ�
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

        RaycastHit hit; // ����ĳ��Ʈ �� �浹 ���� �����ϱ� ���� ����
        // RaycastHit2D �ʹ� �ٸ��� �̸� ������ �����صΰ� ����ؾ���

        Vector3 dir = (playerTr.position - enemyTr.position).normalized;

        if(Physics.Raycast(enemyTr.position, // ���� �߻� ��ġ
                           dir,  // �߻� ����
                           out hit, // �浹 ���� ��� ����
                           viewRange, // ���� ��Ÿ�
                           layerMask)) // ���� ���̾�
        {
            // ��ֹ��� �÷��̾ ���� �� �� ����.
            // ������ �� �߿��� �÷��̾��� ��쿡�� true �� ����
            isView = (hit.collider.CompareTag("PLAYER"));
        }
        return isView;
    }
}
