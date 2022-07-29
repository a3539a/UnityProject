using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveMaps : MonoBehaviour
{
    // 스크롤링 속도
    public float speed;
    // 스프라이트 배열 관련 변수 (시작 인덱스, 마지막 인덱스)
    public int startIdx;
    public int endIdx;

    // 맵들 삽입
    public Transform[] mapSprites;

    // 카메라 높이
    float viewHeight;

    private void Awake()
    {
        // 맵사이즈 공식
        viewHeight = Camera.main.orthographicSize * 2f;
    }

    void Update()
    {
        // 현재 오브젝트의 포지션
        Vector3 currPos = transform.position;
        // 움직일 거리
        Vector3 nextPos = Vector3.down * speed * Time.deltaTime;

        // 맵그룹을 nextPos 만큼 내려준다.
        transform.position = currPos + nextPos;

        // 맵 배열의 마지막 오브젝트의 포지션 y 값이 카메라의 세로축 사이즈 보다 낮을 때
        if (mapSprites[endIdx].position.y < -viewHeight)
        {
            // Sprite Reuse
            // 맨위의 맵오브젝트의 localPosition 값
            Vector3 upperMapPos = mapSprites[startIdx].localPosition;
            // 맨아래의 맵오브젝트의 localPosition 값
            Vector3 lowerMapPos = mapSprites[endIdx].localPosition;

            // 맨아래의 맵오브젝트의 포지션을 맨위의 맵오브젝트 포지션에서 위로 카메라의 세로축 사이즈 만큼 위로 바꿔준다.
            mapSprites[endIdx].transform.localPosition = upperMapPos + Vector3.up * viewHeight;

            // Cursor Idx Change
            // 스타트인덱스 저장
            int saveIdx = startIdx;
            // 스타트 인덱스에 마지막 인덱스 대입
            startIdx = endIdx;
            // 마지막 인덱스는 세이브인덱스 -1 로 대입, -1 이 될 경우 (mapSprites.Length - 1) 로 대입
            endIdx = (saveIdx - 1 == -1) ? mapSprites.Length - 1 : saveIdx - 1;
        }
    }
}
