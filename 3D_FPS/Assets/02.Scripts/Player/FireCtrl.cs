using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// 직렬화 특성
// 바로아래에 public 으로 선언된 클래스/구조체를 인스펙터에 표시
[System.Serializable]
public struct PlayerSfx
{
    public AudioClip[] fire;
    public AudioClip[] reload;
}

public class FireCtrl : MonoBehaviour
{
    public enum WeaponType
    {
        RIFLE = 0,
        SHOTGUN
    }
    // 현재 무기타입을 RIFLE로 지정해두기
    public WeaponType currWeapon = WeaponType.RIFLE;

    // 총알 프리팹
    public GameObject bullet;
    // 총알 발사 좌표
    public Transform firePos;

    AudioSource _audio;

    public PlayerSfx playerSfx;

    public ParticleSystem catridge;
    ParticleSystem muzzleFlash;

    Shake shake;

    // 총알 UI 관련
    public Image magazineImage; // 탄창 이미지
    public Text magazineText; // 남은 총알 표현 UI

    public int maxBullet = 10; // 최대 총알
    public int remainingBullet = 10; // 남은 총알 수

    public float reloadTime = 2f;
    bool isReloading = false;

    void Start()
    {
        _audio = GetComponent<AudioSource>();

        // GetComponentInChildren 은 하위(자식) 오브젝트 중 제일 처음거 찾음
        muzzleFlash = firePos.GetComponentInChildren<ParticleSystem>();

        shake = GameObject.Find("CameraRig").GetComponent<Shake>();
    }

    void Update()
    {
        // 마우스 왼쪽 클릭때 발사 메서드 실행
        if (!isReloading && Input.GetMouseButtonDown(0))
        {
            remainingBullet--;
            Fire();

            if(remainingBullet == 0)
            {
                // 남은 총알이 없을 때 재장전 코루틴 함수 호출
                StartCoroutine(Reloading());
            }
        }
    }

    void Fire()
    {
        // Shake 스크립트에 있는 ShakeCmera 코루틴 함수 호출
        StartCoroutine(shake.ShakeCamera());

        // Instantiate : 오브젝트 동적생성 메소드
        // Bullet 프리팹을 동적으로 생성 (생성할 객체, 생성 위치, 생성시 회전)
        //Instantiate(bullet, firePos.position, firePos.rotation);

        //
        GameObject _bullet = GameManager.instance.GetBullet();
        if(_bullet != null)
        {
            _bullet.transform.position = firePos.position;
            _bullet.transform.rotation = firePos.rotation;
            _bullet.SetActive(true);
        }

        // Play 메소드로 파티클시스템 재생하여 이펙트 재생
        catridge.Play();
        muzzleFlash.Play();

        FireSfx();

        // fillAmount 의 속성이 float 이므로 형변환을 해서 값 손실을 없앤다.
        magazineImage.fillAmount = (float)remainingBullet / (float)maxBullet;

        // 남은 총알 표시 텍스트 갱신 함수 호출
        UpdateBulletText();
    }

    void FireSfx()
    {
        var _sfx = playerSfx.fire[(int)currWeapon];
        // PlayOneShot(재생할 클립, 시작볼륨스케일)
        _audio.PlayOneShot(_sfx, 1f);
    }

    IEnumerator Reloading()
    {
        isReloading = true;
        _audio.PlayOneShot(playerSfx.reload[(int)currWeapon], 1f);

        // 현재 장착중인 무기의 재장전 사운드의 출력시간 + 추가시간
        // 각 무기의 재장전 사운드에 맞추어 동적으로 변화
        yield return new WaitForSeconds(playerSfx.reload[(int)currWeapon].length + 0.3f);

        isReloading = false;
        magazineImage.fillAmount = 1f;
        remainingBullet = maxBullet;

        // 총알 표시 텍스트 함수 호출
        UpdateBulletText();
    }

    void UpdateBulletText()
    {
        magazineText.text = string.Format("<color=red>{0}</color>/{1}", remainingBullet, maxBullet);
    }
}
