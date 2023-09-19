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

    [SerializeField]
    private AudioSource audioSource;

    private void Start()
    {
        audioSource.volume = 0.5f;
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
                case "rocket":
                    StartCoroutine(RocketAttack(attackPower, weaponRange, cooldown));
                    break;
                case "homingrocket":
                    StartCoroutine(HomingRocketAttack(attackPower));
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

        audioSource.clip = Resources.Load<AudioClip>($"Audio/Turret Gun");
        audioSource.Play();

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

        audioSource.clip = Resources.Load<AudioClip>($"Audio/Turret Gun");
        audioSource.Play();
        bullet.GetComponent<Rigidbody2D>().AddRelativeForce(transform.up * 600);
        StartCoroutine(FlashMuzzleFlash());
        yield return new WaitForSeconds(0.05f);
        GameObject bullet2 = Instantiate(Resources.Load<GameObject>("Prefabs/Enemy Bullet"), transform.position, Quaternion.identity);
        bullet2.GetComponent<EnemyBulletScript>().Initialise(attackPower, weaponRange);

        audioSource.clip = Resources.Load<AudioClip>($"Audio/Turret Gun");
        audioSource.Play();
        bullet2.GetComponent<Rigidbody2D>().AddRelativeForce(transform.up * 600);
        StartCoroutine(FlashMuzzleFlash());
        yield return new WaitForSeconds(0.05f);
        GameObject bullet3 = Instantiate(Resources.Load<GameObject>("Prefabs/Enemy Bullet"), transform.position, Quaternion.identity);
        bullet3.GetComponent<EnemyBulletScript>().Initialise(attackPower, weaponRange);

        audioSource.clip = Resources.Load<AudioClip>($"Audio/Turret Gun");
        audioSource.Play();
        bullet3.GetComponent<Rigidbody2D>().AddRelativeForce(transform.up * 600);
        StartCoroutine(FlashMuzzleFlash());
        yield return new WaitForSeconds(cooldown);
        limitAttack = false;
        yield return null;
    }

    IEnumerator RocketAttack(float attackPower, float weaponRange, float cooldown)
    {
        limitAttack = true;
        StartCoroutine(FlashMuzzleFlash());
        GameObject rocket = Instantiate(Resources.Load<GameObject>("Prefabs/Enemy Rocket"), transform.position, Quaternion.identity);
        rocket.GetComponent<EnemyRocketScript>().Initialise(attackPower, weaponRange);

        audioSource.clip = Resources.Load<AudioClip>($"Audio/Turret Rocket");
        audioSource.Play();
        //can add Projectile Speed to CSV (600 here)
        rocket.GetComponent<Rigidbody2D>().AddForce(transform.up * 150);

        yield return new WaitForSeconds(cooldown);
        limitAttack = false;
        yield return null;
    }

    IEnumerator HomingRocketAttack(float attackPower)
    {
        limitAttack = true;
        StartCoroutine(FlashMuzzleFlash());
        GameObject rocket = Instantiate(Resources.Load<GameObject>("Prefabs/Enemy Homing Rocket"), transform.position, Quaternion.identity);
        rocket.GetComponent<EnemyHomingRocketScript>().Initialise(attackPower);

        audioSource.clip = Resources.Load<AudioClip>($"Audio/Turret Rocket");
        audioSource.Play();
        yield return new WaitForSeconds(0);
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
