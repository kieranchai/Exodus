using UnityEngine;
using UnityEngine.AI;

public class EnemyHomingRocketScript : MonoBehaviour
{
    private float attackPower;
    public float splashRange;
    NavMeshAgent agent;

    public void Initialise(float attackPower)
    {
        this.attackPower = attackPower;
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;
    }

    private void Update()
    {
        agent.SetDestination(PlayerScript.instance.transform.position);
        transform.up = PlayerScript.instance.transform.position - new Vector3(transform.position.x, transform.position.y);
    }

    private void Explode()
    {
        GameObject explosion = Instantiate(Resources.Load<GameObject>("Prefabs/Enemy Explosion"), transform.position, Quaternion.identity);
        explosion.GetComponent<EnemyExplosionScript>().Initialise(attackPower, splashRange);
        MothershipScript.instance.isHoming = false;
        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player") || collision.gameObject.CompareTag("Tilemap Collider"))
        {
            Explode();
        }
    }
}
