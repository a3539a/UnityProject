using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video; // VideoPlayer 기능을 사용하기 위한 네임스페이스
// 360 스피어에 비디오 플레이어를 통해 영상을 재생하자.
// 두 가지 서로 다른 영상을 교체하며 재생한다.

public class Video360Play : MonoBehaviour
{
    VideoPlayer vp;
    // 다수의 비디오 클립을 배열로 관리
    public VideoClip[] vcList;
    // 현재 재생 중인 클립dml 리스트 번호를 저장
    int currVCidex;

    void Start()
    {
        // 비디오 플레이어 컴포넌트 불러오기
        vp = GetComponent<VideoPlayer>();
        vp.clip = vcList[0];
        currVCidex = 0;
        vp.Stop();
    }

    void Update()
    {
        // '[' 입력시 이전영상
        if (Input.GetKeyDown(KeyCode.LeftBracket))
        {
            SwapVideoClip(false);
        }
        // ']' 입력시 다음영상
        else if (Input.GetKeyDown(KeyCode.RightBracket))
        {
            SwapVideoClip(true);
        }
    }

    // 인터랙션을 위해 함수를 퍼블릭으로 선언한다
    // 리스트의 인덱스 번호를 기준으로 영상을 교체, 재생하기위한 변수
    // 인자 값인 isNext가 true 일 때 다음 영상, false 일 때 이전 영상
    public void SwapVideoClip(bool isNext)
    {
        // 현재 재생 중인 영상의 인덱스 넘버를 기준으로 체크한다.
        // 이전 영상 번호는 현재 영상보다 리스트에서 인덱스 번호가 1이 작다.
        // 다음 영상 번호는 현재 영상보다 인덱스번호가 1이 크다/
        int setVCnum = currVCidex; // 현재 재생 중인 영상의 인덱스번호 대입
        vp.Stop(); // 현재 재생중인 영상을 정지

        // 재생될 영상을 고르기 위한 과정
        if (isNext)
        {
            // 다음 영상 재생
            // ex) 0 = 1 % 3 == 1;
            setVCnum = (setVCnum + 1) % vcList.Length;
        }
        else
        {
            // 이전 영상 재생
            // ex) 0 = -1 + 3 % 3 == 2;
            setVCnum = (setVCnum - 1) + vcList.Length % vcList.Length;
        }
        vp.clip = vcList[setVCnum]; // setVCnum 으로 클립을 변경
        vp.Play(); // 재생
        currVCidex = setVCnum; // 현재 번호로 인덱스 업데이트
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
