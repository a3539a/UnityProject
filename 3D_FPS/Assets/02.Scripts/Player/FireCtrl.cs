using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// ����ȭ Ư��
// �ٷξƷ��� public ���� ����� Ŭ����/����ü�� �ν����Ϳ� ǥ��
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
    // ���� ����Ÿ���� RIFLE�� �����صα�
    public WeaponType currWeapon = WeaponType.RIFLE;

    // �Ѿ� ������
    public GameObject bullet;
    // �Ѿ� �߻� ��ǥ
    public Transform firePos;

    AudioSource _audio;

    public PlayerSfx playerSfx;

    public ParticleSystem catridge;
    ParticleSystem muzzleFlash;

    Shake shake;

    // �Ѿ� UI ����
    public Image magazineImage; // źâ �̹���
    public Text magazineText; // ���� �Ѿ� ǥ�� UI

    public int maxBullet = 10; // �ִ� �Ѿ�
    public int remainingBullet = 10; // ���� �Ѿ� ��

    public float reloadTime = 2f;
    bool isReloading = false;

    void Start()
    {
        _audio = GetComponent<AudioSource>();

        // GetComponentInChildren �� ����(�ڽ�) ������Ʈ �� ���� ó���� ã��
        muzzleFlash = firePos.GetComponentInChildren<ParticleSystem>();

        shake = GameObject.Find("CameraRig").GetComponent<Shake>();
    }

    void Update()
    {
        // ���콺 ���� Ŭ���� �߻� �޼��� ����
        if (!isReloading && Input.GetMouseButtonDown(0))
        {
            remainingBullet--;
            Fire();

            if(remainingBullet == 0)
            {
                // ���� �Ѿ��� ���� �� ������ �ڷ�ƾ �Լ� ȣ��
                StartCoroutine(Reloading());
            }
        }
    }

    void Fire()
    {
        // Shake ��ũ��Ʈ�� �ִ� ShakeCmera �ڷ�ƾ �Լ� ȣ��
        StartCoroutine(shake.ShakeCamera());

        // Instantiate : ������Ʈ �������� �޼ҵ�
        // Bullet �������� �������� ���� (������ ��ü, ���� ��ġ, ������ ȸ��)
        //Instantiate(bullet, firePos.position, firePos.rotation);

        //
        GameObject _bullet = GameManager.instance.GetBullet();
        if(_bullet != null)
        {
            _bullet.transform.position = firePos.position;
            _bullet.transform.rotation = firePos.rotation;
            _bullet.SetActive(true);
        }

        // Play �޼ҵ�� ��ƼŬ�ý��� ����Ͽ� ����Ʈ ���
        catridge.Play();
        muzzleFlash.Play();

        FireSfx();

        // fillAmount �� �Ӽ��� float �̹Ƿ� ����ȯ�� �ؼ� �� �ս��� ���ش�.
        magazineImage.fillAmount = (float)remainingBullet / (float)maxBullet;

        // ���� �Ѿ� ǥ�� �ؽ�Ʈ ���� �Լ� ȣ��
        UpdateBulletText();
    }

    void FireSfx()
    {
        var _sfx = playerSfx.fire[(int)currWeapon];
        // PlayOneShot(����� Ŭ��, ���ۺ���������)
        _audio.PlayOneShot(_sfx, 1f);
    }

    IEnumerator Reloading()
    {
        isReloading = true;
        _audio.PlayOneShot(playerSfx.reload[(int)currWeapon], 1f);

        // ���� �������� ������ ������ ������ ��½ð� + �߰��ð�
        // �� ������ ������ ���忡 ���߾� �������� ��ȭ
        yield return new WaitForSeconds(playerSfx.reload[(int)currWeapon].length + 0.3f);

        isReloading = false;
        magazineImage.fillAmount = 1f;
        remainingBullet = maxBullet;

        // �Ѿ� ǥ�� �ؽ�Ʈ �Լ� ȣ��
        UpdateBulletText();
    }

    void UpdateBulletText()
    {
        magazineText.text = string.Format("<color=red>{0}</color>/{1}", remainingBullet, maxBullet);
    }
}
