using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundMove : MonoBehaviour
{
    float _distance = 7.67f;
    int _count = 2;
    int _index = 2;

    public GameObject[] grounds;

    void Update()
    {
        if(Time.timeScale == 0)
        {
            return;
            // return 코드 아래쪽은 실행안함
        }

        // 오브젝트의 위치값 x 를 -0.05만큼 이동 시킴
        transform.localPosition += new Vector3(-0.08f, 0, 0);

        _count = 2 + (int)(-gameObject.transform.localPosition.x / _distance);
        if (_index != _count)
        {
            grounds[(_index - 2) % 3].transform.localPosition = new Vector3(_distance * _count, -5, 0);
            _index = _count;
        }
    }
}
