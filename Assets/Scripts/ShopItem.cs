using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShopItem : MonoBehaviour
{
    [SerializeField] private TMP_Text Name;
    [SerializeField] private TMP_Text Cost;

    private string itemName;
    private string description;
    private string thumbnailPath;
    private int cost;
    private string type;

    private string itemType;
    private string primaryValue;
    private string secondaryValue;

    private float attackPower;
    private float cooldown;
    private float range;

    private Weapon weaponData;
    private Item itemData;
    private GameObject itemDetailPanel;

    public void Initialise(ScriptableObject itemData, string type)
    {
        if (itemData.GetType().IsAssignableFrom(typeof(Weapon)))
        {
            this.weaponData = (Weapon)itemData;

            this.Name.text = this.weaponData.weaponName;
            this.Cost.text = $"${this.weaponData.cost}";

            gameObject.transform.GetChild(0).GetComponent<Image>().sprite = Resources.Load<Sprite>(this.weaponData.thumbnailPath);

            this.itemName = this.weaponData.weaponName;
            this.description = this.weaponData.description;
            this.thumbnailPath = this.weaponData.thumbnailPath;
            this.cost = this.weaponData.cost;

            this.attackPower = this.weaponData.attackPower;
            this.cooldown = this.weaponData.cooldown;
            this.range = this.weaponData.weaponRange;
            this.itemType = null;

            this.type = type;

            this.itemDetailPanel = gameObject.transform.parent.parent.parent.Find("Item Detail Panel").gameObject;
            gameObject.transform.GetComponent<Button>().onClick.AddListener(() => Select(this.type, true));
        }

        if (itemData.GetType().IsAssignableFrom(typeof(Item)))
        {
            this.itemData = (Item)itemData;
            this.Name.text = this.itemData.itemName;
            this.Cost.text = $"${this.itemData.cost}";
            gameObject.transform.GetChild(0).GetComponent<Image>().sprite = Resources.Load<Sprite>(this.itemData.thumbnailPath);

            this.itemName = this.itemData.itemName;
            this.itemType = this.itemData.type;
            this.primaryValue = this.itemData.primaryValue;
            this.secondaryValue = this.itemData.secondaryValue;
            this.cost = this.itemData.cost;
            this.thumbnailPath = this.itemData.thumbnailPath;
            this.description = FormatDescription(this.itemData.description);

            this.type = type;

            this.itemDetailPanel = gameObject.transform.parent.parent.parent.Find("Item Detail Panel").gameObject;
            gameObject.transform.GetComponent<Button>().onClick.AddListener(() => Select(this.type, false));
        }
        if (this.type == "sell") this.Cost.text = PlayerScript.instance.inventory[itemData].ToString();
    }

    public string FormatDescription(string description)
    {
        string formattedDesc = description.Replace("[type]", this.itemType);
        formattedDesc = formattedDesc.Replace("[primaryValue]", this.primaryValue);
        formattedDesc = formattedDesc.Replace("[secondaryValue]", this.secondaryValue);

        return formattedDesc;
    }

    public void Select(string type, bool isWeapon)
    {
        this.itemDetailPanel.transform.Find("Action Button").GetComponent<Button>().onClick.RemoveAllListeners();
        this.itemDetailPanel.transform.Find("Item Name").GetComponent<TMP_Text>().text = this.itemName;
        this.itemDetailPanel.transform.Find("Item Description").GetComponent<TMP_Text>().text = this.description;
        this.itemDetailPanel.transform.Find("Item Cost").GetComponent<TMP_Text>().text = $"${this.cost}";
        this.itemDetailPanel.transform.Find("Item Thumbnail").GetComponent<Image>().sprite = Resources.Load<Sprite>(this.thumbnailPath);

        if (type == "buy")
        {
            this.itemDetailPanel.transform.Find("Action Button").GetChild(0).GetComponent<TMP_Text>().text = "PURCHASE";
            this.itemDetailPanel.transform.Find("Action Button").GetComponent<Button>().onClick.AddListener(() => Buy(this.itemType));

            if (this.weaponData && PlayerScript.instance.inventory.ContainsKey(this.weaponData) || PlayerScript.instance.equippedWeapon && PlayerScript.instance.equippedWeapon == this.weaponData)
            {
                this.itemDetailPanel.transform.Find("Action Button").GetChild(0).GetComponent<TMP_Text>().text = "OWNED";
                this.itemDetailPanel.transform.Find("Action Button").GetComponent<Button>().onClick.RemoveAllListeners();
            }
        }
        else
        {
            this.itemDetailPanel.transform.Find("Action Button").GetChild(0).GetComponent<TMP_Text>().text = "SELL";
            this.itemDetailPanel.transform.Find("Item Cost").GetComponent<TMP_Text>().text = $"${this.cost / 2}";
            this.itemDetailPanel.transform.Find("Action Button").GetComponent<Button>().onClick.AddListener(() => Sell(this.itemType));
        }

        foreach (Transform child in this.itemDetailPanel.transform)
        {
            if (child.name.Contains("Weapon")) child.gameObject.SetActive(false);
        }

        if (isWeapon)
        {
            this.itemDetailPanel.transform.Find("Weapon AP").GetComponent<TMP_Text>().text = "<color=#fff000>" + this.attackPower + "dmg</color>";
            this.itemDetailPanel.transform.Find("Weapon CD").GetComponent<TMP_Text>().text = "<color=#fff000>" + this.cooldown + "/s</color>";
            this.itemDetailPanel.transform.Find("Weapon Range").GetComponent<TMP_Text>().text = "<color=#fff000>" + this.range + "m</color>";

            foreach (Transform child in this.itemDetailPanel.transform)
            {
                if (child.name.Contains("Weapon")) child.gameObject.SetActive(true);
            }
        }

        this.itemDetailPanel.SetActive(true);
    }

    public void Buy(string itemType)
    {
        if (PlayerScript.instance.cash < this.cost) return;

        switch (itemType)
        {
            case "SMALL HEALTH":
            case "BIG HEALTH":
            case "MOVEMENT SPEED":
            case "GAS":
                PlayerScript.instance.AddToInventory(this.itemData);
                break;
            default:
                if (PlayerScript.instance.inventory.ContainsKey(this.weaponData) || PlayerScript.instance.equippedWeapon == this.weaponData) return;
                PlayerScript.instance.AddToInventory(this.weaponData);
                this.itemDetailPanel.transform.Find("Action Button").GetChild(0).GetComponent<TMP_Text>().text = "OWNED";
                this.itemDetailPanel.transform.Find("Action Button").GetComponent<Button>().onClick.RemoveAllListeners();
                break;
        }
        PlayerScript.instance.UpdateCash(-this.cost);
        ShopController.instance.audioSource.clip = Resources.Load<AudioClip>($"Audio/Cash");
        ShopController.instance.audioSource.Play();
    }

    public void Sell(string itemType)
    {
        ScriptableObject _data;

        if (itemType == null)
        {
            this.weaponData.currentAmmoCount = this.weaponData.clipSize;
            _data = this.weaponData;
            PlayerScript.instance.UpdateCash(this.weaponData.cost / 2);
        }
        else
        {
            _data = this.itemData;
            PlayerScript.instance.UpdateCash(this.itemData.cost / 2);
        }
        ShopController.instance.audioSource.clip = Resources.Load<AudioClip>($"Audio/Cash");
        ShopController.instance.audioSource.Play();
        PlayerScript.instance.RemoveFromInventory(_data);
        if (!PlayerScript.instance.inventory.ContainsKey(_data))
        {
            this.itemDetailPanel.SetActive(false);
            Destroy(gameObject);
            return;
        }
        this.Cost.text = PlayerScript.instance.inventory[_data].ToString();
    }
}
