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
        //Invoke("�Լ��̸�", ������Ÿ��)
        // �ش� �Լ��� ������Ÿ�� �Ŀ� ȣ��
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
            Debug.Log("��������");
            // -1�� ���ؼ� ���� �ٲ��ֱ�
            nextMove = nextMove * -1;
            spriteRenderer.flipX = (nextMove > 0);
            CancelInvoke(); // Invoke�� ȣ��� �Լ��� ����
            Invoke("Think", 3);

        }
    }

    void Think()
    {
        // -1, 0, 1
        nextMove = Random.Range(-1, 2);
        // ���ȣ�� - �ڱ� �ڽ��� �ڽ��� ȣ���ϴ� ��
        // ������ ������
        float thinkTime = Random.Range(2f, 5f);
        Invoke("Think", thinkTime);

        anim.SetInteger(hashSpeed, nextMove);
        spriteRenderer.flipX = (nextMove > 0);
    }

    public void OnDamaged()
    {
        // Color(R, G, B, A) A�� ������ ��Ī��
        spriteRenderer.color = new Color(1, 1, 1, 0.4f);
        spriteRenderer.flipY = true;

        // ���� �ڿ� �浹 �����ϵ��� �ݶ��̴� ����
        GetComponent<CapsuleCollider2D>().enabled = false;
        // ���� �ڿ� ���� ��¦ Ƣ����
        rb.AddForce(Vector2.up * 5, ForceMode2D.Impulse);


        // 2 �ʵڿ� ����
        Destroy(gameObject, 2);

    }
}
