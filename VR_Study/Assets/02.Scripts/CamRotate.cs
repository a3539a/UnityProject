using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamRotate : MonoBehaviour
{
    // ���� ����
    Vector3 angle;
    // ���콺 ����
    public float sensitivity = 200f;

    void Awake()
    {
        // ���� ī�޶��� ������ ������ ������ ������ ����
        angle.y = -Camera.main.transform.eulerAngles.x;
        angle.x = Camera.main.transform.eulerAngles.y;
        angle.z = Camera.main.transform.eulerAngles.z;
    }

    void LateUpdate()
    {
        // 1) ���콺�� �����ӿ� ���� ����/���� �� ���ϱ�
        float x = Input.GetAxis("Mouse X");
        float y = Input.GetAxis("Mouse Y");

        // 2) ������ ���ϱ� ���� ����/���� �Ӽ����� ȸ���� ����
        angle.x += x * sensitivity * Time.deltaTime;
        angle.y += y * sensitivity * Time.deltaTime;

        // 3) ȸ����Ű�� �ʹ�.
        // ī�޶��� ȸ�� ���� ���� ������� ȸ�� ���� �Ҵ��Ѵ�.
        transform.eulerAngles = new Vector3(-angle.y, angle.x, transform.eulerAngles.z);
    }
}
