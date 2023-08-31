using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoisonGas : MonoBehaviour
{
    public static PoisonGas instance { get; private set; }

    public float expandSpeed;
    public float damage;

    private float savedTimer = 0f;
    private float damageInterval = 1f;

    [SerializeField]
    private Transform endLocation;

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
    }

    private void Update()
    {
        transform.position = Vector3.MoveTowards(transform.position, new Vector3(endLocation.position.x, endLocation.position.y + (gameObject.GetComponent<Collider2D>().bounds.size.y / 2)), expandSpeed * Time.deltaTime);
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if((Time.time - savedTimer) > damageInterval)
            {
                savedTimer = Time.time;
                collision.GetComponent<PlayerScript>().TakeDamage(damage);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag("Safe Zone"))
        {
            // LOSE
        }
    }
}
