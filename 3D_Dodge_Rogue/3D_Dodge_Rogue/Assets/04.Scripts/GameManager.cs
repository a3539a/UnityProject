using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    static GameManager instance; // �������� ��������Ʈ�ѷ� ����

    // instance ��ü�� DataController ã�Ƽ� �ֱ�
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

    // ������Ƽ�� PlayerPrefs �� �ڷḦ �ٷ� ����/�ҷ�����
    public float pHP
    {
        get
        {
            if (!PlayerPrefs.HasKey("PlayerHP"))
            {
                return 100f;
            }

            float tmpHP = PlayerPrefs.GetFloat("PlayerHP", 100f); // ����� ���� ������ 100f ��ȯ
            return tmpHP;
        }
        set
        {
            PlayerPrefs.SetFloat("PlayerHP", value);
        }
    }
    
    // ���� ����
    GameObject[] walls;
    public GameObject enemy;

    void Start()
    {
        StartCoroutine(EnemySpawn());
        walls = GameObject.FindGameObjectsWithTag("WALLS");
    }

    IEnumerator EnemySpawn()
    {
        int i = 0;

        while (true)
        {
            float randSpawn = Random.Range(-20f, 20f);

            yield return new WaitForSeconds(2f);
            i++;
            if (i % 2 == 0)
            {
                Instantiate(enemy, walls[i % 4].transform.localPosition + (randSpawn * Vector3.right), Quaternion.identity);
            }
            else
            {
                Instantiate(enemy, walls[i % 4].transform.localPosition + (randSpawn * Vector3.forward), Quaternion.identity);
            }
        }
    }
}
