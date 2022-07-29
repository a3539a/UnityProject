using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjManager : MonoBehaviour
{
    static ObjManager instance; // 전역으로 데이터컨트롤러 생성

    // instance 객체로 GameManager 찾아서 넣기
    public static ObjManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<ObjManager>();

                if (instance == null) // 없으면 만들기
                {
                    GameObject container = new GameObject("ObjectManager");

                    instance = container.AddComponent<ObjManager>();
                }
            }
            return instance;
        }
    }

    // 등록할 프리팹들
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

    // 오브젝트 풀 배열
    GameObject[] enemyL;
    GameObject[] enemyM;
    GameObject[] enemyS;
    GameObject[] boss;

    // 배열의 크기
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

    // 대입용 배열
    GameObject[] targetPool;

    private void Awake()
    {
        // 배열 선언
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

    // 배열에 있는 오브젝트 생성
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

    // 오브젝트 생성 함수
    public GameObject MakeObj(string type)
    {
        // 매개변수 type으로 생성되는 오브젝트를 구분해준다.
        switch (type)
        {
            // type 이 EnemyL 일 경우
            case "EnemyL":
                // targetPool 배열에 enemyL 배열 대입
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

        // targetPool에 대입되어 있는 배열의 오브젝트들 활성화
        for (int idx = 0; idx < targetPool.Length; idx++)
        {
            // targetPool[idx].activeSelf : 해당 오브젝트가 활성화 되어 있을 때 true 값 반환
            // ! 붙여서 비활성화 일 때 true 받음
            if (!targetPool[idx].activeSelf)
            {
                // 오브젝트들 켜기
                targetPool[idx].SetActive(true);
                // 오브젝트들 반환
                return targetPool[idx];
            }
        }

        // 생성 안될땐 null 반환
        return null;
    }

    public GameObject[] GetPool(string type)
    { 
        // 매개변수 type으로 생성되는 오브젝트를 구분해준다.
        switch (type)
        {
            // type 이 EnemyL 일 경우
            case "EnemyL":
                // targetPool 배열에 enemyL 배열 대입
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
