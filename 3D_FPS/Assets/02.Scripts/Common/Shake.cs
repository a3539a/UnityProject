using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shake : MonoBehaviour
{
    public Transform shakeCamera;

    public bool shakeRotate = false;

    // 카메라의 초기 상태 저장용
    Vector3 originPos;
    Quaternion originRot;

    void Start()
    {
        // 흔들기 전에 카메라의 원래 위치와 회전값 보존
        originPos = shakeCamera.localPosition;
        originRot = shakeCamera.localRotation;
    }

    // 유니티에서 매개변수값을 별도로 설정하지 않아도
    // 아래와 같이 기본값을 설정할수 있음
    public IEnumerator ShakeCamera(float duration = 0.05f, 
                                   float magnitudePos = 0.03f, 
                                   float magnitudeRot = 0.1f)
    {
        float passTime = 0f;// 지나간 시간을 누적하기 위한 변수
        while(passTime < duration)
        {
            // insideUnitSphere 불규칙한 주파수 발생기
            // 인터벌이 있는 난수 발생기
            Vector3 shakePos = Random.insideUnitSphere;
            // 카메라의 위치를 변경
            shakeCamera.localPosition = shakePos * magnitudePos;
            // 회전을 허용할 때
            if (shakeRotate)
            {
                // Mathf.PerlinNoise 함수를 활용하여 z 축을 회전시키기 위한 난수 생성
                Vector3 shakeRot = new Vector3(0, 0, Mathf.PerlinNoise(Time.time * magnitudeRot, 0f));
                shakeCamera.localRotation = Quaternion.Euler(shakeRot);
            }
            passTime += Time.deltaTime; // 진동 시간 누적

            yield return null;
        }
        // 흔들기 끝나고 나면 원래 위치로 원복 시키기
        shakeCamera.localPosition = originPos;
        shakeCamera.localRotation = originRot;
    }
}
