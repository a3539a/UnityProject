using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Box : MonoBehaviour
{
    public GameObject box_object;   // 대상 객체와 연결필요

    public Subject subject;         // 관찰자 주체 연결

    private void Start()
    {
        if (subject != null)
        {
            // 이벤트에서 += 는 실행
            subject.evnethandler += new Subject.event_handler(OnNotify);
        }
    }

    public void OnNotify()
    {
        if (box_object != null)
        {
            RandomMove();
            Debug.Log("이벤트 수신!");
        }

        void RandomMove()
        {
            box_object.transform.position = new Vector3(Random.Range(0, 10f), Random.Range(0, 10f), Random.Range(0, 10f));
        }
    }
}