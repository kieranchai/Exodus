using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyRocketScript : MonoBehaviour
{
    private float attackPower;
    private float weaponRange;
    private Vector3 initialPosition;
    public float splashRange;

    private float timer = 0f;
    private float duration = 2.5f;

    public void Initialise(float attackPower, float weaponRange)
    {
        this.attackPower = attackPower;
        this.weaponRange = weaponRange;
        this.initialPosition = transform.localPosition;
    }

    private void Explode()
    {
        GameObject explosion = Instantiate(Resources.Load<GameObject>("Prefabs/Enemy Explosion"), transform.position, Quaternion.identity);
        explosion.GetComponent<EnemyExplosionScript>().Initialise(attackPower, splashRange);
        Destroy(gameObject);
    }

    void Update()
    {
        if ((transform.position - initialPosition).magnitude >= this.weaponRange)
        {
            Explode();
        }

        timer += Time.deltaTime;
        if(timer >= duration)
        {
            Explode();
        }

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player") || collision.gameObject.CompareTag("Tilemap Collider"))
        {
            Explode();
        }
    }
}
