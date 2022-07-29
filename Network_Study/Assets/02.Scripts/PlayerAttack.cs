using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class PlayerAttack : MonoBehaviourPun
{
    public Animator anim;

    // 체력UI 관련
    public float maxHp = 10;
    public float attPow = 4;
    public Slider hpSlider;
    public BoxCollider weaponCol;

    float currHp = 0;

    void Start()
    {
        // 현재 체력을 최대로
        currHp = maxHp;
        hpSlider.value = currHp / maxHp; // 슬라이더 채우기
    }

    void Update()
    {
    //    if (OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger, OVRInput.Controller.RTouch))
    //    {
    //        // 사용자 자신의 캐릭터일 떄만
    //        // 자신의 오브젝트의 공격 애니메이션 함수호출
    //        if (photonView.IsMine)
    //        {
    //            photonView.RPC("AttackAnimation", RpcTarget.AllBuffered);
    //        }
    //    }

        // PC 버전
        if (Input.GetMouseButtonDown(0))
        {
            // 사용자 자신의 캐릭터일 떄만
            // 자신의 오브젝트의 공격 애니메이션 함수호출
            if (photonView.IsMine)
            {
                photonView.RPC("AttackAnimation", RpcTarget.AllBuffered);
            }
        }
    }

    // 공격 애니메이션 호출 함수 + RPC 어트리뷰트
    // RPC 함수로서 기능을 수행할 때
    [PunRPC] // 반드시 PunRPC Attribute 명시해줘야함
    public void AttackAnimation()
    {
        anim.SetTrigger("Attack");
    }

    [PunRPC] // 반드시 PunRPC Attribute 명시해줘야함
    public void OnDamage(float pow)
    {
        // 0을 하한으로 해 현재 체력에서 공격력만큼 감소
        currHp = Mathf.Max(0, currHp - pow);

        // hp 슬라이더에 현재 체력상태를 출력
        hpSlider.value = currHp / maxHp; // 슬라이더 채우기
    }

    private void OnTriggerEnter(Collider other)
    {
        // 만일, 자신의 캐릭터이면서 검에 닿은 대상의 이름이 Player라는 글자가 포함되어 있으면
        if (photonView.IsMine && other.gameObject.name.Contains("Player"))
        {
            // 무기에 닿은 대상의 포톤뷰에서 데미지 처리 함수를 RPC로 호출
            PhotonView pv = other.GetComponent<PhotonView>();
            pv.RPC("OnDamage", RpcTarget.AllBuffered, attPow);

            // 무기 콜라이더 비활성화 여러번 충돌 방지
            weaponCol.enabled = false;
        }
    }
}
