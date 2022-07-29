using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarrelCtrl : MonoBehaviour
{
    // 폭발효과 파티클 할당할 변수
    public GameObject expEffect;

    Transform tr;
    Rigidbody rb;
    public Mesh[] meshs; // 찌그러진 모양을 저장할 변수
    public Texture[] textures; // 각종 텍스쳐 저장

    MeshFilter meshFilter; // 모양을 관리하는 컴포넌트
    MeshRenderer meshRenderer; // 메티리얼을 관리하는 컴포넌트

    // 총알 맞은 횟수
    int hitCount = 0;
    public float radius = 10f; // 폭발 범위

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
            // 총알 맞은 횟수를  증가 시키고 12회 이상이면 폭발처리
            hitCount++;
            if (hitCount == 12)
            {
                ExpBarrel();
            }
        }
    }

    void ExpBarrel()
    {
        // 폭발효과 파티클 생성, 드럼통 위치에서
        GameObject exp = Instantiate(expEffect, tr.position, Quaternion.identity);
        // 폭발효과를 0.5초뒤 제거
        Destroy(exp, 4f);

        // rigidbody 컴포넌트의 mass를 1.0으로 수정해 무게를 가볍게 함
        //rb.mass = 1f;
        // 위로 솟구치는 힘을 가함
        //rb.AddForce(Vector3.up * 700f);
        InDirectDamage(tr.position);

        // 등록한 메쉬들 중에서 임의로 하나를 추출하기 위하여
        // 랜덤한 인덱스값 추출
        int idx = Random.Range(0, meshs.Length);
        // 등록한 메쉬중 하나를 메쉬필터에 설정
        meshFilter.sharedMesh = meshs[idx];

        StartCoroutine(shake.ShakeCamera(0.1f, 0.2f, 0.5f));

        // 3초 뒤 드럼통 제거
        Destroy(gameObject, 10f);
    }

    void InDirectDamage(Vector3 pos)
    {
        // Physics.OverlapSphere(시작위치, 반경, 영향받을 레이어)
        // 반경내에 해당되는 레이어에 있는 모든 충돌체 검출해냄
        int layerIdx = LayerMask.GetMask("BARREL");
        Collider[] colls = Physics.OverlapSphere(pos, radius, layerIdx);

        foreach (Collider coll in colls)
        {
            // 폭발 버뮈에 포함 된 드럼통의 Rigidboy 컴포넌트 검출
            rb = coll.GetComponent<Rigidbody>();
            // 검출된 드럼통의 리지드바디의 무게 1로
            rb.mass = 1.0f;
            // freeze 해제
            rb.constraints = RigidbodyConstraints.None;
            // 폭발력을 전달
            // AddExplosionForce(가로폭발, 폭발 위치, 반경, 세로폭발), 힘이 정비례하지 않음
            rb.AddExplosionForce(1500f, pos, radius, 1200f);
        }
    }
}
