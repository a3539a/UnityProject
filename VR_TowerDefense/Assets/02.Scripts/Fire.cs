using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fire : MonoBehaviour
{
    public Transform bulletImpact; // 총알 파편 효과
    ParticleSystem bulletEffect; // 총알 파편 파티클 시스템
    AudioSource bulletAudio; // 총알 발사 사운드

    public Transform crossHair; // crossHair를 위한 속성

    void Start()
    {
        bulletEffect = bulletImpact.GetComponent<ParticleSystem>();
        bulletAudio = bulletImpact.GetComponent<AudioSource>();
    }

    void Update()
    {
        // 크로스헤어 표시
        ARAVRInput.DrawCrosshair(crossHair);

        // 엄지버튼 : IndexTrigger
        if (ARAVRInput.GetDown(ARAVRInput.Button.IndexTrigger))
        {
            // 총알 오디오 재생
            bulletAudio.Stop();
            bulletAudio.Play();

            // Ray를 카메라의 위치로부터 나가도록 한다.
            Ray ray = new Ray(ARAVRInput.RHandPosition, ARAVRInput.RHandDirection);
            // Ray 의 충돌 정보를 저장하기 위한 변수 지정
            RaycastHit hitInfo;

            // 플레이어 레이어 얻어오기
            // 레이어를 이름으로 검색한 후에 비트이동 한 결과를 저장
            int playerLayer = 1 << LayerMask.NameToLayer("PLAYER");
            // 타워 레이어 얻어오기
            int towerLayer = 1 << LayerMask.NameToLayer("TOWER");

            int layerMask = playerLayer | towerLayer;

            // Ray 를 쏜다. ray가 부딪힌 정보는 hitinfo에 담긴다.
            if (Physics.Raycast(ray, out hitInfo, 200, ~layerMask))
            {
                // 총알 이펙트 진행되고 있으면 멈추고 재생
                bulletEffect.Stop();
                bulletEffect.Play();
                
                // 부딪힌 지점 바로 위에서 이펙트가 보이도록 설정
                bulletImpact.position = hitInfo.point;

                // 부딪힌 지점의 방향으로 총알 이펙트의 방향을 설정
                bulletImpact.forward = hitInfo.normal;

                // ray 와 부딪힌 객체의 이름에 drone 이 포함되면 피격 처리 (쓰지말자)
                // ray 와 부딪힌 객체의 태그가 ENEMY면
                if (hitInfo.collider.CompareTag("ENEMY"))
                {
                    DroneAI drone = hitInfo.transform.GetComponent<DroneAI>();
                    if (drone)
                    {
                        drone.OnDamageProcess();
                    }
                }
            }
        }
    }
}
