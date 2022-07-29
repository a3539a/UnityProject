using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fire : MonoBehaviour
{
    public Transform bulletImpact; // �Ѿ� ���� ȿ��
    ParticleSystem bulletEffect; // �Ѿ� ���� ��ƼŬ �ý���
    AudioSource bulletAudio; // �Ѿ� �߻� ����

    public Transform crossHair; // crossHair�� ���� �Ӽ�

    void Start()
    {
        bulletEffect = bulletImpact.GetComponent<ParticleSystem>();
        bulletAudio = bulletImpact.GetComponent<AudioSource>();
    }

    void Update()
    {
        // ũ�ν���� ǥ��
        ARAVRInput.DrawCrosshair(crossHair);

        // ������ư : IndexTrigger
        if (ARAVRInput.GetDown(ARAVRInput.Button.IndexTrigger))
        {
            // �Ѿ� ����� ���
            bulletAudio.Stop();
            bulletAudio.Play();

            // Ray�� ī�޶��� ��ġ�κ��� �������� �Ѵ�.
            Ray ray = new Ray(ARAVRInput.RHandPosition, ARAVRInput.RHandDirection);
            // Ray �� �浹 ������ �����ϱ� ���� ���� ����
            RaycastHit hitInfo;

            // �÷��̾� ���̾� ������
            // ���̾ �̸����� �˻��� �Ŀ� ��Ʈ�̵� �� ����� ����
            int playerLayer = 1 << LayerMask.NameToLayer("PLAYER");
            // Ÿ�� ���̾� ������
            int towerLayer = 1 << LayerMask.NameToLayer("TOWER");

            int layerMask = playerLayer | towerLayer;

            // Ray �� ���. ray�� �ε��� ������ hitinfo�� ����.
            if (Physics.Raycast(ray, out hitInfo, 200, ~layerMask))
            {
                // �Ѿ� ����Ʈ ����ǰ� ������ ���߰� ���
                bulletEffect.Stop();
                bulletEffect.Play();
                
                // �ε��� ���� �ٷ� ������ ����Ʈ�� ���̵��� ����
                bulletImpact.position = hitInfo.point;

                // �ε��� ������ �������� �Ѿ� ����Ʈ�� ������ ����
                bulletImpact.forward = hitInfo.normal;

                // ray �� �ε��� ��ü�� �̸��� drone �� ���ԵǸ� �ǰ� ó�� (��������)
                // ray �� �ε��� ��ü�� �±װ� ENEMY��
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
