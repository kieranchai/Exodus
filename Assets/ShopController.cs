using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopController : MonoBehaviour
{
    [SerializeField]
    private GameObject shopWeaponPrefab;

    [SerializeField]
    private GameObject shopPanel;

    private void Start()
    {
        Weapon[] allWeapons = Resources.LoadAll<Weapon>("ScriptableObjects/Weapons");
        foreach(Weapon weaponData in allWeapons)
        {
            GameObject shopWeapon = Instantiate(shopWeaponPrefab, shopPanel.transform);
            shopWeapon.GetComponent<ShopWeapon>().Initialise(weaponData);
        }
    }

    private void Update()
    {
        if(PlayerScript.instance.CanSeeShop) shopPanel.SetActive(true); else shopPanel.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            PlayerScript.instance.isInShop = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PlayerScript.instance.isInShop = false;
            PlayerScript.instance.CanSeeShop = false;
        }
    }

}
