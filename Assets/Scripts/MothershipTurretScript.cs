using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MothershipTurretScript : MonoBehaviour
{
    public bool limitAttack = false;
    public SpriteRenderer weaponSprite;
    public Sprite sprite;
    public Sprite flash;
    public int framesToFlash = 2;

    private AudioSource SFXSource;
    [Header("Mothership Turret Audio Clips")]
    public AudioClip gunAttack;

    private void Awake()
    {
        SFXSource = GetComponent<AudioSource>();
    }

    public void DoAttack(string attackType, float attackPower, float weaponRange, float cooldown)
    {
        if (!limitAttack)
        {
            switch (attackType)
            {
                case "normal":
                    StartCoroutine(NormalAttack(attackPower, weaponRange, cooldown));
                    break;
                case "burst":
                    StartCoroutine(BurstAttack(attackPower, weaponRange, cooldown));
                    break;
                default:
                    break;
            }
        }
    }

    IEnumerator NormalAttack(float attackPower, float weaponRange, float cooldown)
    {
        StartCoroutine(FlashMuzzleFlash());
        limitAttack = true;
        GameObject bullet = Instantiate(Resources.Load<GameObject>("Prefabs/Enemy Bullet"), transform.position, Quaternion.identity);
        bullet.GetComponent<EnemyBulletScript>().Initialise(attackPower, weaponRange);
        SFXSource.PlayOneShot(gunAttack);
        bullet.GetComponent<Rigidbody2D>().AddRelativeForce(transform.up * 600);
        yield return new WaitForSeconds(cooldown);
        limitAttack = false;
        yield return null;
    }

    IEnumerator BurstAttack(float attackPower, float weaponRange, float cooldown)
    {
        limitAttack = true;
        GameObject bullet = Instantiate(Resources.Load<GameObject>("Prefabs/Enemy Bullet"), transform.position, Quaternion.identity);
        bullet.GetComponent<EnemyBulletScript>().Initialise(attackPower, weaponRange);
        SFXSource.PlayOneShot(gunAttack);
        bullet.GetComponent<Rigidbody2D>().AddRelativeForce(transform.up * 600);
        StartCoroutine(FlashMuzzleFlash());
        yield return new WaitForSeconds(0.05f);
        GameObject bullet2 = Instantiate(Resources.Load<GameObject>("Prefabs/Enemy Bullet"), transform.position, Quaternion.identity);
        bullet2.GetComponent<EnemyBulletScript>().Initialise(attackPower, weaponRange);
        SFXSource.PlayOneShot(gunAttack);
        bullet2.GetComponent<Rigidbody2D>().AddRelativeForce(transform.up * 600);
        StartCoroutine(FlashMuzzleFlash());
        yield return new WaitForSeconds(0.05f);
        GameObject bullet3 = Instantiate(Resources.Load<GameObject>("Prefabs/Enemy Bullet"), transform.position, Quaternion.identity);
        bullet3.GetComponent<EnemyBulletScript>().Initialise(attackPower, weaponRange);
        SFXSource.PlayOneShot(gunAttack);
        bullet3.GetComponent<Rigidbody2D>().AddRelativeForce(transform.up * 600);
        StartCoroutine(FlashMuzzleFlash());
        yield return new WaitForSeconds(cooldown);
        limitAttack = false;
        yield return null;
    }

    IEnumerator FlashMuzzleFlash() {
        weaponSprite.sprite = flash;
        int ratio = (int)((1/Time.deltaTime) / 60);
        if (ratio < 1) ratio = 1;
        int dynamicflash = (framesToFlash * ratio); 
        for (int i = 0; i < dynamicflash ; i ++) {
            yield return 0;
        }
        weaponSprite.sprite = sprite;
    }
}
