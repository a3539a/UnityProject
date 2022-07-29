using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerMoveCtrl : MonoBehaviour
{
    Animator anim;
    SpriteRenderer sr;

    void Start()
    {
        anim = GetComponent<Animator>();
        sr = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        // Player Walk Ani 출력
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        bool hasHInput = !Mathf.Approximately(h, 0.0f);
        bool hasVInput = !Mathf.Approximately(v, 0.0f);
        bool isWalk = hasHInput || hasVInput;

        anim.SetBool("isWalk", isWalk);

        if(h < 0)
        {
            sr.flipX = true;
        }
        else if(h > 0)
        {
            sr.flipX = false;
        }

        if(GameManager.Instance.pHP <= 0)
        {
            anim.SetBool("IsDie", true);
            Destroy(gameObject, 1f);
        }
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.CompareTag("ENEMY"))
        {
            OnDamaged();
            GameManager.Instance.pHP -= 25;
        }
    }

    void OnDamaged()
    {
        // Color(R, G, B, A) A는 투명도를 지칭함
        sr.color = new Color(0, 0, 1, 0.4f);

        Invoke("OffDamaged", 0.5f);
    }

    void OffDamaged()
    {
        sr.color = new Color(1, 1, 1, 1);
    }

    private void OnDestroy()
    {
        GameManager.Instance.pHP = 100;
        SceneManager.LoadScene(0);
    }
}
