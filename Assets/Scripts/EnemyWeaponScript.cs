using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyWeaponScript : MonoBehaviour
{
    private string weaponName;
    private float attackPower;
    private string weaponType;
    private float cooldown;
    private float weaponRange;
    private bool limitAttack;
    public string spritePath;

    public void SetWeaponData(Weapon weaponData)
    {
        this.weaponName = weaponData.name;
        this.attackPower = weaponData.attackPower;
        this.weaponType = weaponData.weaponType;
        this.cooldown = weaponData.cooldown;
        this.weaponRange = weaponData.weaponRange;
        this.spritePath = weaponData.spritePath;

        SpriteRenderer weaponSprite = gameObject.GetComponent<SpriteRenderer>();
        Sprite sprite = Resources.Load<Sprite>(this.spritePath);
        weaponSprite.sprite = sprite;

    }

    public void TryAttack()
    {
        if (!limitAttack)
        {
            switch (this.weaponType)
            {
                case "line":
                    StartCoroutine(LineAttack());
                    break;
                default:
                    break;
            }

        }
    }

    IEnumerator LineAttack()
    {
        limitAttack = true;
        float spread = Random.Range(-5.0f, 5.0f);
        GameObject bullet = Instantiate(Resources.Load<GameObject>("Prefabs/Enemy Bullet"), transform.position, Quaternion.Euler(0, 0, spread));
        bullet.GetComponent<EnemyBulletScript>().Initialise(this.attackPower, this.weaponRange);
        bullet.GetComponent<Rigidbody2D>().AddRelativeForce(transform.right * 600);
        yield return new WaitForSeconds(this.cooldown);

        limitAttack = false;
        yield return null;
    }
}
