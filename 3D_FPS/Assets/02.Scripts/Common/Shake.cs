using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shake : MonoBehaviour
{
    public Transform shakeCamera;

    public bool shakeRotate = false;

    // ī�޶��� �ʱ� ���� �����
    Vector3 originPos;
    Quaternion originRot;

    void Start()
    {
        // ���� ���� ī�޶��� ���� ��ġ�� ȸ���� ����
        originPos = shakeCamera.localPosition;
        originRot = shakeCamera.localRotation;
    }

    // ����Ƽ���� �Ű��������� ������ �������� �ʾƵ�
    // �Ʒ��� ���� �⺻���� �����Ҽ� ����
    public IEnumerator ShakeCamera(float duration = 0.05f, 
                                   float magnitudePos = 0.03f, 
                                   float magnitudeRot = 0.1f)
    {
        float passTime = 0f;// ������ �ð��� �����ϱ� ���� ����
        while(passTime < duration)
        {
            // insideUnitSphere �ұ�Ģ�� ���ļ� �߻���
            // ���͹��� �ִ� ���� �߻���
            Vector3 shakePos = Random.insideUnitSphere;
            // ī�޶��� ��ġ�� ����
            shakeCamera.localPosition = shakePos * magnitudePos;
            // ȸ���� ����� ��
            if (shakeRotate)
            {
                // Mathf.PerlinNoise �Լ��� Ȱ���Ͽ� z ���� ȸ����Ű�� ���� ���� ����
                Vector3 shakeRot = new Vector3(0, 0, Mathf.PerlinNoise(Time.time * magnitudeRot, 0f));
                shakeCamera.localRotation = Quaternion.Euler(shakeRot);
            }
            passTime += Time.deltaTime; // ���� �ð� ����

            yield return null;
        }
        // ���� ������ ���� ���� ��ġ�� ���� ��Ű��
        shakeCamera.localPosition = originPos;
        shakeCamera.localRotation = originRot;
    }
}
