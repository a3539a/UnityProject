using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCam : MonoBehaviour
{
    public Transform targetTr;
    Transform camTr; // ī�޶� �ڱ��ڽ��� Ʈ������

    // �ٷ� ���ٿ� ��ġ�� ������ ���� ���� ����
    // �ν����� â���� ���������� �����̵�� �����
    // ī�޶�� ������ �Ÿ�
    [Range(2f, 20f)]
    public float distance = 10f;
    
    // y�� ����
    [Range(0f, 10f)]
    public float height = 2f;

    [Range(0f, 2f)] // ���� Ű�� 2���ͷ� ����
    public float targetOffset = 1f;

    public float damping = 0.3f; // �����ӵ�

    Vector3 velocity = Vector3.zero;

    [Header("�� ������ ��")]
    public float heightAboveWall = 7f; // ���� ����
    public float colliderRadius = 1.8f; // �浹ü ������
    public float overDamping = 5f; // ���� �ӵ�
    float originHeight; // ������ ����� �� ������ ��ġ

    [Header("�ڽ�(��Ÿ) ������ ��")]
    public float heightAboveObstacle = 12f;
    public float castOffset = 1f;

    void Start()
    {
        camTr = GetComponent<Transform>();
        originHeight = height;
    }

    private void Update()
    {
        // �ݶ��̴��� colliderRadius �ݰ� ���� �����Ǹ�
        if (Physics.CheckSphere(camTr.position, colliderRadius))
        {
            // �������� �Լ��� Ȱ���Ͽ� ī�޶��� ���̸� �ε巴�� ���
            // ������̹Ƿ� SLerp �� �ƴ϶� Lerp ���°͵� ����
            height = Mathf.Lerp(height, heightAboveWall, Time.deltaTime * overDamping);
        }
        else // �浹ü�� ������ ���� ���̷� ����
        {
            height = Mathf.Lerp(height, originHeight, Time.deltaTime * overDamping);
        }

        // �÷��̾ ��ֹ��� ���������� �Ǵ��ϱ� ���� ���̰� ������ ��ġ
        Vector3 castTarget = targetTr.position + (targetTr.up * castOffset);
        // ������ ���� ���� ����
        Vector3 castDir = (castTarget - camTr.position).normalized;

        RaycastHit hit;

        if (Physics.Raycast(camTr.position, castDir, out hit, Mathf.Infinity))
        {
            // ���̰� �ε������� �÷��̾ �ƴ϶�� ��, ��ֹ��̶��
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
        // �����ؾ� �� ����� �������� distance ��ŭ �̵�
        // ���̸� height ��ŭ �̵�
        Vector3 pos = targetTr.position + (-targetTr.forward * distance) + (Vector3.up * height);

        // ī�޶� �̵��� Slerp �Լ��� �ε巴�� ����
        // Slerp(�����ġ, ������, �ҿ�ð�)
        // ����� ���� ������������ �����ϱ����� �ҿ�ð�
        //camTr.position = Vector3.Slerp(camTr.position, pos, damping * Time.deltaTime);

        camTr.position = Vector3.SmoothDamp(camTr.position, // ���
            pos, // ������
            ref velocity, // ���� �ӵ�
            damping); // ���޽ð�

        // Camera �� �ǹ���ǥ�� ���� �̵�
        // Camera �� �ǹ���ġ�� ���ߵ���
        camTr.LookAt(targetTr.position + (targetTr.up * targetOffset));
    }

}
