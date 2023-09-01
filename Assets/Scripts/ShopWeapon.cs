using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShopWeapon : MonoBehaviour
{
    [SerializeField] private TMP_Text Name;
    [SerializeField] private TMP_Text Cost;
    private Weapon weaponData;
    public string type;

    private string weaponName;
    private float attackPower;
    private string thumbnailPath;
    private float cooldown;
    private float range;
    private int cost;
    private float weight;
    private string ammoType;

    private GameObject itemDetailPanel;

    public void Initialise(Weapon weaponData, string type)
    {
        this.weaponData = weaponData;
        this.Name.text = weaponData.weaponName;
        this.Cost.text = $"${weaponData.cost}";
        gameObject.transform.GetChild(0).GetComponent<Image>().sprite = Resources.Load<Sprite>(weaponData.thumbnailPath);

        this.weaponName = weaponData.weaponName;
        this.attackPower = weaponData.attackPower;
        this.thumbnailPath = weaponData.thumbnailPath;
        this.cooldown = weaponData.cooldown;
        this.range = weaponData.weaponRange;
        this.cost = weaponData.cost;
        this.weight = weaponData.weight;
        this.ammoType = weaponData.ammoType;

        this.type = type;

        this.itemDetailPanel = gameObject.transform.parent.parent.parent.Find("Item Detail Panel").gameObject;
        gameObject.transform.GetComponent<Button>().onClick.AddListener(() => Select(this.type));
    }

    public void Select(string type)
    {
        this.itemDetailPanel.transform.Find("Item Name").GetComponent<TMP_Text>().text = this.weaponName;
/*        this.itemDetailPanel.transform.Find("Item Description").GetComponent<TMP_Text>().text = this.description;*/
        this.itemDetailPanel.transform.Find("Item Cost").GetComponent<TMP_Text>().text = $"${this.cost}";
        this.itemDetailPanel.transform.Find("Item Thumbnail").GetComponent<Image>().sprite = Resources.Load<Sprite>(this.thumbnailPath);
        this.itemDetailPanel.transform.Find("Action Button").GetComponent<Button>().onClick.RemoveAllListeners();
        if(type == "buy")
        {
            this.itemDetailPanel.transform.Find("Action Button").GetChild(0).GetComponent<TMP_Text>().text = "PURCHASE";
            this.itemDetailPanel.transform.Find("Action Button").GetComponent<Button>().onClick.AddListener(() => Buy());
        } else
        {
            this.itemDetailPanel.transform.Find("Action Button").GetChild(0).GetComponent<TMP_Text>().text = "SELL";
            this.itemDetailPanel.transform.Find("Action Button").GetComponent<Button>().onClick.AddListener(() => Sell());
        }

        this.itemDetailPanel.transform.Find("Weapon AP").GetComponent<TMP_Text>().text = this.attackPower.ToString();
        this.itemDetailPanel.transform.Find("Weapon CD").GetComponent<TMP_Text>().text = this.cooldown.ToString();
        this.itemDetailPanel.transform.Find("Weapon Range").GetComponent<TMP_Text>().text = this.range.ToString();
        this.itemDetailPanel.transform.Find("Weapon Weight").GetComponent<TMP_Text>().text = this.weight.ToString();
        this.itemDetailPanel.transform.Find("Weapon Ammo Type").GetComponent<TMP_Text>().text = this.ammoType;

        foreach (Transform child in this.itemDetailPanel.transform)
        {
            if (child.name.Contains("Weapon")) child.gameObject.SetActive(true);
        }

        this.itemDetailPanel.SetActive(true);
    }

    public void Buy()
    {
        if (PlayerScript.instance.cash < this.weaponData.cost) return;
        if (PlayerScript.instance.inventory.Count == PlayerScript.instance.inventory.Capacity) return;

        PlayerScript.instance.cash -= this.weaponData.cost;
        PlayerScript.instance.AddWeapon(this.weaponData);

        PlayerScript.instance.UpdateAmmoCount(this.weaponData.defaultAmmo, this.weaponData.ammoType);
    }

    public void Sell()
    {
        Cursor.SetCursor(PlayerScript.instance.cursor_Normal, PlayerScript.instance.normalCursorHotspot, CursorMode.Auto);
        PlayerScript.instance.cash += this.weaponData.cost;
        PlayerScript.instance.RemoveFromInventory(this.weaponData);
        this.itemDetailPanel.SetActive(false);
        Destroy(gameObject);
    }

}
