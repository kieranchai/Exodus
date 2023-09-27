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
    [SerializeField] private AudioSource audioSource;

    private int clipSize;
    private float reloadSpeed;
    private bool isReloading;

    public Vector3 initialPos;

    private void Start()
    {
        audioSource.volume = 0.5f;
        initialPos = transform.localPosition;
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

        if(gameObject.transform.name != "Akimbo Slot")
        {
            transform.localPosition = initialPos;
            Vector3 eulerRotation = transform.rotation.eulerAngles;
            transform.localRotation = Quaternion.Euler(eulerRotation.x, eulerRotation.y, 0);
        }
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
                    case "akimbo":
                        StartCoroutine(FlashMuzzleFlash());
                        StartCoroutine(LineAttack());
                        break;
                    default:
                        break;
                }
            }
        }
    }

    IEnumerator Reload()
    {
        yield return new WaitForSeconds(this.reloadSpeed);
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
        if (PlayerScript.instance.equippedWeapon.currentAmmoCount > 0)
        {
            GameObject bullet;
            switch (this.weaponName)
            {
                case "Sniper Rifle":
                    bullet = Instantiate(Resources.Load<GameObject>("Prefabs/Sniper Bullet"), transform.position, transform.rotation);
                    bullet.GetComponent<SniperBulletScript>().Initialise(this.attackPower, this.weaponRange);
                    --PlayerScript.instance.equippedWeapon.currentAmmoCount;
                    break;
                default:
                    bullet = Instantiate(Resources.Load<GameObject>("Prefabs/Bullet"), transform.position, transform.rotation);
                    bullet.GetComponent<BulletScript>().Initialise(this.attackPower, this.weaponRange);
                    --PlayerScript.instance.equippedWeapon.currentAmmoCount;
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
