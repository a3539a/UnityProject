using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Box : MonoBehaviour
{
    public string type;
    public float value;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.layer == 7)
        {
            Rigidbody2D playerRb = collision.GetComponent<Rigidbody2D>();
            // ������ ������ ���θ� ���� �ΰ����ְ�
            playerRb.AddForce(new Vector2(0, value), ForceMode2D.Impulse);
        }
    }
}
