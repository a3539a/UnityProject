using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyGizmos : MonoBehaviour
{
    public enum Type { NORMAL, WAYPOINT}
    public Type type = new Type();

    public Color _color = Color.yellow;
    public float _radius = 0.1f;

    private void OnDrawGizmos()
    {
        if (type == Type.WAYPOINT)
        {
            // ����� ���� ����
            Gizmos.color = _color;
            // ��ü����� ����� ����, ���ڴ� (������ġ, �ݰ�)
            Gizmos.DrawSphere(transform.position, _radius);
        }
        else if (type == Type.NORMAL)
        {
            // ����� ���� ����
            Gizmos.color = _color;
            // DrawIcon(��ġ, ������ ������ �̸�, ������ ���� ���� ����)
            Gizmos.DrawIcon(transform.position + (Vector3.up * 1.2f), "Enemy", true);
            Gizmos.DrawWireSphere(transform.position, _radius);
        }
    }
}
