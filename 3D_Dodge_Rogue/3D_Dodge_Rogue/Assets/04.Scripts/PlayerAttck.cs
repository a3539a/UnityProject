using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerAttck : MonoBehaviour
{
    public GameObject noShadowPrefab1;
    public GameObject noShadowPrefab2;
    public GameObject noShadowPrefab3;
    public GameObject noShadowPrefab4;
    public GameObject noShadowPrefab5;


    private void Start()
    {
        StartCoroutine(NoShadow());
    }

    void NoShadowAttack(Vector3 pos)
    {
        // Physics.OverlapSphere(시작위치, 반경, 영향받을 레이어)
        // 반경내에 해당되는 레이어에 있는 모든 충돌체 검출해냄
        int layerIdx = LayerMask.GetMask("ENEMY");
        Collider[] colls = Physics.OverlapSphere(pos, 20f, layerIdx);

        Collider[] colls2 = new Collider[colls.Length];

        colls2 = colls.OrderBy(coll => Vector3.Distance(transform.position, coll.transform.position)).ToArray();

        if (colls2.Length >= 5)
        {
            for (int i = 0; i < 5; i++)
            {
                if (i == 0)
                {
                    Instantiate(noShadowPrefab1, colls2[i].transform.position, colls2[i].transform.rotation);
                }
                else if (i == 1)
                {
                    StartCoroutine(Attack2(colls2[i]));
                }
                else if (i == 2)
                {
                    StartCoroutine(Attack3(colls2[i]));
                }
                else if (i == 3)
                {
                    StartCoroutine(Attack4(colls2[i]));
                }
                else if (i == 4)
                {
                    StartCoroutine(Attack5(colls2[i]));
                }
            }
        }
        else
        {
            for (int i = 0; i < colls2.Length; i++)
            {
                if (i == 0)
                {
                    Instantiate(noShadowPrefab1, colls2[i].transform.position, colls2[i].transform.rotation);
                }
                else if (i == 1)
                {
                    StartCoroutine(Attack2(colls2[i]));
                }
                else if (i == 2)
                {
                    StartCoroutine(Attack3(colls2[i]));
                }
                else if (i == 3)
                {
                    StartCoroutine(Attack4(colls2[i]));
                }
            }
        }

        colls = new Collider[] { };
        colls2 = new Collider[] { };
    }

    IEnumerator NoShadow()
    {
        while (true)
        {
            yield return new WaitForSeconds(5f);
            NoShadowAttack(transform.position);
        }
    }

    IEnumerator Attack2(Collider coll)
    {
        yield return new WaitForSeconds(0.2f);
        Instantiate(noShadowPrefab2, coll.transform.position, coll.transform.rotation);
    }
    IEnumerator Attack3(Collider coll)
    {
        yield return new WaitForSeconds(0.4f);
        Instantiate(noShadowPrefab3, coll.transform.position, coll.transform.rotation);
    }
    IEnumerator Attack4(Collider coll)
    {
        yield return new WaitForSeconds(0.6f);
        Instantiate(noShadowPrefab4, coll.transform.position, coll.transform.rotation);
    }
    IEnumerator Attack5(Collider coll)
    {
        yield return new WaitForSeconds(0.8f);
        Instantiate(noShadowPrefab5, coll.transform.position, coll.transform.rotation);
    }

}
