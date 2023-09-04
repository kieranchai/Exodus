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

    public ContactFilter2D movementFilter;
    private List<RaycastHit2D> castCollisions = new List<RaycastHit2D>();
    public float collisionOffset;

    [SerializeField]
    private Player _data;

    public float weaponWeight = 0;
    public WeaponScript weaponSlot;
    public Weapon equippedWeapon;

    public Dictionary<string, int> ammoCount = new();

    public Dictionary<ScriptableObject, int> inventory = new();

    [SerializeField] private GameObject playerPanel;
    [SerializeField] private GameObject inventoryItemPrefab;

    public bool CanSeeInventory = false;
    public bool CanSeeShop = false;
    public bool isInShop = false;

    public float initialMovementSpeed;

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;

        rb = GetComponent<Rigidbody2D>();
        weaponSlot = transform.GetChild(0).GetComponent<WeaponScript>();
    }

    private void Start()
    {
        SetPlayerData(_data);

        EquipDefaultWeapon(Resources.Load<Weapon>($"ScriptableObjects/Weapons/{this.defaultWeapon}"));

        UpdateAmmoCount(this.equippedWeapon.defaultAmmo, this.equippedWeapon.ammoType);
        UpdateCash(500);
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

    public void TakeDamage(float damage)
    {
        if (GameController.instance.currentState == GameController.GAME_STATE.DEAD) return;

        currentHealth -= damage;
        if (currentHealth <= 0) GameController.instance.currentState = GameController.GAME_STATE.DEAD;
    }

    public void UpdateExperience(float experience)
    {
        this.experience += experience;

        //hardcode 
        if (this.experience > 100)
        {
            this.experience = 0;
            this.level++;
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
        this.weaponWeight = weaponData.weight;
        RefreshEquippedUI();
    }

    public void UnequipWeapon()
    {
        equippedWeapon = null;

        // Set Equipped Weapon Data to UNARMED
        weaponSlot.SetWeaponData(Resources.Load<Weapon>("ScriptableObjects/Weapons/Fists"));

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
    }

    public void LookAtMouse()
    {
        Vector2 mousePos = (Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition);
        transform.up = (Vector3)(mousePos - new Vector2(transform.position.x, transform.position.y));

        transform.GetChild(0).gameObject.transform.right = this.transform.up.normalized;
    }

    public bool MovePlayer(Vector2 direction)
    {
        int count = rb.Cast(
            direction,
            movementFilter,
            castCollisions,
            (movementSpeed - weaponWeight) * Time.fixedDeltaTime + collisionOffset);

        if (count == 0)
        {
            Vector2 moveVector = direction * (movementSpeed - weaponWeight) * Time.fixedDeltaTime;

            rb.MovePosition(rb.position + moveVector);
            return true;
        }
        else
        {
            return false;
        }
    }

}
