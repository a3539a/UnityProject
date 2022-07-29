using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

// 유니티의 편집기(Editor)를 부분적으로 수정할 수 있음
// 해당 스크립트는 이러한 기능을 사용하기 위하여
// 모노비헤이비어가 아니라 에디터 클래스를 상속받음
// 그 후 필요한 기능을 작성하여 에디터의 기능 일부를
// 수정할 수 있게됨

[CustomEditor(typeof(EnemyFOV))]
public class FOVEditor : Editor
{
    // 씬뷰에 표현할 기능을 구현하는 함수
    private void OnSceneGUI()
    {
        EnemyFOV fov = (EnemyFOV)target;

        // 시야각의 시작점 좌표를 계산하기 위한 각도(위치) 구함
        // fov.viewAngle * -0.5f : 전체 각도의 반
        Vector3 fromAnglePos = fov.CiclePoint(fov.viewAngle * -0.5f);
        // 씬 뷰에서 표현할 색상을 흰색으로
        Handles.color = Color.white;
        Handles.DrawWireDisc(fov.transform.position, // 원점 좌표
                             Vector3.up,             // 노멀 벡터
                             fov.viewRange);         // 반지름

        Handles.color = new Color(1, 1, 1, 0.2f);
        Handles.DrawSolidArc(fov.transform.position,
                             Vector3.up,
                             fromAnglePos, // 시작 위치
                             fov.viewAngle, // 부채꼴의 각도
                             fov.viewRange); // 반지름

        // 오브젝트의 조금 앞에 앵글각도 스트링으로 표시
        Handles.Label(fov.transform.position + (fov.transform.forward * 2f), fov.viewAngle.ToString());
    }
}
