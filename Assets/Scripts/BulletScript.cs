using UnityEngine;
using UnityEngine.Audio;

public class BulletScript : MonoBehaviour
{
    private float attackPower;
    private float weaponRange;
    private bool crit;
    private Vector3 initialPosition;
    private bool isPiercing = false;

    [Header("Bullet Audio Clips")]
    public AudioClip tilemapImpact;
    public AudioClip enemyImpact;

    public void Initialise(float attackPower, float weaponRange, bool crit = false)
    {
        this.attackPower = attackPower;
        this.crit = crit;
        this.weaponRange = weaponRange;
        this.initialPosition = transform.localPosition;
        if (PlayerScript.instance.weaponSlot.pierceChance > 0 && Random.Range(0, 1f) < PlayerScript.instance.weaponSlot.pierceChance - 1)
        {
            isPiercing = true;
        }
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
        if (collision.gameObject.CompareTag("Enemy"))
        {
            AudioSource.PlayClipAtPoint(enemyImpact, collision.transform.position);
            collision.gameObject.GetComponent<EnemyScript>().TakeDamage(this.attackPower, crit);

            //Bleed
            if (PlayerScript.instance.weaponSlot.gunBleedChance > 0 && Random.Range(0, 1f) < PlayerScript.instance.weaponSlot.gunBleedChance - 1)
            {
                collision.gameObject.GetComponent<EnemyScript>().StartCoroutine(collision.gameObject.GetComponent<EnemyScript>().Bleed(PlayerScript.instance.weaponSlot.gunBleedDmg));
            }

            //Lightning
            if (PlayerScript.instance.weaponSlot.lightningChance > 0 && Random.Range(0, 1f) < PlayerScript.instance.weaponSlot.lightningChance - 1)
            {
                GameObject chainLightningEffect = Resources.Load<GameObject>("Prefabs/Chain Lightning");
                chainLightningEffect.GetComponent<ChainLightningScript>().damage = PlayerScript.instance.weaponSlot.lightningDmg;
                Instantiate(chainLightningEffect, collision.transform.position, Quaternion.identity);
            }

            if (!isPiercing) Destroy(gameObject);
        }

        if (collision.gameObject.CompareTag("Tilemap Collider") || collision.gameObject.CompareTag("Forcefield"))
        {
            AudioSource.PlayClipAtPoint(tilemapImpact, collision.transform.position);
            Destroy(gameObject);
        }

        if (collision.gameObject.CompareTag("Boss"))
        {
            collision.gameObject.GetComponent<MothershipScript>().TakeDamage(this.attackPower);
            //Bleed
            if (PlayerScript.instance.weaponSlot.gunBleedChance > 0 && Random.Range(0, 1f) < PlayerScript.instance.weaponSlot.gunBleedChance - 1)
            {
                collision.gameObject.GetComponent<MothershipScript>().StartCoroutine(collision.gameObject.GetComponent<MothershipScript>().Bleed(PlayerScript.instance.weaponSlot.gunBleedDmg));
            }
            Destroy(gameObject);
        }
    }
}
