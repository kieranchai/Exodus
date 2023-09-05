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
    }

    public void TryAttack()
    {
        if (!PlayerScript.instance.equippedWeapon) return;

        if (!limitAttack)
        {
            switch (this.weaponType)
            {
                case "line":
                    StartCoroutine(FlashMuzzleFlash());
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
        if (PlayerScript.instance.ammoCount[this.ammoType] > 0)
        {
            GameObject bullet = Instantiate(Resources.Load<GameObject>("Prefabs/Bullet"), transform.position, transform.rotation);
            bullet.GetComponent<BulletScript>().Initialize(this.attackPower, this.weaponRange);
            //can add Projectile Speed to CSV (600 here)
            bullet.GetComponent<Rigidbody2D>().AddForce(transform.right * 600);

            --PlayerScript.instance.ammoCount[this.ammoType];
            PlayerScript.instance.RefreshAmmoUI();
            yield return new WaitForSeconds(this.cooldown);
        }
        limitAttack = false;
        yield return null;
    }

    IEnumerator FlashMuzzleFlash() {
        weaponSprite.sprite = flash;
        for (int i = 0; i < framesToFlash; i ++) {
            yield return 0;
        }
        weaponSprite.sprite = sprite;
    }

}
