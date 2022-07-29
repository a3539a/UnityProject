using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMoveSet : MonoBehaviour
{
    Rigidbody2D rb;
    SpriteRenderer spriteRenderer;
    Animator anim;

    public int nextMove;

    readonly int hashSpeed = Animator.StringToHash("Speed");

    void Awake()
    {
        anim = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        //Invoke("함수이름", 딜레이타임)
        // 해당 함수를 딜레이타임 후에 호출
        Invoke("Think", 1);
    }

    void FixedUpdate()
    {
        rb.velocity = new Vector2 (nextMove, rb.velocity.y);

        Vector2 frontVec = new Vector2 (rb.position.x + nextMove, rb.position.y);
        //Debug.DrawRay(frontVec, Vector3.down, new Color(0, 1, 0));
        RaycastHit2D hit = Physics2D.Raycast(frontVec, Vector2.down, 1, LayerMask.GetMask("PLATFORM"));

        if (hit.collider == null)
        {
            Debug.Log("낭떠러지");
            // -1을 곱해서 방향 바꿔주기
            nextMove = nextMove * -1;
            spriteRenderer.flipX = (nextMove > 0);
            CancelInvoke(); // Invoke로 호출된 함수를 종료
            Invoke("Think", 3);

        }
    }

    void Think()
    {
        // -1, 0, 1
        nextMove = Random.Range(-1, 2);
        // 재귀호출 - 자기 자신이 자신을 호출하는 것
        // 성능이 ㅈ같음
        float thinkTime = Random.Range(2f, 5f);
        Invoke("Think", thinkTime);

        anim.SetInteger(hashSpeed, nextMove);
        spriteRenderer.flipX = (nextMove > 0);
    }

    public void OnDamaged()
    {
        // Color(R, G, B, A) A는 투명도를 지칭함
        spriteRenderer.color = new Color(1, 1, 1, 0.4f);
        spriteRenderer.flipY = true;

        // 죽은 뒤에 충돌 방지하도록 콜라이더 끄기
        GetComponent<CapsuleCollider2D>().enabled = false;
        // 밟힌 뒤에 위로 살짝 튀도록
        rb.AddForce(Vector2.up * 5, ForceMode2D.Impulse);


        // 2 초뒤에 삭제
        Destroy(gameObject, 2);

    }
}
