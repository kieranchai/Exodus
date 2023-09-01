using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyScript : MonoBehaviour
{

    NavMeshAgent agent;
    public Animator anim;
    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;
    }
    void Update()
    {
        if (agent.velocity.magnitude > 0) {
            anim.SetBool("isWalking", true);
        }else {
            anim.SetBool("isWalking", false);
        }
        agent.SetDestination(PlayerScript.instance.transform.position);
        transform.up = (PlayerScript.instance.transform.position - new Vector3(transform.position.x, transform.position.y));
    }
}
