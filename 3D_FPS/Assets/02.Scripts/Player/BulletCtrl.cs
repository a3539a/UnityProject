using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletCtrl : MonoBehaviour
{
    // �Ѿ��� ������
    public float damage = 20f;
    // �Ѿ��� �߻� ��(�ӵ�)
    public float force = 1500f;

    Rigidbody rb;
    Transform tr;
    TrailRenderer trailRenderer;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        tr = rb.GetComponent<Transform>();
        trailRenderer = GetComponent<TrailRenderer>();
    }

    private void OnEnable()
    {
        // �Ѿ��� ���� �������� ��(force)�� ���Ѵ�.
        rb.AddForce(transform.forward * force);
    }

    private void OnDisable()
    {
        trailRenderer.Clear();
        tr.position = Vector3.zero;
        tr.rotation = Quaternion.identity;
        rb.Sleep();
    }

    void Update()
    {
        
    }
}
