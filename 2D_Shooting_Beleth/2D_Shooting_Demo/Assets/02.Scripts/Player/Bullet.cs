using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    // 총알 데미지
    public int dmg;

    // 회전 총알용 불체크 변수
    public bool isRotate;

    private void Update()
    {
        if (isRotate)
        {
            transform.Rotate(Vector3.forward * 10);
        }
    }

    // 콜라이더 통과 시
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 충돌체가 BORDERBULLET 태그를 가지고 있을 때
        if (collision.CompareTag("BORDERBULLET"))
        {
            // 현재 오브젝트 파괴
            //Destroy(gameObject);

            // 오브젝트 풀링 사용시
            // 현재 오브젝트 비활성화
            gameObject.SetActive(false);
        }
    }
}
