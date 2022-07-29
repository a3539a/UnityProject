using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RemoveBullet : MonoBehaviour
{
    public GameObject sparkEffect;

    // �ݶ��̴� �浹�� ����
    private void OnCollisionEnter(Collision collision)
    {
        // �浹�� ���ӿ�����Ʈ�� �±װ� ��
        if (collision.collider.CompareTag("BULLET"))
        {
            // �浹�� ������ ���� ����(����Ʈ)�� ������
            ContactPoint cp = collision.GetContact(0);
            Quaternion rot = Quaternion.LookRotation(-cp.normal);

            // ����ũ ����Ʈ ��ƼŬ�� �浹 ��ġ���� ���� ����
            // �浹 ������, �������������� ����ũ�� Ƣ���� ����
            GameObject spark = Instantiate(sparkEffect, cp.point, rot);

            // ���ӿ�����Ʈ ����
            // Destroy(collision.gameObject);

            Destroy(spark, 0.3f);

            // ������Ʈ Ǯ�� ��Ȱ��ȭ
            collision.gameObject.SetActive(false);

        }
    }
}
