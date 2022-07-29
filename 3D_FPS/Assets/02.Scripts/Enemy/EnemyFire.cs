using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyFire : MonoBehaviour
{
    AudioSource _audio;
    Animator animator;
    Transform playerTr;
    Transform enemyTr;

    readonly int hashFire = Animator.StringToHash("Fire");
    readonly int hashReload = Animator.StringToHash("Reload");

    // 총알 발사용 변수
    float nextFire = 0f;
    readonly float fireRate = 0.1f; // 총알 발사 간격
    readonly float damping = 10f; // 플레이어를 향할 때 사용할 회전 계수

    public bool isFire = false; // 총알 발사 여부
    public AudioClip fireSfx;

    // 재장전 관련
    readonly float reloadTime = 2f;
    readonly int maxBullet = 10;
    int currBullet = 10;
    bool isReload = false;

    WaitForSeconds wsReload;

    public AudioClip reloadSfx;

    public GameObject bullet;
    public Transform firePos;

    public MeshRenderer muzzleFlash;

    void Start()
    {
        playerTr = GameObject.FindGameObjectWithTag("PLAYER").GetComponent<Transform>();
        enemyTr = GetComponent<Transform>();
        animator = GetComponent<Animator>();
        _audio = GetComponent<AudioSource>();

        wsReload = new WaitForSeconds(reloadTime);

        muzzleFlash.enabled = false;
    }

    void Update()
    {
        // 재장전 중이 아니면서
        if (!isReload && isFire)// 공격 가능 상태일때만 동작
        {
            // 진행 시간이 지정한 발사 간격보다 클 때
            if (Time.time >= nextFire)
            {
                Fire(); // 공격 함수 호출
                // 다음 발사 시간을 계산한다
                // 이 때에 발사 간격 + 랜덤한 값을 더하여
                // 발사간격을 불규칙정으로 구성
                nextFire = Time.time + fireRate + Random.Range(0f, 0.3f);
            }
            // playerTr.position - enemyTr.position = 방향 계산
            //      A            =        B         = B에서 A를 보는 방향
            Quaternion rot = Quaternion.LookRotation(playerTr.position - enemyTr.position);
            enemyTr.rotation = Quaternion.Slerp(enemyTr.rotation, rot, Time.deltaTime * damping);
        }
    }

    void Fire()
    {
        animator.SetTrigger(hashFire);
        // PlayOneShot(재생음원, 볼륨)
        // 볼륨은 0 ~ 1 까지의 실수값
        _audio.PlayOneShot(fireSfx, 1f);

        StartCoroutine(ShowMuzzleFlash());

        GameObject _bullet = Instantiate(bullet, firePos.position, firePos.rotation);
        Destroy(_bullet, 3f);

        currBullet--; // 총알 감소
        // 대입연산자(=) 오른쪽에 있는 나머지 연산결과가
        // 0 이랑 같아지면 true 아니면 false
        isReload = (currBullet % maxBullet == 0);

        if (isReload)
        {
            // 재장전 코루틴 함수 호출
            StartCoroutine(Reloading());
        }
    }

    IEnumerator ShowMuzzleFlash()
    {
        muzzleFlash.enabled = true; //머즐플래시 활성화
        // 머즐플래시를 z축으로 0 ~ 360도 랜덤 회전
        Quaternion rot = Quaternion.Euler(Vector3.forward * Random.Range(0, 360));
        muzzleFlash.transform.localRotation = rot;
        // 머즐플래시의 스케일을 1~2배 랜덤 조정
        muzzleFlash.transform.localScale = Vector3.one * Random.Range(1f, 2f);
        // 머즐플래시의 텍스처 offset을 조정하기 위해서
        // 0,0 ~ 0.5,0.5 사이의 값을 지니도록 계산
        Vector2 offset = new Vector2(Random.Range(0, 2), Random.Range(0, 2)) * 0.5f;
        // 머터리얼에 등록된 메인 텍스처의 offset을 변경하기 위한 것
        // 아래코드는 유니티 코드로 Shader가 관리하는 MainTexture에 영향을 주는 것
        muzzleFlash.material.SetTextureOffset("_MainTex", offset);

        yield return new WaitForSeconds(Random.Range(0.05f,0.2f));

        muzzleFlash.enabled = false;
    }

    IEnumerator Reloading()
    {
        // 재장전 중일 때도 머즐플래시 꺼줌
        muzzleFlash.enabled = false;

        animator.SetTrigger(hashReload);
        _audio.PlayOneShot(reloadSfx, 1f);

        // 재장전 시간만큼 대기
        yield return wsReload;

        currBullet = maxBullet;
        isReload = false; // 재장전 끝났으므로 false
    }
}
