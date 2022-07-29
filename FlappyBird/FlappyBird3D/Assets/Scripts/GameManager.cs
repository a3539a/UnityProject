using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public Sprite startImg;
    public Sprite pauseImg;

    public Image btnPause; // ���� UI�� �̹����� �����ϱ� ����

    bool isPause = false;

    public void Pause()
    {


        if (!isPause)
        {
            isPause = true;
            Debug.Log("�Ͻ� ����");
            btnPause.sprite = startImg;
            Time.timeScale = 0f;
        }
        else
        {
            isPause = false;
            Debug.Log("���� �簳");
            btnPause.sprite = pauseImg;
            Time.timeScale = 1f;
        }
    }
}
