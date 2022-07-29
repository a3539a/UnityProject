using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletCtrl : MonoBehaviour
{
    // 총알의 데미지
    public float damage = 20f;
    // 총알의 발사 힘(속도)
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
        // 총알의 전진 방향으로 힘(force)를 가한다.
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
