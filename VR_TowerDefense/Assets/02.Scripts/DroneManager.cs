using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroneManager : MonoBehaviour
{
    // 랜덤 시간 범위 
    public float minTime = 1f;
    public float maxTime = 5f;

    // 생성 시간 관련
    float creatTime;
    float currTime;

    // 드론 생성위치와 생성할 드론프리팹
    public Transform[] spawnPoints;
    public GameObject dronePrefab;

    void Start()
    {
        // 생성시간을 랜덤 범위 내에서 결정
        creatTime = Random.Range(minTime, maxTime);
    }

    void Update()
    {
        // 시간이 흘러감
        currTime += Time.deltaTime;

        // 생성시간이 되면
        if(currTime > creatTime)
        {
            // 드론 생성
            GameObject drone = Instantiate(dronePrefab);
            // 랜덤 생성위치
            int index = Random.Range(0, spawnPoints.Length);
            drone.transform.position = spawnPoints[index].position;
            // 시간 초기화
            currTime = 0;
            // 생성시간 재설정
            creatTime = Random.Range(minTime, maxTime);
        }
    }
}
