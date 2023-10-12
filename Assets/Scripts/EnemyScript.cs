using System.Collections;
using System;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public class EnemyScript : MonoBehaviour
{
    NavMeshAgent agent;
    private Animator anim;
    private Collider2D enemyCollider;
    private SpriteRenderer enemySprite;
    private SpriteRenderer weaponSprite;
    private Material originalMaterial;
    public Material flashMaterial;
    private Rigidbody2D rb;

    private string enemyName;
    private float maxHealth;
    private float movementSpeed;
    private float currentHealth;
    private int xpDrop;
    private int hpDrop;
    private int cashDrop;
    private string lootDrop;
    public string spawnZone;
    public int spawnChance;

    public float timer = 0;
    private float duration;
    public float returnDuration;

    private float sightRange; // Can Set in CSV

    public Enemy _data;

    public Weapon equippedWeapon;
    public EnemyWeaponScript weaponSlot;

    [SerializeField]
    private float wanderRadius;

    [SerializeField]
    private Transform damagePopupPrefab;
    [SerializeField]
    private Transform xpCashPopupPrefab;
    private bool hasSetSpawnZone = false;
    public EnemySpawner mySpawner;

    [SerializeField]
    private ParticleSystem spawnParticle;

    [SerializeField]
    private GameObject healthOrbPrefab;

    [SerializeField]
    private GameObject cashOrbPrefab;
    public bool stationary;
    public enum ENEMY_STATE
    {
        SHOP,
        WANDER,
        CHASE,
        ATTACK,
        CHARGING,
        DEAD
    }

    public ENEMY_STATE currentState;
    private void Start()
    {
        anim = GetComponent<Animator>();
        enemyCollider = GetComponent<Collider2D>();
        rb = GetComponent<Rigidbody2D>();
        enemySprite = GetComponent<SpriteRenderer>();
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;
        this.weaponSlot = gameObject.transform.Find("Weapon").gameObject.GetComponent<EnemyWeaponScript>();
        weaponSprite = this.weaponSlot.GetComponent<SpriteRenderer>();
        SetEnemyData(_data);
        EquipWeapon(this.equippedWeapon);
        this.duration = 1f;
        this.sightRange = 5f;

        if (stationary)
        {
            this.currentState = ENEMY_STATE.SHOP;
        }
        else
        {
            this.currentState = ENEMY_STATE.WANDER;
        }

        spawnParticle.Play();
        originalMaterial = enemySprite.material;
    }

    void Update()
    {
        if (!stationary)
        {
            if (agent.velocity.magnitude > 0)
            {
                anim.SetBool("isWalking", true);
            }
            else
            {
                anim.SetBool("isWalking", false);
            }
        }

        switch (this.currentState)
        {
            case ENEMY_STATE.WANDER:
                Wander();
                break;
            case ENEMY_STATE.CHASE:
                Chase();
                break;
            case ENEMY_STATE.ATTACK:
                Attack();
                break;
            case ENEMY_STATE.CHARGING:
                //Turn to player
                transform.up = (PlayerScript.instance.transform.position - new Vector3(transform.position.x, transform.position.y));
                break;
            case ENEMY_STATE.DEAD:
                Dead();
                break;
            case ENEMY_STATE.SHOP:
                Shop();
                break;
            default:
                break;
        }
    }

    public void SetEnemyData(Enemy enemyData)
    {
        this.enemyName = enemyData.enemyName;
        this.maxHealth = enemyData.health;
        this.movementSpeed = enemyData.movementSpeed;
        this.equippedWeapon = Resources.Load<Weapon>($"ScriptableObjects/Weapons/{enemyData.equippedWeapon}");
        this.xpDrop = enemyData.xpDrop;
        this.hpDrop = enemyData.hpDrop;
        this.cashDrop = enemyData.cashDrop;
        this.lootDrop = enemyData.lootDrop;
        this.spawnChance = enemyData.spawnChance;

        this.currentHealth = this.maxHealth;
        agent.speed = this.movementSpeed;
    }

    public void EquipWeapon(Weapon weaponData)
    {
        this.weaponSlot.SetWeaponData(weaponData);
    }

    public Vector3 RandomPointInZone(Bounds bounds)
    {
        return new Vector3(
            Random.Range(bounds.min.x, bounds.max.x),
            Random.Range(bounds.min.y, bounds.max.y),
            Random.Range(bounds.min.z, bounds.max.z)
        );
    }

    public void Wander()
    {
        if (agent.remainingDistance <= agent.stoppingDistance)
        {
            Vector3 target = RandomPointInZone(Array.Find(GameController.instance.Zones, x => x.CompareTag(spawnZone)).bounds);
            agent.SetDestination(target);
            transform.up = (target - new Vector3(transform.position.x, transform.position.y));
        }

        if (PlayerInSight())
        {
            this.currentState = ENEMY_STATE.CHASE;
        }
    }

    public void Shop()
    {
        if (PlayerInSight())
        {
            transform.up = (PlayerScript.instance.transform.position - new Vector3(transform.position.x, transform.position.y));
            if (PlayerInRange())
            {
                this.weaponSlot.TryAttack();
            }
        }
    }
    public void Chase()
    {
        agent.SetDestination(PlayerScript.instance.transform.position);
        transform.up = (PlayerScript.instance.transform.position - new Vector3(transform.position.x, transform.position.y));
        if (PlayerInSight())
        {
            this.timer = 0;
            if (PlayerInRange())
            {
                this.currentState = ENEMY_STATE.ATTACK;
            }
        }
        else
        {
            this.timer += Time.deltaTime;
            if (this.timer >= this.duration) this.currentState = ENEMY_STATE.WANDER;
        }
    }

    public void Attack()
    {
        if (!PlayerInRange() || !PlayerInSight())
        {
            this.currentState = ENEMY_STATE.CHASE;
        }
        transform.up = (PlayerScript.instance.transform.position - new Vector3(transform.position.x, transform.position.y));
        this.weaponSlot.TryAttack();
    }

    public void AttackCharging(float chargeTime)
    {
        this.currentState = ENEMY_STATE.CHARGING;
        StartCoroutine(ChargingFlash(chargeTime));
    }

    IEnumerator ChargingFlash(float chargeTime)
    {
        bool isFlashing = true;
        float timer = 0;
        while (isFlashing)
        {
            timer += Time.deltaTime;
            enemySprite.material = flashMaterial;
            weaponSprite.material = flashMaterial;
            yield return new WaitForSeconds(0.1f);
            enemySprite.material = originalMaterial;
            weaponSprite.material = originalMaterial;
            yield return new WaitForSeconds(0.1f);
            if (timer >= chargeTime) isFlashing = false;
        }
        this.currentState = ENEMY_STATE.CHASE;
    }

    public bool PlayerInSight()
    {
        bool inRange = Vector3.Distance(PlayerScript.instance.transform.position, transform.position) <= this.sightRange;
        float sightAngle = Vector2.Angle(PlayerScript.instance.transform.position - transform.position, transform.up);
        int mask1 = 1 << LayerMask.NameToLayer("Tilemap Collider");
        int mask2 = 1 << LayerMask.NameToLayer("Safe Zone");
        int combinedMask = mask1 | mask2;

        RaycastHit2D hit = Physics2D.Linecast(transform.position, PlayerScript.instance.transform.position, combinedMask);
        if (hit.collider != null)
        {
            return false;
        }

        return inRange && (sightAngle < 30f);
    }

    public bool PlayerInRange()
    {
        if (Vector3.Distance(PlayerScript.instance.transform.position, transform.position) <= this.equippedWeapon.weaponRange)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public void TakeDamage(float damage)
    {
        if (this.currentHealth <= 0) return;
        Transform damagePopupTransform = Instantiate(damagePopupPrefab, transform.position, Quaternion.identity);
        DamagePopup damagePopup = damagePopupTransform.GetComponent<DamagePopup>();
        damagePopup.Setup(damage);

        StopCoroutine(EnemyHit());
        StartCoroutine(EnemyHit());

        if (this.currentHealth - damage > 0)
        {
            this.currentHealth -= damage;
            this.currentState = ENEMY_STATE.CHASE;
        }
        else
        {
            this.currentHealth -= damage;
            DeathEvent();
        }
    }

    public void DeathEvent()
    {
        // Drop XP, Cash, Loot
        Transform xpCashPopupTransform = Instantiate(xpCashPopupPrefab, transform.position + new Vector3(0.5f, 0), Quaternion.identity);
        DamagePopup xpCashPopup = xpCashPopupTransform.GetComponent<DamagePopup>();
        xpCashPopup.SetupXP(xpDrop);

        for (int i = 0; i < hpDrop / 5; i++)
        {
            float offset = Random.Range(-0.2f, 0.2f);
            Vector3 pos = new Vector3(transform.position.x + offset, transform.position.y + offset, transform.position.z);

            GameObject orb = Instantiate(healthOrbPrefab, pos, transform.rotation);
            float explosion = 100f + Random.Range(-30, 30);
            orb.GetComponent<Rigidbody2D>().AddRelativeForce(Random.onUnitSphere * explosion);
        }

        for (int i = 0; i < cashDrop / 5; i++)
        {
            float offset = Random.Range(-0.2f, 0.2f);
            Vector3 pos = new Vector3(transform.position.x + offset, transform.position.y + offset, transform.position.z);

            GameObject orb = Instantiate(cashOrbPrefab, pos, transform.rotation);
            float explosion = 100f + Random.Range(-30, 30);
            orb.GetComponent<Rigidbody2D>().AddRelativeForce(Random.onUnitSphere * explosion);
        }

        PlayerScript.instance.UpdateExperience(xpDrop);

        --this.mySpawner.enemyCounter;
        this.currentState = ENEMY_STATE.DEAD;
    }

    public void Dead()
    {
        agent.isStopped = true;
        if(!stationary)anim.enabled = false;
        enemyCollider.enabled = false;
        rb.bodyType = RigidbodyType2D.Static;
        if(!stationary)enemySprite.sprite = Resources.Load<Sprite>($"Sprites/{this.enemyName}_Death");

        Destroy(gameObject, 2f);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Safe Zone"))
        {
            this.currentState = ENEMY_STATE.WANDER;
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (!this.hasSetSpawnZone && collision.CompareTag(this.spawnZone))
        {
            this.mySpawner = collision.gameObject.GetComponent<EnemySpawner>();
            this.hasSetSpawnZone = true;
        } 
        
    }

    IEnumerator EnemyHit()
    {
        float duration = 0.05f;
        while (duration > 0)
        {
            duration -= Time.deltaTime;
            enemySprite.material = flashMaterial;
            weaponSprite.material = flashMaterial;
            yield return null;
        }
        enemySprite.material = originalMaterial;
        weaponSprite.material = originalMaterial;
    }

    public IEnumerator Bleed(float damage)
    {
        for (int i = 0; i < 5; i++)
        {
            this.TakeDamage(damage);
            //TODO: Play particle/vfx etc
            yield return new WaitForSeconds(1);
        }

        yield return null;
    }
}
