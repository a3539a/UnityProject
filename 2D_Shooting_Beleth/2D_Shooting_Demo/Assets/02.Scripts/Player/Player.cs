using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    // �ִϸ����� �ҷ�����
    Animator animator;

    // ĳ���� �ӵ�
    public float speed;
    // ĳ������ �Ŀ� �ܰ�
    public int power;
    public int maxPower = 3;
    // ��ź ȸ��
    public int boom;
    public int maxBoom = 5;
    // �Ѿ� �߻� ������
    public float maxShotDelay;
    public float currShotDelay;

    // UI ��Ʈ
    public int life;
    public int score;

    // �÷��̾� �ǰ� üũ
    public bool isDamaged;

    // ��ź ����� üũ
    public bool isBoomTime = false;

    // �÷��̾�� �� �浹üũ
    public bool isTouchTop;
    public bool isTouchBottom;
    public bool isTouchLeft;
    public bool isTouchRight;

    // �Ѿ� ������Ʈ ����
    public GameObject bulletObjA;
    public GameObject bulletObjB;
    // ��ź ������Ʈ ����
    public GameObject boomEffect;

    // �������� ������Ʈ �迭
    public GameObject[] followers;

    // �ִϸ����� �Ķ���� ����
    readonly int hashMoveInput = Animator.StringToHash("HInput");

    private void Awake()
    {
        // ���� ������Ʈ�� �ִϸ����� ������Ʈ �ҷ�����
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
        // Fire1 ��ư ������ ���� ���� ��
        if (!Input.GetButton("Jump"))
        {
            // ����!
            return;
        }
        // �����̽ð��� ������ �ʾ��� ���
        if (currShotDelay < maxShotDelay)
        {
            return; // ����
        }
        // �Ŀ� �ܰ躰 �Ѿ� ����
        switch (power)
        {
            // Power One
            case 1:
                // ������Ʈ Ǯ ����
                // ������ �� �����
                GameObject bullet = ObjManager.Instance.MakeObj("BulletPlayerA");
                bullet.transform.position = transform.position;
                bullet.transform.rotation = transform.rotation;

                // ������ ���� (������Ʈ, ������ġ, ������ ȸ����)
                //GameObject bullet = Instantiate(bulletObjA, transform.position, transform.rotation);

                // ������ ������Ʈ�� ������ٵ� ������Ʈ �ҷ�����
                Rigidbody2D rigid = bullet.GetComponent<Rigidbody2D>();
                // �ҷ��� ������ٵ� ���������� AddForce
                rigid.AddForce(Vector2.up * 10, ForceMode2D.Impulse);
                break;
            // Power Two
            case 2:
                // ������ ���� (������Ʈ, ������ġ, ������ ȸ����)
                //GameObject bulletR = Instantiate(bulletObjA, transform.position + (Vector3.right * 0.15f), transform.rotation);
                //GameObject bulletL = Instantiate(bulletObjA, transform.position + (Vector3.left * 0.15f), transform.rotation);

                // ������Ʈ Ǯ ����
                // ������ �� �����
                GameObject bulletR = ObjManager.Instance.MakeObj("BulletPlayerA");
                GameObject bulletL = ObjManager.Instance.MakeObj("BulletPlayerA");
                bulletR.transform.position = transform.position + Vector3.right * 0.2f;
                bulletL.transform.position = transform.position + Vector3.right * -0.2f;
                bulletR.transform.rotation = transform.rotation;
                bulletL.transform.rotation = transform.rotation;

                // ������ ������Ʈ�� ������ٵ� ������Ʈ �ҷ�����
                Rigidbody2D rigidR = bulletR.GetComponent<Rigidbody2D>();
                Rigidbody2D rigidL = bulletL.GetComponent<Rigidbody2D>();
                // �ҷ��� ������ٵ� ���������� AddForce
                rigidR.AddForce(Vector2.up * 10, ForceMode2D.Impulse);
                rigidL.AddForce(Vector2.up * 10, ForceMode2D.Impulse);
                break;
            // Power 1, 2 ���¸� ������ ������
            default:
                //GameObject bulletRR = Instantiate(bulletObjA, transform.position + (Vector3.right * 0.35f), transform.rotation);
                //GameObject bulletC = Instantiate(bulletObjB, transform.position, transform.rotation);
                //GameObject bulletLL = Instantiate(bulletObjA, transform.position + (Vector3.left * 0.35f), transform.rotation);
                // ������Ʈ Ǯ ����
                // ������ �� �����
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

        // �߻� �� �� ������ �ʱ�ȭ
        currShotDelay = 0;
    }

    void Reload()
    {
        // Time.deltaTime ��ŭ ������ ����
        currShotDelay += Time.deltaTime;
    }

    void Boom()
    {
        if (Input.GetKeyDown("z") && (boom > 0))
        {
            if (!isBoomTime)
            {
                // ��ź ���� ����
                boom--;
                // 1) Effect Visible
                boomEffect.SetActive(true);
                isBoomTime = true;
                // 3�� �ڿ� ��
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
        // �¿� �Է°� 1, 0, -1 �ޱ�
        float h = Input.GetAxisRaw("Horizontal");
        // �¿� �� �浹 ���� �� �¿� �Է°� 0���� �ʱ�ȭ
        if (((h == 1) && (isTouchRight)) || ((h == -1) && (isTouchLeft)))
        {
            h = 0;
        }

        // ���� �Է°� 1, 0, -1 �ޱ�
        float v = Input.GetAxisRaw("Vertical");
        // ���� �� �浹 ���� �� ���� �Է°� 0���� �ʱ�ȭ
        if (((v == 1) && (isTouchTop)) || ((v == -1) && (isTouchBottom)))
        {
            v = 0;
        }
        // ������Ʈ�� ���� �����ǰ��� ����
        Vector3 currPos = transform.position;
        // �Է� ���� ���� speed ��ŭ ���Ͱ����� ��ȯ
        Vector3 nextPos = new Vector3(h, v, 0) * speed * Time.deltaTime;
        // ���� �������� �Է¹��� ����ŭ �̵�
        transform.position = currPos + nextPos;
        // �¿� �̵�Ű�� ���� �ų� �� ��
        if (Input.GetButtonDown("Horizontal") || Input.GetButtonUp("Horizontal"))
        {
            // �ִϸ����� ��Ʈ �Ķ���Ϳ� �¿� �̵��� ���� (int)(1, 0, -1)
            animator.SetInteger(hashMoveInput, (int)h);
        }
    }

    // �ݶ��̴� ��� ��
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // �±װ� BORDER�� ���
        if (collision.gameObject.CompareTag("BORDER"))
        {
            // �ش� �浹ü�� �̸���
            switch (collision.gameObject.name)
            {
                // Top �� ���
                case "Top":
                    // �浹 üũ
                    isTouchTop = true;
                    break;
                // Bottom �� ���
                case "Bottom":
                    isTouchBottom = true;
                    break;
                // Left �� ���
                case "Left":
                    isTouchLeft = true;
                    break;
                // Right �� ���
                case "Right":
                    isTouchRight = true;
                    break;
            }
        }
        // ���� �ε��� ���, �� �Ѿ� �� ���� ���
        else if (collision.CompareTag("ENEMY") || collision.CompareTag("ENEMYBULLET"))
        {
            // �������� �޾�����
            if (isDamaged)
            {
                return;// ����
            }

            // ������ �Ծ��� �� true
            isDamaged = true;
            life--;
            // ���ӸŴ����� �ִ� UpdateLifeIcon() �̱������� �Լ� ȣ��
            // ���� ���� ��� ������ŭ �̹��� ���
            GameManager.Instance.UpdateLifeIcon(life);

            if (life == 0)
            {
                // UI ȣ��
                GameManager.Instance.GameOver();
            }
            else
            {
                // ���ӸŴ����� �ִ� RespawnPlayer() �̱������� �Լ� ȣ��
                // 2�� �ڿ� ������Ʈ Ű�鼭 ������������ ������ �̵�
                GameManager.Instance.RespawnPlayer();
            }

            // �÷��̾��� ������Ʈ ����
            gameObject.SetActive(false);

            // �浹ü ����
            collision.gameObject.SetActive(false);
            collision.gameObject.transform.rotation = Quaternion.identity;
        }
        else if (collision.gameObject.CompareTag("ITEM"))
        {
            Item item = collision.gameObject.GetComponent<Item>();
            switch (item.type)
            {
                case "Boom":
                    // ��ź ������ �ִ��� ���
                    if(boom == maxBoom)
                    {
                        // ���� 200
                        score += 200;
                    }
                    else
                    {
                        // ��ź ���� ��
                        boom++;
                    }
                    break;

                case "Coin":
                    // ���� ���� ���� ���ھ� 100��
                    score += 100;
                    break;

                case "Power":
                    // �Ŀ��� �ƽ��Ŀ� �ܰ���
                    if(power == maxPower)
                    {
                        // ���ھ� 100��
                        score += 100;
                    }
                    else // �ƴ� ���
                    {
                        // �Ŀ� ��
                        power++;
                        AddFollower();
                    }
                    break;
            }
            // ���� ������ ����
            //Destroy(collision.gameObject);

            // ������Ʈ Ǯ�� ����
            // ������ ������Ʈ ��Ȱ��ȭ
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

    // �ݶ��̴� ����� ��
    private void OnTriggerExit2D(Collider2D collision)
    {
        // �浹üũ false
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
