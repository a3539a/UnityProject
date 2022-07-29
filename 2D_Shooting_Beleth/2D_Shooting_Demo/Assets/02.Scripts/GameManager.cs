using System.Collections;
using System.Collections.Generic;
// 외부 파일 읽어들일 때 using System.IO;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    static GameManager instance; // 전역으로 데이터컨트롤러 생성

    // instance 객체로 GameManager 찾아서 넣기
    public static GameManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<GameManager>();

                if (instance == null) // 없으면 만들기
                {
                    GameObject container = new GameObject("GameManager");

                    instance = container.AddComponent<GameManager>();
                }
            }
            return instance;
        }
    }

    // 생성할 적 오브젝트 배열
    public string[] enemyObjs;
    // 생성 위치
    public Transform[] spwanPoints;

    // 플레이어
    public GameObject player;

    public Text scoreText;
    public Text boomCount;
    public Image[] lifeImg;
    public GameObject gameOverSet;

    // 적들의 스폰 지연시간
    public float nextSpawnDelay;
    public float currSpawnDelay;

    // 구조체 리스트
    public List<Spawn> spawnList;
    // 구조체 리스트 인덱스번호
    public int spawnIdx;
    // 소환 끝났을 때 불 체크
    public bool spawnEnd;

    private void Awake()
    {
        spawnList = new List<Spawn>();
        enemyObjs = new string[] { "EnemyS" , "EnemyM" , "EnemyL", "Boss" };
        ReadSpawnFile();
    }

    void ReadSpawnFile()
    {
        // 1) 변수 초기화
        spawnList.Clear();
        spawnIdx = 0;
        spawnEnd = false;

        // 2) 리스폰 파일 불러오기
        // Resources 폴더 안에 Stage0 파일 읽기
        // as TextAsset : 텍스트 파일이 아닐 경우 null 반환
        TextAsset textFile = Resources.Load("Stage0") as TextAsset;
        // 문자 읽어주는 클래스 new StringReader(읽을파일의 text)
        StringReader stringReader = new StringReader(textFile.text);

        while(stringReader != null)
        {
            // 3) 리스폰 데이터 생성
            // ReadLine() : 1줄 읽는 함수
            string line = stringReader.ReadLine();
            Debug.Log(line);



            // 읽을 자료가 없으면 멈춤
            if(line == null)
            {
                break;
            }

            // Spawn 스크립트 변수 자료 불러오기
            Spawn spawnData = new Spawn();
            // line.Split(',') : , 를 기준으로 문자열 나눔
            spawnData.delay = float.Parse(line.Split(',')[0]);
            spawnData.type = line.Split(',')[1];
            spawnData.point = int.Parse(line.Split(',')[2]);
            spawnList.Add(spawnData);
        }
        // 4) 텍스트파일 닫기
        stringReader.Close();

        // 스폰이 끝난 후 첫번 째 스폰 딜레이 적용
        nextSpawnDelay = spawnList[0].delay;
    }

    private void Update()
    {
        // 스폰 딜레이에 델타타임 더하기
        currSpawnDelay += Time.deltaTime;

        // 스폰 시간이 되었을 경우
        if(currSpawnDelay > nextSpawnDelay && !spawnEnd)
        {
            // 적 생성함수 호출
            SpawnEnemy();
            // 현재 딜레이 초기화
            currSpawnDelay = 0;
        }

        // UI Score Update
        Player playerLogic = player.GetComponent<Player>();
        // scoreText.text 에 {0:n0} : 3자리수 마다 , 찍어주는 양식 으로 스코어 대입
        scoreText.text = string.Format("{0:n0}", playerLogic.score);
        boomCount.text = "X " + playerLogic.boom;
    }

    // 적 생성 함수
    void SpawnEnemy()
    {
        // 인덱스 변수 생성
        int enemyIdx = 0;
        // 타입별로 인덱스 번호 바꿔줌
        switch (spawnList[spawnIdx].type)
        {
            case "S":
                enemyIdx = 0;
                break;
            case "M":
                enemyIdx = 1;
                break;
            case "L":
                enemyIdx = 2;
                break;
            case "B":
                enemyIdx = 3;
                break;
        }
        // 구조체 리스트에 기입한 point
        int enemyPoint = spawnList[spawnIdx].point;

        // 적오브젝트, 생성위치 배열의 값들로 생성
        // 오브젝트 매니저의 MakeObj() 함수로 적 생성
        GameObject enemy = ObjManager.Instance.MakeObj(enemyObjs[enemyIdx]);
        enemy.transform.position = spwanPoints[enemyPoint].position;

        // 생성과 동시에 벡터값 부여 하기 위해
        // 생성 오브젝트의 컴포넌트들 가져옴
        Rigidbody2D rigid = enemy.GetComponent<Rigidbody2D>();
        // Enemy 스크립트의 speed 값 불러오기 위해
        Enemy enemyLogic = enemy.GetComponent<Enemy>();
        // 적 생성 직후에 플레이어 변수에 넘겨준다.
        enemyLogic.player = player;

        if (enemyPoint == 5 || enemyPoint == 7) // Left Point
        {
            // 회전 값 2D에서는 Z축만 쓰인다
            // Quaternion.Euler(x축, y축, z축)
            enemy.transform.rotation = Quaternion.Euler(0, 0, 60f);
            // 가져온 리지드바디에 방향 * 속도 주기
            rigid.velocity = new Vector2(enemyLogic.speed, enemyLogic.speed * (-0.5f));
        }
        else if(enemyPoint == 6 || enemyPoint == 8) // Right Point
        {
            enemy.transform.rotation = Quaternion.Euler(0, 0, -60f);
            rigid.velocity = new Vector2(enemyLogic.speed * (-1), enemyLogic.speed * (-0.5f));
        }
        else // Top point
        {
            rigid.velocity = new Vector2(0, enemyLogic.speed * (-1));
        }

        // 리스폰 인덱스 증가
        spawnIdx++;
        // 리스트 끝까지 소환 했으면
        if (spawnIdx == spawnList.Count)
        {
            // 불 체크 true
            spawnEnd = true;
            return; // 해주고 종료
        }

        // 다음 리스폰 딜레이 갱신
        nextSpawnDelay = spawnList[spawnIdx].delay;
    }

    // 플레이어 목숨 아이콘 출력 함수
    public void UpdateLifeIcon(int life)
    {
        // UI Life Icon Disabled
        for(int index = 0; index < 3; index++)
        {
            // Icon 3개의 투명도를 0으로 만듬
            lifeImg[index].color = new Color(1, 1, 1, 0);
        }

        // UI Life Icon Enabled
        for(int index = 0; index < life; index++)
        {
            // 현재 라이프만큼 이미지의 투명도를 1로 만듬
            lifeImg[index].color = new Color(1, 1, 1, 1);
        }
    }

    // 2초 시간차를 두기 위해 함수를 따로 설정
    // ㅅㅂ 코루틴 쓰면 되자나
    public void RespawnPlayer()
    {
        // 2초 뒤에 RespawnPlayerExe() 함수 호출
        Invoke("RespawnPlayerExe", 2f);
    }    

    // 플레이어 살려주기 함수
    public void RespawnPlayerExe()
    {
        // 플레이어 오브젝트의 위치를 y -3.75 로 맞춤
        player.transform.position = Vector3.down * 3.75f;
        // 오브젝트 켜준다
        player.SetActive(true);

        // 플레이어가 리스폰 될 때, 데미지 체크 false로 초기화
        Player playerLogic = player.GetComponent<Player>();
        playerLogic.isDamaged = false;
    }

    // 게임오버 함수
    public void GameOver()
    {
        gameOverSet.SetActive(true);
    }

    // 게임 재시작 함수, Button UI에 OnClick() 이벤트에 할당해준다.
    public void GameRetry()
    {
        SceneManager.LoadScene(0);
    }
}
