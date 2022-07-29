using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    // �� ������Ʈ �̸� ����
    public string enemyName;
    // �� ������Ʈ ����
    public int enemyScore;
    // �� �ӷ�
    public float speed;
    // �� ü��
    public int health;
    // �ǰ� �̹��� ����
    public Sprite[] sprites;

    // �Ѿ� ������Ʈ ����
    public GameObject bulletObjA;
    public GameObject bulletObjB;
    // �Ѿ� �߻� ������
    public float maxShotDelay;
    public float currShotDelay;

    // ������ ������Ʈ ����
    public GameObject itemPower;
    public GameObject itemCoin;
    public GameObject itemBoom;

    // ������ȭ �Ȼ��¿����� ���� ������� ������Ʈ�� ���ԺҰ�
    // ���ӸŴ������� �÷��̾� ������Ʈ �޾ƿð���
    public GameObject player;

    // �ʿ��� ������Ʈ
    // �ǰ������� �ʿ�
    SpriteRenderer spriteRenderer;

    // ���� �ǰ� �ִϸ��̼ǿ�
    Animator anim;
    readonly int hashOnHit = Animator.StringToHash("OnHit");

    // ���� ���Ͽ� �Լ�
    // ���� ������
    public int patternIdx;
    // ���� ���� �ݺ�üũ��
    public int currPatternCount;
    // ��� �ݺ�����
    public int[] maxPatternCount;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        if(enemyName == "B")
        {
            anim = GetComponent<Animator>();
        }
    }

    // ������Ʈ�� Ȱ��ȭ �ɶ����� ����
    private void OnEnable()
    {
        switch (enemyName)
        {
            case "B":
                health = 10000;
                // 2�� �ڿ� ����
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
        // OnEnable �Լ����� 2�� ����� ����� �־
        // Ȱ��ȭ �ǰ����� ����ǰ� ����
        if (!gameObject.activeSelf)
        {
            return;
        }
        Rigidbody2D rigid = GetComponent<Rigidbody2D>();
        rigid.velocity = Vector2.zero;

        // 3�� �� �������� ����
        Invoke("BossPattern", 3f);
    }

    void BossPattern()
    {
        // ���� 4���� ������
        // �ε����� 3 �� �� 0���� �ʱ�ȭ ���ְ� �ƴϸ� ������
        patternIdx = patternIdx == 3 ? 0 : patternIdx + 1;
        // �������� Ƚ�� �ʱ�ȭ
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
        // ������Ʈ Ǯ ����
        GameObject bulletRR = ObjManager.Instance.MakeObj("BulletBossB");
        GameObject bulletR = ObjManager.Instance.MakeObj("BulletBossB");
        GameObject bulletL = ObjManager.Instance.MakeObj("BulletBossB");
        GameObject bulletLL = ObjManager.Instance.MakeObj("BulletBossB");
        bulletRR.transform.position = transform.position + Vector3.right * 0.8f;
        bulletR.transform.position = transform.position + Vector3.right * 0.5f;
        bulletL.transform.position = transform.position + Vector3.right * -0.5f;
        bulletLL.transform.position = transform.position + Vector3.right * -0.8f;

        // ������ ������Ʈ�� ������ٵ� ������Ʈ �ҷ�����
        Rigidbody2D rigidRR = bulletRR.GetComponent<Rigidbody2D>();
        Rigidbody2D rigidR = bulletR.GetComponent<Rigidbody2D>();
        Rigidbody2D rigidL = bulletL.GetComponent<Rigidbody2D>();
        Rigidbody2D rigidLL = bulletLL.GetComponent<Rigidbody2D>();

        rigidRR.AddForce(Vector2.down * 6f, ForceMode2D.Impulse);
        rigidR.AddForce(Vector2.down * 6f, ForceMode2D.Impulse);
        rigidL.AddForce(Vector2.down * 6f, ForceMode2D.Impulse);
        rigidLL.AddForce(Vector2.down * 6f, ForceMode2D.Impulse);

        // �������� ���� ȸ�� ����
        currPatternCount++;

        // ���� ���� Ƚ���� �ִ� ���� �ݺ� Ƚ������ ������
        if (currPatternCount < maxPatternCount[patternIdx])
        {
            // �ڱ��ڽ� �ٽ� ����
            Invoke("BossFireFoward", 2f);
        }
        else // ������
        {
            // ���� �ٲٱ� ���� �������� �Լ� �ҷ���
            Invoke("BossPattern", 3f);
        }
    }

    void BossFireShot()
    {
        // Shot Gun 5 Bullet
        // 5�� ���
        for(int i = 0; i < 5; i++)
        {
            // ������Ʈ Ǯ ����
            GameObject bullet = ObjManager.Instance.MakeObj("BulletEnemyB");
            bullet.transform.position = transform.position;
            bullet.transform.rotation = transform.rotation;

            // ������ ������Ʈ�� ������ٵ� ������Ʈ �ҷ�����
            Rigidbody2D rigid = bullet.GetComponent<Rigidbody2D>();

            // ��ǥ��ġ - ������ġ = ��ǥ���� * ũ��
            // normalized �� ���⸸ �����
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
        // ������Ʈ Ǯ ����
        GameObject bullet = ObjManager.Instance.MakeObj("BulletEnemyA");
        bullet.transform.position = transform.position;
        bullet.transform.rotation = Quaternion.identity;

        // ������ ������Ʈ�� ������ٵ� ������Ʈ �ҷ�����
        Rigidbody2D rigid = bullet.GetComponent<Rigidbody2D>();

        // Mathf.Sin ���� ����ü�� x�� ������ �������� �ٲ�� ���ش�.
        // Mathf.PI ���̰� * 10 ����ü ����? 2���̴� 1���̴ϱ� * currPatternCount / maxPatternCount[patternIdx] 99�� �򲨴ϱ� for�� ���� �ε�����ȣ ��Ȱ��
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
        // ���� ���� Ƚ���� ���� roundNumA �� roundNumB �� �Ѿ� ���� ������
        int roundNum = currPatternCount % 2 == 0 ? roundNumA : roundNumB;

        for (int i = 0; i < roundNum; i++)
        {
            // ������Ʈ Ǯ ����
            GameObject bullet = ObjManager.Instance.MakeObj("BulletBossA");
            bullet.transform.position = transform.position;
            bullet.transform.rotation = Quaternion.identity;

            // ������ ������Ʈ�� ������ٵ� ������Ʈ �ҷ�����
            Rigidbody2D rigid = bullet.GetComponent<Rigidbody2D>();

            // �������� ���� �𸣰����ϱ� �׳� �ܿ��߰ڴ�
            Vector2 dir = new Vector2(Mathf.Cos(Mathf.PI * 2 * i / roundNum), Mathf.Sin(Mathf.PI * 2 * i / roundNum));

            rigid.AddForce(dir.normalized * 1.5f, ForceMode2D.Impulse);

            // �Ѿ� ���� ���ߴ� ����
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
        // �����̽ð��� ������ �ʾ��� ���
        if (currShotDelay < maxShotDelay)
        {
            return; // ����
        }

        if (enemyName == "S")
        {
            // ������ ���� (������Ʈ, ������ġ, ������ ȸ����)
            //GameObject bullet = Instantiate(bulletObjA, transform.position, transform.rotation);

            // ������Ʈ Ǯ ����
            GameObject bullet = ObjManager.Instance.MakeObj("BulletEnemyA");
            bullet.transform.position = transform.position;
            bullet.transform.rotation = transform.rotation;

            // ������ ������Ʈ�� ������ٵ� ������Ʈ �ҷ�����
            Rigidbody2D rigid = bullet.GetComponent<Rigidbody2D>();

            // ��ǥ��ġ - ������ġ = ��ǥ���� * ũ��
            // normalized �� ���⸸ �����
            Vector3 dir = player.transform.position - transform.position;
            rigid.AddForce(dir.normalized * (speed + 0.5f), ForceMode2D.Impulse);
        }
        else if (enemyName == "L")
        {
            // ������ ���� (������Ʈ, ������ġ, ������ ȸ����)
            //GameObject bulletR = Instantiate(bulletObjB, transform.position + (Vector3.right * 0.3f), transform.rotation);
            //GameObject bulletL = Instantiate(bulletObjB, transform.position + (Vector3.left * 0.3f), transform.rotation);

            // ������Ʈ Ǯ ����
            GameObject bulletR = ObjManager.Instance.MakeObj("BulletEnemyB");
            GameObject bulletL = ObjManager.Instance.MakeObj("BulletEnemyB");
            bulletR.transform.position = transform.position + Vector3.right * 0.3f;
            bulletL.transform.position = transform.position + Vector3.right * -0.3f;
            bulletR.transform.rotation = transform.rotation;
            bulletL.transform.rotation = transform.rotation;

            // ������ ������Ʈ�� ������ٵ� ������Ʈ �ҷ�����
            Rigidbody2D rigidR = bulletR.GetComponent<Rigidbody2D>();
            Rigidbody2D rigidL = bulletL.GetComponent<Rigidbody2D>();

            Vector3 dirR = player.transform.position - (transform.position + (Vector3.right * 0.3f));
            Vector3 dirL = player.transform.position - (transform.position + (Vector3.left * 0.3f));

            rigidR.AddForce(dirR.normalized * (speed + 1), ForceMode2D.Impulse);
            rigidL.AddForce(dirL.normalized * (speed + 1), ForceMode2D.Impulse);
        }

        // �߻� �� �� ������ �ʱ�ȭ
        currShotDelay = 0;
    }

    void Reload()
    {
        // Time.deltaTime ��ŭ ������ ����
        currShotDelay += Time.deltaTime;
    }

    public void OnHit(int dmg)
    {
        if (health <= 0)
        {
            return;
        }
        // ü�¿� ��������ŭ ����
        health -= dmg;
        if (enemyName == "B")
        {
            anim.SetTrigger(hashOnHit);
        }
        else
        {
            // ���� ��������Ʈ�� 2��° ��������Ʈ��
            spriteRenderer.sprite = sprites[1];
            // Invoke("ReturnSprite", 0.1f) : 0.1�� �� ReturnSprite() ȣ��
            Invoke("ReturnSprite", 0.1f);
        }

        // ü���� 0�� ���ų� ���� ���
        if (health <= 0)
        {
            // �÷��̾� ��ũ��Ʈ ȣ��
            Player playerLogic = player.GetComponent<Player>();
            // �÷��̾� ��ũ��Ʈ�� ������ enemyScore ������
            playerLogic.score += enemyScore;

            // #.Random Ratio Item Drop
            // ���� ���� �� ��� ������ �ȶ���Ʈ���� ���׿����� ���
            int rand = enemyName == "B" ? 0 : Random.Range(0, 100);
            if (rand < 50)
            {
                Debug.Log("���� ����");
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

            // ���� ������Ʈ ����
            //Destroy(gameObject);

            // ������Ʈ Ǯ�� ����
            // ���� ������Ʈ ��Ȱ��ȭ
            gameObject.SetActive(false);
            // ���� �ϱ� �� ���� �ʱ�ȭ
            transform.rotation = Quaternion.identity;
        }
    }
    // �ǰ� �� ��������Ʈ �ʱ�ȭ
    void ReturnSprite()
    {
        spriteRenderer.sprite = sprites[0];
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // ���� ������ ���ֱ� && ������ �ƴ� ����
        if (collision.CompareTag("BORDERBULLET") && enemyName != "B")
        {
            // ������Ʈ Ǯ�� ����
            // ���� ������Ʈ ��Ȱ��ȭ
            gameObject.SetActive(false);
            // ���� �ϱ� �� ���� �ʱ�ȭ
            transform.rotation = Quaternion.identity;
        }
        // �÷��̾� �Ѿ˿� ������ OnHit �Լ� ȣ��
        else if (collision.CompareTag("PLAYERBULLET"))
        {
            // ����� �浹ü�� Bullet ��ũ��Ʈ ������Ʈ ������
            Bullet bullet = collision.GetComponent<Bullet>();
            // �Ű������� Bullet ��ũ��Ʈ�� ������ ���� ����
            OnHit(bullet.dmg);

            // �浹ü(�Ѿ�) ����
            //Destroy(collision.gameObject);

            // ������Ʈ Ǯ�� ����
            // �Ѿ� ������Ʈ ��Ȱ��ȭ
            collision.gameObject.SetActive(false);
        }
    }
}
