using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    static GameManager instance; // 전역으로 데이터컨트롤러 생성

    // instance 객체로 DataController 찾아서 넣기
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

    // 프로퍼티로 PlayerPrefs 에 자료를 바로 저장/불러오기
    public float pHP
    {
        get
        {
            if (!PlayerPrefs.HasKey("PlayerHP"))
            {
                return 100f;
            }

            float tmpHP = PlayerPrefs.GetFloat("PlayerHP", 100f); // 저장된 값이 없으면 100f 반환
            return tmpHP;
        }
        set
        {
            PlayerPrefs.SetFloat("PlayerHP", value);
        }
    }
    
    // 스폰 지점
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
