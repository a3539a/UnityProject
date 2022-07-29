using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RemoveBullet : MonoBehaviour
{
    public GameObject sparkEffect;

    // 콜라이더 충돌시 실행
    private void OnCollisionEnter(Collision collision)
    {
        // 충돌한 게임오브젝트의 태그값 비교
        if (collision.collider.CompareTag("BULLET"))
        {
            // 충돌한 지점의 최초 지점(포인트)를 가져옴
            ContactPoint cp = collision.GetContact(0);
            Quaternion rot = Quaternion.LookRotation(-cp.normal);

            // 스파크 이펙트 파티클을 충돌 위치에서 동적 생성
            // 충돌 지점과, 법선벡터쪽으로 스파크가 튀도록 변경
            GameObject spark = Instantiate(sparkEffect, cp.point, rot);

            // 게임오브젝트 삭제
            // Destroy(collision.gameObject);

            Destroy(spark, 0.3f);

            // 오브젝트 풀의 비활성화
            collision.gameObject.SetActive(false);

        }
    }
}
