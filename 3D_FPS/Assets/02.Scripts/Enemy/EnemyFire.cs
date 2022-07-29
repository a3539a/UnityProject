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

    // �Ѿ� �߻�� ����
    float nextFire = 0f;
    readonly float fireRate = 0.1f; // �Ѿ� �߻� ����
    readonly float damping = 10f; // �÷��̾ ���� �� ����� ȸ�� ���

    public bool isFire = false; // �Ѿ� �߻� ����
    public AudioClip fireSfx;

    // ������ ����
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
        // ������ ���� �ƴϸ鼭
        if (!isReload && isFire)// ���� ���� �����϶��� ����
        {
            // ���� �ð��� ������ �߻� ���ݺ��� Ŭ ��
            if (Time.time >= nextFire)
            {
                Fire(); // ���� �Լ� ȣ��
                // ���� �߻� �ð��� ����Ѵ�
                // �� ���� �߻� ���� + ������ ���� ���Ͽ�
                // �߻簣���� �ұ�Ģ������ ����
                nextFire = Time.time + fireRate + Random.Range(0f, 0.3f);
            }
            // playerTr.position - enemyTr.position = ���� ���
            //      A            =        B         = B���� A�� ���� ����
            Quaternion rot = Quaternion.LookRotation(playerTr.position - enemyTr.position);
            enemyTr.rotation = Quaternion.Slerp(enemyTr.rotation, rot, Time.deltaTime * damping);
        }
    }

    void Fire()
    {
        animator.SetTrigger(hashFire);
        // PlayOneShot(�������, ����)
        // ������ 0 ~ 1 ������ �Ǽ���
        _audio.PlayOneShot(fireSfx, 1f);

        StartCoroutine(ShowMuzzleFlash());

        GameObject _bullet = Instantiate(bullet, firePos.position, firePos.rotation);
        Destroy(_bullet, 3f);

        currBullet--; // �Ѿ� ����
        // ���Կ�����(=) �����ʿ� �ִ� ������ ��������
        // 0 �̶� �������� true �ƴϸ� false
        isReload = (currBullet % maxBullet == 0);

        if (isReload)
        {
            // ������ �ڷ�ƾ �Լ� ȣ��
            StartCoroutine(Reloading());
        }
    }

    IEnumerator ShowMuzzleFlash()
    {
        muzzleFlash.enabled = true; //�����÷��� Ȱ��ȭ
        // �����÷��ø� z������ 0 ~ 360�� ���� ȸ��
        Quaternion rot = Quaternion.Euler(Vector3.forward * Random.Range(0, 360));
        muzzleFlash.transform.localRotation = rot;
        // �����÷����� �������� 1~2�� ���� ����
        muzzleFlash.transform.localScale = Vector3.one * Random.Range(1f, 2f);
        // �����÷����� �ؽ�ó offset�� �����ϱ� ���ؼ�
        // 0,0 ~ 0.5,0.5 ������ ���� ���ϵ��� ���
        Vector2 offset = new Vector2(Random.Range(0, 2), Random.Range(0, 2)) * 0.5f;
        // ���͸��� ��ϵ� ���� �ؽ�ó�� offset�� �����ϱ� ���� ��
        // �Ʒ��ڵ�� ����Ƽ �ڵ�� Shader�� �����ϴ� MainTexture�� ������ �ִ� ��
        muzzleFlash.material.SetTextureOffset("_MainTex", offset);

        yield return new WaitForSeconds(Random.Range(0.05f,0.2f));

        muzzleFlash.enabled = false;
    }

    IEnumerator Reloading()
    {
        // ������ ���� ���� �����÷��� ����
        muzzleFlash.enabled = false;

        animator.SetTrigger(hashReload);
        _audio.PlayOneShot(reloadSfx, 1f);

        // ������ �ð���ŭ ���
        yield return wsReload;

        currBullet = maxBullet;
        isReload = false; // ������ �������Ƿ� false
    }
}
