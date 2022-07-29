using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHpBar : MonoBehaviour
{
    Camera uiCamera;

    Canvas canvas;
    RectTransform rectParent;
    RectTransform rectHp;

    // public ������ �ν����Ϳ� ǥ�õ�����
    // �Ʒ� �Ӽ��� ����ϸ� ������ ���������� �ν����Ϳ��� ǥ�� �ȵ�
    [HideInInspector]
    public Vector3 offset = Vector3.zero;

    [HideInInspector]
    public Transform targetTr; // ��

    void Start()
    {
        // �θ�κ��� ĵ���� ����
        canvas = GetComponentInParent<Canvas>(); // UI Canvas
        uiCamera = canvas.worldCamera;
        rectParent = canvas.GetComponent<RectTransform>(); // UI Canvas
        // �� ��ũ��Ʈ�� ���� ������Ʈ��
        rectHp = GetComponent<RectTransform>();
    }

    private void LateUpdate()
    {
        // Camera.main = ���� ī�޶� �߱��� (����ī�޶� �±װ� �ִ� ī�޶�)
        // 1�ܰ� . WorldToScreenPoint �޼ҵ�� ���� ��ǥ�� ��ũ�� ��ǥ�� ��ȯ
        var screenPos = Camera.main.WorldToScreenPoint(targetTr.position + offset);
        if (screenPos.z < 0f) // ī�޶� HpBar�� �������� ���
        {
            // HpBar ����
            screenPos *= -1f;
        }

        var localPos = Vector2.zero;
        // 2�ܰ� . ��ũ�� ��ǥ�� RectTransform ��ǥ�� �ٽ� ��ȯ
        // ScreenPointToLocalPointInRectangle(�θ��� ��Ʈ Ʈ������, ��ũ�� ��ǥ, ������ ī�޶�, ��ȯ�� ��ǥ)
        RectTransformUtility.ScreenPointToLocalPointInRectangle(rectParent, screenPos, uiCamera, out localPos);
        // ��ȯ�� localPos��ǥ�� ����Ͽ� ü�¹��� RectTransform ��ġ�� ����
        rectHp.localPosition = localPos;
    }

    void Update()
    {
        
    }
}
