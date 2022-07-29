using System.Collections;
using System.Collections.Generic;
// �ܺ� ���� �о���� �� using System.IO;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    static GameManager instance; // �������� ��������Ʈ�ѷ� ����

    // instance ��ü�� GameManager ã�Ƽ� �ֱ�
    public static GameManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<GameManager>();

                if (instance == null) // ������ �����
                {
                    GameObject container = new GameObject("GameManager");

                    instance = container.AddComponent<GameManager>();
                }
            }
            return instance;
        }
    }

    // ������ �� ������Ʈ �迭
    public string[] enemyObjs;
    // ���� ��ġ
    public Transform[] spwanPoints;

    // �÷��̾�
    public GameObject player;

    public Text scoreText;
    public Text boomCount;
    public Image[] lifeImg;
    public GameObject gameOverSet;

    // ������ ���� �����ð�
    public float nextSpawnDelay;
    public float currSpawnDelay;

    // ����ü ����Ʈ
    public List<Spawn> spawnList;
    // ����ü ����Ʈ �ε�����ȣ
    public int spawnIdx;
    // ��ȯ ������ �� �� üũ
    public bool spawnEnd;

    private void Awake()
    {
        spawnList = new List<Spawn>();
        enemyObjs = new string[] { "EnemyS" , "EnemyM" , "EnemyL", "Boss" };
        ReadSpawnFile();
    }

    void ReadSpawnFile()
    {
        // 1) ���� �ʱ�ȭ
        spawnList.Clear();
        spawnIdx = 0;
        spawnEnd = false;

        // 2) ������ ���� �ҷ�����
        // Resources ���� �ȿ� Stage0 ���� �б�
        // as TextAsset : �ؽ�Ʈ ������ �ƴ� ��� null ��ȯ
        TextAsset textFile = Resources.Load("Stage0") as TextAsset;
        // ���� �о��ִ� Ŭ���� new StringReader(���������� text)
        StringReader stringReader = new StringReader(textFile.text);

        while(stringReader != null)
        {
            // 3) ������ ������ ����
            // ReadLine() : 1�� �д� �Լ�
            string line = stringReader.ReadLine();
            Debug.Log(line);



            // ���� �ڷᰡ ������ ����
            if(line == null)
            {
                break;
            }

            // Spawn ��ũ��Ʈ ���� �ڷ� �ҷ�����
            Spawn spawnData = new Spawn();
            // line.Split(',') : , �� �������� ���ڿ� ����
            spawnData.delay = float.Parse(line.Split(',')[0]);
            spawnData.type = line.Split(',')[1];
            spawnData.point = int.Parse(line.Split(',')[2]);
            spawnList.Add(spawnData);
        }
        // 4) �ؽ�Ʈ���� �ݱ�
        stringReader.Close();

        // ������ ���� �� ù�� ° ���� ������ ����
        nextSpawnDelay = spawnList[0].delay;
    }

    private void Update()
    {
        // ���� �����̿� ��ŸŸ�� ���ϱ�
        currSpawnDelay += Time.deltaTime;

        // ���� �ð��� �Ǿ��� ���
        if(currSpawnDelay > nextSpawnDelay && !spawnEnd)
        {
            // �� �����Լ� ȣ��
            SpawnEnemy();
            // ���� ������ �ʱ�ȭ
            currSpawnDelay = 0;
        }

        // UI Score Update
        Player playerLogic = player.GetComponent<Player>();
        // scoreText.text �� {0:n0} : 3�ڸ��� ���� , ����ִ� ��� ���� ���ھ� ����
        scoreText.text = string.Format("{0:n0}", playerLogic.score);
        boomCount.text = "X " + playerLogic.boom;
    }

    // �� ���� �Լ�
    void SpawnEnemy()
    {
        // �ε��� ���� ����
        int enemyIdx = 0;
        // Ÿ�Ժ��� �ε��� ��ȣ �ٲ���
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
        // ����ü ����Ʈ�� ������ point
        int enemyPoint = spawnList[spawnIdx].point;

        // ��������Ʈ, ������ġ �迭�� ����� ����
        // ������Ʈ �Ŵ����� MakeObj() �Լ��� �� ����
        GameObject enemy = ObjManager.Instance.MakeObj(enemyObjs[enemyIdx]);
        enemy.transform.position = spwanPoints[enemyPoint].position;

        // ������ ���ÿ� ���Ͱ� �ο� �ϱ� ����
        // ���� ������Ʈ�� ������Ʈ�� ������
        Rigidbody2D rigid = enemy.GetComponent<Rigidbody2D>();
        // Enemy ��ũ��Ʈ�� speed �� �ҷ����� ����
        Enemy enemyLogic = enemy.GetComponent<Enemy>();
        // �� ���� ���Ŀ� �÷��̾� ������ �Ѱ��ش�.
        enemyLogic.player = player;

        if (enemyPoint == 5 || enemyPoint == 7) // Left Point
        {
            // ȸ�� �� 2D������ Z�ุ ���δ�
            // Quaternion.Euler(x��, y��, z��)
            enemy.transform.rotation = Quaternion.Euler(0, 0, 60f);
            // ������ ������ٵ� ���� * �ӵ� �ֱ�
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

        // ������ �ε��� ����
        spawnIdx++;
        // ����Ʈ ������ ��ȯ ������
        if (spawnIdx == spawnList.Count)
        {
            // �� üũ true
            spawnEnd = true;
            return; // ���ְ� ����
        }

        // ���� ������ ������ ����
        nextSpawnDelay = spawnList[spawnIdx].delay;
    }

    // �÷��̾� ��� ������ ��� �Լ�
    public void UpdateLifeIcon(int life)
    {
        // UI Life Icon Disabled
        for(int index = 0; index < 3; index++)
        {
            // Icon 3���� ������ 0���� ����
            lifeImg[index].color = new Color(1, 1, 1, 0);
        }

        // UI Life Icon Enabled
        for(int index = 0; index < life; index++)
        {
            // ���� ��������ŭ �̹����� ������ 1�� ����
            lifeImg[index].color = new Color(1, 1, 1, 1);
        }
    }

    // 2�� �ð����� �α� ���� �Լ��� ���� ����
    // ���� �ڷ�ƾ ���� ���ڳ�
    public void RespawnPlayer()
    {
        // 2�� �ڿ� RespawnPlayerExe() �Լ� ȣ��
        Invoke("RespawnPlayerExe", 2f);
    }    

    // �÷��̾� ����ֱ� �Լ�
    public void RespawnPlayerExe()
    {
        // �÷��̾� ������Ʈ�� ��ġ�� y -3.75 �� ����
        player.transform.position = Vector3.down * 3.75f;
        // ������Ʈ ���ش�
        player.SetActive(true);

        // �÷��̾ ������ �� ��, ������ üũ false�� �ʱ�ȭ
        Player playerLogic = player.GetComponent<Player>();
        playerLogic.isDamaged = false;
    }

    // ���ӿ��� �Լ�
    public void GameOver()
    {
        gameOverSet.SetActive(true);
    }

    // ���� ����� �Լ�, Button UI�� OnClick() �̺�Ʈ�� �Ҵ����ش�.
    public void GameRetry()
    {
        SceneManager.LoadScene(0);
    }
}
