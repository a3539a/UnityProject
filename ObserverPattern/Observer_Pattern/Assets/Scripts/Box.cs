using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Box : MonoBehaviour
{
    public GameObject box_object;   // ��� ��ü�� �����ʿ�

    public Subject subject;         // ������ ��ü ����

    private void Start()
    {
        if (subject != null)
        {
            // �̺�Ʈ���� += �� ����
            subject.evnethandler += new Subject.event_handler(OnNotify);
        }
    }

    public void OnNotify()
    {
        if (box_object != null)
        {
            RandomMove();
            Debug.Log("�̺�Ʈ ����!");
        }

        void RandomMove()
        {
            box_object.transform.position = new Vector3(Random.Range(0, 10f), Random.Range(0, 10f), Random.Range(0, 10f));
        }
    }
}