using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VoxelMaker : MonoBehaviour
{
    public Transform crossHair;

    public GameObject voxelPrefab;

    // 오브젝트 풀링용 리스트
    public static List<GameObject> list = new List<GameObject>();

    public int maxPools = 10;

    void Start()
    {
        GameObject poolObjects = new GameObject("VoxelPools");
        for (int i = 0; i < maxPools; i++)
        {
            GameObject voxel = Instantiate(voxelPrefab);
            voxel.name = "Voxel_" + i.ToString("00");
            voxel.SetActive(false);
            voxel.transform.SetParent(poolObjects.transform);
            list.Add(voxel); // 위에서 생성한 풀에다가 오브젝트 추가
        }
    }

    void Update()
    {
        ARAVRInput.DrawCrosshair(crossHair);

        // VR 컨트롤러의 발사버튼을 눌렀을 때
        if (ARAVRInput.GetDown(ARAVRInput.Button.One))
        {
            //Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            ////RaycastHit hit;

            // 오른쪽 컨트롤러의 위치에서 컨트롤러가 가리키는 방향으로 레이를 뿌림
            Ray ray = new Ray(ARAVRInput.RHandPosition, ARAVRInput.RHandDirection);

            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                // 오브젝트 풀 안에 생성된 오브젝트가 있다면
                if (list.Count > 0)
                {
                    // 오브젝트 풀에서 복셀을 하나 가져온다
                    GameObject voxel = list[0];
                    // 복셀 활성화
                    voxel.SetActive(true);
                    // 히트포인트에 복셀 포지션 잡아줌
                    voxel.transform.position = hit.point;
                    // 리스트에서 지운다
                    list.RemoveAt(0);
                }
            }

        }
    }
}
