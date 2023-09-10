using UnityEngine;

public class BulletScript : MonoBehaviour
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
        if (collision.gameObject.CompareTag("Enemy"))
        {
            collision.gameObject.GetComponent<EnemyScript>().TakeDamage(this.attackPower);
            Destroy(gameObject);
        }

        if (collision.gameObject.CompareTag("Tilemap Collider") || collision.gameObject.CompareTag("Forcefield"))
        {
            Destroy(gameObject);
        }

        if(collision.gameObject.CompareTag("Boss"))
        {
            collision.gameObject.GetComponent<MothershipScript>().TakeDamage(this.attackPower);
            Destroy(gameObject);
        }

        if(collision.gameObject.CompareTag("TargetDummy"))
        {
            GameController.instance.dummyShot = true;
            collision.gameObject.GetComponent<TargetDummyScript>().TakeDamage(this.attackPower);
            Destroy(gameObject);
        }

    }
    
}
