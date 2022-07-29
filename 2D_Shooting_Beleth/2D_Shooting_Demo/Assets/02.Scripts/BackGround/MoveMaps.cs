using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveMaps : MonoBehaviour
{
    // ��ũ�Ѹ� �ӵ�
    public float speed;
    // ��������Ʈ �迭 ���� ���� (���� �ε���, ������ �ε���)
    public int startIdx;
    public int endIdx;

    // �ʵ� ����
    public Transform[] mapSprites;

    // ī�޶� ����
    float viewHeight;

    private void Awake()
    {
        // �ʻ����� ����
        viewHeight = Camera.main.orthographicSize * 2f;
    }

    void Update()
    {
        // ���� ������Ʈ�� ������
        Vector3 currPos = transform.position;
        // ������ �Ÿ�
        Vector3 nextPos = Vector3.down * speed * Time.deltaTime;

        // �ʱ׷��� nextPos ��ŭ �����ش�.
        transform.position = currPos + nextPos;

        // �� �迭�� ������ ������Ʈ�� ������ y ���� ī�޶��� ������ ������ ���� ���� ��
        if (mapSprites[endIdx].position.y < -viewHeight)
        {
            // Sprite Reuse
            // ������ �ʿ�����Ʈ�� localPosition ��
            Vector3 upperMapPos = mapSprites[startIdx].localPosition;
            // �ǾƷ��� �ʿ�����Ʈ�� localPosition ��
            Vector3 lowerMapPos = mapSprites[endIdx].localPosition;

            // �ǾƷ��� �ʿ�����Ʈ�� �������� ������ �ʿ�����Ʈ �����ǿ��� ���� ī�޶��� ������ ������ ��ŭ ���� �ٲ��ش�.
            mapSprites[endIdx].transform.localPosition = upperMapPos + Vector3.up * viewHeight;

            // Cursor Idx Change
            // ��ŸƮ�ε��� ����
            int saveIdx = startIdx;
            // ��ŸƮ �ε����� ������ �ε��� ����
            startIdx = endIdx;
            // ������ �ε����� ���̺��ε��� -1 �� ����, -1 �� �� ��� (mapSprites.Length - 1) �� ����
            endIdx = (saveIdx - 1 == -1) ? mapSprites.Length - 1 : saveIdx - 1;
        }
    }
}
