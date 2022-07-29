using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grab : MonoBehaviour
{
    // �ʿ� �Ӽ� : ��ü�� ��� �ִ��� ����, ����ִ� ��ü, ���� ��ü�� ����, ������ �ִ� �Ÿ�
    // ��üũ
    bool isGrabbing = false;
    // ���� ��ü
    GameObject grabbledObj;
    // ���� ��ü�� ����
    public LayerMask grabbledLayer;
    // ���� �� �ִ� �Ÿ�
    public float grabRange = 0.2f;

    [Header("Obj Throw")]
    // ��ü ������ ����
    // ���� ��ġ
    Vector3 prevPos;
    // ���� ��
    public float throwPower = 10;
    // ���� ȸ��
    Quaternion prevRot;
    // ȸ����
    public float rotPower = 5;

    [Header("���Ÿ� ��ü ���")]
    // ���Ÿ����� ��ü�� ��� ��� Ȱ��ȭ ����
    public bool isRemoteGrab = true;
    // ���Ÿ����� ��ü�� ���� �� �ִ� �Ÿ�
    public float remoteGrabDistance = 20;

    private void Update()
    {
        // ��ü ��� 
        // 1. ��ü�� ���� �ʰ� ���� ���
        if (!isGrabbing)
        {
            // ��� �õ�
            TryGrab();
        }
        else
        {
            // ��ü ����
            TryUnGrab();
        }
    }

    void TryGrab()
    {
        if (ARAVRInput.GetDown(ARAVRInput.Button.HandTrigger, ARAVRInput.Controller.RTouch))
        {
            // ���Ÿ� �׷� Ȱ��ȭ �Ǹ�
            if (isRemoteGrab)
            {
                Ray ray = new Ray(ARAVRInput.RHandPosition, ARAVRInput.RHandDirection);
                RaycastHit hitInfo;

                // SphereCast(����, ������, ����, �Ÿ�, ���� ���̾�)
                if (Physics.SphereCast(ray, 0.5f, out hitInfo, remoteGrabDistance, grabbledLayer))
                {
                    // ���� ����
                    isGrabbing = true;
                    // ������ ����
                    grabbledObj = hitInfo.transform.gameObject;
                    // ���� ������ ���� ����
                    StartCoroutine(GrabbingAnimation());
                }
                return;
            }

            // Ʈ���Ÿ� ���� ���� ������ ��Ʈ�ѷ��� ��ġ����
            // ���������Ǿ ���Ͽ� �׷����� ��ŭ�� �ݶ��̴� ����
            // �ش� ���� ������ grabbledLayer�� �ش�Ǵ� ��� ������Ʈ�� �������
            Collider[] hitObj = Physics.OverlapSphere(ARAVRInput.RHandPosition, grabRange, grabbledLayer);

            // ���� ����� ��ü �ε���
            int closest = 0;

            // �հ� ���� ����� ��ü ����
            for (int i = 0; i < hitObj.Length; i++)
            {
                // �հ� ���� ����� ��ü���� �Ÿ�
                Vector3 closestPos = hitObj[closest].transform.position;
                float closestDistance = Vector3.Distance(closestPos, ARAVRInput.RHandPosition);

                // ���� ��ü�� ���� �Ÿ�
                Vector3 nextPos = hitObj[i].transform.position;
                float nextDistance = Vector3.Distance(nextPos, ARAVRInput.RHandPosition);

                // ���� ��ü���� �Ÿ��� �� �����ٸ�
                if (nextDistance < closestDistance)
                {
                    // �ε��� ��ü
                    closest = i;
                }
            }

            // ����� ��ü�� �ִ� ���
            if (hitObj.Length > 0)
            {
                // ���� ���·� ��ȯ
                isGrabbing = true;
                // ���� ��ü�� ���� ���
                grabbledObj = hitObj[closest].gameObject;
                // ���� ��ü�� ���� �ڽ����� ���
                grabbledObj.transform.parent = ARAVRInput.RHand;
                // ���� ������Ʈ�� �������� ȿ�� ���� �ʵ��� ����
                grabbledObj.GetComponent<Rigidbody>().isKinematic = true;

                // ������ �� �ʱ� ��ġ ����
                prevPos = ARAVRInput.RHandPosition;
                // ������ �� �ʱ� ȸ�� ����
                prevRot = ARAVRInput.RHand.rotation;
            }
        }
    }

    void TryUnGrab()
    {
        // ���� ����
        Vector3 throwDir = ARAVRInput.RHandPosition - prevPos;
        // ��ġ ���
        prevPos = ARAVRInput.RHandPosition;

        // ���ʹϾ� ����
        // angle1 = Q1, angle2 = Q2
        // angle1 + angle2 = Q1 * Q2
        // -angle2 = Quaternion.Inverse(Q2)
        // angle2 - angle1 = Q2 * Quaternion.Inverse(Q1)
        // ȸ������ = current - previous �� ���� ���� (- �� Inverse ���)
        Quaternion deltaRotation = ARAVRInput.RHand.rotation * Quaternion.Inverse(prevRot);
        // ���� ȸ�� ����
        prevRot = ARAVRInput.RHand.rotation;

        if (ARAVRInput.GetUp(ARAVRInput.Button.HandTrigger, ARAVRInput.Controller.RTouch))
        {
            // ���� ���� ���·� ��ȯ
            isGrabbing = false;
            // ���� ��� Ȱ��ȭ
            grabbledObj.GetComponent<Rigidbody>().isKinematic = false;
            // �ڽ� ������Ʈ ���ֱ�
            grabbledObj.transform.parent = null;
            // ������
            grabbledObj.GetComponent<Rigidbody>().velocity = throwDir * throwPower;

            // �� �ӵ� = (1/dt) * d0(Ư�� �� ���� ���� ����)
            float angle;
            Vector3 axis;

            deltaRotation.ToAngleAxis(out angle, out axis);
            Vector3 anglurVelocity = (1.0f / Time.deltaTime) * angle * axis;
            grabbledObj.GetComponent<Rigidbody>().angularVelocity = anglurVelocity;

            // ���� ��ü�� ������ ����
            grabbledObj = null;
        }
    }

    IEnumerator GrabbingAnimation()
    {
        // ���� ��� ����
        grabbledObj.GetComponent<Rigidbody>().isKinematic = true;
        prevPos = ARAVRInput.RHandPosition; // �ʱ� ��ġ
        prevRot = ARAVRInput.RHand.rotation; // �ʱ� ȸ��

        Vector3 startLocation = grabbledObj.transform.position;
        Vector3 targetLocation = ARAVRInput.RHandPosition + ARAVRInput.RHandDirection * 0.1f;

        float currTime = 0;
        float finishTime = 0.2f;

        // �ð� �����
        float elapsedRate = currTime / finishTime;

        while (elapsedRate < 1)
        {
            currTime += Time.deltaTime;
            elapsedRate = currTime / finishTime;
            grabbledObj.transform.position = Vector3.Lerp(startLocation, targetLocation, elapsedRate);

            yield return null;
        }

        // ���� ��ü�� ���� �ڽ����� ���
        grabbledObj.transform.position = targetLocation;
        grabbledObj.transform.parent = ARAVRInput.RHand;
    }

}
