using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using SimpleJSON;// JSON 인코딩 / 디코딩 하기 위한 라이브러리

public class Gamemanager : MonoBehaviour
{

    [Header("LoginPanel")]
    public InputField IDInputField;
    public InputField PassInputField;
    [Header("CreateAccountPanel")]
    public InputField New_IDInputField;
    public InputField New_PassInputField;
    public GameObject CreateAccountPanelObj;

    public string LoginUrl;
    public string CreateUrl;
    public string UpdateUrl;
    public string DeleteUrl;
    // Use this for initialization
    void Start()
    {
        //로그인에 쓰이는 php와
        LoginUrl = "127.0.0.1/unity/Login.php";
        //계정생성(신규가입)에 쓰이는 php 가 각자 다른 php
        CreateUrl = "127.0.0.1/unity/Join.php";
        UpdateUrl = "127.0.0.1/unity/Update.php";
        DeleteUrl = "127.0.0.1/unity/Delete.php";
    }
    //로그인
    public void LoginBtn()
    {
        StartCoroutine(LoginCo());
    }
    //수정
    public void UpdateBtn()
    {
        StartCoroutine(UpdateCo());
    }
    //삭제
    public void DeleteBtn()
    {
        StartCoroutine(DeleteCo());
    }

    IEnumerator LoginCo()
    {
        //우리가 요청하는 URL 경로가 127.0.0.1/Unity/Login.php
        //GET 공개형 이기 때문에 키와 값을 주소창에서 확인가능

        //localhost/Unity/Login.php?Input_user='성훈52'&Input_pass='5252'

        //비공개 방식
        //POST방식의 요청방법
        WWWForm form = new WWWForm();
        //Input_user 키 에다가 IDInputField.text 데이터를 전송
        form.AddField("Input_user", IDInputField.text);
        form.AddField("Input_pass", PassInputField.text);

        // 예)
        // form.AddField("Input_Position", "(0,0,0)");
        // form.AddField("Input_ITem", "검입니다. !");
        WWW webRequest = new WWW(LoginUrl, form);
        // 웹에서 일단 작업 후 아래코드에 결과깞 날라온다고 이해하자
        yield return webRequest;

        //LoginUrl로 값을 전달한 후 되돌아온 결과값이 에러가 아니면
        if (string.IsNullOrEmpty(webRequest.error))
        {
            //값이 정상적이면 아래 조건문을 수행한다.
            DisplayJSON(webRequest.text);
        }
    }
    void DisplayJSON(string _jsonData)
    {
        //내가 LoginCo() 에서 전달받은 webRequest.text 값을 파싱(번역) 하는 단계.
        //{"results":[{"ID":"\ubc15\uc131\ud6c8","PASS":"1234"}]}
        var N = JSON.Parse(_jsonData);
        //N 변수에 저장된 값은 아래와 같다.
        //{"results":[{"ID":"\ubc15\uc131\ud6c8","PASS":"1234"}]}

        var Array = N["results"];
        //결과로 전송되어진 JSON 데이터가 "results" 오브젝트이므로
        //Array 라는 변수에 1차 가공한다. 
        //그결과
        //[{ "ID":"\ubc15\uc131\ud6c8","PASS":"1234"}]

        //결과값이 1개 이상일 경우만 반복문 수행
        //즉, 내가 입력한 아이디가 존재할 경우
        if (Array.Count > 0)
        {
            for (int i = 0; i < Array.Count; i++)
            {
                //결과값이 있다면 Password 값을 가져온다
                string pass = Array[i]["PASS"].Value;
                Debug.Log(pass.ToString());
                //입력한 패스워드와 디비에서 가져온 패스워드를 비교
                if (PassInputField.text == pass)
                    Debug.Log("로그인 성공");
                else
                    Debug.Log("패스워드가 틀립니다.");
            }
        }
        else
            Debug.Log("해당 아이디가 존재하지 않습니다.");
    }


    public void OpenCreateAccountBtn()
    {
        CreateAccountPanelObj.SetActive(true);
    }


    public void CreateAccountBtn()
    {
        StartCoroutine(CreateCo());
    }

    IEnumerator CreateCo()
    {
        WWWForm form = new WWWForm();
        form.AddField("Input_user", New_IDInputField.text);
        form.AddField("Input_pass", New_PassInputField.text);
        form.AddField("Input_Info", "안녕하세요 뉴비입니다.");
        // 예)
        // form.AddField("Input_Position", "(0,0,0)");
        // form.AddField("Input_ITem", "검입니다. !");
        WWW webRequest = new WWW(CreateUrl, form);
        yield return webRequest;

        Debug.Log(webRequest.text);

        yield return null;
    }
    IEnumerator UpdateCo()
    {
        WWWForm form = new WWWForm();
        form.AddField("Input_user", IDInputField.text);
        form.AddField("Input_pass", PassInputField.text);
        form.AddField("Input_Info", "변경 되었습니다.");
        // 예)
        // form.AddField("Input_Position", "(0,0,0)");
        // form.AddField("Input_ITem", "검입니다. !");
        WWW webRequest = new WWW(UpdateUrl, form);
        yield return webRequest;

        Debug.Log(webRequest.text);

        yield return null;
    }
    IEnumerator DeleteCo()
    {
        WWWForm form = new WWWForm();
        form.AddField("Input_user", IDInputField.text);
        form.AddField("Input_pass", PassInputField.text);
        // 예)
        // form.AddField("Input_Position", "(0,0,0)");
        // form.AddField("Input_ITem", "검입니다. !");
        WWW webRequest = new WWW(DeleteUrl, form);
        yield return webRequest;

        Debug.Log(webRequest.text);

        yield return null;
    }

}
