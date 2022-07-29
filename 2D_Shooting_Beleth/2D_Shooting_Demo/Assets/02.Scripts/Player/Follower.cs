using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Follower : MonoBehaviour
{
    // �Ѿ� �߻� ������
    public float maxShotDelay;
    public float currShotDelay;

    // �ȷο� �̵��� ���� ����
    public Vector3 followPos;
    // ����� �� �� �Ÿ�
    public int followDelay;
    // �θ������Ʈ ������
    public Transform parentObj;
    public Queue<Vector3> parentPos;

    private void Awake()
    {
        // ť �ڷ��� ����
        // ť : ���Լ���
        parentPos = new Queue<Vector3>();
    }

    void Update()
    {
        Watch();
        Follow();
        Fire();
        Reload();
    }

    void Watch()
    {
        // ť �ڷ� ��ǲ
        // Contains : ����Ʈ�� ���� �ڷᰡ ������ true !�ٿ��� �� ���� �ߺ� X
        if (!parentPos.Contains(parentObj.position))
        {
            // �÷��̾� ������Ʈ�� �������� ���� ������ �Է�
            parentPos.Enqueue(parentObj.position);
        }

        // ����� �÷��̾��� ������ ���� �迭�� followDelay ���� Ŀ���� ���
        if (parentPos.Count > followDelay)
        {
            // ť �ڷ� �ƿ�ǲ
            // �÷��̾� ������Ʈ�� �����ǰ����� ������� �ȴ�.
            followPos = parentPos.Dequeue();
        }
        // ť�� ä������ ������ �÷��̾������ǰ� �����ϰ� �����̵���
        else if (parentPos.Count < followDelay)
        {
            followPos = parentObj.position;
        }
    }

    private void Follow()
    {
        transform.position = followPos;
    }

    void Fire()
    {
        // Fire1 ��ư ������ ���� ���� ��
        if (!Input.GetButton("Jump"))
        {
            // ����!
            return;
        }
        // �����̽ð��� ������ �ʾ��� ���
        if (currShotDelay < maxShotDelay)
        {
            return; // ����
        }
        
        // ������Ʈ Ǯ ����
        // ������ �� �����
        GameObject bullet = ObjManager.Instance.MakeObj("BulletFollower");
        bullet.transform.position = transform.position;
        bullet.transform.rotation = transform.rotation;

        // ������ ���� (������Ʈ, ������ġ, ������ ȸ����)
        //GameObject bullet = Instantiate(bulletObjA, transform.position, transform.rotation);

        // ������ ������Ʈ�� ������ٵ� ������Ʈ �ҷ�����
        Rigidbody2D rigid = bullet.GetComponent<Rigidbody2D>();
        // �ҷ��� ������ٵ� ���������� AddForce
        rigid.AddForce(Vector2.up * 10, ForceMode2D.Impulse);

        // �߻� �� �� ������ �ʱ�ȭ
        currShotDelay = 0;
    }

    void Reload()
    {
        // Time.deltaTime ��ŭ ������ ����
        currShotDelay += Time.deltaTime;
    }


}
