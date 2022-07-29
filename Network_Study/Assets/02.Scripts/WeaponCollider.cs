using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponCollider : MonoBehaviour
{
    public BoxCollider weaponCol;

    void Start()
    {
        // 충돌영역 비활성화
        
    }

    public void DeactiveCollider()
    {
        weaponCol.enabled = false;
    }

    public void ActiveCollider()
    {
        weaponCol.enabled = true;
    }
}
