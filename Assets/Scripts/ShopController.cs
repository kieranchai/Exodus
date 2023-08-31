using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopController : MonoBehaviour
{
    [SerializeField]
    private GameObject shopWeaponPrefab;

    [SerializeField]
    private GameObject shopItemPrefab;

    [SerializeField]
    private GameObject shopPanel;

    private GameObject buyPanel;
    private GameObject sellPanel;

    private void Start()
    {
        buyPanel = shopPanel.transform.GetChild(2).gameObject;
        buyPanel.SetActive(true);
        sellPanel = shopPanel.transform.GetChild(3).gameObject;
        sellPanel.SetActive(false);

        Weapon[] allWeapons = Resources.LoadAll<Weapon>("ScriptableObjects/Weapons");
        foreach(Weapon weaponData in allWeapons)
        {
            if (weaponData.inShop == "NO") continue;
            GameObject shopWeapon = Instantiate(shopWeaponPrefab, buyPanel.transform.GetChild(0));
            shopWeapon.GetComponent<ShopWeapon>().Initialise(weaponData, "buy");
        }

        //Hardcode in for now
        GameObject shopItem = Instantiate(shopItemPrefab, buyPanel.transform.GetChild(0));
        shopItem.GetComponent<ShopItem>().Initialise();

    }

    private void Update()
    {
        if(PlayerScript.instance.CanSeeShop) shopPanel.SetActive(true); else shopPanel.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            PlayerScript.instance.isInShop = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PlayerScript.instance.isInShop = false;
            PlayerScript.instance.CanSeeShop = false;
            Cursor.SetCursor(PlayerScript.instance.cursor_Normal, PlayerScript.instance.normalCursorHotspot, CursorMode.Auto); 
        }
    }

    public void DisplayBuyPanel()
    {
        sellPanel.SetActive(false);
        buyPanel.SetActive(true);
    }

    public void DisplaySellPanel()
    {
        buyPanel.SetActive(false);
        UpdateSellInventory();
        sellPanel.SetActive(true);
    }

    public void UpdateSellInventory()
    {
        foreach (Transform child in sellPanel.transform.GetChild(0))
        {
            Destroy(child.gameObject);
        }

        foreach (Weapon inventoryWeapon in PlayerScript.instance.inventory)
        {
            GameObject shopInventoryWeapon = Instantiate(shopWeaponPrefab, sellPanel.transform.GetChild(0));
            shopInventoryWeapon.GetComponent<ShopWeapon>().Initialise(inventoryWeapon, "sell");
        }
    }
}
