using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public Sprite startImg;
    public Sprite pauseImg;

    public Image btnPause; // 실제 UI의 이미지를 변경하기 위해

    bool isPause = false;

    public void Pause()
    {


        if (!isPause)
        {
            isPause = true;
            Debug.Log("일시 정지");
            btnPause.sprite = startImg;
            Time.timeScale = 0f;
        }
        else
        {
            isPause = false;
            Debug.Log("게임 재개");
            btnPause.sprite = pauseImg;
            Time.timeScale = 1f;
        }
    }
}
