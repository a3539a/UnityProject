using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerMoveSet : MonoBehaviour
{
    public float maxSpeed;
    public float jumpPower;

    Rigidbody2D rb;
    SpriteRenderer spriteRenderer;
    Animator anim;

    // 애니메이터 파라메터를 hash(int)로 변경하여 활용
    // 파라메터를 맵핑시켜 성능저하 회피
    readonly int hashisRun = Animator.StringToHash("isRun");
    readonly int hashisJump = Animator.StringToHash("isJump");

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();

        // 게임 매니저 생성
        GameManager.GetInstance();
    }

    private void Update()
    {
        if (Input.GetButtonDown("Jump") && !anim.GetBool(hashisJump))
        {
            // Impulse - 순간적인 힘 (폭탄 같은 것)
            // Force - 연속적으로 주어지는 힘 (바람 같은 것)
            rb.AddForce(Vector2.up * jumpPower, ForceMode2D.Impulse);
            anim.SetBool(hashisJump, true);
        }

        if (Input.GetButtonDown("Horizontal"))
        {
            // 좌우 입력값을 연산자를 이용하여 true, false 값 반환
            spriteRenderer.flipX = (Input.GetAxisRaw("Horizontal") == -1);
        }

        // normalized 는 벡터를 정규화 하는 것으로
        // 대각선 이동 등 벡터의 값이 1을 넘어서는 경우에
        // 1값으로 정규화 해주는 기능
        //if (rb.velocity.normalized.x == 0)
        //{
        //}
        //else
        //{
        //}
    }

    void FixedUpdate()
    {
        float h = Input.GetAxisRaw("Horizontal");
        //rb.AddForce(Vector2.right * h * 3, ForceMode2D.Impulse);

        //// 힘을 계속 받으면 속도가 계속 증가하니까 maxSpeed 값으로
        //// 제한속도를 걸려고 만든 코드
        //if (rb.velocity.x > maxSpeed)
        //{
        //    rb.velocity = new Vector2(maxSpeed, rb.velocity.y);
        //}
        //else if (rb.velocity.x < -maxSpeed)
        //{
        //    rb.velocity = new Vector2(-maxSpeed, rb.velocity.y);
        //}
        Vector3 moveVelocity = Vector3.zero;
        if (h < 0)
        {
            moveVelocity = Vector3.left;
            anim.SetBool(hashisRun, true);
        }
        else if (h > 0)
        {
            moveVelocity = Vector3.right;
            anim.SetBool(hashisRun, true);
        }
        else
        {
            anim.SetBool(hashisRun, false);
        }
        transform.position += moveVelocity * maxSpeed * Time.deltaTime;

        // DrawRay 함수는 Ray를 시각화 하는 함수
        // DrawRay (시작위치, 방향, 색상) 으로 설정하여 씬뷰에서 확인 가능
        // Debug.DrawRay(rb.position, Vector3.down, new Color(0, 1, 0));

        // Raycast(시작위치, 방향, 길이, 충돌 레이어)
        // 위 설정한 방향과 길이로 레이를 뿌려서 충돌레이어를 감지하도록 하는 함수
        // 충돌할 경우 해당 정보는 RaycastHit2D 변수에 저장하도록 함

        if (rb.velocity.y < 0)
        {
            RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, 1, LayerMask.GetMask("PLATFORM"));

            if (hit.collider != null) // 충돌하는 것이 없지 않다면, 레이에 감지되는것이 있다면
            {
                if (hit.distance < 0.5f)
                {
                    anim.SetBool(hashisJump, false);
                }
            }
        }

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "ITEM")
        {
            Coin coin = collision.gameObject.GetComponent<Coin>();

            if(coin != null)
            {
                GameManager.GetInstance().stagePoint += coin.point;
            }

            // 디버그 용
            //Debug.Log(GameManager.GetInstance().stagePoint);
            Destroy(collision.gameObject);
        }
        else if (collision.CompareTag("FINISH"))
        {
            //GameManager.GetInstance().totalPoint += GameManager.GetInstance().stagePoint;
            // stagePoint 초기화
            //GameManager.GetInstance().stagePoint = 0;

            //Debug.Log("Ending!");
            //Debug.Log(GameManager.GetInstance().totalPoint);

            int idx = GameManager.GetInstance().NextStage();
            if (idx >= 3) // 총신의 크기
            {
                Debug.Log("이걸 왜하고 있냐");
            }
            else
            {
                // 새로운 씬으로 전환
                SceneManager.LoadScene(idx);
            }
        }

    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("SPIKE"))
        {
            if (rb.velocity.y < 0 && transform.position.y > collision.transform.position.y)
            {
                // 몬스터 공격 관련 함수 호출
                OnAttack(collision.transform);

                // 밟았을 때 살짝 위로 튀어오르게
                rb.AddForce(Vector2.up * 20, ForceMode2D.Impulse);
            }
            else
            {
                // 충돌한 오브젝트의 위치값을 매개변수로 전달
                // 충돌한 위치에서 뒤로 밀어내기 위한 함수 호출
                OnDamaged(collision.transform.position);
            }
        }
    }

    void OnAttack(Transform enemy)
    {
        EnemyMoveSet ems = enemy.GetComponent<EnemyMoveSet>();
        ems?.OnDamaged();
    }

    void OnDamaged(Vector2 targetPos)
    {
        // Debug.Log("Die");

        // 현재 레이어를 레이어목록에 중 9번 레이어로 변경해주기
        gameObject.layer = 9;

        // Color(R, G, B, A) A는 투명도를 지칭함
        spriteRenderer.color = new Color(1, 1, 1, 0.4f);

        int dir = (transform.position.x - targetPos.x) > 0 ? 1: -1;
        int bouncePow = 12;
        rb.AddForce(new Vector2(dir, 1) * bouncePow, ForceMode2D.Impulse);

        Invoke("OffDamaged", 1);
    }

    void OffDamaged()
    {
        gameObject.layer = 7;
        spriteRenderer.color = new Color(1, 1, 1, 1);
    }
}
