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
                // ������ ������Ʈ�� �Ҹ�(���� ���� �������� �Ҹ�X)
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
