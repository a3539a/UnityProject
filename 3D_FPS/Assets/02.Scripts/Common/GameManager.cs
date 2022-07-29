using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("�� ������ ���� ����")]
    public Transform[] points; // ������ ��ġ
    public GameObject enemy;
    public float createTime = 2f; // ���� �ֱ�
    public int maxEnemy = 10; // �ִ� ���� ���� ��

    public bool isGameOver = false;

    public static GameManager instance; // �̱��� ���� ����

    [Header("������Ʈ Ǯ ���� ����")]
    public GameObject bulletPrefab;
    public int maxPool = 10;
    // ������Ʈ Ǯ�� ����Ʈ
    // �Ѿ��� �̸� ���� �ش� ����Ʈ�� �߰� �ص� ����
    public List<GameObject> bulletPool = new List<GameObject>();

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        // instance�� �Ҵ�� Ŭ������ �ν��Ͻ��� �ٸ����
        else if (instance != this)
        {
            Destroy(gameObject);
        }
        // ���� ����Ǵ��� �ش� ������Ʈ�� ����
        // ������ ���� �ɶ�����
        DontDestroyOnLoad(gameObject);

        CreatePool(); // ������Ʈ Ǯ ���� �Լ� ȣ��
    }

    public void CreatePool() // ������Ʈ Ǯ ���� �Լ�
    {
        // ���̾��Ű�� �ش� �̸��� �� ������Ʈ ����
        GameObject objectsPool = new GameObject("ObjectsPool");
        for(int i = 0; i < maxPool; i++)
        {
            // �Ѿ��� ���� �����ؼ� ������ ���� objectsPool �� ��ġ�� ����
            // �� ������ �ڽ����� �߰�
            var obj = Instantiate(bulletPrefab, objectsPool.transform);
            obj.name = "Bullet_" + i.ToString("00");
            obj.SetActive(false);// ������ �Ѿ��� �켱 ��Ȱ��ȭ

            bulletPool.Add(obj); // ����Ʈ�� ������ �Ѿ� �߰�
        }
    }

    // ����Ʈ�� �ִ� �Ѿ˵� ��
    // ���� ��밡���� �Ѿ��� ������ͼ� ��ȯ�ϴ� �Լ�
    public GameObject GetBullet()
    {
        foreach(GameObject bullet in bulletPool)
        {
            // ������ �Ѿ��� ��Ȱ��ȭ �Ǿ� �ִ��� Ȯ��
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

        if(points.Length > 0) // ������ ����Ʈ�� ���� �Ѵٸ�
        {
            // ������ �ڷ�ƾ �Լ� ȣ��
            StartCoroutine(CreateEnemy());
        }
    }

    IEnumerator CreateEnemy()
    {
        // ���ӿ����� �ƴ� ��� �ݺ�
        while (!isGameOver)
        {
            int enemyCount = GameObject.FindGameObjectsWithTag("ENEMY").Length;

            // ������ ���� ������ ������ �ִ� �������� ���� �� ����
            if(enemyCount < maxEnemy)
            {
                yield return new WaitForSeconds(createTime);

                // ������ ��ġ�� �߿��� ������ ���� �����ϱ� ����
                // ���� �ε��� ����
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
