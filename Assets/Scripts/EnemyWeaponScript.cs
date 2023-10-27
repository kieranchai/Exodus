using System.Collections;
using System.Drawing;
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
    public int framesToFlash = 3;
    private SpriteRenderer weaponSprite;
    private Sprite sprite;
    private Sprite flash;
    public Animator anim;
    public Transform circleOrigin;
    public float radius;

    // magnet enemy type
    private PointEffector2D PointEffector;
    public bool magnetDamageActive;
    private float savedTimer = 0;
    private float magnetdamageInterval = 1f;
    private float collisiondamageInterval = 0.5f;    
    public ParticleSystem MagnetParticle;

    private AudioSource SFXSource;
    [Header("Enemy Weapon Audio Clips")]
    public AudioClip pincerHit;
    public AudioClip suicideBombTrigger;
    public AudioClip alienRifleFire;
    public AudioClip alienPistolFire;
    public AudioClip alienMouthHit;
    public AudioClip shopTurretFire;
    public AudioClip magnetEffect;

    public bool playingmagnetSound = false;
    private void Awake()
    {
        SFXSource = GetComponent<AudioSource>();
    }

    public void SetWeaponData(Weapon weaponData)
    {
        this.weaponName = weaponData.name;
        this.attackPower = weaponData.attackPower;
        this.weaponType = weaponData.weaponType;
        this.cooldown = weaponData.cooldown;
        this.weaponRange = weaponData.weaponRange;
        this.spritePath = weaponData.spritePath;

        weaponSprite = gameObject.GetComponent<SpriteRenderer>();
        sprite = Resources.Load<Sprite>(this.spritePath);
        flash = Resources.Load<Sprite>(this.spritePath + " Flash");
        weaponSprite.sprite = sprite;

        if (weaponData.weaponType == "melee" || weaponData.weaponType == "magnet")
        {
            anim.enabled = true;
        }
        else
        {
            anim.enabled = false;
        }

        if (weaponData.weaponType == "magnet")
        {
            this.PointEffector = this.gameObject.GetComponent<PointEffector2D>();
            PointEffector.enabled = false;
        }
    }

    public void TryAttack()
    {
        if (!limitAttack)
        {
            switch (this.weaponType)
            {
                case "line":
                    StartCoroutine(FlashMuzzleFlash());
                    StartCoroutine(LineAttack());
                    break;
                case "melee":
                    StartCoroutine(MeleeAttack());
                    break;
                case "suicide":
                    StartCoroutine(SuicideAttack());
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

        //Audio
        switch (this.weaponName)
        {
            case "Alien Rifle":
                SFXSource.PlayOneShot(alienRifleFire);
                break;
            case "Alien Pistol":
                SFXSource.PlayOneShot(alienPistolFire);
                break;
            case "Banshee Turret":
                SFXSource.PlayOneShot(shopTurretFire);
                break;
            default:
                break;
        }

        bullet.GetComponent<Rigidbody2D>().AddRelativeForce(transform.up * 600);
        yield return new WaitForSeconds(this.cooldown);
        limitAttack = false;
        yield return null;
    }

    IEnumerator MeleeAttack()
    {
        limitAttack = true;
        foreach (Collider2D collider in Physics2D.OverlapCircleAll(circleOrigin.position, radius))
        {
            if (collider.gameObject.CompareTag("Player"))
            {
                PlayerScript.instance.TakeDamage(this.attackPower, false);
            }
        }

        //Audio
        switch (this.weaponName)
        {
            case "Pincers":
                SFXSource.PlayOneShot(pincerHit);
                anim.SetTrigger("Pincers");
                break;
            case "Alien Mouth":
                SFXSource.PlayOneShot(alienMouthHit);
                anim.SetTrigger("Mouth");
                break;
            default:
                break;
        }

        yield return new WaitForSeconds(this.cooldown);
        limitAttack = false;
        yield return null;
    }

    public void MagnetActive()
    {
        if (!magnetDamageActive)
        {
            if (!playingmagnetSound)
            {
                SFXSource.PlayOneShot(magnetEffect);
                playingmagnetSound = true;
            }
            PointEffector.enabled = true;
            magnetDamageActive = true;
            anim.SetBool("MagnetActive", true);
            MagnetParticle.Play();
        }
    }

    public void MagnetDisable()
    {
        playingmagnetSound = false;
        SFXSource.Stop();
        PointEffector.enabled = false;
        magnetDamageActive = false;
        anim.SetBool("MagnetActive", false);
        MagnetParticle.Stop();
    }
    IEnumerator SuicideAttack()
    {
        limitAttack = true;
        //Charging state to stop enemy from moving
        transform.parent.gameObject.GetComponent<EnemyScript>().AttackCharging(0.5f);
        SFXSource.PlayOneShot(suicideBombTrigger);
        yield return new WaitForSeconds(0.5f);

        GameObject explosion = Instantiate(Resources.Load<GameObject>("Prefabs/Heretic Explosion"), transform.position, Quaternion.identity);
        explosion.GetComponent<EnemyExplosionScript>().Initialise(attackPower, 4f);
        if(transform.parent.GetComponent<EnemyScript>().mySpawner) transform.parent.GetComponent<EnemyScript>().mySpawner.enemyCounter--;
        if (transform.parent.GetComponent<EnemyScript>().mothershipSpawner) transform.parent.GetComponent<EnemyScript>().mothershipSpawner.enemyCounter--;
        limitAttack = false;
        AudioManager.instance.threatLevel--;
        Destroy(transform.parent.gameObject);
        yield return null;
    }

    IEnumerator FlashMuzzleFlash()
    {
        weaponSprite.sprite = flash;
        int ratio = (int)((1 / Time.deltaTime) / 60);
        if (ratio < 1) ratio = 1;
        int dynamicflash = (framesToFlash * ratio);
        for (int i = 0; i < dynamicflash; i++)
        {
            yield return 0;
        }
        weaponSprite.sprite = sprite;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = UnityEngine.Color.blue;
        Vector3 position = circleOrigin.position;
        Gizmos.DrawWireSphere(position, radius);
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (this.weaponType == "magnet")
        {
            if (!magnetDamageActive) return;
            if (collision.gameObject.CompareTag("Player"))
            {
                if ((Time.time - savedTimer) > magnetdamageInterval)
                {
                    savedTimer = Time.time;
                    collision.GetComponent<PlayerScript>().TakeDamage(this.attackPower, false);
                }
            }
        }
        if (this.weaponType == "collision")
        {
            if (collision.gameObject.CompareTag("Player"))
            {
                if ((Time.time - savedTimer) > collisiondamageInterval)
                {
                    savedTimer = Time.time;
                    collision.GetComponent<PlayerScript>().TakeDamage(this.attackPower, false);
                }
            }
        }
    }
}
