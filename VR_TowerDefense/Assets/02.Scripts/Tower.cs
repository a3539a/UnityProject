using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Tower : MonoBehaviour
{
    // 데미지 표현할 UI
    public Transform damageUI;
    public Image damageImg;

    // 타워의 최초 HP
    public int initialHP = 10;
    int _hp = 0; // 내부 hp 변수

    // 깜박거리는 시간
    public float damageTime = 0.1f;

    // Tower 의 싱글톤 객체
    public static Tower instance;

    private void Awake()
    {
        // 싱글톤 객체 할당
        if(instance == null)
        {
            instance = this;
        }
    }

    public int HP
    {
        get { return _hp; }
        set 
        {
            _hp = value;

            // 기존에 진행중인 코루틴 정지
            StopAllCoroutines();
            // 코루틴 호출
            StartCoroutine(DamageEvent());

            if (_hp <= 0)
            {
                Destroy(gameObject);
            }
        }
    }

    void Start()
    {
        _hp = initialHP;
        // 카메라의 nearClipPlane 값을 기억 해둔다.
        float z = Camera.main.nearClipPlane + 0.01f;
        // damageUI 객체와 부모를 카메라로 설정
        damageUI.parent = Camera.main.transform;
        // damageUI 의 위치를 X,Y 는 0, Z 값은 카메라의 near 값으로 설정
        damageUI.localPosition = new Vector3(0, 0, z);
        // damageImg 는 보이지 않도록 비활성화
        damageImg.enabled = false;
    }

    void Update()
    {
        
    }

    // 데미지 처리를 위한 코루틴 함수
    IEnumerator DamageEvent()
    {
        // damageImg 컴포넌트 활성화
        damageImg.enabled = true;
        // damageTime 만큼 기다린다.
        yield return new WaitForSeconds(damageTime);
        // 다시 비활성화
        damageImg.enabled = false;
    }
}
