using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing         ;

public class Teleport : MonoBehaviour
{
    // �ڷ���Ʈ�� ǥ���� UI
    public Transform teleportCircleUI;
    // ���� �׸� ���� ������
    LineRenderer lr;

    Vector3 originScale = Vector3.one * 0.02f;

    [Header("����Ʈ ���μ��� ����")]
    public bool isWarp = false;
    public float warpTime = 0.1f;
    public PostProcessVolume post;

    void Start()
    {
        // ���� �� �� ��Ȱ��ȭ
        teleportCircleUI.gameObject.SetActive(false);
        lr = GetComponent<LineRenderer>();
    }

    void Update()
    {
        // ���� ��Ʈ�ѷ� ��ư One ���� ��
        if (ARAVRInput.GetDown(ARAVRInput.Button.One, ARAVRInput.Controller.LTouch))
        {
            // ���η����� Ȱ��ȭ
            lr.enabled = true;
        }
        // ���� ��Ʈ�ѷ� ��ư One ���� ��
        else if(ARAVRInput.GetUp(ARAVRInput.Button.One, ARAVRInput.Controller.LTouch))
        {
            // ���η����� ��Ȱ��ȭ
            lr.enabled = false;

            if (teleportCircleUI.gameObject.activeSelf)
            {

                if (!isWarp)
                {
                    GetComponent<CharacterController>().enabled = false;
                    // �ڷ���Ʈ UI ��ġ�� �����ǰ� ����
                    transform.position = teleportCircleUI.position + Vector3.up;
                    GetComponent<CharacterController>().enabled = true;
                }
                else
                {
                    // ���� ����� ��� �Ҷ��� Warp �ڷ�ƾ ȣ��
                    StartCoroutine(Warp());
                }
            }

            // UI ����
            teleportCircleUI.gameObject.SetActive(false);
        }
        // ���� ��Ʈ�ѷ� ��ư One ������ ������
        else if (ARAVRInput.Get(ARAVRInput.Button.One, ARAVRInput.Controller.LTouch))
        {
            // ���� ��Ʈ�ѷ��� �������� Ray�� �����.
            Ray ray = new Ray(ARAVRInput.LHandPosition, ARAVRInput.LHandDirection);
            RaycastHit hitInfo;

            // TERRAIN ���̾� ����
            int layer = 1 << LayerMask.NameToLayer("TERRAIN");

            if (Physics.Raycast(ray, out hitInfo, 200, layer))
            {
                // �ε��� ������ �ڷ���Ʈ UI ǥ��
                // ray �� �ε��� ������ ���α׸���
                lr.SetPosition(0, ray.origin);
                lr.SetPosition(1, hitInfo.point);
                
                // ray�� �ε��� ������ �ڷ���ƮUI ǥ��
                teleportCircleUI.gameObject.SetActive(true);
                teleportCircleUI.position = hitInfo.point;

                // �ڷ���Ʈ UI�� ���� ���� �ֵ��� ���� ����
                teleportCircleUI.forward = hitInfo.normal;

                // �ڷ���Ʈ UI�� ũ�Ⱑ �Ÿ��� ���� �����ǰ�
                teleportCircleUI.localScale = originScale * Mathf.Max(1, hitInfo.distance);
            }
            else
            {
                // Ray �浹�� ������ ���� �׷������� ó��
                lr.SetPosition(0, ray.origin);
                lr.SetPosition(1, ray.origin + ARAVRInput.LHandDirection * 200);

                // �ڷ���Ʈ UI�� ȭ�鿡�� ����
                teleportCircleUI.gameObject.SetActive(true);
            }
        }
    }

    IEnumerator Warp()
    {
        // ���� ������ ǥ���� ��� ��
        MotionBlur blur;
        // ���� ������ ���
        Vector3 pos = transform.position;
        // ������
        Vector3 targetPos = teleportCircleUI.position + Vector3.up;
        // ��������ð�
        float currTime = 0;
        // ����Ʈ ���μ��̿��� ��� ���� �������Ͽ��� ��Ǻ� ��� ����
        post.profile.TryGetSettings<MotionBlur>(out blur);
        // ���� ���� �� �� �ѱ�
        blur.active = true;
        GetComponent<CharacterController>().enabled = false;

        while(currTime < warpTime)
        {
            // ��� �ð� �帣�� �ϱ�
            currTime += Time.deltaTime;
            // ������ ���������� �������� �����ϱ� ���� �����ð� ���� �̵�
            transform.position = Vector3.Lerp(pos, targetPos, currTime / warpTime);
            // �ڷ�ƾ ���
            yield return null;
        }

        // �ڷ���Ʈ UI ��ġ�� ���� �̵�
        transform.position = teleportCircleUI.position + Vector3.up;
        // ĳ���� ��Ʈ�ѷ� �ٽ� �ѱ�
        GetComponent<CharacterController>().enabled = true;
        // ����Ʈ ȿ�� ����
        blur.active = false;
    }
}
