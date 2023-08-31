using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShopWeapon : MonoBehaviour
{
    [SerializeField] private TMP_Text Name;
    private Weapon weaponData;
    public string type;

    public void Initialise(Weapon weaponData, string type)
    {
        this.weaponData = weaponData;
        this.Name.text = weaponData.weaponName;
        this.type = type;
        gameObject.transform.GetChild(0).GetComponent<Image>().sprite = Resources.Load<Sprite>(weaponData.thumbnailPath);
        if (this.type == "buy") gameObject.transform.GetComponent<Button>().onClick.AddListener(() => Buy());
        if (this.type == "sell") gameObject.transform.GetComponent<Button>().onClick.AddListener(() => Sell());
    }

    public void Buy()
    {
        if (PlayerScript.instance.cash < this.weaponData.cost) return;
        if (PlayerScript.instance.inventory.Count == PlayerScript.instance.inventory.Capacity) return;

        PlayerScript.instance.cash -= this.weaponData.cost;
        PlayerScript.instance.AddWeapon(this.weaponData);
    }

    public void Sell()
    {
        PlayerScript.instance.cash += this.weaponData.cost;
        PlayerScript.instance.RemoveFromInventory(this.weaponData);
        Destroy(gameObject);
    }

}
