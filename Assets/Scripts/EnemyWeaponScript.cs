using System.Collections;
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

        if (weaponData.weaponType == "melee") {
            anim.enabled = true;
        }else {
            anim.enabled = false;
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
        switch (this.weaponName)
        {
            case "Pincers":
                anim.SetTrigger("Pincers");
                break;
            default:
                break;
        }
        yield return new WaitForSeconds(this.cooldown);
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

    private void OnDrawGizmosSelected() {
        Gizmos.color = Color.blue;
        Vector3 position = circleOrigin.position;
        Gizmos.DrawWireSphere(position,radius);
    }

}
