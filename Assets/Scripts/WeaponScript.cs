using System.Collections;
using UnityEngine;

public class WeaponScript : MonoBehaviour
{
    public int id;
    public string weaponName;
    public float attackPower;
    public string spritePath;
    public string thumbnailPath;
    public string weaponType;
    public float cooldown;
    public float weaponRange;
    public int cost;
    public string inShop;
    public bool limitAttack;
    public int framesToFlash = 3;
    private SpriteRenderer weaponSprite;
    private Sprite sprite;
    private Sprite flash;
    public Animator anim;
    public Transform circleOrigin;
    public float radius;

    private int clipSize;
    private float reloadSpeed;
    private bool isReloading;

    public Vector3 initialPos;

    public float meleeDmgMultiplier;
    public float meleeBleedChance;
    public float meleeBleedDmg;
    public float meleeFireRateMultiplier;
    public float meleeCritChance;
    public float meleeCritDamageMultiplier;
    public float meleeExplodeDmg;
    public float meleeLifeStealMultiplier;

    public float gunCritChance;
    public float gunCritDamageMultiplier;
    public float rangeMultiplier;
    public float gunFireRateMultiplier;
    public float instaReloadChance;
    public float gunBleedChance;
    public float gunBleedDmg;
    public float pierceChance;
    public float lightningChance;
    public float lightningDmg;

    private void Awake()
    {
        initialPos = transform.localPosition;

        meleeDmgMultiplier = 1;
        meleeBleedChance = 0;
        meleeBleedDmg = 0;
        meleeFireRateMultiplier = 1;
        meleeCritChance = 0;
        meleeCritDamageMultiplier = 0;
        meleeExplodeDmg = 0;
        meleeLifeStealMultiplier = 0;
        gunCritChance = 0;
        gunCritDamageMultiplier = 0;
        rangeMultiplier = 1;
        gunFireRateMultiplier = 1;
        instaReloadChance = 0;
        gunBleedChance = 0;
        gunBleedDmg = 0;
        pierceChance = 0;
        lightningChance = 0;
        lightningDmg = 0;
    }

    private void Update()
    {
        if (PlayerScript.instance.equippedWeapon.weaponType != "melee" && isReloading == false && PlayerScript.instance.equippedWeapon.currentAmmoCount <= 0)
        {
            isReloading = true;
            StartCoroutine(Reload());
        }
    }

    public void SetWeaponData(Weapon weaponData)
    {
        StopAllCoroutines();

        this.id = weaponData.id;
        this.weaponName = weaponData.weaponName;
        this.attackPower = weaponData.attackPower;
        this.spritePath = weaponData.spritePath;
        this.thumbnailPath = weaponData.thumbnailPath;
        this.weaponType = weaponData.weaponType;
        this.cooldown = weaponData.cooldown;
        this.weaponRange = weaponData.weaponRange;
        this.cost = weaponData.cost;
        this.inShop = weaponData.inShop;

        this.reloadSpeed = weaponData.reloadSpeed;
        this.clipSize = weaponData.clipSize;

        weaponSprite = gameObject.GetComponent<SpriteRenderer>();
        sprite = Resources.Load<Sprite>(this.spritePath);
        flash = Resources.Load<Sprite>(this.spritePath + " Flash");
        weaponSprite.sprite = sprite;

        if (weaponData.weaponType == "melee")
        {
            anim.enabled = true;
            this.isReloading = false;
        }
        else
        {
            anim.enabled = false;
            if (weaponData.currentAmmoCount <= 0)
            {
                this.isReloading = true;
                StartCoroutine(Reload());
            }
            else this.isReloading = false;
        }

        transform.localPosition = initialPos;
        Vector3 eulerRotation = transform.rotation.eulerAngles;
        transform.localRotation = Quaternion.Euler(eulerRotation.x, eulerRotation.y, 0);
        limitAttack = true;
        StartCoroutine(BufferTime());
    }

    IEnumerator BufferTime()
    {
        yield return new WaitForSeconds(this.cooldown);
        limitAttack = false;
    }

    public void TryAttack()
    {
        if (!PlayerScript.instance.equippedWeapon) return;
        if (!limitAttack)
        {
            if (!isReloading)
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
                    default:
                        break;
                }
            }
        }
    }

    IEnumerator Reload()
    {
        if (this.instaReloadChance > 0 && Random.Range(0, 1f) < this.instaReloadChance - 1)
        {
            yield return new WaitForSeconds(0);
        }
        else
        {
            yield return new WaitForSeconds(this.reloadSpeed);
        }
        PlayerScript.instance.equippedWeapon.currentAmmoCount = this.clipSize;
        isReloading = false;
        yield return null;
    }

    IEnumerator MeleeAttack()
    {
        limitAttack = true;
        foreach (Collider2D collider in Physics2D.OverlapCircleAll(circleOrigin.position, radius))
        {
            if (collider.gameObject.CompareTag("Enemy"))
            {
                //Crit
                if (this.meleeCritChance > 0 && Random.Range(0, 1f) < this.meleeCritChance - 1)
                {
                    collider.gameObject.GetComponent<EnemyScript>().TakeDamage(this.attackPower * this.meleeDmgMultiplier * this.meleeCritDamageMultiplier);
                    PlayerScript.instance.UpdateHealth(this.attackPower * this.meleeDmgMultiplier * this.meleeCritDamageMultiplier * (this.meleeLifeStealMultiplier - 1));
                }
                else
                {
                    collider.gameObject.GetComponent<EnemyScript>().TakeDamage(this.attackPower * this.meleeDmgMultiplier);
                    PlayerScript.instance.UpdateHealth(this.attackPower * this.meleeDmgMultiplier * (this.meleeLifeStealMultiplier - 1));
                }

                //Bleed
                if (this.meleeBleedChance > 0 && Random.Range(0, 1f) < this.meleeBleedChance - 1)
                {
                    collider.gameObject.GetComponent<EnemyScript>().StartCoroutine(collider.gameObject.GetComponent<EnemyScript>().Bleed(this.meleeBleedDmg));
                }

                //Explode
                if (this.meleeExplodeDmg > 0)
                {
                    ContactPoint2D[] contacts = new ContactPoint2D[1];
                    collider.GetContacts(contacts);
                    GameObject explosion = Instantiate(Resources.Load<GameObject>("Prefabs/Melee Explosion"), contacts[0].point, Quaternion.identity);
                    explosion.GetComponent<MeleeExplosion>().Initialise(this.meleeExplodeDmg);
                }
            }

            if (collider.gameObject.CompareTag("Boss"))
            {
                //Crit
                if (this.meleeCritChance > 0 && Random.Range(0, 1f) < this.meleeCritChance - 1)
                {
                    collider.gameObject.GetComponent<MothershipScript>().TakeDamage(this.attackPower * this.meleeDmgMultiplier * this.meleeCritDamageMultiplier);
                    PlayerScript.instance.UpdateHealth(this.attackPower * this.meleeDmgMultiplier * this.meleeCritDamageMultiplier * (this.meleeLifeStealMultiplier - 1));
                }
                else
                {
                    collider.gameObject.GetComponent<MothershipScript>().TakeDamage(this.attackPower * this.meleeDmgMultiplier);
                    PlayerScript.instance.UpdateHealth(this.attackPower * this.meleeDmgMultiplier * (this.meleeLifeStealMultiplier - 1));
                }

                //Bleed
                if (this.meleeBleedChance > 0 && Random.Range(0, 1f) < this.meleeBleedChance - 1)
                {
                    collider.gameObject.GetComponent<MothershipScript>().StartCoroutine(collider.gameObject.GetComponent<MothershipScript>().Bleed(this.meleeBleedDmg));
                }

                //Explode
                if (this.meleeExplodeDmg > 0)
                {
                    ContactPoint2D[] contacts = new ContactPoint2D[1];
                    collider.GetContacts(contacts);
                    GameObject explosion = Instantiate(Resources.Load<GameObject>("Prefabs/Melee Explosion"), contacts[0].point, Quaternion.identity);
                    explosion.GetComponent<MeleeExplosion>().Initialise(this.meleeExplodeDmg);
                }
            }

            if (collider.gameObject.CompareTag("TargetDummy"))
            {
                collider.gameObject.GetComponent<TargetDummyScript>().TakeDamage(this.attackPower * this.meleeDmgMultiplier);
                GameController.instance.dummyShot = true;
            }
        }

        switch (this.weaponName)
        {
            case "Fists":
                anim.SetTrigger("Fists");
                break;
            case "Katana":
                anim.SetTrigger("Katana");
                break;
            case "Baseball Bat":
                anim.SetTrigger("Baseball");
                break;
            default:
                break;
        }

        yield return new WaitForSeconds(this.cooldown * this.meleeFireRateMultiplier);
        limitAttack = false;
        yield return null;
    }
    IEnumerator LineAttack()
    {
        limitAttack = true;
        if (PlayerScript.instance.equippedWeapon.currentAmmoCount > 0)
        {
            GameObject bullet;
            switch (this.weaponName)
            {
                case "Sniper Rifle":
                    bullet = Instantiate(Resources.Load<GameObject>("Prefabs/Sniper Bullet"), transform.position, transform.rotation);

                    //Crit
                    if (this.gunCritChance > 0 && Random.Range(0, 1f) < this.gunCritChance - 1)
                    {
                        bullet.GetComponent<SniperBulletScript>().Initialise(this.attackPower * this.gunCritDamageMultiplier, this.weaponRange * this.rangeMultiplier);
                    }
                    else
                    {
                        bullet.GetComponent<SniperBulletScript>().Initialise(this.attackPower, this.weaponRange * this.rangeMultiplier);
                    }

                    --PlayerScript.instance.equippedWeapon.currentAmmoCount;
                    break;
                default:
                    bullet = Instantiate(Resources.Load<GameObject>("Prefabs/Bullet"), transform.position, transform.rotation);

                    //Crit
                    if (this.gunCritChance > 0 && Random.Range(0, 1f) < this.gunCritChance - 1)
                    {
                        bullet.GetComponent<BulletScript>().Initialise(this.attackPower * this.gunCritDamageMultiplier, this.weaponRange * this.rangeMultiplier);
                    }
                    else
                    {
                        bullet.GetComponent<BulletScript>().Initialise(this.attackPower, this.weaponRange * this.rangeMultiplier);
                    }

                    --PlayerScript.instance.equippedWeapon.currentAmmoCount;
                    break;
            }

            //can add Projectile Speed to CSV (600 here)
            bullet.GetComponent<Rigidbody2D>().AddForce(transform.up * 600);
            yield return new WaitForSeconds(this.cooldown * this.gunFireRateMultiplier);
        }
        limitAttack = false;
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
        Gizmos.color = Color.blue;
        Vector3 position = circleOrigin.position;
        Gizmos.DrawWireSphere(position, radius);
    }
}
