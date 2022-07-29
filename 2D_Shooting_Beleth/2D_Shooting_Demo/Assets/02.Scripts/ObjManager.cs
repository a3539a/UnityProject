using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjManager : MonoBehaviour
{
    static ObjManager instance; // �������� ��������Ʈ�ѷ� ����

    // instance ��ü�� GameManager ã�Ƽ� �ֱ�
    public static ObjManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<ObjManager>();

                if (instance == null) // ������ �����
                {
                    GameObject container = new GameObject("ObjectManager");

                    instance = container.AddComponent<ObjManager>();
                }
            }
            return instance;
        }
    }

    // ����� �����յ�
    public GameObject enemyLPrefab;
    public GameObject enemyMPrefab;
    public GameObject enemySPrefab;
    public GameObject bossPrefab;
    public GameObject itemCoinPrefab;
    public GameObject itemPowerPrefab;
    public GameObject itemBoomPrefab;
    public GameObject bulletPlayerAPrefab;
    public GameObject bulletPlayerBPrefab;
    public GameObject bulletEnemyAPrefab;
    public GameObject bulletEnemyBPrefab;
    public GameObject bulletBossAPrefab;
    public GameObject bulletBossBPrefab;
    public GameObject bulletFollowerPrefab;

    // ������Ʈ Ǯ �迭
    GameObject[] enemyL;
    GameObject[] enemyM;
    GameObject[] enemyS;
    GameObject[] boss;

    // �迭�� ũ��
    public int enemyLMaxPool = 5;
    public int enemyMMaxPool = 3;
    public int enemySMaxPool = 10;

    GameObject[] itemCoin;
    GameObject[] itemPower;
    GameObject[] itemBoom;

    public int itemMaxPool = 10;

    GameObject[] bulletPlayerA;
    GameObject[] bulletPlayerB;
    GameObject[] bulletEnemyA;
    GameObject[] bulletEnemyB;
    GameObject[] bulletBossA;
    GameObject[] bulletBossB;
    GameObject[] bulletFollower;

    public int bulletMaxPool = 10;
    public int bulletBossMaxPool = 10;

    // ���Կ� �迭
    GameObject[] targetPool;

    private void Awake()
    {
        // �迭 ����
        enemyL = new GameObject[enemyLMaxPool];
        enemyM = new GameObject[enemyMMaxPool];
        enemyS = new GameObject[enemySMaxPool];
        boss = new GameObject[1];

        itemCoin = new GameObject[itemMaxPool];
        itemPower = new GameObject[itemMaxPool];
        itemBoom = new GameObject[itemMaxPool];

        bulletPlayerA = new GameObject[bulletMaxPool];
        bulletPlayerB = new GameObject[bulletMaxPool];
        bulletEnemyA = new GameObject[bulletMaxPool];
        bulletEnemyB = new GameObject[bulletMaxPool];
        bulletBossA = new GameObject[bulletMaxPool];
        bulletBossB = new GameObject[bulletBossMaxPool];
        bulletFollower = new GameObject[bulletMaxPool];

        Generate();
    }

    // �迭�� �ִ� ������Ʈ ����
    void Generate()
    {
        // 1) Enemy
        for (int idx = 0; idx < enemyL.Length; idx++)
        {
            enemyL[idx] = Instantiate(enemyLPrefab);
            enemyL[idx].SetActive(false);
        }
        for (int idx = 0; idx < enemyM.Length; idx++)
        {
            enemyM[idx] = Instantiate(enemyMPrefab);
            enemyM[idx].SetActive(false);
        }
        for (int idx = 0; idx < enemyS.Length; idx++)
        {
            enemyS[idx] = Instantiate(enemySPrefab);
            enemyS[idx].SetActive(false);
        }
        for (int idx = 0; idx < boss.Length; idx++)
        {
            boss[idx] = Instantiate(bossPrefab);
            boss[idx].SetActive(false);
        }

        // 2) Item
        for (int idx = 0; idx < itemCoin.Length; idx++)
        {
            itemCoin[idx] = Instantiate(itemCoinPrefab);
            itemCoin[idx].SetActive(false);
        }
        for (int idx = 0; idx < itemPower.Length; idx++)
        {
            itemPower[idx] = Instantiate(itemPowerPrefab);
            itemPower[idx].SetActive(false);
        }
        for (int idx = 0; idx < itemBoom.Length; idx++)
        {
            itemBoom[idx] = Instantiate(itemBoomPrefab);
            itemBoom[idx].SetActive(false);
        }

        // 3) Bullet
        for (int idx = 0; idx < bulletPlayerA.Length; idx++)
        {
            bulletPlayerA[idx] = Instantiate(bulletPlayerAPrefab);
            bulletPlayerA[idx].SetActive(false);
        }
        for (int idx = 0; idx < bulletPlayerB.Length; idx++)
        {
            bulletPlayerB[idx] = Instantiate(bulletPlayerBPrefab);
            bulletPlayerB[idx].SetActive(false);
        }
        for (int idx = 0; idx < bulletEnemyA.Length; idx++)
        {
            bulletEnemyA[idx] = Instantiate(bulletEnemyAPrefab);
            bulletEnemyA[idx].SetActive(false);
        }
        for (int idx = 0; idx < bulletEnemyB.Length; idx++)
        {
            bulletEnemyB[idx] = Instantiate(bulletEnemyBPrefab);
            bulletEnemyB[idx].SetActive(false);
        }
        for (int idx = 0; idx < bulletBossA.Length; idx++)
        {
            bulletBossA[idx] = Instantiate(bulletBossAPrefab);
            bulletBossA[idx].SetActive(false);
        }
        for (int idx = 0; idx < bulletBossB.Length; idx++)
        {
            bulletBossB[idx] = Instantiate(bulletBossBPrefab);
            bulletBossB[idx].SetActive(false);
        }
        for (int idx = 0; idx < bulletFollower.Length; idx++)
        {
            bulletFollower[idx] = Instantiate(bulletFollowerPrefab);
            bulletFollower[idx].SetActive(false);
        }
    }

    // ������Ʈ ���� �Լ�
    public GameObject MakeObj(string type)
    {
        // �Ű����� type���� �����Ǵ� ������Ʈ�� �������ش�.
        switch (type)
        {
            // type �� EnemyL �� ���
            case "EnemyL":
                // targetPool �迭�� enemyL �迭 ����
                targetPool = enemyL;
                break;
            case "EnemyM":
                targetPool = enemyM;
                break;
            case "EnemyS":
                targetPool = enemyS;
                break;
            case "Boss":
                targetPool = boss;
                break;
            case "ItemCoin":
                targetPool = itemCoin;
                break;
            case "ItemPower":
                targetPool = itemPower;
                break;
            case "ItemBoom":
                targetPool = itemBoom;
                break;
            case "BulletPlayerA":
                targetPool = bulletPlayerA;
                break;
            case "BulletPlayerB":
                targetPool = bulletPlayerB;
                break;
            case "BulletEnemyA":
                targetPool = bulletEnemyA;
                break;
            case "BulletEnemyB":
                targetPool = bulletEnemyB;
                break;
            case "BulletBossA":
                targetPool = bulletBossA;
                break;
            case "BulletBossB":
                targetPool = bulletBossB;
                break;
            case "BulletFollower":
                targetPool = bulletFollower;
                break;
        }

        // targetPool�� ���ԵǾ� �ִ� �迭�� ������Ʈ�� Ȱ��ȭ
        for (int idx = 0; idx < targetPool.Length; idx++)
        {
            // targetPool[idx].activeSelf : �ش� ������Ʈ�� Ȱ��ȭ �Ǿ� ���� �� true �� ��ȯ
            // ! �ٿ��� ��Ȱ��ȭ �� �� true ����
            if (!targetPool[idx].activeSelf)
            {
                // ������Ʈ�� �ѱ�
                targetPool[idx].SetActive(true);
                // ������Ʈ�� ��ȯ
                return targetPool[idx];
            }
        }

        // ���� �ȵɶ� null ��ȯ
        return null;
    }

    public GameObject[] GetPool(string type)
    { 
        // �Ű����� type���� �����Ǵ� ������Ʈ�� �������ش�.
        switch (type)
        {
            // type �� EnemyL �� ���
            case "EnemyL":
                // targetPool �迭�� enemyL �迭 ����
                targetPool = enemyL;
                break;
            case "EnemyM":
                targetPool = enemyM;
                break;
            case "EnemyS":
                targetPool = enemyS;
                break;
            case "Boss":
                targetPool = boss;
                break;
            case "ItemCoin":
                targetPool = itemCoin;
                break;
            case "ItemPower":
                targetPool = itemPower;
                break;
            case "ItemBoom":
                targetPool = itemBoom;
                break;
            case "BulletPlayerA":
                targetPool = bulletPlayerA;
                break;
            case "BulletPlayerB":
                targetPool = bulletPlayerB;
                break;
            case "BulletEnemyA":
                targetPool = bulletEnemyA;
                break;
            case "BulletEnemyB":
                targetPool = bulletEnemyB;
                break;
            case "BulletBossA":
                targetPool = bulletBossA;
                break;
            case "BulletBossB":
                targetPool = bulletBossB;
                break;
            case "BulletFollower":
                targetPool = bulletFollower;
                break;
        }
        return targetPool;
    }
}
