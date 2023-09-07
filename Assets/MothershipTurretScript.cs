using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MothershipTurretScript : MonoBehaviour
{
    public bool limitAttack = false;

    public void DoAttack(string attackType, float attackPower, float weaponRange, float cooldown)
    {
        if (!limitAttack)
        {
            switch (attackType)
            {
                case "normal":
                    StartCoroutine(NormalAttack(attackPower, weaponRange, cooldown));
                    break;
                default:
                    break;
            }
        }
    }

    IEnumerator NormalAttack(float attackPower, float weaponRange, float cooldown)
    {
        limitAttack = true;
        GameObject bullet = Instantiate(Resources.Load<GameObject>("Prefabs/Enemy Bullet"), transform.position, Quaternion.identity);
        bullet.GetComponent<EnemyBulletScript>().Initialise(attackPower, weaponRange);
        bullet.GetComponent<Rigidbody2D>().AddRelativeForce(transform.up * 600);
        yield return new WaitForSeconds(cooldown);

        limitAttack = false;
        yield return null;
    }

}
