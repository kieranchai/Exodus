using UnityEngine;

public class EnemyExplosionScript : MonoBehaviour
{
    private float splashRange;
    public float explosionRate;
    private float attackPower;
    [SerializeField] private AudioSource audioSource;

    void Start()
    {
        transform.localScale = new Vector3(0, 0, 0);
        audioSource.volume = 0.5f;
    }

    public void Initialise(float attackPower, float splashRange)
    {
        this.attackPower = attackPower;
        this.splashRange = splashRange;
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
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.gameObject.GetComponent<PlayerScript>().TakeDamage(attackPower, false);
        }

        if (collision.gameObject.CompareTag("Tilemap Collider"))
        {
            Destroy(gameObject);
        }
    }
}
