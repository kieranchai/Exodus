using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBulletScript : MonoBehaviour
{
    private float attackPower;
    private float weaponRange;
    private Vector3 initialPosition;

    public void Initialise(float attackPower, float weaponRange)
    {
        this.attackPower = attackPower;
        this.weaponRange = weaponRange;
        this.initialPosition = transform.localPosition;
    }

    private void Update()
    {
        if ((transform.position - initialPosition).magnitude >= this.weaponRange)
        {
            Destroy(gameObject);
        }
    }
     private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerScript.instance.TakeDamage(this.attackPower, false);
            Destroy(gameObject);
        }

        if (collision.gameObject.CompareTag("Tilemap Collider"))
        {
            Destroy(gameObject);
        }
    }
}
