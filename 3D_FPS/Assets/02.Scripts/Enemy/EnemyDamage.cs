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
    Image hpBarImg; // fillamount �� ������ ���� hpBar (������)

    void Start()
    {
        // Resources ��� �������� ���ο� �ִ� ������Ʈ�� ���� 
        // ���ϸ� �ش�Ǵ� ������Ʈ�� ������ �ͼ� ����ϵ��� ����
        bloodEffect = Resources.Load<GameObject>("Effect/Blood");
        SetHpBar(); // ü�¹� ���ÿ� �Լ�
    }

    void SetHpBar()
    {
        uiCanvas = GameObject.Find("UI_Canvas").GetComponent<Canvas>();
        // hpBarPrefab �� UI_Canvas �� �ڽ����� ���� ����
        GameObject hpBar = Instantiate(hpBarPrefab, uiCanvas.transform);
        hpBarImg = hpBar.GetComponentsInChildren<Image>()[1];

        // ������ ü�¹��� ��ġ�� �����ϱ� ���� �κ�
        var _hpBar = hpBar.GetComponent<EnemyHpBar>();
        _hpBar.targetTr = transform; // ü�¹ٰ� ������ ����� ������ ����
        _hpBar.offset = hpBarOffset; // ü�¹ٰ� 2.2f ��ŭ ���� ǥ�� �ǰ�
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag(bulletTag))
        {
            // ���� ȿ�� �Լ� ȣ��
            ShowBloodEffect(collision);

            hp -= collision.gameObject.GetComponent<BulletCtrl>().damage;

            hpBarImg.fillAmount = hp / iniHp;

            // �ε��� �Ѿ� ����
            // Destroy(collision.gameObject);

            // ������Ʈ Ǯ�� ��Ȱ��ȭ
            collision.gameObject.SetActive(false);

            if (hp <= 0)
            {
                GetComponent<EnemyAI>().state = EnemyAI.State.DIE;
                // ü�¹� Ŭ�����ؼ� �Ⱥ��̵��� ó��
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
