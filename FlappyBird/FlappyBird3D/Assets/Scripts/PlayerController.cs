using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class PlayerController : MonoBehaviour
{
    public GameObject RestartText;

    MakePipe mp;

    // Start is called before the first frame update
    void Start()
    {
        Screen.SetResolution(480, 800, false);
        mp = GameObject.Find("Grounds").GetComponent<MakePipe>();
    }

    // Update is called once per frame
    void Update()
    {
        // GetMouseButtonDown 메소드는 매개변수에 0 : 좌클릭, 1 : 우클릭
        if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space))
        {
            // IsPointerOverGameObject() UI가 터치되면 true 아니면 false
            if (!EventSystem.current.IsPointerOverGameObject())
            {
                GetComponent<Rigidbody>().velocity = Vector3.zero;
                GetComponent<Rigidbody>().AddForce(Vector3.up * 200);
            }
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            Debug.Log("ReStart!");
            Time.timeScale = 1;
            // Application.LoadLevel("SceneName");
            SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().name);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("GameOver!");

        // GetComponent<Animator>().SetBool("isDie", true);
        GetComponent<Animator>().Play("die");

        // 리스타트 문구 활성화
        RestartText.SetActive(true);

        // MakePipe에 있는 SaveScore함수 호출하여 점수저장
        mp.SaveScore();

        // 일시정지
        // timeScale의 기본값은 1
        // 커질수록 배속증가 낮을수록 감소
        Time.timeScale = 0;
    }
}
