using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHpBar : MonoBehaviour
{
    Camera uiCamera;

    Canvas canvas;
    RectTransform rectParent;
    RectTransform rectHp;

    // public 변수는 인스펙터에 표시되지만
    // 아래 속성을 사용하면 접근은 가능하지만 인스펙터에는 표시 안됨
    [HideInInspector]
    public Vector3 offset = Vector3.zero;

    [HideInInspector]
    public Transform targetTr; // 적

    void Start()
    {
        // 부모로부터 캔버스 추출
        canvas = GetComponentInParent<Canvas>(); // UI Canvas
        uiCamera = canvas.worldCamera;
        rectParent = canvas.GetComponent<RectTransform>(); // UI Canvas
        // 이 스크립트가 들어가는 오브젝트꺼
        rectHp = GetComponent<RectTransform>();
    }

    private void LateUpdate()
    {
        // Camera.main = 메인 카메라를 야기함 (메인카메라 태그가 있는 카메라)
        // 1단계 . WorldToScreenPoint 메소드는 월드 좌표를 스크린 좌표로 변환
        var screenPos = Camera.main.WorldToScreenPoint(targetTr.position + offset);
        if (screenPos.z < 0f) // 카메라가 HpBar를 지나쳤을 경우
        {
            // HpBar 반전
            screenPos *= -1f;
        }

        var localPos = Vector2.zero;
        // 2단계 . 스크린 좌표를 RectTransform 좌표로 다시 변환
        // ScreenPointToLocalPointInRectangle(부모의 렉트 트랜스폼, 스크린 좌표, 렌더링 카메라, 변환된 좌표)
        RectTransformUtility.ScreenPointToLocalPointInRectangle(rectParent, screenPos, uiCamera, out localPos);
        // 변환된 localPos좌표를 사용하여 체력바의 RectTransform 위치를 조정
        rectHp.localPosition = localPos;
    }

    void Update()
    {
        
    }
}
