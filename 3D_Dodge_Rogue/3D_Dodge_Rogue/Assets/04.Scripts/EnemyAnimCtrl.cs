using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAnimCtrl : MonoBehaviour
{
    SpriteRenderer sr;
    GameObject player;
    Animator anim;
    EnemyMoveCtrl eMoveCtrl;

    int hitCount = 0;

    void Start()
    {
        eMoveCtrl = GetComponent<EnemyMoveCtrl>();
        sr = GetComponent<SpriteRenderer>();
        player = GameObject.FindGameObjectWithTag("Player");
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        // Enemy Anim Flip
        if (player.transform.position.x > transform.position.x)
        {
            sr.flipX = true;
        }
        else
        {
            sr.flipX = false;
        }

        float pos = Vector3.Distance(transform.position, player.transform.position);

        if (pos < 5f)
        {
            anim.SetBool("IsPunch", true);
        }
        else
        {
            anim.SetBool("IsPunch", false);
        }

        if (hitCount == 5)
        {
            anim.SetBool("IsDie", true);
        }
    }

    void OnDamaged()
    {
        // Color(R, G, B, A) A는 투명도를 지칭함
        sr.color = new Color(0, 1, 0, 0.4f);
        hitCount++;

        Invoke("OffDamaged", 0.5f);
    }

    void OffDamaged()
    {
        sr.color = new Color(1, 1, 1, 1);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("ATTACK"))
        {
            OnDamaged();
        }
    }
}
