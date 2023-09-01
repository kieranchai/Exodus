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
    private string type;
    private string primaryValue;
    private string secondaryValue;
    private float cost;
    private string thumbnailPath;

    private GameObject itemDetailPanel;

    public void Initialise(Item itemData)
    {
        this.Name.text = itemData.itemName;
        this.Cost.text = $"${itemData.cost}";
        gameObject.transform.GetChild(0).GetComponent<Image>().sprite = Resources.Load<Sprite>(itemData.thumbnailPath);

        this.itemName = itemData.itemName;
        this.type = itemData.type;
        this.primaryValue = itemData.primaryValue;
        this.secondaryValue = itemData.secondaryValue;
        this.cost = itemData.cost;
        this.thumbnailPath = itemData.thumbnailPath;
        this.description = FormatDescription(itemData.description);

        this.itemDetailPanel = gameObject.transform.parent.parent.parent.Find("Item Detail Panel").gameObject;
        gameObject.transform.GetComponent<Button>().onClick.AddListener(() => Select());
    }

    public string FormatDescription(string description)
    {
        string formattedDesc = description.Replace("[type]", this.type);
        formattedDesc = formattedDesc.Replace("[primaryValue]", this.primaryValue);
        formattedDesc = formattedDesc.Replace("[secondaryValue]", this.secondaryValue);

        return formattedDesc;
    }

    public void Select()
    {
        this.itemDetailPanel.transform.Find("Item Name").GetComponent<TMP_Text>().text = this.itemName;
        this.itemDetailPanel.transform.Find("Item Description").GetComponent<TMP_Text>().text = this.description;
        this.itemDetailPanel.transform.Find("Item Cost").GetComponent<TMP_Text>().text = $"${this.cost}";
        this.itemDetailPanel.transform.Find("Item Thumbnail").GetComponent<Image>().sprite = Resources.Load<Sprite>(this.thumbnailPath);
        this.itemDetailPanel.transform.Find("Action Button").GetChild(0).GetComponent<TMP_Text>().text = "PURCHASE";
        this.itemDetailPanel.transform.Find("Action Button").GetComponent<Button>().onClick.RemoveAllListeners();
        this.itemDetailPanel.transform.Find("Action Button").GetComponent<Button>().onClick.AddListener(() => Buy(this.type));

        foreach (Transform child in this.itemDetailPanel.transform)
        {
            if (child.name.Contains("Weapon")) child.gameObject.SetActive(false);
        }

        this.itemDetailPanel.SetActive(true);
    }

    public void Buy(string type)
    {
        if (PlayerScript.instance.cash < this.cost) return;
        PlayerScript.instance.cash -= this.cost;

        switch (type)
        {
            case "LIGHT AMMO":
                PlayerScript.instance.UpdateAmmoCount(int.Parse(this.primaryValue), "LIGHT");
                break;
            case "MEDIUM AMMO":
                PlayerScript.instance.UpdateAmmoCount(int.Parse(this.primaryValue), "MEDIUM");
                break;
            case "HEAVY AMMO":
                PlayerScript.instance.UpdateAmmoCount(int.Parse(this.primaryValue), "HEAVY");
                break;
            case "SMALL HEALTH":
                break;
            case "BIG HEALTH":
                break;
            case "MOVEMENT SPEED":
                break;
            case "GAS":
                break;
            default:
                break;
        }

    }
}
