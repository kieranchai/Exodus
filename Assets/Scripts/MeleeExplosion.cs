using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeExplosion : MonoBehaviour
{
    private float splashRange = 2f;
    public float explosionRate;
    private float attackPower;
    [SerializeField] private AudioSource audioSource;

    void Start()
    {
        transform.localScale = new Vector3(0, 0, 0);
        audioSource.volume = 0.5f;
    }

    public void Initialise(float attackPower)
    {
        this.attackPower = attackPower;
        AudioSource.PlayClipAtPoint(audioSource.clip, transform.position);
    }

    void Update()
    {
        if (transform.localScale.x < splashRange)
        {
            transform.localScale += new Vector3(explosionRate, explosionRate, explosionRate) * Time.deltaTime;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            collision.gameObject.GetComponent<EnemyScript>().TakeDamage(attackPower);
        }
    }
}
