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
    public int defaultAmmo;
    public float reloadSpeed;
    public float weight;
    public string inShop;
    public string ammoType;
    public bool limitAttack;
    public int framesToFlash = 3;
    private SpriteRenderer weaponSprite;
    private Sprite sprite;
    private Sprite flash;
    public Animator anim;
    public Transform circleOrigin;
    public float radius;
    [SerializeField] private AudioSource audioSource;

    private void Start()
    {
        audioSource.volume = 0.5f;
    }

    public void SetWeaponData(Weapon weaponData)
    {
        this.id = weaponData.id;
        this.weaponName = weaponData.weaponName;
        this.attackPower = weaponData.attackPower;
        this.spritePath = weaponData.spritePath;
        this.thumbnailPath = weaponData.thumbnailPath;
        this.weaponType = weaponData.weaponType;
        this.cooldown = weaponData.cooldown;
        this.weaponRange = weaponData.weaponRange;
        this.defaultAmmo = weaponData.defaultAmmo;
        this.cost = weaponData.cost;
        this.weight = weaponData.weight;
        this.ammoType = weaponData.ammoType;
        this.inShop = weaponData.inShop;

        weaponSprite = gameObject.GetComponent<SpriteRenderer>();
        sprite = Resources.Load<Sprite>(this.spritePath);
        flash = Resources.Load<Sprite>(this.spritePath + " Flash");
        weaponSprite.sprite = sprite;

        if (weaponData.weaponType == "melee")
        {
            anim.enabled = true;
        }
        else
        {
            anim.enabled = false;
        }
    }

    public void TryAttack()
    {
        if (!PlayerScript.instance.equippedWeapon) return;
        if (!limitAttack)
        {
            switch (this.weaponType)
            {
                case "line":
                    if (PlayerScript.instance.ammoCount[this.ammoType] > 0) StartCoroutine(FlashMuzzleFlash());
                    StartCoroutine(LineAttack());
                    break;
                case "melee":
                    StartCoroutine(MeleeAttack());
                    break;
                case "akimbo":
                    if (PlayerScript.instance.ammoCount[this.ammoType] > 0) StartCoroutine(FlashMuzzleFlash());
                    StartCoroutine(LineAttack());
                    break;
                default:
                    break;
            }
        }
    }
    IEnumerator MeleeAttack()
    {
        limitAttack = true;
        foreach (Collider2D collider in Physics2D.OverlapCircleAll(circleOrigin.position, radius))
        {
            if (collider.gameObject.CompareTag("Enemy"))
            {
                collider.gameObject.GetComponent<EnemyScript>().TakeDamage(this.attackPower);
            }

            if (collider.gameObject.CompareTag("Boss"))
            {
                collider.gameObject.GetComponent<MothershipScript>().TakeDamage(this.attackPower);
            }

            if (collider.gameObject.CompareTag("TargetDummy"))
            {
                collider.gameObject.GetComponent<TargetDummyScript>().TakeDamage(this.attackPower);
                GameController.instance.dummyShot = true;
            }
        }

        switch (this.weaponName)
        {
            case "Fists":
                anim.SetTrigger("Fists");
                audioSource.clip = Resources.Load<AudioClip>($"Audio/Fists");
                audioSource.Play();
                break;
            case "Katana":
                anim.SetTrigger("Katana");
                audioSource.clip = Resources.Load<AudioClip>($"Audio/Katana");
                audioSource.Play();
                break;
            case "Baseball Bat":
                anim.SetTrigger("Baseball");
                audioSource.clip = Resources.Load<AudioClip>($"Audio/Baseball Bat");
                audioSource.Play();
                break;
            default:
                break;
        }

        yield return new WaitForSeconds(this.cooldown);
        limitAttack = false;
        yield return null;
    }
    IEnumerator LineAttack()
    {
        limitAttack = true;
        if (PlayerScript.instance.ammoCount[this.ammoType] > 0)
        {
            GameObject bullet;
            switch (this.weaponName)
            {
                case "Sniper Rifle":
                    bullet = Instantiate(Resources.Load<GameObject>("Prefabs/Sniper Bullet"), transform.position, transform.rotation);
                    bullet.GetComponent<SniperBulletScript>().Initialise(this.attackPower, this.weaponRange);
                    break;
                default:
                    bullet = Instantiate(Resources.Load<GameObject>("Prefabs/Bullet"), transform.position, transform.rotation);
                    bullet.GetComponent<BulletScript>().Initialise(this.attackPower, this.weaponRange);
                    break;
            }

            //Audio
            switch (this.weaponName)
            {
                case "Sniper Rifle":
                    audioSource.clip = Resources.Load<AudioClip>($"Audio/Sniper Rifle");
                    audioSource.Play();
                    break;
                case "Light Pistol":
                case "Dual Elites":
                case "Submachine Gun":
                    audioSource.clip = Resources.Load<AudioClip>($"Audio/Light Pistol");
                    audioSource.Play();
                    break;
                case "Assault Rifle":
                case "Dual ARs":
                    audioSource.clip = Resources.Load<AudioClip>($"Audio/Assault Rifle");
                    audioSource.Play();
                    break;
                default:
                    break;
            }

            //can add Projectile Speed to CSV (600 here)
            bullet.GetComponent<Rigidbody2D>().AddForce(transform.up * 600);
            --PlayerScript.instance.ammoCount[this.ammoType];
            PlayerScript.instance.UpdateEquippedAmmoUI();
            PlayerScript.instance.UpdateInventoryAmmoUI();
            yield return new WaitForSeconds(this.cooldown);
        }
        else
        {
            audioSource.Stop();
            audioSource.clip = Resources.Load<AudioClip>($"Audio/Empty");
            audioSource.Play();
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
