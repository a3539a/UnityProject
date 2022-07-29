using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Follower : MonoBehaviour
{
    // 총알 발사 딜레이
    public float maxShotDelay;
    public float currShotDelay;

    // 팔로워 이동에 관한 변수
    public Vector3 followPos;
    // 따라올 때 의 거리
    public int followDelay;
    // 부모오브젝트 포지션
    public Transform parentObj;
    public Queue<Vector3> parentPos;

    private void Awake()
    {
        // 큐 자료형 선언
        // 큐 : 선입선출
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
        // 큐 자료 인풋
        // Contains : 리스트에 같은 자료가 있으면 true !붙여서 안 넣음 중복 X
        if (!parentPos.Contains(parentObj.position))
        {
            // 플레이어 오브젝트의 포지션이 변할 때마다 입력
            parentPos.Enqueue(parentObj.position);
        }

        // 등록한 플레이어의 포지션 값의 배열이 followDelay 보다 커졌을 경우
        if (parentPos.Count > followDelay)
        {
            // 큐 자료 아웃풋
            // 플레이어 오브젝트의 포지션값들을 따라오게 된다.
            followPos = parentPos.Dequeue();
        }
        // 큐가 채워지기 전에는 플레이어포지션과 동일하게 움직이도록
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
        // Fire1 버튼 누르고 있지 않을 때
        if (!Input.GetButton("Jump"))
        {
            // 정지!
            return;
        }
        // 딜레이시간이 지나지 않았을 경우
        if (currShotDelay < maxShotDelay)
        {
            return; // 정지
        }
        
        // 오브젝트 풀 사용시
        // 포지션 값 잡아줌
        GameObject bullet = ObjManager.Instance.MakeObj("BulletFollower");
        bullet.transform.position = transform.position;
        bullet.transform.rotation = transform.rotation;

        // 프리팹 생성 (오브젝트, 생성위치, 생성시 회전값)
        //GameObject bullet = Instantiate(bulletObjA, transform.position, transform.rotation);

        // 생성된 오브젝트의 리지드바디 컴포넌트 불러오기
        Rigidbody2D rigid = bullet.GetComponent<Rigidbody2D>();
        // 불러온 리지드바디에 위방향으로 AddForce
        rigid.AddForce(Vector2.up * 10, ForceMode2D.Impulse);

        // 발사 한 후 딜레이 초기화
        currShotDelay = 0;
    }

    void Reload()
    {
        // Time.deltaTime 만큼 딜레이 충전
        currShotDelay += Time.deltaTime;
    }


}
