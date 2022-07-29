using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VoxelMaker : MonoBehaviour
{
    public Transform crossHair;

    public GameObject voxelPrefab;

    // ������Ʈ Ǯ���� ����Ʈ
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
            list.Add(voxel); // ������ ������ Ǯ���ٰ� ������Ʈ �߰�
        }
    }

    void Update()
    {
        ARAVRInput.DrawCrosshair(crossHair);

        // VR ��Ʈ�ѷ��� �߻��ư�� ������ ��
        if (ARAVRInput.GetDown(ARAVRInput.Button.One))
        {
            //Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            ////RaycastHit hit;

            // ������ ��Ʈ�ѷ��� ��ġ���� ��Ʈ�ѷ��� ����Ű�� �������� ���̸� �Ѹ�
            Ray ray = new Ray(ARAVRInput.RHandPosition, ARAVRInput.RHandDirection);

            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                // ������Ʈ Ǯ �ȿ� ������ ������Ʈ�� �ִٸ�
                if (list.Count > 0)
                {
                    // ������Ʈ Ǯ���� ������ �ϳ� �����´�
                    GameObject voxel = list[0];
                    // ���� Ȱ��ȭ
                    voxel.SetActive(true);
                    // ��Ʈ����Ʈ�� ���� ������ �����
                    voxel.transform.position = hit.point;
                    // ����Ʈ���� �����
                    list.RemoveAt(0);
                }
            }

        }
    }
}
