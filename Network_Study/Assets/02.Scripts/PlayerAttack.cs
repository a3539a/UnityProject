using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class PlayerAttack : MonoBehaviourPun
{
    public Animator anim;

    // ü��UI ����
    public float maxHp = 10;
    public float attPow = 4;
    public Slider hpSlider;
    public BoxCollider weaponCol;

    float currHp = 0;

    void Start()
    {
        // ���� ü���� �ִ��
        currHp = maxHp;
        hpSlider.value = currHp / maxHp; // �����̴� ä���
    }

    void Update()
    {
    //    if (OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger, OVRInput.Controller.RTouch))
    //    {
    //        // ����� �ڽ��� ĳ������ ����
    //        // �ڽ��� ������Ʈ�� ���� �ִϸ��̼� �Լ�ȣ��
    //        if (photonView.IsMine)
    //        {
    //            photonView.RPC("AttackAnimation", RpcTarget.AllBuffered);
    //        }
    //    }

        // PC ����
        if (Input.GetMouseButtonDown(0))
        {
            // ����� �ڽ��� ĳ������ ����
            // �ڽ��� ������Ʈ�� ���� �ִϸ��̼� �Լ�ȣ��
            if (photonView.IsMine)
            {
                photonView.RPC("AttackAnimation", RpcTarget.AllBuffered);
            }
        }
    }

    // ���� �ִϸ��̼� ȣ�� �Լ� + RPC ��Ʈ����Ʈ
    // RPC �Լ��μ� ����� ������ ��
    [PunRPC] // �ݵ�� PunRPC Attribute ����������
    public void AttackAnimation()
    {
        anim.SetTrigger("Attack");
    }

    [PunRPC] // �ݵ�� PunRPC Attribute ����������
    public void OnDamage(float pow)
    {
        // 0�� �������� �� ���� ü�¿��� ���ݷ¸�ŭ ����
        currHp = Mathf.Max(0, currHp - pow);

        // hp �����̴��� ���� ü�»��¸� ���
        hpSlider.value = currHp / maxHp; // �����̴� ä���
    }

    private void OnTriggerEnter(Collider other)
    {
        // ����, �ڽ��� ĳ�����̸鼭 �˿� ���� ����� �̸��� Player��� ���ڰ� ���ԵǾ� ������
        if (photonView.IsMine && other.gameObject.name.Contains("Player"))
        {
            // ���⿡ ���� ����� ����信�� ������ ó�� �Լ��� RPC�� ȣ��
            PhotonView pv = other.GetComponent<PhotonView>();
            pv.RPC("OnDamage", RpcTarget.AllBuffered, attPow);

            // ���� �ݶ��̴� ��Ȱ��ȭ ������ �浹 ����
            weaponCol.enabled = false;
        }
    }
}
