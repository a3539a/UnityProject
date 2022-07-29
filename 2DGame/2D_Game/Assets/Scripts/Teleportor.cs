using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Teleportor : MonoBehaviour
{

    private void Awake()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 7)
        {
            Rigidbody2D playerRb = collision.GetComponent<Rigidbody2D>();
            playerRb.position = FindObjectOfType<EdgeCollider2D>().transform.position;
        }
    }
}
