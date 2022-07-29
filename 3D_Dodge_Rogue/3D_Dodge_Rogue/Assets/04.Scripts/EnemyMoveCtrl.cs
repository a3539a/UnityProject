using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyMoveCtrl : MonoBehaviour
{
    
    // 길을 찾아서 이동할 에이전트
    NavMeshAgent agent;
    // 에이전트의 목적지
    GameObject target;
    Rigidbody rb;

    public float moveSpeed = 5f;

    public int eHP = 30;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;

        target = GameObject.FindGameObjectWithTag("Player");
    }

    void Update()
    {
        // Monster Move Set
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        Vector3 moveVelocityH = Vector3.zero;
        Vector3 moveVelocityV = Vector3.zero;
        if (h < 0)
        {
            moveVelocityH = Vector3.left;
        }
        else if (h > 0)
        {
            moveVelocityH = Vector3.right;
        }

        if (v < 0)
        {
            moveVelocityV = Vector3.back;
        }
        else if (v > 0)
        {
            moveVelocityV = Vector3.forward;
        }

        transform.position -= moveVelocityH * moveSpeed * Time.deltaTime;
        transform.position -= moveVelocityV * moveSpeed * Time.deltaTime;
        agent.SetDestination(target.transform.position);
    }


    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("ATTACK"))
        {
            eHP -= 5;
            Debug.Log(eHP);
            if (eHP == 0)
            {
                agent.isStopped = true;
                Destroy(gameObject, 1f);
            }
        }
    }

}
