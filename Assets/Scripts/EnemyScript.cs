using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyScript : MonoBehaviour
{
    NavMeshAgent agent;
    private Animator anim;
    private Collider2D enemyCollider;
    private SpriteRenderer enemySprite;

    private string enemyName;
    private float maxHealth;
    private float movementSpeed;
    private float currentHealth;
    private int xpDrop;
    private int cashDrop;
    private string lootDrop;

    private float timer = 0;
    private float duration = 3f;

    [SerializeField]
    private float sightRange = 7f;

    [SerializeField]
    private Enemy _data;

    public Weapon equippedWeapon;
    public EnemyWeaponScript weaponSlot;

    public enum ENEMY_STATE
    {
        WANDER,
        CHASE,
        ATTACK,
        DEAD
    }

    ENEMY_STATE currentState = ENEMY_STATE.WANDER;

    private void Start()
    {
        anim = GetComponent<Animator>();
        enemyCollider = GetComponent<Collider2D>();
        enemySprite = GetComponent<SpriteRenderer>();
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;
        this.weaponSlot = gameObject.transform.Find("Weapon").gameObject.GetComponent<EnemyWeaponScript>();
        SetEnemyData(_data);
        EquipWeapon(this.equippedWeapon);
    }

    void Update()
    {
        if (agent.velocity.magnitude > 0)
        {
            anim.SetBool("isWalking", true);
        }
        else
        {
            anim.SetBool("isWalking", false);
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
            case ENEMY_STATE.DEAD:
                Dead();
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
        this.cashDrop = enemyData.cashDrop;
        this.lootDrop = enemyData.lootDrop;

        this.currentHealth = this.maxHealth;
        agent.speed = this.movementSpeed;
    }

    public void EquipWeapon(Weapon weaponData)
    {
        this.weaponSlot.SetWeaponData(weaponData);
    }

    public void Wander()
    {
        //Wander

        if (PlayerInSight())
        {
            this.currentState = ENEMY_STATE.CHASE;
        }
    }

    public void Chase()
    {
        agent.SetDestination(PlayerScript.instance.transform.position);
        transform.up = (PlayerScript.instance.transform.position - new Vector3(transform.position.x, transform.position.y));

        if (PlayerInSight())
        {
            // Play Event 

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

        this.weaponSlot.TryAttack();
    }

    public bool PlayerInSight()
    {
        bool inRange = Vector3.Distance(PlayerScript.instance.transform.position, transform.position) <= this.sightRange;
        float sightAngle = Vector2.Angle(PlayerScript.instance.transform.position - transform.position, transform.up);

        int mask1 = 1 << LayerMask.NameToLayer("Tilemap Colliders");
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

        this.currentState = ENEMY_STATE.DEAD;
    }

    public void Dead()
    {
        anim.enabled = false;
        enemyCollider.enabled = false;

        // Change Enemy Sprite to Death Sprite
/*        enemySprite.sprite = Resources.Load<Sprite>($"Sprites/{this.enemyName}_Death");*/

        Destroy(gameObject, 4f);
    }
}
