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
    private float rollCD = 5f;
    private float rollTimer;

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

    public bool CanSeeInventory = false;
    public bool CanSeeShop = false;
    public bool isInShop = false;

    public float initialMovementSpeed;

    public enum PLAYER_STATE
    {
        NORMAL,
        ROLLING,
        DEAD
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
        UpdateCash(500);
    }

    private void Update()
    {
        if (GameController.instance.currentState == GameController.GAME_STATE.PLAYING)
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
                        this.rollSpeed = this.initialMovementSpeed * 1.5f;
                        this.currentState = PLAYER_STATE.ROLLING;
                        this.rollTimer = 0;

                        this.playerPanel.transform.Find("Roll").Find("Roll Image").Find("Cooldown").gameObject.SetActive(true);
                        //Play Roll Anims
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
            if (this.rollTimer >= this.rollCD) this.playerPanel.transform.Find("Roll").Find("Roll Image").Find("Cooldown").gameObject.SetActive(false);
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
        RefreshAmmoUI();
    }

    public void TakeDamage(float damage, bool fromZone)
    {
        if (GameController.instance.currentState == GameController.GAME_STATE.DEAD) return;
        if (this.currentState == PLAYER_STATE.ROLLING && fromZone == false) return;

        Transform damagePopupTransform = Instantiate(damagePopupPrefab, transform.position, Quaternion.identity);
        DamagePopup damagePopup = damagePopupTransform.GetComponent<DamagePopup>();
        damagePopup.Setup(damage);

        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            this.rb.velocity = Vector3.zero;
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
            LevelController.instance.UpdateLevelsModifier();
            maxHealth = LevelController.instance.healthBuff;
            currentHealth = maxHealth;
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
        AddToInventory(weaponData);
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
    }

    public void UnequipWeapon()
    {
        if (equippedWeapon.weaponType == "akimbo") transform.GetChild(1).gameObject.SetActive(false);
        // equippedWeapon = null;

        // Set Equipped Weapon Data to UNARMED
        weaponSlot.SetWeaponData(Resources.Load<Weapon>("ScriptableObjects/Weapons/Fists"));
        equippedWeapon = Resources.Load<Weapon>("ScriptableObjects/Weapons/Fists");
        this.weaponWeight = 0;
        RefreshEquippedUI();
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
                    RefreshAmmoUI();
                    return;
                }
                break;
            case "MEDIUM":
                if (this.ammoCount[ammoType] + ammo >= this.mediumAmmoCap)
                {
                    this.ammoCount[ammoType] = this.mediumAmmoCap;
                    RefreshAmmoUI();
                    return;
                }
                break;
            case "HEAVY":
                if (this.ammoCount[ammoType] + ammo >= this.heavyAmmoCap)
                {
                    this.ammoCount[ammoType] = this.heavyAmmoCap;
                    RefreshAmmoUI();
                    return;
                }
                break;
            default:
                break;
        }
        this.ammoCount[ammoType] += ammo;
        RefreshAmmoUI();
    }

    public void RefreshAmmoUI()
    {
        playerPanel.transform.Find("Ammo Count").Find("LIGHT").Find("Count").GetComponent<TMP_Text>().text = this.ammoCount["LIGHT"] + " LIGHT";
        playerPanel.transform.Find("Ammo Count").Find("MEDIUM").Find("Count").GetComponent<TMP_Text>().text = this.ammoCount["MEDIUM"] + " MEDIUM";
        playerPanel.transform.Find("Ammo Count").Find("HEAVY").Find("Count").GetComponent<TMP_Text>().text = this.ammoCount["HEAVY"] + " HEAVY";
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
            playerPanel.transform.Find("Equipped Weapon").Find("Weapon Ammo Type").GetComponent<TMP_Text>().text = equippedWeapon.ammoType;
            playerPanel.transform.Find("Equipped Weapon").Find("Weapon").GetComponent<Image>().sprite = Resources.Load<Sprite>(equippedWeapon.thumbnailPath);
        }
        else
        {
            playerPanel.transform.Find("Equipped Weapon").Find("Weapon Ammo Type").GetComponent<TMP_Text>().text = "UNARMED";
            playerPanel.transform.Find("Equipped Weapon").Find("Weapon").GetComponent<Image>().sprite = Resources.Load<Sprite>("UISprites/testItemSprite");
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
        playerPanel.transform.parent.Find("Shop Panel").Find("Cash").GetComponent<SlidingNumber>().SetNumber(this.cash);
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
            default:
                break;
        }
        playerPanel.transform.Find("Alert").Find("Text").gameObject.GetComponent<TMP_Text>().text = alertText;
        StopCoroutine(DisplayAlert());
        StartCoroutine(DisplayAlert());
    }

    public void LookAtMouse()
    {
        Vector2 mousePos = (Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition);
        transform.up = (Vector3)(mousePos - new Vector2(transform.position.x, transform.position.y));
    }

    IEnumerator DisplayAlert()
    {
        playerPanel.transform.Find("Alert").gameObject.SetActive(true);
        yield return new WaitForSeconds(2);
        playerPanel.transform.Find("Alert").gameObject.SetActive(false);
    }
}
