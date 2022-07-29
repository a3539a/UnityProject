using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyDamage : MonoBehaviour
{
    const string bulletTag = "BULLET";

    float hp = 50f;
    float iniHp = 50f;
    GameObject bloodEffect;

    public GameObject hpBarPrefab;
    public Vector3 hpBarOffset = new Vector3 (0, 2.2f, 0);

    Canvas uiCanvas;
    Image hpBarImg; // fillamount 를 조정할 실제 hpBar (빨간색)

    void Start()
    {
        // Resources 라는 예약폴더 내부에 있는 오브젝트를 참조 
        // 파일명에 해당되는 오브젝트를 가지고 와서 사용하도록 해줌
        bloodEffect = Resources.Load<GameObject>("Effect/Blood");
        SetHpBar(); // 체력바 세팅용 함수
    }

    void SetHpBar()
    {
        uiCanvas = GameObject.Find("UI_Canvas").GetComponent<Canvas>();
        // hpBarPrefab 을 UI_Canvas 의 자식으로 동적 생성
        GameObject hpBar = Instantiate(hpBarPrefab, uiCanvas.transform);
        hpBarImg = hpBar.GetComponentsInChildren<Image>()[1];

        // 생성된 체력바의 위치를 조정하기 위한 부분
        var _hpBar = hpBar.GetComponent<EnemyHpBar>();
        _hpBar.targetTr = transform; // 체력바가 추적할 대상을 적으로 설정
        _hpBar.offset = hpBarOffset; // 체력바가 2.2f 만큼 위에 표시 되게
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag(bulletTag))
        {
            // 혈흔 효과 함수 호출
            ShowBloodEffect(collision);

            hp -= collision.gameObject.GetComponent<BulletCtrl>().damage;

            hpBarImg.fillAmount = hp / iniHp;

            // 부딪힌 총알 제거
            // Destroy(collision.gameObject);

            // 오브젝트 풀의 비활성화
            collision.gameObject.SetActive(false);

            if (hp <= 0)
            {
                GetComponent<EnemyAI>().state = EnemyAI.State.DIE;
                // 체력바 클리어해서 안보이도록 처리
                hpBarImg.GetComponentsInParent<Image>()[1].color = Color.clear;
            }
        }
    }

    void ShowBloodEffect(Collision coll)
    {
        ContactPoint cp = coll.GetContact(0);
        Quaternion rot = Quaternion.LookRotation(-cp.normal);
        GameObject blood = Instantiate(bloodEffect, cp.point, rot);
        Destroy(blood, 1f);
    }
}
