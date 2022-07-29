using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    static GameManager instance;

    public static GameManager GetInstance()
    {
        if (instance == null)
        {
            instance = FindObjectOfType<GameManager>();
            if(instance == null)
            {
                GameObject container = new GameObject("GameManager");
                instance = container.AddComponent<GameManager>();
                // 생성된 오브젝트를 불멸(게임 종료 직전까지 소멸X)
                DontDestroyOnLoad(container);
            }
        }
        return instance;
    }

    public int totalPoint;
    public int stagePoint;
    public int stageIndex = 0;
    public int health;

    public int NextStage()
    {
        stageIndex++;
        totalPoint += stagePoint;
        stagePoint = 0;

        return stageIndex;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            health--;
            collision.transform.position = new Vector3(0, 0, -1);
        }
    }
}
