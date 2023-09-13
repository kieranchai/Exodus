using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventoryItem : MonoBehaviour
{
    [SerializeField] private TMP_Text Name;
    [SerializeField] private TMP_Text Count;

    private string itemName;
    private string description;
    private string thumbnailPath;

    private string itemType;
    private string primaryValue;
    private string secondaryValue;

    private float attackPower;
    private float cooldown;
    private float range;
    private float weight;
    private string ammoType;

    private Weapon weaponData;
    private Item itemData;
    private GameObject itemDetailPanel;

    private Color defaultColor;

    public void Initialise(ScriptableObject itemData)
    {
        if (itemData.GetType().IsAssignableFrom(typeof(Weapon)))
        {
            this.weaponData = (Weapon)itemData;

            this.Name.text = this.weaponData.weaponName;
            this.Count.text = PlayerScript.instance.inventory[itemData].ToString();
            gameObject.transform.GetChild(0).GetComponent<Image>().sprite = Resources.Load<Sprite>(weaponData.thumbnailPath);

            this.itemName = this.weaponData.weaponName;
            if (PlayerScript.instance.equippedWeapon == this.weaponData) this.Name.text = $"[EQUIPPED] {this.weaponData.weaponName}";
            this.description = this.weaponData.description;
            this.thumbnailPath = this.weaponData.thumbnailPath;

            this.attackPower = this.weaponData.attackPower;
            this.cooldown = this.weaponData.cooldown;
            this.range = this.weaponData.weaponRange;
            this.weight = this.weaponData.weight;
            this.ammoType = this.weaponData.ammoType;
            this.itemType = null;

            this.itemDetailPanel = gameObject.transform.parent.parent.parent.Find("Item Detail Panel").gameObject;
            gameObject.transform.GetComponent<Button>().onClick.AddListener(() => Select(true));

            if (PlayerScript.instance.equippedWeapon == this.weaponData)
            {
                this.itemDetailPanel.transform.Find("Item Name").GetComponent<TMP_Text>().text = this.itemName + " (EQUIPPED)";
                transform.GetComponent<Image>().color = Color.green;
            }
        }

        if (itemData.GetType().IsAssignableFrom(typeof(Item)))
        {
            this.itemData = (Item)itemData;
            this.Name.text = this.itemData.itemName;
            this.Count.text = PlayerScript.instance.inventory[itemData].ToString();
            gameObject.transform.GetChild(0).GetComponent<Image>().sprite = Resources.Load<Sprite>(this.itemData.thumbnailPath);


            this.itemName = this.itemData.itemName;
            this.itemType = this.itemData.type;
            this.primaryValue = this.itemData.primaryValue;
            this.secondaryValue = this.itemData.secondaryValue;
            this.thumbnailPath = this.itemData.thumbnailPath;
            this.description = FormatDescription(this.itemData.description);

            this.itemDetailPanel = gameObject.transform.parent.parent.parent.Find("Item Detail Panel").gameObject;
            gameObject.transform.GetComponent<Button>().onClick.AddListener(() => Select(false));
        }
    }

    public string FormatDescription(string description)
    {
        string formattedDesc = description.Replace("[type]", this.itemType);
        formattedDesc = formattedDesc.Replace("[primaryValue]", this.primaryValue);
        formattedDesc = formattedDesc.Replace("[secondaryValue]", this.secondaryValue);

        return formattedDesc;
    }

    public void Select(bool isWeapon)
    {
        this.itemDetailPanel.transform.Find("Action Button").GetComponent<Button>().onClick.RemoveAllListeners();
        this.itemDetailPanel.transform.Find("Item Name").GetComponent<TMP_Text>().text = this.itemName;
        this.itemDetailPanel.transform.Find("Item Description").GetComponent<TMP_Text>().text = this.description;
        this.itemDetailPanel.transform.Find("Item Thumbnail").GetComponent<Image>().sprite = Resources.Load<Sprite>(this.thumbnailPath);

        foreach (Transform child in this.itemDetailPanel.transform)
        {
            if (child.name.Contains("Weapon")) child.gameObject.SetActive(false);
        }

        if (isWeapon)
        {

            if (PlayerScript.instance.equippedWeapon == this.weaponData)
            {
                this.itemDetailPanel.transform.Find("Action Button").GetChild(0).GetComponent<TMP_Text>().text = "UNEQUIP";
                this.itemDetailPanel.transform.Find("Action Button").GetComponent<Button>().onClick.AddListener(() => UseWeapon());
            }
            else
            {
                this.itemDetailPanel.transform.Find("Action Button").GetChild(0).GetComponent<TMP_Text>().text = "EQUIP";
                this.itemDetailPanel.transform.Find("Action Button").GetComponent<Button>().onClick.AddListener(() => UseWeapon());
            }

            this.itemDetailPanel.transform.Find("Weapon AP").GetComponent<TMP_Text>().text = "<color=#fff000>" + this.attackPower + "dmg</color>";
            this.itemDetailPanel.transform.Find("Weapon CD").GetComponent<TMP_Text>().text = "<color=#fff000>" + this.cooldown + "/s</color>";
            this.itemDetailPanel.transform.Find("Weapon Range").GetComponent<TMP_Text>().text = "<color=#fff000>" + this.range + "m</color>";
            this.itemDetailPanel.transform.Find("Weapon Weight").GetComponent<TMP_Text>().text = "<color=#fff000>" + this.weight + "kg</color>";
            this.itemDetailPanel.transform.Find("Weapon Ammo Type").GetComponent<TMP_Text>().text = this.ammoType.ToString();
            if (this.ammoType == "NULL") this.itemDetailPanel.transform.Find("Weapon Ammo Type").GetComponent<TMP_Text>().text = "MELEE";
            foreach (Transform child in this.itemDetailPanel.transform)
            {
                if (child.name.Contains("Weapon")) child.gameObject.SetActive(true);
            }
        }
        else
        {
            this.itemDetailPanel.transform.Find("Action Button").GetChild(0).GetComponent<TMP_Text>().text = "USE";
            this.itemDetailPanel.transform.Find("Action Button").GetComponent<Button>().onClick.AddListener(() => Use());
        }

        this.itemDetailPanel.SetActive(true);
    }

    public void Use()
    {
        if (GameController.instance.UseItem(this.itemData))
        {
            UpdateItem();
        }
    }

    private void UpdateItem()
    {
        PlayerScript.instance.RemoveFromInventory(this.itemData);
        if (!PlayerScript.instance.inventory.ContainsKey(this.itemData))
        {
            this.itemDetailPanel.SetActive(false);
            Destroy(gameObject);
            return;
        }
        this.Count.text = PlayerScript.instance.inventory[itemData].ToString();
    }

    public void UseWeapon()
    {
        if (PlayerScript.instance.equippedWeapon == this.weaponData)
        {
            PlayerScript.instance.UnequipWeapon();
            this.itemDetailPanel.transform.Find("Action Button").GetChild(0).GetComponent<TMP_Text>().text = "EQUIP";
        }
        else
        {
            PlayerScript.instance.EquipWeapon(this.weaponData);
            this.itemDetailPanel.transform.Find("Action Button").GetChild(0).GetComponent<TMP_Text>().text = "UNEQUIP";
        }

        foreach (Transform child in transform.parent)
        {
            if (child.GetComponent<InventoryItem>().weaponData)
            {
                if (child.GetComponent<InventoryItem>().weaponData == PlayerScript.instance.equippedWeapon)
                {
                    child.GetComponent<InventoryItem>().Name.text = $"[EQUIPPED] {child.GetComponent<InventoryItem>().weaponData.weaponName}";
                    child.GetComponent<Image>().color = Color.green;
                }
                else
                {
                    child.GetComponent<InventoryItem>().Name.text = child.GetComponent<InventoryItem>().weaponData.weaponName;
                    child.GetComponent<Image>().color = defaultColor;
                }
            }
        }
    }
}
