using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Damage : MonoBehaviour
{
    float iniHp = 100f;
    float currHp;

    // delegate 선언
    // 델리게이트 타입이 void 형인것은 EnemyAI에 있는
    // void OnPlayerDie() 함수와 타입을 맞추려고 하는것
    public delegate void PlayerDieHandler();
    // 델리게이트 사용방법은 몇 가지가 있으며
    // 그 중 이벤트 생성 델리게이트를 사용하여 이벤트를 만들고
    // 해당 이벤트에 호출할 메소드를 연결시켜 호출하도록 함
    public static event PlayerDieHandler OnPlayerDieEvent;

    // UI 요소들
    public Image bloodScreen; // 피격효과
    public Image hpBar; // 플레이어 체력
    readonly Color iniColor = new Vector4(0, 1f, 0, 1f);
    Color currColor;

    void Start()
    {
        currHp = iniHp;

        hpBar.color = iniColor; // hpBar 색상 설정
        currColor = iniColor; // 현재 체력 색상 설정
    }

    IEnumerator ShowBloodScreen()
    {
        bloodScreen.color = new Color(1, 0, 0, Random.Range(0.3f, 0.6f));

        yield return new WaitForSeconds(0.1f);

        bloodScreen.color = Color.clear; // 색상 초기화
    }

    void DisplayHpBar()
    {
        // 현재 체력의 수치가 50%보다 많을 때
        float hpPercent = currHp / iniHp;
        if (hpPercent > 0.5f)
        {
            // 현재 체력의 비율이 감소하면서
            // 상대적으로 hp색상의 붉은색을 증가시켜서
            // 녹색 > 노란색으로 변경하는 부분
            currColor.r = (1 - hpPercent) * 2f;
        }
        else // 현재 체력이 50% 이하일 경우
        {
            // 녹색의 비율은 점차 감소
            // 노란색 > 붉은색
            currColor.g = hpPercent * 2f;
        }

        hpBar.color = currColor; // 체력바에 색상 설정
        hpBar.fillAmount = hpPercent; // 체력바 게이지도 설정
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("BULLET"))
        {
            // 피격효과 코루틴 함수 호출
            StartCoroutine(ShowBloodScreen());

            Destroy(other.gameObject);

            currHp -= 5f;

            DisplayHpBar();

            if (currHp <= 0f)
            {
                // 플레이어 죽음 함수 호출
                PlayerDie();
            }
        }
    }

    void PlayerDie()
    {
        // 이벤트 할당
        OnPlayerDieEvent();
        // 게임매니저에게 게임오버를 알림
        GameManager.instance.isGameOver = true;

        //print("플레이어 사망");
        // ENEMY 태그를 지닌 모든 오브젝트 찾아서 배열에 저장
        /*
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("ENEMY");
        foreach (GameObject enemy in enemies)
        {
                // enemy.GetComponent<EnemyAI>().OnPlayerDie(); 일반적인 함수 호출
                // SendMessage 함수는 특정 게임오브젝트에 포함된 스크립트를 자동으로 훑어봄
                // SendMessage("함수이름", 응답유무)
                // 해당 함수 이름이 있으면 함수 호출하는 방식
                enemy.SendMessage("OnPlayerDie", SendMessageOptions.DontRequireReceiver);
        // 단점 : 순차적 호출이기 때문에 처음과 끝의 지연시간이 발생
        // 즉, 동시에 행동을 행할 때 차이가 발생한다.
        }
        */
    }

    void Update()
    {
        
    }
}
