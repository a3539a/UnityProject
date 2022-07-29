using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    Vector3 direction;

    public void Shoot(Vector3 dir)
    {
        direction = dir;
    }

    void DestoyBullet()
    {
        ObjectPool.ReturnObject(this);
    }

    private void Update()
    {
        transform.Translate(direction);
    }
}
