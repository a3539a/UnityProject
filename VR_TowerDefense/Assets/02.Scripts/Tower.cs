using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Tower : MonoBehaviour
{
    // ������ ǥ���� UI
    public Transform damageUI;
    public Image damageImg;

    // Ÿ���� ���� HP
    public int initialHP = 10;
    int _hp = 0; // ���� hp ����

    // ���ڰŸ��� �ð�
    public float damageTime = 0.1f;

    // Tower �� �̱��� ��ü
    public static Tower instance;

    private void Awake()
    {
        // �̱��� ��ü �Ҵ�
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

            // ������ �������� �ڷ�ƾ ����
            StopAllCoroutines();
            // �ڷ�ƾ ȣ��
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
        // ī�޶��� nearClipPlane ���� ��� �صд�.
        float z = Camera.main.nearClipPlane + 0.01f;
        // damageUI ��ü�� �θ� ī�޶�� ����
        damageUI.parent = Camera.main.transform;
        // damageUI �� ��ġ�� X,Y �� 0, Z ���� ī�޶��� near ������ ����
        damageUI.localPosition = new Vector3(0, 0, z);
        // damageImg �� ������ �ʵ��� ��Ȱ��ȭ
        damageImg.enabled = false;
    }

    void Update()
    {
        
    }

    // ������ ó���� ���� �ڷ�ƾ �Լ�
    IEnumerator DamageEvent()
    {
        // damageImg ������Ʈ Ȱ��ȭ
        damageImg.enabled = true;
        // damageTime ��ŭ ��ٸ���.
        yield return new WaitForSeconds(damageTime);
        // �ٽ� ��Ȱ��ȭ
        damageImg.enabled = false;
    }
}
