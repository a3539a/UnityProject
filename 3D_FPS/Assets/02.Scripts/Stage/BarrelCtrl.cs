using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarrelCtrl : MonoBehaviour
{
    // ����ȿ�� ��ƼŬ �Ҵ��� ����
    public GameObject expEffect;

    Transform tr;
    Rigidbody rb;
    public Mesh[] meshs; // ��׷��� ����� ������ ����
    public Texture[] textures; // ���� �ؽ��� ����

    MeshFilter meshFilter; // ����� �����ϴ� ������Ʈ
    MeshRenderer meshRenderer; // ��Ƽ������ �����ϴ� ������Ʈ

    // �Ѿ� ���� Ƚ��
    int hitCount = 0;
    public float radius = 10f; // ���� ����

    Shake shake;

    void Start()
    {
        tr = GetComponent<Transform>();
        rb = GetComponent<Rigidbody>();
        shake = GameObject.Find("CameraRig").GetComponent<Shake>();

        meshRenderer = GetComponent<MeshRenderer>();
        meshFilter = GetComponent<MeshFilter>();

        int idx = Random.Range(0, textures.Length);
        meshRenderer.material.mainTexture = textures[idx];
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("BULLET"))
        {
            // �Ѿ� ���� Ƚ����  ���� ��Ű�� 12ȸ �̻��̸� ����ó��
            hitCount++;
            if (hitCount == 12)
            {
                ExpBarrel();
            }
        }
    }

    void ExpBarrel()
    {
        // ����ȿ�� ��ƼŬ ����, �巳�� ��ġ����
        GameObject exp = Instantiate(expEffect, tr.position, Quaternion.identity);
        // ����ȿ���� 0.5�ʵ� ����
        Destroy(exp, 4f);

        // rigidbody ������Ʈ�� mass�� 1.0���� ������ ���Ը� ������ ��
        //rb.mass = 1f;
        // ���� �ڱ�ġ�� ���� ����
        //rb.AddForce(Vector3.up * 700f);
        InDirectDamage(tr.position);

        // ����� �޽��� �߿��� ���Ƿ� �ϳ��� �����ϱ� ���Ͽ�
        // ������ �ε����� ����
        int idx = Random.Range(0, meshs.Length);
        // ����� �޽��� �ϳ��� �޽����Ϳ� ����
        meshFilter.sharedMesh = meshs[idx];

        StartCoroutine(shake.ShakeCamera(0.1f, 0.2f, 0.5f));

        // 3�� �� �巳�� ����
        Destroy(gameObject, 10f);
    }

    void InDirectDamage(Vector3 pos)
    {
        // Physics.OverlapSphere(������ġ, �ݰ�, ������� ���̾�)
        // �ݰ泻�� �ش�Ǵ� ���̾ �ִ� ��� �浹ü �����س�
        int layerIdx = LayerMask.GetMask("BARREL");
        Collider[] colls = Physics.OverlapSphere(pos, radius, layerIdx);

        foreach (Collider coll in colls)
        {
            // ���� ������ ���� �� �巳���� Rigidboy ������Ʈ ����
            rb = coll.GetComponent<Rigidbody>();
            // ����� �巳���� ������ٵ��� ���� 1��
            rb.mass = 1.0f;
            // freeze ����
            rb.constraints = RigidbodyConstraints.None;
            // ���߷��� ����
            // AddExplosionForce(��������, ���� ��ġ, �ݰ�, ��������), ���� ��������� ����
            rb.AddExplosionForce(1500f, pos, radius, 1200f);
        }
    }
}
