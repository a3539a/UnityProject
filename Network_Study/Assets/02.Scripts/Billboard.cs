using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Billboard : MonoBehaviour
{
    public Transform canvas;

    void Update()
    {
        // ĵ������ ī�޶��� ��� �Ĵٺ�����
        canvas.forward = Camera.main.transform.forward;
    }
}
