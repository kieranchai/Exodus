using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerScript : MonoBehaviour
{
    public static PlayerScript instance { get; private set; }

    public Rigidbody2D rb;

    public string playerName;
    public float maxHealth;
    public float movementSpeed;
    public float currentHealth;

    public ContactFilter2D movementFilter;
    private List<RaycastHit2D> castCollisions = new List<RaycastHit2D>();
    public float collisionOffset;

    [SerializeField]
    private Player _data;

    //test
    [SerializeField]
    private Weapon _weaponData;
    [SerializeField]
    private Weapon _weaponData2;
    //

    public WeaponScript weaponSlot;
    public Weapon equippedWeapon;

    public List<Weapon> inventory = new(5);

    [SerializeField] private GameObject playerPanel;
    [SerializeField] private GameObject inventoryWeaponPrefab;
    private bool CanSeeInventory = false;

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
        AddWeapon(_weaponData);
    }

    private void Update()
    {
        //test
        if (Input.GetKeyDown(KeyCode.M)) AddWeapon(_weaponData2);
    }

    public void SetPlayerData(Player playerData)
    {
        this.playerName = playerData.playerName;
        this.maxHealth = playerData.health;
        this.movementSpeed = playerData.movementSpeed;
        this.currentHealth = this.maxHealth;
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
        if (equippedWeapon)
        {
            inventory.Add(weaponData);
            GameObject inventoryWeapon = Instantiate(inventoryWeaponPrefab, playerPanel.transform.GetChild(1).GetChild(0));
            inventoryWeapon.GetComponent<InventoryWeapon>().Initialise(weaponData);
        } else
        {
            equippedWeapon = weaponData;
            weaponSlot.SetWeaponData(weaponData);
            RefreshUI();
        }
    }

    public void RemoveFromInventory(Weapon weaponData)
    {
        inventory.Remove(weaponData);
    }

    public void RefreshUI()
    {
        playerPanel.transform.GetChild(0).GetChild(1).GetChild(0).GetComponent<TMP_Text>().text = equippedWeapon.weaponName;
        playerPanel.transform.GetChild(0).GetChild(0).GetComponent<Image>().sprite = Resources.Load<Sprite>(equippedWeapon.thumbnailPath);
    }

    public void ToggleInventoryView()
    {
        CanSeeInventory = !CanSeeInventory;
        playerPanel.transform.GetChild(1).gameObject.SetActive(CanSeeInventory);
        playerPanel.transform.GetChild(0).GetChild(1).gameObject.SetActive(CanSeeInventory);
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
            movementSpeed * Time.fixedDeltaTime + collisionOffset);

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
