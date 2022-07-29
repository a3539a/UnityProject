using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Voxel : MonoBehaviour
{
    // 1) 복셀이 날아갈 속도 변수
    public float speed = 5f;

    void Start()
    {
        // 2) 랜덤 방향을 찾는다.
        // Random.insideUnitSphere : 구형의 3차원 좌표 랜덤으로 생성
        Vector3 direction = Random.insideUnitSphere;
        // 3) 랜덤 방향을 날아가는 속도를 준다.
        GetComponent<Rigidbody>().velocity = direction * speed;
        Destroy(gameObject, 3f);
    }

    void Update()
    {
    }
}
