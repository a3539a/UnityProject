using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    Rigidbody2D rb;
    SpriteRenderer sr;
    Animator anim;

    public float maxSpeed;
    public float jumpPow;

    readonly int hashIsRun = Animator.StringToHash("isRun");
    readonly int hashIsJump = Animator.StringToHash("isJump");

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
    }

    private void Update()
    {
        // Jump
        if (Input.GetButtonDown("Jump") && !anim.GetBool(hashIsJump))
        {
            rb.AddForce(Vector2.up * jumpPow, ForceMode2D.Impulse);
            anim.SetBool(hashIsJump, true); // Jump Animation
        }

        // Move Stop
        if (Input.GetButtonUp("Horizontal"))
        {
            rb.velocity = new Vector2(0, rb.velocity.y);
            anim.SetBool(hashIsRun, false); // Run Animation
        }

        // Player Flip
        if (rb.velocity.x > 0)
        {
            sr.flipX = false;
            anim.SetBool(hashIsRun, true); // Run Animation
        }
        if (rb.velocity.x < 0)
        {
            sr.flipX = true;
            anim.SetBool(hashIsRun, true); // Run Animation
        }
    }

    private void FixedUpdate()
    {
        // Move by Key Control
        float h = Input.GetAxisRaw("Horizontal");

        rb.AddForce(Vector2.right * h, ForceMode2D.Impulse);

        // Max Speed
        if (rb.velocity.x > maxSpeed)
        {
            rb.velocity = new Vector2(maxSpeed, rb.velocity.y);
        }
        else if (rb.velocity.x < -maxSpeed)
        {
            rb.velocity = new Vector2(-maxSpeed, rb.velocity.y);
        }

        // Landing Platform
        //Debug.DrawRay(transform.position, Vector3.down, new Color(0, 1, 0)); // ·¹ÀÌ ºö ½÷¼­ ½Ã°¢È­

        RaycastHit2D rayHit = Physics2D.Raycast(transform.position, Vector3.down, 1, LayerMask.GetMask("PLATFORM"));
        // LayerMask.GetMask("PLATFORM") ÇØ´ç·¹ÀÌ¾î¸¸ Hit ½ÃÅ´

        if (rayHit.collider != null)
        {
            if (rb.velocity.y == 0)
            {
                anim.SetBool(hashIsJump, false); // Jump Animation
            }
        }
    }
}
