using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

// ����Ƽ�� ������(Editor)�� �κ������� ������ �� ����
// �ش� ��ũ��Ʈ�� �̷��� ����� ����ϱ� ���Ͽ�
// �������̺� �ƴ϶� ������ Ŭ������ ��ӹ���
// �� �� �ʿ��� ����� �ۼ��Ͽ� �������� ��� �Ϻθ�
// ������ �� �ְԵ�

[CustomEditor(typeof(EnemyFOV))]
public class FOVEditor : Editor
{
    // ���信 ǥ���� ����� �����ϴ� �Լ�
    private void OnSceneGUI()
    {
        EnemyFOV fov = (EnemyFOV)target;

        // �þ߰��� ������ ��ǥ�� ����ϱ� ���� ����(��ġ) ����
        // fov.viewAngle * -0.5f : ��ü ������ ��
        Vector3 fromAnglePos = fov.CiclePoint(fov.viewAngle * -0.5f);
        // �� �信�� ǥ���� ������ �������
        Handles.color = Color.white;
        Handles.DrawWireDisc(fov.transform.position, // ���� ��ǥ
                             Vector3.up,             // ��� ����
                             fov.viewRange);         // ������

        Handles.color = new Color(1, 1, 1, 0.2f);
        Handles.DrawSolidArc(fov.transform.position,
                             Vector3.up,
                             fromAnglePos, // ���� ��ġ
                             fov.viewAngle, // ��ä���� ����
                             fov.viewRange); // ������

        // ������Ʈ�� ���� �տ� �ޱ۰��� ��Ʈ������ ǥ��
        Handles.Label(fov.transform.position + (fov.transform.forward * 2f), fov.viewAngle.ToString());
    }
}
