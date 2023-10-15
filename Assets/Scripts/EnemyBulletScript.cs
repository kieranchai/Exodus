using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBulletScript : MonoBehaviour
{
    private float attackPower;
    private float weaponRange;
    private Vector3 initialPosition;

    private AudioSource SFXSource;
    [Header("Enemy Bullet Audio Clips")]
    public AudioClip tilemapImpact;
    public AudioClip playerImpact;

    private void Awake()
    {
        SFXSource = GetComponent<AudioSource>();
    }

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
            SFXSource.PlayOneShot(playerImpact);
            PlayerScript.instance.TakeDamage(this.attackPower, false);
            Destroy(gameObject);
        }

        if (collision.gameObject.CompareTag("Tilemap Collider"))
        {
            SFXSource.PlayOneShot(tilemapImpact);
            Destroy(gameObject);
        }
    }
}
