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
            // 기즈모 색상 설정
            Gizmos.color = _color;
            // 구체모양의 기즈모 생성, 인자는 (생성위치, 반경)
            Gizmos.DrawSphere(transform.position, _radius);
        }
        else if (type == Type.NORMAL)
        {
            // 기즈모 색상 설정
            Gizmos.color = _color;
            // DrawIcon(위치, 생성할 아이콘 이름, 스케일 조정 가능 여부)
            Gizmos.DrawIcon(transform.position + (Vector3.up * 1.2f), "Enemy", true);
            Gizmos.DrawWireSphere(transform.position, _radius);
        }
    }
}
