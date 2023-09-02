using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;

public class ShopController : MonoBehaviour
{
    [SerializeField]
    private GameObject shopItemPrefab;

    [SerializeField]
    private GameObject shopPanel;

    private GameObject buyPanel;
    private GameObject sellPanel;

    private void Awake()
    {
        buyPanel = shopPanel.transform.GetChild(2).gameObject;
        buyPanel.SetActive(true);
        sellPanel = shopPanel.transform.GetChild(3).gameObject;
        sellPanel.SetActive(false);

        Weapon[] allWeapons = Resources.LoadAll<Weapon>("ScriptableObjects/Weapons");
        foreach (Weapon weaponData in allWeapons)
        {
            if (weaponData.inShop == "NO") continue;
            GameObject shopItem = Instantiate(shopItemPrefab, buyPanel.transform.GetChild(0));
            shopItem.GetComponent<ShopItem>().Initialise(weaponData, "buy");
        }

        Item[] allItems = Resources.LoadAll<Item>("ScriptableObjects/Items");
        foreach (Item itemData in allItems)
        {
            if (itemData.inShop == "NO") continue;
            GameObject shopItem = Instantiate(shopItemPrefab, buyPanel.transform.GetChild(0));
            shopItem.GetComponent<ShopItem>().Initialise(itemData, "buy");
        }
    }

    private void Update()
    {
        if (PlayerScript.instance.CanSeeShop)
        {
            shopPanel.SetActive(true);
        }
        else
        {
            shopPanel.SetActive(false);
            shopPanel.transform.Find("Item Detail Panel").gameObject.SetActive(false);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
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

        foreach (KeyValuePair<ScriptableObject, int> item in PlayerScript.instance.inventory)
        {
            GameObject shopInventoryItem = Instantiate(shopItemPrefab, sellPanel.transform.GetChild(0));
            shopInventoryItem.GetComponent<ShopItem>().Initialise(item.Key, "sell");
        }
    }
}
