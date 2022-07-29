using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Voxel : MonoBehaviour
{
    // 1) ������ ���ư� �ӵ� ����
    public float speed = 5f;

    void Start()
    {
        // 2) ���� ������ ã�´�.
        // Random.insideUnitSphere : ������ 3���� ��ǥ �������� ����
        Vector3 direction = Random.insideUnitSphere;
        // 3) ���� ������ ���ư��� �ӵ��� �ش�.
        GetComponent<Rigidbody>().velocity = direction * speed;
        Destroy(gameObject, 3f);
    }

    void Update()
    {
    }
}
