using UnityEngine;

public class GrenadeExplosion : MonoBehaviour
{
    private float splashRange = 8f;
    public float explosionRate;
    private float attackPower;

    [Header("Explosion Audio Clip")]
    public AudioClip explosion;

    void Start()
    {
        transform.localScale = new Vector3(0, 0, 0);
    }

    public void Initialise(float attackPower)
    {
        this.attackPower = attackPower;
        AudioSource.PlayClipAtPoint(explosion, transform.position);
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
            collision.gameObject.GetComponent<EnemyScript>().TakeDamage(attackPower, false, true);
        }

        if (collision.gameObject.CompareTag("Boss") && MothershipScript.instance.currentState != MothershipScript.BOSS_STATE.DEAD)
        {
            collision.gameObject.GetComponent<MothershipScript>().TakeDamage(attackPower);
        }
    }
}
