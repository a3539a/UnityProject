using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    // 애니메이터 불러오기
    Animator animator;

    // 캐릭터 속도
    public float speed;
    // 캐릭터의 파워 단계
    public int power;
    public int maxPower = 3;
    // 폭탄 회수
    public int boom;
    public int maxBoom = 5;
    // 총알 발사 딜레이
    public float maxShotDelay;
    public float currShotDelay;

    // UI 파트
    public int life;
    public int score;

    // 플레이어 피격 체크
    public bool isDamaged;

    // 폭탄 사용중 체크
    public bool isBoomTime = false;

    // 플레이어와 벽 충돌체크
    public bool isTouchTop;
    public bool isTouchBottom;
    public bool isTouchLeft;
    public bool isTouchRight;

    // 총알 오브젝트 변수
    public GameObject bulletObjA;
    public GameObject bulletObjB;
    // 폭탄 오브젝트 변수
    public GameObject boomEffect;

    // 보조무기 오브젝트 배열
    public GameObject[] followers;

    // 애니메이터 파라미터 저장
    readonly int hashMoveInput = Animator.StringToHash("HInput");

    private void Awake()
    {
        // 현재 오브젝트의 애니메이터 컴포넌트 불러오기
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        Move();
        Fire();
        Boom();
        Reload();
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
        // 파워 단계별 총알 생성
        switch (power)
        {
            // Power One
            case 1:
                // 오브젝트 풀 사용시
                // 포지션 값 잡아줌
                GameObject bullet = ObjManager.Instance.MakeObj("BulletPlayerA");
                bullet.transform.position = transform.position;
                bullet.transform.rotation = transform.rotation;

                // 프리팹 생성 (오브젝트, 생성위치, 생성시 회전값)
                //GameObject bullet = Instantiate(bulletObjA, transform.position, transform.rotation);

                // 생성된 오브젝트의 리지드바디 컴포넌트 불러오기
                Rigidbody2D rigid = bullet.GetComponent<Rigidbody2D>();
                // 불러온 리지드바디에 위방향으로 AddForce
                rigid.AddForce(Vector2.up * 10, ForceMode2D.Impulse);
                break;
            // Power Two
            case 2:
                // 프리팹 생성 (오브젝트, 생성위치, 생성시 회전값)
                //GameObject bulletR = Instantiate(bulletObjA, transform.position + (Vector3.right * 0.15f), transform.rotation);
                //GameObject bulletL = Instantiate(bulletObjA, transform.position + (Vector3.left * 0.15f), transform.rotation);

                // 오브젝트 풀 사용시
                // 포지션 값 잡아줌
                GameObject bulletR = ObjManager.Instance.MakeObj("BulletPlayerA");
                GameObject bulletL = ObjManager.Instance.MakeObj("BulletPlayerA");
                bulletR.transform.position = transform.position + Vector3.right * 0.2f;
                bulletL.transform.position = transform.position + Vector3.right * -0.2f;
                bulletR.transform.rotation = transform.rotation;
                bulletL.transform.rotation = transform.rotation;

                // 생성된 오브젝트의 리지드바디 컴포넌트 불러오기
                Rigidbody2D rigidR = bulletR.GetComponent<Rigidbody2D>();
                Rigidbody2D rigidL = bulletL.GetComponent<Rigidbody2D>();
                // 불러온 리지드바디에 위방향으로 AddForce
                rigidR.AddForce(Vector2.up * 10, ForceMode2D.Impulse);
                rigidL.AddForce(Vector2.up * 10, ForceMode2D.Impulse);
                break;
            // Power 1, 2 상태를 제외한 나머지
            default:
                //GameObject bulletRR = Instantiate(bulletObjA, transform.position + (Vector3.right * 0.35f), transform.rotation);
                //GameObject bulletC = Instantiate(bulletObjB, transform.position, transform.rotation);
                //GameObject bulletLL = Instantiate(bulletObjA, transform.position + (Vector3.left * 0.35f), transform.rotation);
                // 오브젝트 풀 사용시
                // 포지션 값 잡아줌
                GameObject bulletRR = ObjManager.Instance.MakeObj("BulletPlayerA");
                GameObject bulletC = ObjManager.Instance.MakeObj("BulletPlayerB");
                GameObject bulletLL = ObjManager.Instance.MakeObj("BulletPlayerA");
                bulletRR.transform.position = transform.position + Vector3.right * 0.25f;
                bulletC.transform.position = transform.position;
                bulletLL.transform.position = transform.position + Vector3.right * -0.25f;
                bulletLL.transform.rotation = transform.rotation;
                bulletC.transform.rotation = transform.rotation;
                bulletRR.transform.rotation = transform.rotation;
                Rigidbody2D rigidRR = bulletRR.GetComponent<Rigidbody2D>();
                Rigidbody2D rigidC = bulletC.GetComponent<Rigidbody2D>();
                Rigidbody2D rigidLL = bulletLL.GetComponent<Rigidbody2D>();
                rigidRR.AddForce(Vector2.up * 10, ForceMode2D.Impulse);
                rigidC.AddForce(Vector2.up * 10, ForceMode2D.Impulse);
                rigidLL.AddForce(Vector2.up * 10, ForceMode2D.Impulse);
                break;
        }

        // 발사 한 후 딜레이 초기화
        currShotDelay = 0;
    }

    void Reload()
    {
        // Time.deltaTime 만큼 딜레이 충전
        currShotDelay += Time.deltaTime;
    }

    void Boom()
    {
        if (Input.GetKeyDown("z") && (boom > 0))
        {
            if (!isBoomTime)
            {
                // 폭탄 개수 감소
                boom--;
                // 1) Effect Visible
                boomEffect.SetActive(true);
                isBoomTime = true;
                // 3초 뒤에 끔
                Invoke("OffBoomEffect", 3f);

                // 2) Remove Enemy
                GameObject[] enemiesL = ObjManager.Instance.GetPool("EnemyL");
                GameObject[] enemiesM = ObjManager.Instance.GetPool("EnemyM");
                GameObject[] enemiesS = ObjManager.Instance.GetPool("EnemyS");
                GameObject[] boss = ObjManager.Instance.GetPool("Boss");

                for (int i = 0; i < enemiesL.Length; i++)
                {
                    if (enemiesL[i].activeSelf)
                    {
                        Enemy enemyLogic = enemiesL[i].GetComponent<Enemy>();
                        enemyLogic.OnHit(100);
                    }
                }
                for (int i = 0; i < enemiesM.Length; i++)
                {
                    if (enemiesM[i].activeSelf)
                    {
                        Enemy enemyLogic = enemiesM[i].GetComponent<Enemy>();
                        enemyLogic.OnHit(100);
                    }
                }
                for (int i = 0; i < enemiesS.Length; i++)
                {
                    if (enemiesS[i].activeSelf)
                    {
                        Enemy enemyLogic = enemiesS[i].GetComponent<Enemy>();
                        enemyLogic.OnHit(100);
                    }
                }
                for (int i = 0; i < boss.Length; i++)
                {
                    if (boss[i].activeSelf)
                    {
                        Enemy enemyLogic = enemiesS[i].GetComponent<Enemy>();
                        enemyLogic.OnHit(1000);
                    }
                }

                // 2) Remove EnemyBullet
                GameObject[] enemyBulletsA = ObjManager.Instance.GetPool("BulletEnemyA");
                GameObject[] enemyBulletsB = ObjManager.Instance.GetPool("BulletEnemyB");

                for (int i = 0; i < enemyBulletsA.Length; i++)
                {
                    if (enemyBulletsA[i].activeSelf)
                    {
                        enemyBulletsA[i].SetActive(false);
                    }
                }
                for (int i = 0; i < enemyBulletsB.Length; i++)
                {
                    if (enemyBulletsB[i].activeSelf)
                    {
                        enemyBulletsB[i].SetActive(false);
                    }
                }
            }
            else
            {
                return;
            }
        }
    }

    private void Move()
    {
        // 좌우 입력값 1, 0, -1 받기
        float h = Input.GetAxisRaw("Horizontal");
        // 좌우 벽 충돌 중일 때 좌우 입력값 0으로 초기화
        if (((h == 1) && (isTouchRight)) || ((h == -1) && (isTouchLeft)))
        {
            h = 0;
        }

        // 상하 입력값 1, 0, -1 받기
        float v = Input.GetAxisRaw("Vertical");
        // 상하 벽 충돌 중일 때 상하 입력값 0으로 초기화
        if (((v == 1) && (isTouchTop)) || ((v == -1) && (isTouchBottom)))
        {
            v = 0;
        }
        // 오브젝트의 현재 포지션값을 저장
        Vector3 currPos = transform.position;
        // 입력 받은 값을 speed 만큼 벡터값으로 전환
        Vector3 nextPos = new Vector3(h, v, 0) * speed * Time.deltaTime;
        // 현재 포지션을 입력받은 값만큼 이동
        transform.position = currPos + nextPos;
        // 좌우 이동키를 누르 거나 땔 때
        if (Input.GetButtonDown("Horizontal") || Input.GetButtonUp("Horizontal"))
        {
            // 애니메이터 인트 파라미터에 좌우 이동값 대입 (int)(1, 0, -1)
            animator.SetInteger(hashMoveInput, (int)h);
        }
    }

    // 콜라이더 통과 시
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 태그가 BORDER인 경우
        if (collision.gameObject.CompareTag("BORDER"))
        {
            // 해당 충돌체의 이름이
            switch (collision.gameObject.name)
            {
                // Top 인 경우
                case "Top":
                    // 충돌 체크
                    isTouchTop = true;
                    break;
                // Bottom 인 경우
                case "Bottom":
                    isTouchBottom = true;
                    break;
                // Left 인 경우
                case "Left":
                    isTouchLeft = true;
                    break;
                // Right 인 경우
                case "Right":
                    isTouchRight = true;
                    break;
            }
        }
        // 적과 부딪힌 경우, 적 총알 에 맞을 경우
        else if (collision.CompareTag("ENEMY") || collision.CompareTag("ENEMYBULLET"))
        {
            // 데미지를 받았으면
            if (isDamaged)
            {
                return;// 종료
            }

            // 데미지 입었을 때 true
            isDamaged = true;
            life--;
            // 게임매니저에 있는 UpdateLifeIcon() 싱글톤으로 함수 호출
            // 현재 남은 목숨 갯수만큼 이미지 출력
            GameManager.Instance.UpdateLifeIcon(life);

            if (life == 0)
            {
                // UI 호출
                GameManager.Instance.GameOver();
            }
            else
            {
                // 게임매니저에 있는 RespawnPlayer() 싱글톤으로 함수 호출
                // 2초 뒤에 오브젝트 키면서 시작지점으로 포지션 이동
                GameManager.Instance.RespawnPlayer();
            }

            // 플레이어의 오브젝트 꺼줌
            gameObject.SetActive(false);

            // 충돌체 삭제
            collision.gameObject.SetActive(false);
            collision.gameObject.transform.rotation = Quaternion.identity;
        }
        else if (collision.gameObject.CompareTag("ITEM"))
        {
            Item item = collision.gameObject.GetComponent<Item>();
            switch (item.type)
            {
                case "Boom":
                    // 폭탄 개수가 최대일 경우
                    if(boom == maxBoom)
                    {
                        // 점수 200
                        score += 200;
                    }
                    else
                    {
                        // 폭탄 개수 업
                        boom++;
                    }
                    break;

                case "Coin":
                    // 코인 먹을 때는 스코어 100점
                    score += 100;
                    break;

                case "Power":
                    // 파워가 맥스파워 단계라면
                    if(power == maxPower)
                    {
                        // 스코어 100점
                        score += 100;
                    }
                    else // 아닌 경우
                    {
                        // 파워 업
                        power++;
                        AddFollower();
                    }
                    break;
            }
            // 먹은 아이템 삭제
            //Destroy(collision.gameObject);

            // 오브젝트 풀링 사용시
            // 아이템 오브젝트 비활성화
            collision.gameObject.SetActive(false);
        }
    }

    void AddFollower()
    {
        if (power == 4)
        {
            followers[0].SetActive(true);
        }
        else if(power == 5)
        {
            followers[1].SetActive(true);
        }
        else if(power == 6)
        {
            followers[2].SetActive(true);
        }
    }

    // 콜라이더 통과한 후
    private void OnTriggerExit2D(Collider2D collision)
    {
        // 충돌체크 false
        if (collision.gameObject.CompareTag("BORDER"))
        {
            switch (collision.gameObject.name)
            {
                case "Top":
                    isTouchTop = false;
                    break;
                case "Bottom":
                    isTouchBottom = false;
                    break;
                case "Left":
                    isTouchLeft = false;
                    break;
                case "Right":
                    isTouchRight = false;
                    break;
            }
        }
    }

    void OffBoomEffect()
    {
        boomEffect.SetActive(false);
        isBoomTime = false;
    }
}
