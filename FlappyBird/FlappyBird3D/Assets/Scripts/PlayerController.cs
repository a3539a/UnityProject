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
        // GetMouseButtonDown �޼ҵ�� �Ű������� 0 : ��Ŭ��, 1 : ��Ŭ��
        if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space))
        {
            // IsPointerOverGameObject() UI�� ��ġ�Ǹ� true �ƴϸ� false
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

        // ����ŸƮ ���� Ȱ��ȭ
        RestartText.SetActive(true);

        // MakePipe�� �ִ� SaveScore�Լ� ȣ���Ͽ� ��������
        mp.SaveScore();

        // �Ͻ�����
        // timeScale�� �⺻���� 1
        // Ŀ������ ������� �������� ����
        Time.timeScale = 0;
    }
}
