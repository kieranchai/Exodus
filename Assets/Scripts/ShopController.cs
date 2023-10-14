using System.Collections;
using System.Collections.Generic;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopController : MonoBehaviour
{
    [SerializeField]
    private GameObject shopItemPrefab;

    public GameObject shopPanel;

    private GameObject buyPanel;
    private GameObject sellPanel;

    [SerializeField]
    private GameObject shopButtonPrefab;

    private GameObject shopButton;

    [SerializeField]
    private Weapon[] weaponsList;

    private void Awake()
    {
        buyPanel = shopPanel.transform.Find("Buy Panel").gameObject;
        buyPanel.SetActive(true);
        sellPanel = shopPanel.transform.Find("Sell Panel").gameObject;
        sellPanel.SetActive(false);
    }

    private void Start()
    {
        Array.Sort(weaponsList, (a, b) => a.cost - b.cost);
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

        if (shopButton)
        {
            shopButton.transform.position = PlayerScript.instance.transform.position + new Vector3(0, 0.5f);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (gameObject.transform.parent.GetComponent<EnemySpawner>().zoneUnlocked)
            {
                RefreshShop();
                shopButton = Instantiate(shopButtonPrefab, PlayerScript.instance.transform.position + new Vector3(0, 0.5f), Quaternion.identity);
                PlayerScript.instance.isInShop = true;
                ResetShopPanels();
            }
            else
            {
                Debug.Log("Instantiated Locked shop button");
                //TODO: Display alert cannot access yet
            }
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (gameObject.transform.parent.GetComponent<EnemySpawner>().zoneUnlocked && !shopButton)
            {
                shopButton = Instantiate(shopButtonPrefab, PlayerScript.instance.transform.position + new Vector3(0, 0.5f), Quaternion.identity);
                PlayerScript.instance.isInShop = true;
                RefreshShop();
                ResetShopPanels();
            }
        }
    }

    public void RefreshShop()
    {
        foreach (Transform child in buyPanel.transform.Find("Weapon Slots"))
        {
            Destroy(child.gameObject);
        }

        foreach (Weapon weaponData in weaponsList)
        {
            weaponData.currentAmmoCount = weaponData.clipSize;
            GameObject shopItem = Instantiate(shopItemPrefab, buyPanel.transform.Find("Weapon Slots"));
            shopItem.GetComponent<ShopItem>().Initialise(weaponData, "buy");
        }
    }

    private void ResetShopPanels()
    {
        DisplaySellPanel();
        DisplayBuyPanel();
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (gameObject.transform.parent.GetComponent<EnemySpawner>().zoneUnlocked)
            {
                Destroy(shopButton);
                shopButton = null;
            }
            else
            {
                Debug.Log("Locked shop button destroyed");
            }

            PlayerScript.instance.isInShop = false;
            if (PlayerScript.instance.CanSeeShop) GameController.instance.isOverUI = false;
            PlayerScript.instance.CanSeeShop = false;
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
        UpdateSellInventory();
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

        foreach (Weapon weapon in PlayerScript.instance.inventory)
        {
            GameObject shopInventoryItem = Instantiate(shopItemPrefab, sellPanel.transform.Find("Weapon Slots"));
            shopInventoryItem.GetComponent<ShopItem>().Initialise(weapon, "sell");
        }
    }
}
