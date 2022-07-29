using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Billboard : MonoBehaviour
{
    public Transform canvas;

    void Update()
    {
        // 캔버스가 카메라의 렌즈를 쳐다보도록
        canvas.forward = Camera.main.transform.forward;
    }
}
