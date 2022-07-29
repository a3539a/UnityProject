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
        vp.Stop(); // �ڵ���� ����
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.S)) // S ������
        {
            vp.Stop(); // ����
        }

        if (Input.GetKeyDown("space")) // �����̽� ���� ��
        {
            if (vp.isPlaying) // ���� ������̶��
            {
                vp.Pause(); // �Ͻ�����
            }
            else
            {
                vp.Play(); // ���
            }
        }
    }

    // GazePointerCtrl ���� ���� ����� ��Ʈ�� �ϱ� ���� �Լ�
    public void CheckVideoFrame(bool Checker)
    {
        if (Checker)
        {
            if (!vp.isPlaying) // Ŭ���� ������� �ƴ� ��
            {
                vp.Play();
            }
        }
        else // ��� ���� ��
        {
            vp.Stop();
        }
    }
}
