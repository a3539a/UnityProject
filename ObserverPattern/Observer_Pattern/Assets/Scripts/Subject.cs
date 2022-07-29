
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Subject : MonoBehaviour
{
    public delegate void event_handler();
    public event_handler evnethandler;

    float time;

    private void Update()
    {
        time += Time.deltaTime;

        if (time > 3.0f)
        {
            time = 0;
            evnethandler();
            Debug.Log("이벤트 발신!");
        }
    }
}