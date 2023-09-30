using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerScript : MonoBehaviour
{
    public static PlayerScript instance { get; private set; }

    public Rigidbody2D rb;
    public Animator anim;
    public Collider2D coll;
    public SpriteRenderer playerSprite;
    [SerializeField] private ParticleSystem dustCloud;
    public AudioSource audioSource;

    public string playerName;
    public float maxHealth;
    public float movementSpeed;
    public string defaultWeapon;

    public float currentHealth;
    public int cash;
    public int experience;
    public int level;

    private Vector3 moveDir;
    private Vector3 rollDir;
    private float rollSpeed;
    private Vector3 lastMoveDir;
    public float rollCD = 3f;
    private float rollTimer;
    private bool rollOnCD = false;

    [SerializeField]
    private Player _data;

    public WeaponScript weaponSlot;
    public Weapon equippedWeapon;

    public List<Weapon> inventory = new();

    public GameObject playerPanel;
    [SerializeField] private GameObject inventoryItemPrefab;
    [SerializeField] private Transform damagePopupPrefab;
    [SerializeField] private Transform healPopUpPrefab;

    public bool CanSeeInventory = false;
    public bool CanSeeShop = false;
    public bool isInShop = false;

    public float initialMovementSpeed;
    private Color initialColor;
    private Sprite initialHPFill;

    public enum PLAYER_STATE
    {
        NORMAL,
        ROLLING,
        DEAD,
        WIN
    }

    public PLAYER_STATE currentState;

    public int weaponNumber;
    private float regenTimer = 0;

    public Dictionary<Buff, int> buffList = new();
    public int buffTokens = 0;
    public float moveSpeedMultiplier = 1;
    public float maxHealthMultiplier = 1;
    public float evasionMultiplier = 0;
    public int regenValue = 0;
    public float meleeDmgBlockMultiplier = 0;

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;

        rb = GetComponent<Rigidbody2D>();
        coll = GetComponent<Collider2D>();
        playerSprite = GetComponent<SpriteRenderer>();
        initialColor = playerSprite.color;
        initialHPFill = playerPanel.transform.Find("Health Bar").Find("Fill").GetComponent<Image>().sprite;
        weaponSlot = transform.GetChild(0).GetComponent<WeaponScript>();
        this.currentState = PLAYER_STATE.NORMAL;
        this.rollTimer = this.rollCD;
    }

    private void Start()
    {
        audioSource.volume = 0.5f;

        SetPlayerData(_data);

        EquipDefaultWeapon(Resources.Load<Weapon>($"ScriptableObjects/Weapons/{this.defaultWeapon}"));
    }

    private void Update()
    {
        if (GameController.instance.currentState == GameController.GAME_STATE.PLAYING || GameController.instance.currentState == GameController.GAME_STATE.TUTORIAL)
        {
            switch (this.currentState)
            {
                case PLAYER_STATE.NORMAL:
                    float moveX = 0f;
                    float moveY = 0f;

                    if (Input.GetKey(KeyCode.W))
                    {
                        moveY = +1f;
                    }
                    if (Input.GetKey(KeyCode.S))
                    {
                        moveY = -1f;
                    }
                    if (Input.GetKey(KeyCode.A))
                    {
                        moveX = -1f;
                    }
                    if (Input.GetKey(KeyCode.D))
                    {
                        moveX = +1f;
                    }

                    this.moveDir = new Vector3(moveX, moveY).normalized;
                    if (moveX != 0 || moveY != 0)
                    {
                        // Moving
                        this.lastMoveDir = this.moveDir;
                        this.anim.SetBool("isWalking", true);
                    }
                    else
                    {
                        this.anim.SetBool("isWalking", false);
                    }

                    if (Input.GetKeyDown(KeyCode.LeftShift))
                    {
                        if (this.rollTimer < this.rollCD) return;
                        this.rollDir = this.lastMoveDir;
                        this.rollSpeed = this.initialMovementSpeed * 2f;
                        this.currentState = PLAYER_STATE.ROLLING;
                        this.rollTimer = 0;
                        this.rollOnCD = true;
                        this.playerPanel.transform.Find("Roll").Find("Roll Image").Find("Cooldown").gameObject.SetActive(true);
                        //Play Roll Anims
                        dustCloud.Play();
                    }
                    break;
                case PLAYER_STATE.ROLLING:
                    float rollSpeedDropMultiplier = 2f;
                    this.rollSpeed -= rollSpeed * rollSpeedDropMultiplier * Time.deltaTime;
                    Physics2D.IgnoreLayerCollision(3, 7, true);
                    float rollSpeedMinimum = 5f;
                    if (rollSpeed < rollSpeedMinimum)
                    {
                        Physics2D.IgnoreLayerCollision(3, 7, false);
                        this.currentState = PLAYER_STATE.NORMAL;
                        //Stop Roll Anims
                    }
                    break;
                default: break;
            }
            this.rollTimer += Time.deltaTime;
            this.playerPanel.transform.Find("Roll").Find("Roll Image").Find("Cooldown").Find("Timer").gameObject.GetComponent<TMP_Text>().text = ((int)this.rollCD - (int)this.rollTimer).ToString();
            if (this.rollOnCD)
            {
                this.playerPanel.transform.Find("Roll").Find("Roll Image").Find("Cooldown").gameObject.GetComponent<Image>().fillAmount = Mathf.Lerp(
                this.playerPanel.transform.Find("Roll").Find("Roll Image").Find("Cooldown").gameObject.GetComponent<Image>().fillAmount, 1 - rollTimer / rollCD, 10f * Time.deltaTime);
            }
            else
            {
                this.playerPanel.transform.Find("Roll").Find("Roll Image").Find("Cooldown").gameObject.GetComponent<Image>().fillAmount = 1;
            }
            if (this.rollTimer >= this.rollCD)
            {
                this.rollOnCD = false;
                this.playerPanel.transform.Find("Roll").Find("Roll Image").Find("Cooldown").gameObject.SetActive(false);
            }

            //Regen
            if (this.regenValue > 0 && this.currentHealth < this.maxHealth)
            {
                this.regenTimer += Time.deltaTime;
                if (this.regenTimer >= 1)
                {
                    this.regenTimer = 0.0f;
                    UpdateHealth(this.regenValue);
                }
            }
        }
    }

    private void FixedUpdate()
    {
        switch (this.currentState)
        {
            case PLAYER_STATE.NORMAL:
                this.rb.velocity = this.moveDir * (this.movementSpeed * this.moveSpeedMultiplier);
                break;
            case PLAYER_STATE.ROLLING:
                this.rb.velocity = this.rollDir * this.rollSpeed;
                break;
            case PLAYER_STATE.DEAD:
                this.rb.velocity = Vector3.zero;
                break;
            case PLAYER_STATE.WIN:
                this.rb.velocity = Vector3.zero;
                break;
            default:
                break;
        }
    }

    public void SetPlayerData(Player playerData)
    {
        this.playerName = playerData.playerName;
        this.maxHealth = playerData.health;
        this.movementSpeed = playerData.movementSpeed;
        this.defaultWeapon = playerData.defaultWeapon;

        this.currentHealth = this.maxHealth;
        this.initialMovementSpeed = this.movementSpeed;

        this.cash = 0;
        this.experience = 0;
        this.level = 1;
    }

    public void TakeDamage(float damage, bool fromZone)
    {
        if (GameController.instance.currentState == GameController.GAME_STATE.DEAD) return;
        if (this.currentState == PLAYER_STATE.ROLLING && fromZone == false) return;

        if (this.evasionMultiplier > 0 && fromZone == false)
        {
            if (Random.Range(0, 1f) < this.evasionMultiplier - 1)
            {
                //TODO: Add DODGED Popup
                Debug.Log("Evaded attack");
                return;
            }
        }

        if (this.equippedWeapon.weaponType == "melee" && this.meleeDmgBlockMultiplier > 0 && fromZone == false)
        {
            if (Random.Range(0, 1f) < this.meleeDmgBlockMultiplier - 1)
            {
                //TODO: Add BLOCKED Popup
                Debug.Log("Blocked attack");
                return;
            }
        }

        UpdateHealth(-damage);
        Transform damagePopupTransform = Instantiate(damagePopupPrefab, transform.position, Quaternion.identity);
        DamagePopup damagePopup = damagePopupTransform.GetComponent<DamagePopup>();
        damagePopup.SetupPlayerDamage(damage);

        playerSprite.color = initialColor;
        StopCoroutine(HitFlicker());
        StartCoroutine(HitFlicker());

        playerPanel.transform.Find("Health Bar").Find("Fill").GetComponent<Image>().sprite = initialHPFill;
        StopCoroutine(FlashHealthBar());
        StartCoroutine(FlashHealthBar());

        CameraController.instance.animator.SetTrigger("CameraShake");
        if (currentHealth <= 0)
        {
            this.currentState = PLAYER_STATE.DEAD;
            GameController.instance.currentState = GameController.GAME_STATE.DEAD;
        }
    }

    public void UpdateExperience(int experience)
    {
        this.experience += experience;

        if (this.experience >= LevelController.instance.xpNeeded)
        {
            this.experience -= LevelController.instance.xpNeeded;
            level++;
            LevelController.instance.CalculateXpNeeded();
            this.buffTokens++;
            BuffController.instance.UpdatePlayerTokensDisplay();

            Transform damagePopupTransform = Instantiate(damagePopupPrefab, transform.position, Quaternion.identity);
            DamagePopup damagePopup = damagePopupTransform.GetComponent<DamagePopup>();
            damagePopup.SetupLevelUp();

            playerPanel.transform.Find("Level").GetComponent<TMP_Text>().text = $"Lvl.{level}";
        }
    }

    public void UpdateMaxHealth()
    {
        if (this.currentHealth == this.maxHealth) this.currentHealth = this.maxHealth * this.maxHealthMultiplier;
        this.maxHealth *= this.maxHealthMultiplier;
    }

    public void UpdateHealth(float value)
    {
        this.currentHealth += value;
        if (this.currentHealth >= this.maxHealth) this.currentHealth = this.maxHealth;
        if (value > 0)
        {
            Transform healPopUpTransform = Instantiate(healPopUpPrefab, transform.position, Quaternion.identity);
            DamagePopup healPopUp = healPopUpTransform.GetComponent<DamagePopup>();
            healPopUp.SetupHeal(value);
        }
    }

    public void UpdateCash(int value)
    {
        playerPanel.transform.Find("Cash").GetComponent<SlidingNumber>().AddToNumber(value);
        this.cash += value;
    }

    public void EquipDefaultWeapon(Weapon weaponData)
    {
        equippedWeapon = weaponData;
        weaponSlot.SetWeaponData(weaponData);
        weaponNumber = -1;
        RefreshEquippedUI();
    }

    public void EquipWeapon(Weapon weaponData)
    {
        equippedWeapon = weaponData;
        weaponSlot.SetWeaponData(weaponData);
        transform.GetChild(1).gameObject.SetActive(false);
        weaponNumber = inventory.FindIndex(a => a == weaponData);
        RefreshEquippedUI();
        RefreshInventoryUI();
    }

    public void UnequipWeapon()
    {
        if (equippedWeapon.weaponType == "akimbo") transform.GetChild(1).gameObject.SetActive(false);

        // Set Equipped Weapon Data to FISTS
        weaponSlot.SetWeaponData(Resources.Load<Weapon>("ScriptableObjects/Weapons/Fists"));
        equippedWeapon = Resources.Load<Weapon>("ScriptableObjects/Weapons/Fists");
        weaponNumber = -1;

        RefreshEquippedUI();
    }

    public void AddToInventory(Weapon weaponData)
    {
        inventory.Add(weaponData);
        GameObject inventoryItem = Instantiate(inventoryItemPrefab, playerPanel.transform.Find("Inventory Panel").Find("Inventory Items Panel").Find("Item Slots"));
        inventoryItem.GetComponent<InventoryItem>().Initialise(weaponData);
        UnequipWeapon();
        EquipWeapon(weaponData);
    }

    public void RemoveFromInventory(Weapon weaponData)
    {
        inventory.Remove(weaponData);
        if (equippedWeapon == weaponData) UnequipWeapon();
        RefreshInventoryUI();
        RefreshEquippedUI();
    }

    public void HideInventoryItemDetailUI()
    {
        playerPanel.transform.Find("Inventory Panel").Find("Item Detail Panel").gameObject.SetActive(false);
    }

    public void RefreshInventoryUI()
    {
        foreach (Transform child in playerPanel.transform.Find("Inventory Panel").Find("Inventory Items Panel").Find("Item Slots"))
        {
            if (!inventory.Contains(child.gameObject.GetComponent<InventoryItem>().weaponData)) Destroy(child.gameObject);
            child.gameObject.GetComponent<InventoryItem>().Check();
        }
    }

    public void RefreshEquippedUI()
    {
        if (equippedWeapon)
        {
            playerPanel.transform.Find("Equipped Weapon").Find("Weapon Name").GetComponent<TMP_Text>().text = equippedWeapon.weaponName;
            playerPanel.transform.Find("Equipped Weapon").Find("Weapon Image").Find("Weapon").GetComponent<Image>().sprite = Resources.Load<Sprite>(equippedWeapon.thumbnailPath);
        }
    }

    public void TurnOffAllViews()
    {
        playerPanel.transform.Find("Inventory Panel").gameObject.SetActive(false);
        playerPanel.transform.parent.Find("Shop Panel").gameObject.SetActive(false);
        CanSeeInventory = false;
        CanSeeShop = false;
    }

    public void ToggleInventoryView()
    {
        if (CanSeeShop) ToggleShopView();

        CanSeeInventory = !CanSeeInventory;
        if (!CanSeeInventory)
        {
            GameController.instance.CursorNotOverUI();
        }
        else
        {
            RefreshInventoryUI();
            HideInventoryItemDetailUI();
        }

        playerPanel.transform.Find("Inventory Panel").gameObject.SetActive(CanSeeInventory);
    }

    public void ToggleShopView()
    {
        if (CanSeeInventory) ToggleInventoryView();
        CanSeeShop = !CanSeeShop;
        if (!CanSeeShop)
        {
            GameController.instance.CursorNotOverUI();
        }
        else
        {
            RefreshShopUI();
        }
    }

    public void RefreshShopUI()
    {
        ShopController.instance.DisplaySellPanel();
        ShopController.instance.DisplayBuyPanel();
    }

    public void UpdateStimTimerUI(float timer, float duration)
    {
        this.playerPanel.transform.Find("Stim Buff").gameObject.SetActive(true);
        this.playerPanel.transform.Find("Stim Buff").Find("Timer").gameObject.GetComponent<Image>().fillAmount = Mathf.Lerp(
            this.playerPanel.transform.Find("Stim Buff").Find("Timer").gameObject.GetComponent<Image>().fillAmount, 1 - timer / duration, 10f * Time.deltaTime);
    }

    public void HideStimTimerUI()
    {
        this.playerPanel.transform.Find("Stim Buff").Find("Timer").gameObject.GetComponent<Image>().fillAmount = 1;
        this.playerPanel.transform.Find("Stim Buff").gameObject.SetActive(false);
    }

    public void LookAtMouse()
    {
        Vector2 mousePos = (Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition);
        transform.up = (Vector3)(mousePos - new Vector2(transform.position.x, transform.position.y));
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (GameController.instance.currentState == GameController.GAME_STATE.TUTORIAL)
        {
            if (collision.gameObject.CompareTag("Tutorial Area 2"))
            {
                GameController.instance.passedArea2 = true;
            }
        }
    }

    public IEnumerator SpawnFlicker()
    {
        Color initialColor = playerSprite.color;
        Color hitFlash = Color.black;
        hitFlash.a = 0.2f;

        playerSprite.color = hitFlash;
        yield return new WaitForSeconds(0.05f);
        playerSprite.color = initialColor;
        yield return new WaitForSeconds(0.05f);
        playerSprite.color = hitFlash;
        yield return new WaitForSeconds(0.05f);
        playerSprite.color = initialColor;
        yield return new WaitForSeconds(0.05f);
        playerSprite.color = hitFlash;
        yield return new WaitForSeconds(0.05f);
        playerSprite.color = initialColor;
    }

    IEnumerator HitFlicker()
    {
        float duration = 0.1f;
        while (duration > 0)
        {
            duration -= Time.deltaTime;

            Color hitFlash = Color.red;
            hitFlash.a = 0.7f;
            playerSprite.color = hitFlash;
            yield return null;
        }
        playerSprite.color = initialColor;
    }

    IEnumerator FlashHealthBar()
    {
        float duration = 0.1f;
        while (duration > 0)
        {
            duration -= Time.deltaTime;
            playerPanel.transform.Find("Health Bar").Find("Fill").GetComponent<Image>().sprite = Resources.Load<Sprite>("UISprites/Player HP Fill Flash");
            yield return null;
        }
        playerPanel.transform.Find("Health Bar").Find("Fill").GetComponent<Image>().sprite = initialHPFill;
    }
}
