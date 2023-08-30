using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventoryWeapon : MonoBehaviour
{
    [SerializeField] private TMP_Text Name;
    private Weapon weaponData;

    public void Initialise(Weapon weaponData)
    {
        this.weaponData = weaponData;
        this.Name.text = weaponData.weaponName;
        gameObject.transform.GetChild(0).GetComponent<Image>().sprite = Resources.Load<Sprite>(weaponData.thumbnailPath);
        gameObject.transform.GetComponent<Button>().onClick.AddListener(() => Swap());
    }

    public void Swap()
    {
        PlayerScript.instance.EquipWeapon(this.weaponData);
        gameObject.transform.GetComponent<Button>().onClick.RemoveAllListeners();
        Destroy(gameObject);
    }


}
