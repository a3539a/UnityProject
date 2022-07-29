using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("적 리스폰 관련 변수")]
    public Transform[] points; // 리스폰 위치
    public GameObject enemy;
    public float createTime = 2f; // 생성 주기
    public int maxEnemy = 10; // 최대 생성 마리 수

    public bool isGameOver = false;

    public static GameManager instance; // 싱글톤 접근 변수

    [Header("오브젝트 풀 관련 변수")]
    public GameObject bulletPrefab;
    public int maxPool = 10;
    // 오브젝트 풀용 리스트
    // 총알을 미리 만들어서 해당 리스트에 추가 해둘 것임
    public List<GameObject> bulletPool = new List<GameObject>();

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        // instance에 할당된 클래스의 인스턴스가 다를경우
        else if (instance != this)
        {
            Destroy(gameObject);
        }
        // 씬이 변경되더라도 해당 오브젝트를 유지
        // 게임이 종료 될때까지
        DontDestroyOnLoad(gameObject);

        CreatePool(); // 오브젝트 풀 생성 함수 호출
    }

    public void CreatePool() // 오브젝트 풀 생성 함수
    {
        // 하이어라키에 해당 이름의 빈 오브젝트 생성
        GameObject objectsPool = new GameObject("ObjectsPool");
        for(int i = 0; i < maxPool; i++)
        {
            // 총알을 동적 생성해서 위에서 만든 objectsPool 의 위치에 생성
            // 그 다음에 자식으로 추가
            var obj = Instantiate(bulletPrefab, objectsPool.transform);
            obj.name = "Bullet_" + i.ToString("00");
            obj.SetActive(false);// 생성된 총알은 우선 비활성화

            bulletPool.Add(obj); // 리스트에 생성한 총알 추가
        }
    }

    // 리스트에 있는 총알들 중
    // 실제 사용가능한 총알을 가지고와서 반환하는 함수
    public GameObject GetBullet()
    {
        foreach(GameObject bullet in bulletPool)
        {
            // 가져온 총알이 비활성화 되어 있는지 확인
            if(bullet.activeSelf == false)
            {
                return bullet;
            }
        }
        return null;
    }

    private void Start()
    {
        points = GameObject.Find("SpawnPointGroup").GetComponentsInChildren<Transform>();

        if(points.Length > 0) // 리스폰 포인트가 존재 한다면
        {
            // 리스폰 코루틴 함수 호출
            StartCoroutine(CreateEnemy());
        }
    }

    IEnumerator CreateEnemy()
    {
        // 게임오버가 아닐 경우 반복
        while (!isGameOver)
        {
            int enemyCount = GameObject.FindGameObjectsWithTag("ENEMY").Length;

            // 생성된 적의 수량이 지정된 최대 수량보다 작을 때 스폰
            if(enemyCount < maxEnemy)
            {
                yield return new WaitForSeconds(createTime);

                // 리스폰 위치들 중에서 랜덤한 곳을 추출하기 위한
                // 랜덤 인덱스 생성
                int idx = Random.Range(1, points.Length);

                Instantiate(enemy, points[idx].position, points[idx].rotation);
            }
            else
            {
                yield return null;
            }
        }
    }
}
