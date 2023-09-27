using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShopItem : MonoBehaviour
{
    [SerializeField] private TMP_Text Name;
    [SerializeField] private TMP_Text Cost;

    private string weaponName;
    private string description;
    private string thumbnailPath;
    private int cost;
    private string type;

    private float attackPower;
    private float cooldown;
    private float range;

    private Weapon weaponData;
    private GameObject itemDetailPanel;

    public void Initialise(Weapon weaponData, string type)
    {
        this.weaponData = weaponData;

        this.Name.text = this.weaponData.weaponName;
        this.Cost.text = $"${this.weaponData.cost}";

        gameObject.transform.GetChild(0).GetComponent<Image>().sprite = Resources.Load<Sprite>(this.weaponData.thumbnailPath);

        this.weaponName = this.weaponData.weaponName;
        this.description = this.weaponData.description;
        this.thumbnailPath = this.weaponData.thumbnailPath;
        this.cost = this.weaponData.cost;

        this.attackPower = this.weaponData.attackPower;
        this.cooldown = this.weaponData.cooldown;
        this.range = this.weaponData.weaponRange;

        this.type = type;

        this.itemDetailPanel = gameObject.transform.parent.parent.parent.Find("Item Detail Panel").gameObject;
        gameObject.transform.GetComponent<Button>().onClick.AddListener(() => Select(this.type));

        if (this.type == "sell") this.Cost.text = $"${this.cost / 2}";
    }

    public void Select(string type)
    {
        this.itemDetailPanel.transform.Find("Action Button").GetComponent<Button>().onClick.RemoveAllListeners();
        this.itemDetailPanel.transform.Find("Item Name").GetComponent<TMP_Text>().text = this.weaponName;
        this.itemDetailPanel.transform.Find("Item Description").GetComponent<TMP_Text>().text = this.description;
        this.itemDetailPanel.transform.Find("Item Cost").GetComponent<TMP_Text>().text = $"${this.cost}";
        this.itemDetailPanel.transform.Find("Item Thumbnail").GetComponent<Image>().sprite = Resources.Load<Sprite>(this.thumbnailPath);

        if (type == "buy")
        {
            this.itemDetailPanel.transform.Find("Action Button").GetChild(0).GetComponent<TMP_Text>().text = "PURCHASE";
            this.itemDetailPanel.transform.Find("Action Button").GetComponent<Button>().onClick.AddListener(() => Buy());

            if (PlayerScript.instance.inventory.Contains(this.weaponData) || PlayerScript.instance.equippedWeapon && PlayerScript.instance.equippedWeapon == this.weaponData)
            {
                this.itemDetailPanel.transform.Find("Action Button").GetChild(0).GetComponent<TMP_Text>().text = "OWNED";
                this.itemDetailPanel.transform.Find("Action Button").GetComponent<Button>().onClick.RemoveAllListeners();
            }
        }
        else
        {
            this.itemDetailPanel.transform.Find("Action Button").GetChild(0).GetComponent<TMP_Text>().text = "SELL";
            this.itemDetailPanel.transform.Find("Item Cost").GetComponent<TMP_Text>().text = $"${this.cost / 2}";
            this.itemDetailPanel.transform.Find("Action Button").GetComponent<Button>().onClick.AddListener(() => Sell());
        }

        this.itemDetailPanel.transform.Find("Weapon AP").GetComponent<TMP_Text>().text = "<color=#fff000>" + this.attackPower + "dmg</color>";
        this.itemDetailPanel.transform.Find("Weapon CD").GetComponent<TMP_Text>().text = "<color=#fff000>" + this.cooldown + "/s</color>";
        this.itemDetailPanel.transform.Find("Weapon Range").GetComponent<TMP_Text>().text = "<color=#fff000>" + this.range + "m</color>";

        this.itemDetailPanel.SetActive(true);
    }

    public void Buy()
    {
        if (PlayerScript.instance.cash < this.cost) return;
        if (PlayerScript.instance.inventory.Contains(this.weaponData) || PlayerScript.instance.equippedWeapon == this.weaponData) return;

        PlayerScript.instance.AddToInventory(this.weaponData);
        this.itemDetailPanel.transform.Find("Action Button").GetChild(0).GetComponent<TMP_Text>().text = "OWNED";
        this.itemDetailPanel.transform.Find("Action Button").GetComponent<Button>().onClick.RemoveAllListeners();
        PlayerScript.instance.UpdateCash(-this.cost);
        ShopController.instance.audioSource.clip = Resources.Load<AudioClip>($"Audio/Cash");
        ShopController.instance.audioSource.Play();
    }

    public void Sell()
    {
        Weapon _data;

        this.weaponData.currentAmmoCount = this.weaponData.clipSize;
        _data = this.weaponData;
        PlayerScript.instance.UpdateCash(this.weaponData.cost / 2);
        ShopController.instance.audioSource.clip = Resources.Load<AudioClip>($"Audio/Cash");
        ShopController.instance.audioSource.Play();
        PlayerScript.instance.RemoveFromInventory(_data);
        if (!PlayerScript.instance.inventory.Contains(_data))
        {
            this.itemDetailPanel.SetActive(false);
            Destroy(gameObject);
            return;
        }
    }
}
