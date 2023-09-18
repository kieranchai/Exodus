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

    public string playerName;
    public float maxHealth;
    public float movementSpeed;
    public int lightAmmoCap;
    public int mediumAmmoCap;
    public int heavyAmmoCap;
    public string defaultWeapon;

    public float currentHealth;
    public int cash;
    public float experience;
    public int level;

    private Vector3 moveDir;
    private Vector3 rollDir;
    private float rollSpeed;
    private Vector3 lastMoveDir;
    private float rollCD = 3f;
    private float rollTimer;
    private bool rollOnCD = false;

    [SerializeField]
    private Player _data;

    public float weaponWeight = 0;
    public WeaponScript weaponSlot;
    public WeaponScript akimboSlot;
    public Weapon equippedWeapon;

    public Dictionary<string, int> ammoCount = new();

    public Dictionary<ScriptableObject, int> inventory = new();

    [SerializeField] private GameObject playerPanel;
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
        akimboSlot = transform.GetChild(1).GetComponent<WeaponScript>();
        this.currentState = PLAYER_STATE.NORMAL;
        this.rollTimer = this.rollCD;
    }

    private void Start()
    {
        SetPlayerData(_data);

        EquipDefaultWeapon(Resources.Load<Weapon>($"ScriptableObjects/Weapons/{this.defaultWeapon}"));

        UpdateAmmoCount(this.equippedWeapon.defaultAmmo, this.equippedWeapon.ammoType);

        StartCoroutine(SpawnFlicker());
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

                    float rollSpeedMinimum = 5f;
                    if (rollSpeed < rollSpeedMinimum)
                    {
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
        }
    }

    private void FixedUpdate()
    {
        switch (this.currentState)
        {
            case PLAYER_STATE.NORMAL:
                this.rb.velocity = this.moveDir * (this.movementSpeed - this.weaponWeight);
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
        this.lightAmmoCap = playerData.lightAmmoCap;
        this.mediumAmmoCap = playerData.mediumAmmoCap;
        this.heavyAmmoCap = playerData.heavyAmmoCap;
        this.defaultWeapon = playerData.defaultWeapon;

        this.currentHealth = this.maxHealth;
        this.initialMovementSpeed = this.movementSpeed;

        this.cash = 0;
        this.experience = 0;
        this.level = 1;

        this.ammoCount.Add("LIGHT", 0);
        this.ammoCount.Add("MEDIUM", 0);
        this.ammoCount.Add("HEAVY", 0);
    }

    public void TakeDamage(float damage, bool fromZone)
    {
        if (GameController.instance.currentState == GameController.GAME_STATE.DEAD) return;
        if (this.currentState == PLAYER_STATE.ROLLING && fromZone == false) return;

        UpdateHealth(-damage);
        Transform damagePopupTransform = Instantiate(damagePopupPrefab, transform.position, Quaternion.identity);
        DamagePopup damagePopup = damagePopupTransform.GetComponent<DamagePopup>();
        damagePopup.Setup(damage);

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

    public void UpdateExperience(float experience)
    {
        this.experience += experience;

        if (this.experience >= LevelController.instance.xpNeeded)
        {
            if (LevelController.instance.isMaxLvl)
            {
                return;
            }
            this.experience -= LevelController.instance.xpNeeded;
            level++;
            playerPanel.transform.Find("Level").GetComponent<TMP_Text>().text = $"Lvl.{level}";
            LevelController.instance.UpdateLevelsModifier();
            maxHealth = LevelController.instance.healthBuff;
            currentHealth = maxHealth;
        }
    }

    public void UpdateHealth(float value)
    {
        this.currentHealth += value;
        if(this.currentHealth >= this.maxHealth) this.currentHealth = this.maxHealth;
        if(value >= 0)
        {
            Transform healPopUpTransform = Instantiate(healPopUpPrefab, transform.position, Quaternion.identity);
            DamagePopup healPopUp = healPopUpTransform.GetComponent<DamagePopup>();
            healPopUp.Setup(value);
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
        this.weaponWeight = weaponData.weight;
        RefreshEquippedUI();
        UpdateEquippedAmmoUI();
    }

    public void EquipWeapon(Weapon weaponData)
    {
        if (!inventory.ContainsKey(weaponData)) return;
        equippedWeapon = weaponData;
        weaponSlot.SetWeaponData(weaponData);
        transform.GetChild(1).gameObject.SetActive(false);
        if (weaponData.weaponType == "akimbo")
        {
            transform.GetChild(1).gameObject.GetComponent<WeaponScript>().SetWeaponData(weaponData);
            transform.GetChild(1).gameObject.SetActive(true);
        }

        this.weaponWeight = weaponData.weight;
        RefreshEquippedUI();
        UpdateEquippedAmmoUI();
    }

    public void UnequipWeapon()
    {
        if (equippedWeapon.weaponType == "akimbo") transform.GetChild(1).gameObject.SetActive(false);

        // Set Equipped Weapon Data to FISTS
        weaponSlot.SetWeaponData(Resources.Load<Weapon>("ScriptableObjects/Weapons/Fists"));
        equippedWeapon = Resources.Load<Weapon>("ScriptableObjects/Weapons/Fists");
        this.weaponWeight = 0;

        RefreshEquippedUI();
        UpdateEquippedAmmoUI();
    }

    public void AddToInventory(ScriptableObject itemData)
    {
        if (!inventory.ContainsKey(itemData))
        {
            inventory.Add(itemData, 1);
        }
        else
        {
            ++inventory[itemData];
        }

        GameObject inventoryItem = Instantiate(inventoryItemPrefab, playerPanel.transform.Find("Inventory Panel").Find("Inventory Items Panel").Find("Item Slots"));
        inventoryItem.GetComponent<InventoryItem>().Initialise(itemData);
        RefreshInventoryUI();
    }

    public void RemoveFromInventory(ScriptableObject itemData)
    {
        if (inventory[itemData] == 1)
        {
            inventory.Remove(itemData);
            if (equippedWeapon == itemData) UnequipWeapon();
            RefreshInventoryUI();
            RefreshEquippedUI();
            UpdateEquippedAmmoUI();
            UpdateInventoryAmmoUI();
        }
        else
        {
            --inventory[itemData];
        }
    }

    public void UpdateAmmoCount(int ammo, string ammoType)
    {
        if (ammoType == "NULL") return;
        switch (ammoType)
        {
            case "LIGHT":
                if (this.ammoCount[ammoType] + ammo >= this.lightAmmoCap)
                {
                    this.ammoCount[ammoType] = this.lightAmmoCap;
                    UpdateEquippedAmmoUI();
                    UpdateInventoryAmmoUI();
                    return;
                }
                break;
            case "MEDIUM":
                if (this.ammoCount[ammoType] + ammo >= this.mediumAmmoCap)
                {
                    this.ammoCount[ammoType] = this.mediumAmmoCap;
                    UpdateEquippedAmmoUI();
                    UpdateInventoryAmmoUI();
                    return;
                }
                break;
            case "HEAVY":
                if (this.ammoCount[ammoType] + ammo >= this.heavyAmmoCap)
                {
                    this.ammoCount[ammoType] = this.heavyAmmoCap;
                    UpdateEquippedAmmoUI();
                    UpdateInventoryAmmoUI();
                    return;
                }
                break;
            default:
                break;
        }
        this.ammoCount[ammoType] += ammo;
        UpdateEquippedAmmoUI();
        UpdateInventoryAmmoUI();
    }

    public void UpdateInventoryAmmoUI()
    {
        playerPanel.transform.Find("Inventory Panel").Find("Light Ammo Count").gameObject.GetComponent<TMP_Text>().text = this.ammoCount["LIGHT"].ToString();
        playerPanel.transform.Find("Inventory Panel").Find("Medium Ammo Count").gameObject.GetComponent<TMP_Text>().text = this.ammoCount["MEDIUM"].ToString();
        playerPanel.transform.Find("Inventory Panel").Find("Heavy Ammo Count").gameObject.GetComponent<TMP_Text>().text = this.ammoCount["HEAVY"].ToString();

    }

    public void HideInventoryItemDetailUI()
    {
        playerPanel.transform.Find("Inventory Panel").Find("Item Detail Panel").gameObject.SetActive(false);
    }

    public void RefreshInventoryUI()
    {
        foreach (Transform child in playerPanel.transform.Find("Inventory Panel").Find("Inventory Items Panel").Find("Item Slots"))
        {
            Destroy(child.gameObject);
        }

        if (inventory.Count > 0)
        {
            foreach (KeyValuePair<ScriptableObject, int> item in inventory)
            {
                GameObject inventoryItem = Instantiate(inventoryItemPrefab, playerPanel.transform.Find("Inventory Panel").Find("Inventory Items Panel").Find("Item Slots"));
                inventoryItem.GetComponent<InventoryItem>().Initialise(item.Key);
            }
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

    public void UpdateEquippedAmmoUI()
    {
        if (this.equippedWeapon.weaponType == "melee")
        {
            this.playerPanel.transform.Find("Ammo Count").gameObject.GetComponent<TMP_Text>().text = "";
            this.playerPanel.transform.Find("Ammo Type").gameObject.GetComponent<TMP_Text>().text = "MELEE";
        }
        else
        {
            this.playerPanel.transform.Find("Ammo Count").gameObject.GetComponent<TMP_Text>().text = this.ammoCount[this.equippedWeapon.ammoType].ToString();
            this.playerPanel.transform.Find("Ammo Type").gameObject.GetComponent<TMP_Text>().text = this.equippedWeapon.ammoType.ToUpper();
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

    public void AlertPopup(string alertType)
    {
        string alertText = "";

        switch (alertType)
        {
            case "health":
                alertText = "You are already at full health.";
                break;
            case "speed":
                alertText = "You are already affected by STIM. Try again later.";
                break;
            case "gas":
                alertText = "You have already stopped the gas. Try again later.";
                break;
            case "gasEnded":
                alertText = "The gas has already finished spreading.";
                break;
            case "gasCovered":
                alertText = "The gas has covered the entire planet. The shop will now stop operating.";
                break;
            default:
                break;
        }
        playerPanel.transform.Find("Alert").Find("Text").gameObject.GetComponent<TMP_Text>().text = alertText;
        StopCoroutine(DisplayAlert());
        StartCoroutine(DisplayAlert());
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

    IEnumerator DisplayAlert()
    {
        playerPanel.transform.Find("Alert").gameObject.SetActive(true);
        yield return new WaitForSeconds(3);
        playerPanel.transform.Find("Alert").gameObject.SetActive(false);
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

    IEnumerator SpawnFlicker()
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
