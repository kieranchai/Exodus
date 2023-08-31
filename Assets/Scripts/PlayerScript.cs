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

    public float currentHealth;
    public float cash;

    public ContactFilter2D movementFilter;
    private List<RaycastHit2D> castCollisions = new List<RaycastHit2D>();
    public float collisionOffset;

    [SerializeField]
    private Player _data;

    //test
    [SerializeField]
    private Weapon _weaponData;
    //

    public float weaponWeight = 1;
    public WeaponScript weaponSlot;
    public Weapon equippedWeapon;

    public Dictionary<string, int> ammoCount = new Dictionary<string, int>();

    public List<Weapon> inventory = new(5);

    [SerializeField] private GameObject playerPanel;
    [SerializeField] private GameObject inventoryWeaponPrefab;

    private bool CanSeeInventory = false;
    public bool CanSeeShop = false;
    public bool isInShop = false;

    public Texture2D cursor_Normal;
    public Vector2 normalCursorHotspot;

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
        EquipWeapon(_weaponData);
    }

    public void LateUpdate() {
        if (Input.GetAxisRaw("Horizontal") != 0 || Input.GetAxisRaw("Vertical") != 0)
        {
            anim.SetBool("isWalking", true);
        }
        else
        {
            anim.SetBool("isWalking", false);
        }
    }

    public void SetPlayerData(Player playerData)
    {
        this.playerName = playerData.playerName;
        this.maxHealth = playerData.health;
        this.movementSpeed = playerData.movementSpeed;

        this.currentHealth = this.maxHealth;
        this.cash = 400;

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
        playerPanel.transform.GetChild(3).GetComponent<HealthBar>().UpdateHealthBar();
    }

    public void EquipWeapon(Weapon weaponData)
    {
        if (equippedWeapon)
        {
            SwapWeapon(weaponData);
        }
        //Set equipped weapon to the weapon I want to equip
        equippedWeapon = weaponData;
        weaponSlot.SetWeaponData(weaponData);
        RefreshUI();
    }

    public void SwapWeapon(Weapon weaponData)
    {
        //Remove weapon that I want to equip from Inventory
        RemoveFromInventory(weaponData);
        //Add currently equipped weapon to inventory and remove from equipped
        AddWeapon(this.equippedWeapon);
        equippedWeapon = null;
    }

    public void AddWeapon(Weapon weaponData)
    {
        inventory.Add(weaponData);
        GameObject inventoryWeapon = Instantiate(inventoryWeaponPrefab, playerPanel.transform.GetChild(1).GetChild(0));
        inventoryWeapon.GetComponent<InventoryWeapon>().Initialise(weaponData);
    }

    public void RemoveFromInventory(Weapon weaponData)
    {
        inventory.Remove(weaponData);
        RefreshUI();
    }

    public void UpdateAmmoCount(int ammo, string ammoType)
    {
        this.ammoCount[ammoType] += ammo;

        RefreshAmmoUI();
    }

    public void RefreshAmmoUI()
    {
        playerPanel.transform.GetChild(2).GetChild(0).GetChild(0).GetComponent<TMP_Text>().text = this.ammoCount["LIGHT"] + " LIGHT";
        playerPanel.transform.GetChild(2).GetChild(1).GetChild(0).GetComponent<TMP_Text>().text = this.ammoCount["MEDIUM"] + " MEDIUM";
        playerPanel.transform.GetChild(2).GetChild(2).GetChild(0).GetComponent<TMP_Text>().text = this.ammoCount["HEAVY"] + " HEAVY";
    }

    public void RefreshUI()
    {
        playerPanel.transform.GetChild(0).GetChild(1).GetChild(0).GetComponent<TMP_Text>().text = equippedWeapon.weaponName;
        playerPanel.transform.GetChild(0).GetChild(0).GetComponent<Image>().sprite = Resources.Load<Sprite>(equippedWeapon.thumbnailPath);
        playerPanel.transform.GetChild(0).GetChild(2).GetComponent<TMP_Text>().text = equippedWeapon.ammoType;

        foreach (Transform child in playerPanel.transform.GetChild(1).GetChild(0))
        {
            Destroy(child.gameObject);
        }

        if (inventory.Count > 0)
        {
            foreach (Weapon weapon in inventory)
            {
                GameObject inventoryWeapon = Instantiate(inventoryWeaponPrefab, playerPanel.transform.GetChild(1).GetChild(0));
                inventoryWeapon.GetComponent<InventoryWeapon>().Initialise(weapon);
            }
        }
    }

    public void ToggleInventoryView()
    {
        if (CanSeeShop) ToggleShopView();

        CanSeeInventory = !CanSeeInventory;
        if (!CanSeeInventory) Cursor.SetCursor(cursor_Normal, normalCursorHotspot, CursorMode.Auto);
        playerPanel.transform.GetChild(1).gameObject.SetActive(CanSeeInventory);
        playerPanel.transform.GetChild(0).GetChild(1).gameObject.SetActive(CanSeeInventory);
    }

    public void ToggleShopView()
    {
        if (!isInShop) return;

        if (CanSeeInventory) ToggleInventoryView();
        CanSeeShop = !CanSeeShop;
        if (!CanSeeShop) Cursor.SetCursor(cursor_Normal, normalCursorHotspot, CursorMode.Auto);
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
            movementSpeed * weaponWeight * Time.fixedDeltaTime + collisionOffset);

        if (count == 0)
        {
            Vector2 moveVector = direction * movementSpeed * Time.fixedDeltaTime;

            rb.MovePosition(rb.position + moveVector);
            return true;
        }
        else
        {
            return false;
        }
    }

}
