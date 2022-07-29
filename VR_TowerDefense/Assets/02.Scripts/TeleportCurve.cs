using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportCurve : MonoBehaviour
{
    // �ڷ���Ʈ�� ǥ���� UI
    public Transform teleportCircleUI;
    // ���� �׸� ���� ������
    LineRenderer lr;
    // ���� �ڷ���Ʈ UI ũ��
    Vector3 originScale = Vector3.one * 0.02f;
    // Ŀ���� �ε巯�� ����
    public int lineSmooth = 40;
    // Ŀ���� ����
    public float curveLength = 50f;
    // Ŀ���� �߷�
    public float gravity = -60f;
    // � �ùķ��̼��� ���� �� �ð�
    public float simulateTime = 0.02f;
    // ��� �̷�� ������ ����� ����Ʈ
    List<Vector3> lines = new List<Vector3>();

    private void Start()
    {
        // ���� �� �� ��Ȱ��ȭ
        teleportCircleUI.gameObject.SetActive(false);
        lr = GetComponent<LineRenderer>();

        // ���� �������� �� �ʺ� ����
        lr.startWidth = 0.0f;
        lr.endWidth = 0.2f;
    }

    private void Update()
    {
        // ���� ��Ʈ�ѷ� ��ư One ���� ��
        if (ARAVRInput.GetDown(ARAVRInput.Button.One, ARAVRInput.Controller.LTouch))
        {
            // ���η����� Ȱ��ȭ
            lr.enabled = true;
        }
        // ���� ��Ʈ�ѷ� ��ư One ���� ��
        else if (ARAVRInput.GetUp(ARAVRInput.Button.One, ARAVRInput.Controller.LTouch))
        {
            // ���η����� ��Ȱ��ȭ
            lr.enabled = false;

            if (teleportCircleUI.gameObject.activeSelf)
            {
                GetComponent<CharacterController>().enabled = false;
                // �ڷ���Ʈ UI ��ġ�� �����ǰ� ����
                transform.position = teleportCircleUI.position + Vector3.up;
                GetComponent<CharacterController>().enabled = true;
            }
            // �ڷ���Ʈ UI ��Ȱ��ȭ
            teleportCircleUI.gameObject.SetActive(false);
        }
        // ���� ��Ʈ�ѷ� ��ư One ������ ������
        else if (ARAVRInput.Get(ARAVRInput.Button.One, ARAVRInput.Controller.LTouch))
        {
            // �־��� ���� ũ���� Ŀ�긦 ����� �ʹ�.
            MakeLines();
        }
    }

    void MakeLines()
    {
        // ����Ʈ�� ��� ��ġ �������� ����ش�.
        lines.Clear();
        // ���� ����� ������ ���Ѵ�.
        Vector3 dir = ARAVRInput.LHandDirection * curveLength;
        // ���� �׷��� ��ġ�� �ʱⰪ�� ����
        Vector3 pos = ARAVRInput.LHandPosition;
        // ���� ��ġ�� ����Ʈ�� ��´�.
        lines.Add(pos);

        // lineSmooth �� ������ŭ �ݺ�
        for (int i = 0; i < lineSmooth; i++)
        {
            // ���� ��ġ ���
            Vector3 lastPos = pos;
            // �߷��� ������ �ӵ� ���
            // v = v0 + at
            dir.y += gravity * simulateTime;
            // ��ӿ���� ���� ��ġ ���
            // P = P0 + vt
            pos += dir * simulateTime;

            // Ray �浹 üũ�� ������
            if (CheckHitRay(lastPos, ref pos))
            {
                // �浹������ ��� �� ����
                lines.Add(pos);
                break;
            }
            else
            {
                // UI ����
                teleportCircleUI.gameObject.SetActive(false);
            }

            // ���� ��ġ�� ���
            lines.Add(pos);
        }

        // ���η������� ǥ���� ���� ������ ��ϵ� ������ ũ��� �Ҵ�
        lr.positionCount = lines.Count;
        // ���η������� ������ ���� ������ ����
        lr.SetPositions(lines.ToArray());
    }

    // �� ���� ��ġ�� ���� ���� ��ġ�� �޾� ������ �浹 üũ
    bool CheckHitRay(Vector3 lastPos, ref Vector3 pos)
    {
        // ���� lastPos���� ���� �� pos�� ���ϴ� ���� ���
        Vector3 rayDir = pos - lastPos;
        Ray ray = new Ray(lastPos, rayDir);
        RaycastHit hitInfo;

        // Raycast �� �� ������ ũ�⸦ �� ���� ���� �� ������ �Ÿ��� �����Ѵ�.
        if (Physics.Raycast(ray, out hitInfo, rayDir.magnitude))
        {
            // ���� ���� ��ġ�� �浹�� �������� ����
            pos = hitInfo.point;

            int layer = LayerMask.NameToLayer("TERRAIN");

            if (hitInfo.transform.gameObject.layer == layer)
            {
                // �ڷ���Ʈ UI Ȱ��ȭ
                teleportCircleUI.gameObject.SetActive(true);
                // UI ��ġ ����
                teleportCircleUI.position = pos;
                // ����
                teleportCircleUI.forward = hitInfo.normal;
                float distance = (pos - ARAVRInput.LHandPosition).magnitude;
                // UI ���̴� ũ�� ����
                teleportCircleUI.localScale = originScale * Mathf.Max(1, distance);
            }
            return true;
        }
        return false;
    }
}
