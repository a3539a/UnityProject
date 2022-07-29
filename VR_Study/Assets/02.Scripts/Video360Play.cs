using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video; // VideoPlayer ����� ����ϱ� ���� ���ӽ����̽�
// 360 ���Ǿ ���� �÷��̾ ���� ������ �������.
// �� ���� ���� �ٸ� ������ ��ü�ϸ� ����Ѵ�.

public class Video360Play : MonoBehaviour
{
    VideoPlayer vp;
    // �ټ��� ���� Ŭ���� �迭�� ����
    public VideoClip[] vcList;
    // ���� ��� ���� Ŭ��dml ����Ʈ ��ȣ�� ����
    int currVCidex;

    void Start()
    {
        // ���� �÷��̾� ������Ʈ �ҷ�����
        vp = GetComponent<VideoPlayer>();
        vp.clip = vcList[0];
        currVCidex = 0;
        vp.Stop();
    }

    void Update()
    {
        // '[' �Է½� ��������
        if (Input.GetKeyDown(KeyCode.LeftBracket))
        {
            SwapVideoClip(false);
        }
        // ']' �Է½� ��������
        else if (Input.GetKeyDown(KeyCode.RightBracket))
        {
            SwapVideoClip(true);
        }
    }

    // ���ͷ����� ���� �Լ��� �ۺ����� �����Ѵ�
    // ����Ʈ�� �ε��� ��ȣ�� �������� ������ ��ü, ����ϱ����� ����
    // ���� ���� isNext�� true �� �� ���� ����, false �� �� ���� ����
    public void SwapVideoClip(bool isNext)
    {
        // ���� ��� ���� ������ �ε��� �ѹ��� �������� üũ�Ѵ�.
        // ���� ���� ��ȣ�� ���� ���󺸴� ����Ʈ���� �ε��� ��ȣ�� 1�� �۴�.
        // ���� ���� ��ȣ�� ���� ���󺸴� �ε�����ȣ�� 1�� ũ��/
        int setVCnum = currVCidex; // ���� ��� ���� ������ �ε�����ȣ ����
        vp.Stop(); // ���� ������� ������ ����

        // ����� ������ ���� ���� ����
        if (isNext)
        {
            // ���� ���� ���
            // ex) 0 = 1 % 3 == 1;
            setVCnum = (setVCnum + 1) % vcList.Length;
        }
        else
        {
            // ���� ���� ���
            // ex) 0 = -1 + 3 % 3 == 2;
            setVCnum = (setVCnum - 1) + vcList.Length % vcList.Length;
        }
        vp.clip = vcList[setVCnum]; // setVCnum ���� Ŭ���� ����
        vp.Play(); // ���
        currVCidex = setVCnum; // ���� ��ȣ�� �ε��� ������Ʈ
    }

    public void SetVideoPlay(int num)
    {
        if (currVCidex != num)
        {
            vp.Stop();
            vp.clip = vcList[num];
            currVCidex = num;
            vp.Play();
        }
    }
}
