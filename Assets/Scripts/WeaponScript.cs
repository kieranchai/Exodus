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
    private bool limitAttack;
    public int framesToFlash = 3;
    private SpriteRenderer weaponSprite;
    private Sprite sprite;
    private Sprite flash;
    public Animator anim;
    public AnimatorOverrideController animatorOverrideController;
    public Transform circleOrigin;
    public float radius;
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

        if (weaponData.weaponType == "melee") {
            anim.enabled = true;
        }else {
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
                break;
            case "Katana":
                anim.SetTrigger("Katana");
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
            GameObject bullet = Instantiate(Resources.Load<GameObject>("Prefabs/Bullet"), transform.position, transform.rotation);
            bullet.GetComponent<BulletScript>().Initialise(this.attackPower, this.weaponRange);
            //can add Projectile Speed to CSV (600 here)
            bullet.GetComponent<Rigidbody2D>().AddForce(transform.up * 600);

            --PlayerScript.instance.ammoCount[this.ammoType];
            PlayerScript.instance.RefreshAmmoUI();
            yield return new WaitForSeconds(this.cooldown);
        }
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

    private void OnDrawGizmosSelected() {
        Gizmos.color = Color.blue;
        Vector3 position = circleOrigin.position;
        Gizmos.DrawWireSphere(position,radius);
    }
}
