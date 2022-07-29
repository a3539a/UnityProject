using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class VideoFrame : MonoBehaviour
{
    VideoPlayer vp;

    void Start()
    {
        vp = GetComponent<VideoPlayer>();
        vp.Stop(); // 자동재생 막기
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.S)) // S 누를떄
        {
            vp.Stop(); // 정지
        }

        if (Input.GetKeyDown("space")) // 스페이스 누를 때
        {
            if (vp.isPlaying) // 현재 재생중이라면
            {
                vp.Pause(); // 일시정지
            }
            else
            {
                vp.Play(); // 재생
            }
        }
    }

    // GazePointerCtrl 에서 영상 재생을 컨트롤 하기 위한 함수
    public void CheckVideoFrame(bool Checker)
    {
        if (Checker)
        {
            if (!vp.isPlaying) // 클립이 재생중이 아닐 때
            {
                vp.Play();
            }
        }
        else // 재생 중일 때
        {
            vp.Stop();
        }
    }
}
