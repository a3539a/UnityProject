using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class GazePointerCtrl : MonoBehaviour
{
    public Video360Play vp360;

    public Transform uiCanvas; // �����ð����� �ü��� �ӹ��� ���� �����ֱ� ���� UI
    public Image gazeImg; // VideoPlayer�� �����ϱ� ���� ���ӽ����̽�

    Vector3 defaultScale; // UI �⺻�������� �����صα� ���� ��
    public float uiScaleVal = 1f; // UI ī�޶� 1m�� �� ����

    bool isHitObj; // �ü�(����)�� ��Ҵ���
    GameObject prevHitObj; // ���� �ü��� �ӹ����� ������Ʈ
    GameObject currHitObj; // ���� �ü��� �ӹ��� ������Ʈ
    float currGazeTime = 0;
    public float gazeChargeTime = 3f;


    void Start()
    {
        defaultScale = uiCanvas.localScale; // ������Ʈ�� ���� �⺻ ������ ��
        currGazeTime = 0;
    }

    void Update()
    {
        // ĵ���� ������Ʈ�� �������� �Ÿ��� ���� �����Ѵ�.
        // 1) ī�޶� �������� ���� ������ ��ǥ�� ���Ѵ�.
        Vector3 dir = transform.TransformPoint(Vector3.forward);
        // 2) ī�޶� �������� ������ ���̸� �����Ѵ�.
        Ray ray = new Ray(transform.position, dir);

        RaycastHit hitInfo; // ��Ʈ�� ������Ʈ�� ������ ��´�.

        // 3) ���̿� �ε��� ��쿡�� �Ÿ����� �̿��� uiCanvas�� ũ�⸦ �����Ѵ�.
        if(Physics.Raycast(ray, out hitInfo))
        {
            uiCanvas.localScale = defaultScale * uiScaleVal * hitInfo.distance;
            uiCanvas.position = transform.forward * hitInfo.distance;

            if (hitInfo.transform.CompareTag("GazeObj"))
            {
                isHitObj = true;
            }
            currHitObj = hitInfo.transform.gameObject;
        }
        // 4) �ƹ��͵� �ε����� ������ �⺻ ������ ������ uiCanvas�� ũ�⸦ �����Ѵ�.
        else
        {
            uiCanvas.localScale = defaultScale * uiScaleVal;
            uiCanvas.position = transform.position + dir;
        }
        // 5) uiCanvas�� �׻� ī�޶� ������Ʈ�� �ٶ󺸰� �Ѵ�.
        uiCanvas.forward = transform.forward * -1;

        if (isHitObj)
        {
            if (currHitObj == prevHitObj)
            {
                currGazeTime += Time.deltaTime;
            }
            else
            {
                prevHitObj = currHitObj;
            }

            // hit �� ������Ʈ�� VideoPlayer ������Ʈ�� �������� Ȯ��
            // CheckVideoFrame() ���� Ȯ�� �� ���
            HitObjChecker(currHitObj, true);
        }
        else // �ü��� ����ų� GazeObj�� �ƴ϶�� �ð��� �ʱ�ȭ
        {
            if (prevHitObj != null)
            {
                HitObjChecker(prevHitObj, false);
                prevHitObj = null;
            }
            currGazeTime = 0;
        }

        // Mathf.Clamp(������ ���ϰ��� �ϴ� ��, �ּҰ�, �ִ밪)
        currGazeTime = Mathf.Clamp(currGazeTime, 0, gazeChargeTime);
        gazeImg.fillAmount = currGazeTime / gazeChargeTime;

        isHitObj = false;
        currHitObj = null;
    }

    // ��Ʈ �� ������Ʈ Ÿ�Ժ��� �۵������ �������ش�.
    void HitObjChecker(GameObject hitObj, bool isActive)
    {
        // �浹�� ������Ʈ�� Video Player ������Ʈ�� �����ϴ��� �Ǵ�
        if (hitObj.GetComponent<VideoPlayer>())
        {
            if (isActive)
            {
                // CheckVideoFrame �Լ��� true �Ű����� ����
                hitObj.GetComponent<VideoFrame>().CheckVideoFrame(true);
            }
            else
            {
                hitObj.GetComponent<VideoFrame>().CheckVideoFrame(false);
            }
        }

        if (currGazeTime / gazeChargeTime >= 1)
        {
            // Contains : �ش� ���ڿ��� ���� �Ǿ� ������ true
            if (hitObj.name.Contains("Right"))
            {
                vp360.SwapVideoClip(true); // ���� ����
            }
            else if (hitObj.name.Contains("Left"))
            {
                vp360.SwapVideoClip(false); // ���� ����
            }
            else
            {
                // GetSiblingIndex : �ڽ� ������Ʈ�� ���̾��Ű ������ ����
                vp360.SetVideoPlay(hitObj.transform.GetSiblingIndex());
            }
            currGazeTime = 0;
        }
    }
}
