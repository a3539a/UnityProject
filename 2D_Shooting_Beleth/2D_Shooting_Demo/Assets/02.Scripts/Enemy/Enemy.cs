using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    // 적 오브젝트 이름 기입
    public string enemyName;
    // 적 오브젝트 점수
    public int enemyScore;
    // 적 속력
    public float speed;
    // 적 체력
    public int health;
    // 피격 이미지 삽입
    public Sprite[] sprites;

    // 총알 오브젝트 변수
    public GameObject bulletObjA;
    public GameObject bulletObjB;
    // 총알 발사 딜레이
    public float maxShotDelay;
    public float currShotDelay;

    // 아이템 오브젝트 변수
    public GameObject itemPower;
    public GameObject itemCoin;
    public GameObject itemBoom;

    // 프리팹화 된상태에서는 씬에 사용중인 오브젝트를 삽입불가
    // 게임매니저에서 플레이어 오브젝트 받아올거임
    public GameObject player;

    // 필요한 컴포넌트
    // 피격판정에 필요
    SpriteRenderer spriteRenderer;

    // 보스 피격 애니메이션용
    Animator anim;
    readonly int hashOnHit = Animator.StringToHash("OnHit");

    // 보스 패턴용 함수
    // 패턴 가지수
    public int patternIdx;
    // 현재 패턴 반복체크용
    public int currPatternCount;
    // 몇번 반복할지
    public int[] maxPatternCount;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        if(enemyName == "B")
        {
            anim = GetComponent<Animator>();
        }
    }

    // 오브젝트가 활성화 될때마다 실행
    private void OnEnable()
    {
        switch (enemyName)
        {
            case "B":
                health = 10000;
                // 2초 뒤에 정지
                Invoke("Stop", 3f);
                break;
            case "L":
                health = 55;
                break;
            case "M":
                health = 15;
                break;
            case "S":
                health = 1;
                break;
        }
    }

    private void Stop()
    {
        // OnEnable 함수에서 2번 실행될 우려가 있어서
        // 활성화 되고나서만 실행되게 해줌
        if (!gameObject.activeSelf)
        {
            return;
        }
        Rigidbody2D rigid = GetComponent<Rigidbody2D>();
        rigid.velocity = Vector2.zero;

        // 3초 뒤 보스패턴 시작
        Invoke("BossPattern", 3f);
    }

    void BossPattern()
    {
        // 패턴 4가지 쓸거임
        // 인덱스가 3 일 때 0으로 초기화 해주고 아니면 다음거
        patternIdx = patternIdx == 3 ? 0 : patternIdx + 1;
        // 현재패턴 횟수 초기화
        currPatternCount = 0;

        switch (patternIdx)
        {
            case 0:
                BossFireFoward();
                break;
            case 1:
                BossFireShot();
                break;
            case 2:
                BossFireArc();
                break;
            case 3:
                BossFireAround();
                break;
        }
    }

    void BossFireFoward()
    {
        // Fire 4 Bullet Forward
        // 오브젝트 풀 사용시
        GameObject bulletRR = ObjManager.Instance.MakeObj("BulletBossB");
        GameObject bulletR = ObjManager.Instance.MakeObj("BulletBossB");
        GameObject bulletL = ObjManager.Instance.MakeObj("BulletBossB");
        GameObject bulletLL = ObjManager.Instance.MakeObj("BulletBossB");
        bulletRR.transform.position = transform.position + Vector3.right * 0.8f;
        bulletR.transform.position = transform.position + Vector3.right * 0.5f;
        bulletL.transform.position = transform.position + Vector3.right * -0.5f;
        bulletLL.transform.position = transform.position + Vector3.right * -0.8f;

        // 생성된 오브젝트의 리지드바디 컴포넌트 불러오기
        Rigidbody2D rigidRR = bulletRR.GetComponent<Rigidbody2D>();
        Rigidbody2D rigidR = bulletR.GetComponent<Rigidbody2D>();
        Rigidbody2D rigidL = bulletL.GetComponent<Rigidbody2D>();
        Rigidbody2D rigidLL = bulletLL.GetComponent<Rigidbody2D>();

        rigidRR.AddForce(Vector2.down * 6f, ForceMode2D.Impulse);
        rigidR.AddForce(Vector2.down * 6f, ForceMode2D.Impulse);
        rigidL.AddForce(Vector2.down * 6f, ForceMode2D.Impulse);
        rigidLL.AddForce(Vector2.down * 6f, ForceMode2D.Impulse);

        // 현재패턴 실행 회수 증가
        currPatternCount++;

        // 현재 패턴 횟수가 최대 패턴 반복 횟수보다 적으면
        if (currPatternCount < maxPatternCount[patternIdx])
        {
            // 자기자신 다시 실행
            Invoke("BossFireFoward", 2f);
        }
        else // 높으면
        {
            // 패턴 바꾸기 위해 보스패턴 함수 불러옴
            Invoke("BossPattern", 3f);
        }
    }

    void BossFireShot()
    {
        // Shot Gun 5 Bullet
        // 5번 쏘기
        for(int i = 0; i < 5; i++)
        {
            // 오브젝트 풀 사용시
            GameObject bullet = ObjManager.Instance.MakeObj("BulletEnemyB");
            bullet.transform.position = transform.position;
            bullet.transform.rotation = transform.rotation;

            // 생성된 오브젝트의 리지드바디 컴포넌트 불러오기
            Rigidbody2D rigid = bullet.GetComponent<Rigidbody2D>();

            // 목표위치 - 현재위치 = 목표방향 * 크기
            // normalized 로 방향만 남긴다
            Vector2 dir = player.transform.position - transform.position;
            Vector2 ranVec = new Vector2(Random.Range(-0.5f, 0.5f), Random.Range(0f, 2f));

            dir += ranVec;

            rigid.AddForce(dir.normalized * 4f, ForceMode2D.Impulse);
        }

        currPatternCount++;

        if (currPatternCount < maxPatternCount[patternIdx])
        {
            Invoke("BossFireShot", 2.5f);
        }
        else
        {
            Invoke("BossPattern", 3f);
        }
    }

    void BossFireArc()
    {
        // Fire Arc Continue Fire
        // 오브젝트 풀 사용시
        GameObject bullet = ObjManager.Instance.MakeObj("BulletEnemyA");
        bullet.transform.position = transform.position;
        bullet.transform.rotation = Quaternion.identity;

        // 생성된 오브젝트의 리지드바디 컴포넌트 불러오기
        Rigidbody2D rigid = bullet.GetComponent<Rigidbody2D>();

        // Mathf.Sin 으로 투사체의 x축 방향이 파형으로 바뀌게 해준다.
        // Mathf.PI 파이값 * 10 투사체 간격? 2파이는 1원이니까 * currPatternCount / maxPatternCount[patternIdx] 99발 쏠꺼니까 for문 말고 인덱스번호 재활용
        Vector2 dir = new Vector2(Mathf.Sin(Mathf.PI * 10 * currPatternCount / maxPatternCount[patternIdx]), -1);

        rigid.AddForce(dir.normalized * 6f, ForceMode2D.Impulse);
        

        currPatternCount++;

        if (currPatternCount < maxPatternCount[patternIdx])
        {
            Invoke("BossFireArc", 0.15f);
        }
        else
        {
            Invoke("BossPattern", 3f);
        }
    }

    void BossFireAround()
    {
        // Fire Around
        int roundNumA = 50;
        int roundNumB = 40;
        // 현재 패턴 횟수에 따라 roundNumA 나 roundNumB 로 총알 갯수 나가게
        int roundNum = currPatternCount % 2 == 0 ? roundNumA : roundNumB;

        for (int i = 0; i < roundNum; i++)
        {
            // 오브젝트 풀 사용시
            GameObject bullet = ObjManager.Instance.MakeObj("BulletBossA");
            bullet.transform.position = transform.position;
            bullet.transform.rotation = Quaternion.identity;

            // 생성된 오브젝트의 리지드바디 컴포넌트 불러오기
            Rigidbody2D rigid = bullet.GetComponent<Rigidbody2D>();

            // 원형방향 로직 모르겠으니까 그냥 외워야겠다
            Vector2 dir = new Vector2(Mathf.Cos(Mathf.PI * 2 * i / roundNum), Mathf.Sin(Mathf.PI * 2 * i / roundNum));

            rigid.AddForce(dir.normalized * 1.5f, ForceMode2D.Impulse);

            // 총알 방향 맞추는 로직
            Vector3 rotVec = Vector3.forward * 360 * i / roundNum + Vector3.forward * 90;
            bullet.transform.Rotate(rotVec);
        }

        currPatternCount++;

        if (currPatternCount < maxPatternCount[patternIdx])
        {
            Invoke("BossFireAround", 1f);
        }
        else
        {
            Invoke("BossPattern", 3f);
        }
    }

    void Update()
    {
        if(enemyName == "B")
        {
            return;
        }
        Fire();
        Reload();
    }

    void Fire()
    {
        // 딜레이시간이 지나지 않았을 경우
        if (currShotDelay < maxShotDelay)
        {
            return; // 정지
        }

        if (enemyName == "S")
        {
            // 프리팹 생성 (오브젝트, 생성위치, 생성시 회전값)
            //GameObject bullet = Instantiate(bulletObjA, transform.position, transform.rotation);

            // 오브젝트 풀 사용시
            GameObject bullet = ObjManager.Instance.MakeObj("BulletEnemyA");
            bullet.transform.position = transform.position;
            bullet.transform.rotation = transform.rotation;

            // 생성된 오브젝트의 리지드바디 컴포넌트 불러오기
            Rigidbody2D rigid = bullet.GetComponent<Rigidbody2D>();

            // 목표위치 - 현재위치 = 목표방향 * 크기
            // normalized 로 방향만 남긴다
            Vector3 dir = player.transform.position - transform.position;
            rigid.AddForce(dir.normalized * (speed + 0.5f), ForceMode2D.Impulse);
        }
        else if (enemyName == "L")
        {
            // 프리팹 생성 (오브젝트, 생성위치, 생성시 회전값)
            //GameObject bulletR = Instantiate(bulletObjB, transform.position + (Vector3.right * 0.3f), transform.rotation);
            //GameObject bulletL = Instantiate(bulletObjB, transform.position + (Vector3.left * 0.3f), transform.rotation);

            // 오브젝트 풀 사용시
            GameObject bulletR = ObjManager.Instance.MakeObj("BulletEnemyB");
            GameObject bulletL = ObjManager.Instance.MakeObj("BulletEnemyB");
            bulletR.transform.position = transform.position + Vector3.right * 0.3f;
            bulletL.transform.position = transform.position + Vector3.right * -0.3f;
            bulletR.transform.rotation = transform.rotation;
            bulletL.transform.rotation = transform.rotation;

            // 생성된 오브젝트의 리지드바디 컴포넌트 불러오기
            Rigidbody2D rigidR = bulletR.GetComponent<Rigidbody2D>();
            Rigidbody2D rigidL = bulletL.GetComponent<Rigidbody2D>();

            Vector3 dirR = player.transform.position - (transform.position + (Vector3.right * 0.3f));
            Vector3 dirL = player.transform.position - (transform.position + (Vector3.left * 0.3f));

            rigidR.AddForce(dirR.normalized * (speed + 1), ForceMode2D.Impulse);
            rigidL.AddForce(dirL.normalized * (speed + 1), ForceMode2D.Impulse);
        }

        // 발사 한 후 딜레이 초기화
        currShotDelay = 0;
    }

    void Reload()
    {
        // Time.deltaTime 만큼 딜레이 충전
        currShotDelay += Time.deltaTime;
    }

    public void OnHit(int dmg)
    {
        if (health <= 0)
        {
            return;
        }
        // 체력에 데미지만큼 빼줌
        health -= dmg;
        if (enemyName == "B")
        {
            anim.SetTrigger(hashOnHit);
        }
        else
        {
            // 현재 스프라이트를 2번째 스프라이트로
            spriteRenderer.sprite = sprites[1];
            // Invoke("ReturnSprite", 0.1f) : 0.1초 뒤 ReturnSprite() 호출
            Invoke("ReturnSprite", 0.1f);
        }

        // 체력이 0과 같거나 낮을 경우
        if (health <= 0)
        {
            // 플레이어 스크립트 호출
            Player playerLogic = player.GetComponent<Player>();
            // 플레이어 스크립트의 점수에 enemyScore 더해줌
            playerLogic.score += enemyScore;

            // #.Random Ratio Item Drop
            // 적이 보스 일 경우 아이템 안떨어트리게 삼항연산자 사용
            int rand = enemyName == "B" ? 0 : Random.Range(0, 100);
            if (rand < 50)
            {
                Debug.Log("ㅋㅋ 노템");
            }
            else if (rand < 80)
            {
                // Coin
                GameObject coin = ObjManager.Instance.MakeObj("ItemCoin");
                coin.transform.position = transform.position;
                //Instantiate(itemCoin, transform.position, itemCoin.transform.rotation);
            }
            else if (rand < 90)
            {
                // Power
                GameObject power = ObjManager.Instance.MakeObj("ItemPower");
                power.transform.position = transform.position;
                //Instantiate(itemPower, transform.position, itemPower.transform.rotation);
            }
            else if (rand < 100)
            {
                // Boom
                GameObject boom = ObjManager.Instance.MakeObj("ItemBoom");
                boom.transform.position = transform.position;
                //Instantiate(itemBoom, transform.position, itemBoom.transform.rotation);
            }

            // 현재 오브젝트 삭제
            //Destroy(gameObject);

            // 오브젝트 풀링 사용시
            // 현재 오브젝트 비활성화
            gameObject.SetActive(false);
            // 재사용 하기 전 각도 초기화
            transform.rotation = Quaternion.identity;
        }
    }
    // 피격 후 스프라이트 초기화
    void ReturnSprite()
    {
        spriteRenderer.sprite = sprites[0];
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 벽에 닿으면 없애기 && 보스가 아닐 때만
        if (collision.CompareTag("BORDERBULLET") && enemyName != "B")
        {
            // 오브젝트 풀링 사용시
            // 현재 오브젝트 비활성화
            gameObject.SetActive(false);
            // 재사용 하기 전 각도 초기화
            transform.rotation = Quaternion.identity;
        }
        // 플레이어 총알에 닿으면 OnHit 함수 호출
        else if (collision.CompareTag("PLAYERBULLET"))
        {
            // 검출된 충돌체의 Bullet 스크립트 컴포넌트 가져옴
            Bullet bullet = collision.GetComponent<Bullet>();
            // 매개변수에 Bullet 스크립트의 데미지 변수 대입
            OnHit(bullet.dmg);

            // 충돌체(총알) 삭제
            //Destroy(collision.gameObject);

            // 오브젝트 풀링 사용시
            // 총알 오브젝트 비활성화
            collision.gameObject.SetActive(false);
        }
    }
}
