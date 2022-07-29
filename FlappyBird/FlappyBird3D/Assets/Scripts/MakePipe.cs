using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MakePipe : MonoBehaviour
{
    public GameObject pipe; // pipe ������ ����ϱ� ���� ����

    float nowTime; // ���� �ð� üũ��
    float makeTime = 2f; // ���� ���ݿ� �ð� (2��)

    public Text ScoreBoardText;
    public Text ScoreText;
    int score = 0;
    int bestScore = 0;
    float scoreTime;

    void Start()
    {
        nowTime = Time.time; // ���� �������� �� �ʱ� �ð� ����
        scoreTime = Time.time + 2;
        
        bestScore = PlayerPrefs.GetInt("bestScore", 0);
        ScoreBoardText.text = "BEST : <color=#FFFFFF>" + bestScore + "</color>";
    }

    void Update()
    {
        if (Time.time - nowTime > makeTime)
        {
            // 2�� ��� �߱� ������ nowTime �� ���� �ð����� �ʱ�ȭ
            nowTime = Time.time;

            // Instantiate �Լ��� �������� Ȱ���Ͽ� �������� �ϴ� �Լ�
            // Instantiate(������, ��ġ, ȸ����) �� �Ϲ��� �����
            GameObject _pipe = Instantiate(pipe);
            // ������ �������� �θ� grounds ������Ʈ�� ����
            _pipe.transform.parent = transform;

            // Range(int Min, int Max) �� ��� Min <= �� < Max
            // Range(float Min, float Max) �� ��� Min <= �� <= Max
            float randomY = Random.Range(-2.5f, 5.5f);

            // Random.Range 1 <= �� < 7
            //_pipe.transform.localPosition = new Vector3(Random.Range(1, 7) + 5, 0, 0);
            _pipe.transform.localPosition = new Vector3(-transform.localPosition.x + 5, randomY, 0);

            _pipe.transform.localScale = new Vector3(1, 1, 1);

            // Destroy(�ı��� ������Ʈ, �ð�)
            Destroy(_pipe, 8f);
        }

        if(Time.time - scoreTime > 2)
        {
            scoreTime = Time.time;
            score += 10;
            ScoreText.text = "SCORE : <color=#FFCD00>" + score + "</color>"; // ���ڿ��� ��ȯ

            // �� ���� ������ �ְ��� ���� ���� ��
            if (score > bestScore)
            {
                bestScore = score;
                ScoreBoardText.text = "BEST : <color=#FFFFFF>" + bestScore + "</color>";
            }
        }
    }

    public void SaveScore()
    {
        // Ű, ������ �� ����
        // Ű�� ������ ��й�ȣ�� ���� ����
        PlayerPrefs.SetInt("bestScore", bestScore);
    }
}
