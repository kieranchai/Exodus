using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShopWeapon : MonoBehaviour
{
    [SerializeField] private TMP_Text Name;
    private Weapon weaponData;

    public void Initialise(Weapon weaponData)
    {
        this.weaponData = weaponData;
        this.Name.text = weaponData.weaponName;
        gameObject.transform.GetChild(0).GetComponent<Image>().sprite = Resources.Load<Sprite>(weaponData.thumbnailPath);
        gameObject.transform.GetComponent<Button>().onClick.AddListener(() => Buy());
    }

    public void Buy()
    {
        if (PlayerScript.instance.cash < this.weaponData.cost) return;

        gameObject.transform.GetComponent<Button>().onClick.RemoveAllListeners();
        PlayerScript.instance.cash -= this.weaponData.cost;
        PlayerScript.instance.AddWeapon(this.weaponData);
    }

}
