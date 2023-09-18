using System.Collections;
using System.Collections.Generic;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopController : MonoBehaviour
{
    public static ShopController instance { get; private set; }

    [SerializeField]
    private GameObject shopItemPrefab;

    public GameObject shopPanel;

    private GameObject buyPanel;
    private GameObject sellPanel;

    [SerializeField]
    private GameObject shopButtonPrefab;

    private GameObject shopButton;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;

        buyPanel = shopPanel.transform.Find("Buy Panel").gameObject;
        buyPanel.SetActive(true);
        sellPanel = shopPanel.transform.Find("Sell Panel").gameObject;
        sellPanel.SetActive(false);

        if(GameController.instance.currentState == GameController.GAME_STATE.TUTORIAL)
        {
            Item[] allItems = Resources.LoadAll<Item>("ScriptableObjects/Items");
            foreach (Item itemData in allItems)
            {
                if (itemData.itemName != "HEAVY AMMO") continue;
                GameObject shopItem = Instantiate(shopItemPrefab, buyPanel.transform.Find("Weapon Slots"));
                shopItem.GetComponent<ShopItem>().Initialise(itemData, "buy");
            }
        } else
        {
            Item[] allItems = Resources.LoadAll<Item>("ScriptableObjects/Items");
            Array.Sort(allItems, (a, b) => a.cost - b.cost);
            foreach (Item itemData in allItems)
            {
                if (itemData.inShop == "NO") continue;
                GameObject shopItem = Instantiate(shopItemPrefab, buyPanel.transform.Find("Weapon Slots"));
                shopItem.GetComponent<ShopItem>().Initialise(itemData, "buy");
            }

            Weapon[] allWeapons = Resources.LoadAll<Weapon>("ScriptableObjects/Weapons");
            Array.Sort(allWeapons, (a, b) => a.cost - b.cost);
            foreach (Weapon weaponData in allWeapons)
            {
                if (weaponData.inShop == "NO") continue;
                GameObject shopItem = Instantiate(shopItemPrefab, buyPanel.transform.Find("Weapon Slots"));
                shopItem.GetComponent<ShopItem>().Initialise(weaponData, "buy");
            }
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

        if(shopButton)
        {
            shopButton.transform.position = PlayerScript.instance.transform.position + new Vector3(0, 0.5f);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PlayerScript.instance.isInShop = true;
            shopButton = Instantiate(shopButtonPrefab, PlayerScript.instance.transform.position + new Vector3(0, 0.5f), Quaternion.identity);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PlayerScript.instance.isInShop = false;
            PlayerScript.instance.CanSeeShop = false;
            Destroy(shopButton);
            shopButton = null;
        }
    }

    public void DisplayBuyPanel()
    {
        sellPanel.SetActive(false);
        shopPanel.transform.Find("Sell Button").gameObject.GetComponent<Image>().sprite = Resources.Load<Sprite>("UISprites/Button BG");
        shopPanel.transform.Find("Buy Button").Find("Text (TMP)").gameObject.GetComponent<TMP_Text>().color = Color.black;
        shopPanel.transform.Find("Sell Button").Find("Text (TMP)").gameObject.GetComponent<TMP_Text>().color = Color.white;
        shopPanel.transform.Find("Buy Button").gameObject.GetComponent<Image>().sprite = Resources.Load<Sprite>("UISprites/Button Shop Selected");
        shopPanel.transform.Find("Item Detail Panel").gameObject.SetActive(false);
        buyPanel.SetActive(true);
    }

    public void DisplaySellPanel()
    {
        buyPanel.SetActive(false);
        shopPanel.transform.Find("Item Detail Panel").gameObject.SetActive(false);
        if (GameController.instance.currentState != GameController.GAME_STATE.TUTORIAL)
        {
            UpdateSellInventory();
        }
        shopPanel.transform.Find("Sell Button").gameObject.GetComponent<Image>().sprite = Resources.Load<Sprite>("UISprites/Button Shop Selected");
        shopPanel.transform.Find("Sell Button").Find("Text (TMP)").gameObject.GetComponent<TMP_Text>().color = Color.black;
        shopPanel.transform.Find("Buy Button").Find("Text (TMP)").gameObject.GetComponent<TMP_Text>().color = Color.white;
        shopPanel.transform.Find("Buy Button").gameObject.GetComponent<Image>().sprite = Resources.Load<Sprite>("UISprites/Button BG");
        sellPanel.SetActive(true);
    }

    public void UpdateSellInventory()
    {
        foreach (Transform child in sellPanel.transform.Find("Weapon Slots"))
        {
            Destroy(child.gameObject);
        }

        foreach (KeyValuePair<ScriptableObject, int> item in PlayerScript.instance.inventory)
        {
            GameObject shopInventoryItem = Instantiate(shopItemPrefab, sellPanel.transform.Find("Weapon Slots"));
            shopInventoryItem.GetComponent<ShopItem>().Initialise(item.Key, "sell");
        }
    }
}
