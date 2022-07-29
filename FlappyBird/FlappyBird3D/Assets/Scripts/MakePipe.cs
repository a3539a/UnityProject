using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MakePipe : MonoBehaviour
{
    public GameObject pipe; // pipe 프리팹 사용하기 위한 변수

    float nowTime; // 현재 시간 체크용
    float makeTime = 2f; // 생성 간격용 시간 (2초)

    public Text ScoreBoardText;
    public Text ScoreText;
    int score = 0;
    int bestScore = 0;
    float scoreTime;

    void Start()
    {
        nowTime = Time.time; // 게임 시작했을 때 초기 시간 저장
        scoreTime = Time.time + 2;
        
        bestScore = PlayerPrefs.GetInt("bestScore", 0);
        ScoreBoardText.text = "BEST : <color=#FFFFFF>" + bestScore + "</color>";
    }

    void Update()
    {
        if (Time.time - nowTime > makeTime)
        {
            // 2초 경과 했기 때문에 nowTime 을 현재 시간으로 초기화
            nowTime = Time.time;

            // Instantiate 함수는 프리팹을 활용하여 동적생성 하는 함수
            // Instantiate(프리팹, 위치, 회전값) 이 일반적 사용방법
            GameObject _pipe = Instantiate(pipe);
            // 생성된 파이프의 부모를 grounds 오브젝트로 설정
            _pipe.transform.parent = transform;

            // Range(int Min, int Max) 일 경우 Min <= 값 < Max
            // Range(float Min, float Max) 일 경우 Min <= 값 <= Max
            float randomY = Random.Range(-2.5f, 5.5f);

            // Random.Range 1 <= 값 < 7
            //_pipe.transform.localPosition = new Vector3(Random.Range(1, 7) + 5, 0, 0);
            _pipe.transform.localPosition = new Vector3(-transform.localPosition.x + 5, randomY, 0);

            _pipe.transform.localScale = new Vector3(1, 1, 1);

            // Destroy(파괴할 오브젝트, 시간)
            Destroy(_pipe, 8f);
        }

        if(Time.time - scoreTime > 2)
        {
            scoreTime = Time.time;
            score += 10;
            ScoreText.text = "SCORE : <color=#FFCD00>" + score + "</color>"; // 문자열로 변환

            // 즉 현재 점수가 최고점 보다 높을 때
            if (score > bestScore)
            {
                bestScore = score;
                ScoreBoardText.text = "BEST : <color=#FFFFFF>" + bestScore + "</color>";
            }
        }
    }

    public void SaveScore()
    {
        // 키, 값으로 값 저장
        // 키는 일종의 비밀번호와 같은 역할
        PlayerPrefs.SetInt("bestScore", bestScore);
    }
}
