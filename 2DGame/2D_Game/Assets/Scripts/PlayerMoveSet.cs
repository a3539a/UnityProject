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

    // �ִϸ����� �Ķ���͸� hash(int)�� �����Ͽ� Ȱ��
    // �Ķ���͸� ���ν��� �������� ȸ��
    readonly int hashisRun = Animator.StringToHash("isRun");
    readonly int hashisJump = Animator.StringToHash("isJump");

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();

        // ���� �Ŵ��� ����
        GameManager.GetInstance();
    }

    private void Update()
    {
        if (Input.GetButtonDown("Jump") && !anim.GetBool(hashisJump))
        {
            // Impulse - �������� �� (��ź ���� ��)
            // Force - ���������� �־����� �� (�ٶ� ���� ��)
            rb.AddForce(Vector2.up * jumpPower, ForceMode2D.Impulse);
            anim.SetBool(hashisJump, true);
        }

        if (Input.GetButtonDown("Horizontal"))
        {
            // �¿� �Է°��� �����ڸ� �̿��Ͽ� true, false �� ��ȯ
            spriteRenderer.flipX = (Input.GetAxisRaw("Horizontal") == -1);
        }

        // normalized �� ���͸� ����ȭ �ϴ� ������
        // �밢�� �̵� �� ������ ���� 1�� �Ѿ�� ��쿡
        // 1������ ����ȭ ���ִ� ���
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

        //// ���� ��� ������ �ӵ��� ��� �����ϴϱ� maxSpeed ������
        //// ���Ѽӵ��� �ɷ��� ���� �ڵ�
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

        // DrawRay �Լ��� Ray�� �ð�ȭ �ϴ� �Լ�
        // DrawRay (������ġ, ����, ����) ���� �����Ͽ� ���信�� Ȯ�� ����
        // Debug.DrawRay(rb.position, Vector3.down, new Color(0, 1, 0));

        // Raycast(������ġ, ����, ����, �浹 ���̾�)
        // �� ������ ����� ���̷� ���̸� �ѷ��� �浹���̾ �����ϵ��� �ϴ� �Լ�
        // �浹�� ��� �ش� ������ RaycastHit2D ������ �����ϵ��� ��

        if (rb.velocity.y < 0)
        {
            RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, 1, LayerMask.GetMask("PLATFORM"));

            if (hit.collider != null) // �浹�ϴ� ���� ���� �ʴٸ�, ���̿� �����Ǵ°��� �ִٸ�
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

            // ����� ��
            //Debug.Log(GameManager.GetInstance().stagePoint);
            Destroy(collision.gameObject);
        }
        else if (collision.CompareTag("FINISH"))
        {
            //GameManager.GetInstance().totalPoint += GameManager.GetInstance().stagePoint;
            // stagePoint �ʱ�ȭ
            //GameManager.GetInstance().stagePoint = 0;

            //Debug.Log("Ending!");
            //Debug.Log(GameManager.GetInstance().totalPoint);

            int idx = GameManager.GetInstance().NextStage();
            if (idx >= 3) // �ѽ��� ũ��
            {
                Debug.Log("�̰� ���ϰ� �ֳ�");
            }
            else
            {
                // ���ο� ������ ��ȯ
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
                // ���� ���� ���� �Լ� ȣ��
                OnAttack(collision.transform);

                // ����� �� ��¦ ���� Ƣ�������
                rb.AddForce(Vector2.up * 20, ForceMode2D.Impulse);
            }
            else
            {
                // �浹�� ������Ʈ�� ��ġ���� �Ű������� ����
                // �浹�� ��ġ���� �ڷ� �о�� ���� �Լ� ȣ��
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

        // ���� ���̾ ���̾��Ͽ� �� 9�� ���̾�� �������ֱ�
        gameObject.layer = 9;

        // Color(R, G, B, A) A�� ������ ��Ī��
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
