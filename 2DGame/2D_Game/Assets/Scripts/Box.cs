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
            // 발판을 밟으면 위로만 힘을 부가해주게
            playerRb.AddForce(new Vector2(0, value), ForceMode2D.Impulse);
        }
    }
}
