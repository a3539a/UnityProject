using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    // �Ѿ� ������
    public int dmg;

    // ȸ�� �Ѿ˿� ��üũ ����
    public bool isRotate;

    private void Update()
    {
        if (isRotate)
        {
            transform.Rotate(Vector3.forward * 10);
        }
    }

    // �ݶ��̴� ��� ��
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // �浹ü�� BORDERBULLET �±׸� ������ ���� ��
        if (collision.CompareTag("BORDERBULLET"))
        {
            // ���� ������Ʈ �ı�
            //Destroy(gameObject);

            // ������Ʈ Ǯ�� ����
            // ���� ������Ʈ ��Ȱ��ȭ
            gameObject.SetActive(false);
        }
    }
}
