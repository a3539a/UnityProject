using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroneManager : MonoBehaviour
{
    // ���� �ð� ���� 
    public float minTime = 1f;
    public float maxTime = 5f;

    // ���� �ð� ����
    float creatTime;
    float currTime;

    // ��� ������ġ�� ������ ���������
    public Transform[] spawnPoints;
    public GameObject dronePrefab;

    void Start()
    {
        // �����ð��� ���� ���� ������ ����
        creatTime = Random.Range(minTime, maxTime);
    }

    void Update()
    {
        // �ð��� �귯��
        currTime += Time.deltaTime;

        // �����ð��� �Ǹ�
        if(currTime > creatTime)
        {
            // ��� ����
            GameObject drone = Instantiate(dronePrefab);
            // ���� ������ġ
            int index = Random.Range(0, spawnPoints.Length);
            drone.transform.position = spawnPoints[index].position;
            // �ð� �ʱ�ȭ
            currTime = 0;
            // �����ð� �缳��
            creatTime = Random.Range(minTime, maxTime);
        }
    }
}
