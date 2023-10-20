using UnityEngine;

public class GrenadeScript : MonoBehaviour
{
    private float attackPower;
    private float timer = 0f;
    private float duration = 1f;

    public void Initialise(float attackPower)
    {
        this.attackPower = attackPower;
    }

    private void Update()
    {
        timer += Time.deltaTime;
        if (timer >= duration)
        {
            Explode();
        }
    }

    private void Explode()
    {
        GameObject explosion = Instantiate(Resources.Load<GameObject>("Prefabs/Grenade Explosion"), transform.position, Quaternion.identity);
        explosion.GetComponent<GrenadeExplosion>().Initialise(attackPower);
        Destroy(gameObject);
    }
}
