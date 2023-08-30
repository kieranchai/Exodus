using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoisonGas : MonoBehaviour
{
    public static PoisonGas instance { get; private set; }

    public float expandSpeed;

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
    }

    private void FixedUpdate()
    {
        transform.localScale += new Vector3(.02f * expandSpeed, .02f * expandSpeed, 0);
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        
    }
}
