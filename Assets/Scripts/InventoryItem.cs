using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventoryItem : MonoBehaviour
{
    [SerializeField] private TMP_Text Name;

    private string weaponName;
    private string description;
    private string thumbnailPath;

    private float attackPower;
    private float cooldown;
    private float range;

    public Weapon weaponData;
    private GameObject itemDetailPanel;

    public void Initialise(Weapon weaponData)
    {
        this.weaponData = weaponData;

        gameObject.transform.GetChild(0).GetComponent<Image>().sprite = Resources.Load<Sprite>(weaponData.thumbnailPath);
        this.weaponName = this.weaponData.weaponName;
        this.description = this.weaponData.description;
        this.thumbnailPath = this.weaponData.thumbnailPath;

        this.attackPower = this.weaponData.attackPower;
        this.cooldown = this.weaponData.cooldown;
        this.range = this.weaponData.weaponRange;

        this.itemDetailPanel = gameObject.transform.parent.parent.parent.Find("Item Detail Panel").gameObject;
        gameObject.transform.GetComponent<Button>().onClick.AddListener(() => Select());

        Check();
    }

    public void Check()
    {
        transform.GetComponent<Image>().color = Color.white;
        this.Name.text = this.weaponData.weaponName;
        this.itemDetailPanel.transform.Find("Action Button").GetChild(0).GetComponent<TMP_Text>().text = "EQUIP";
        if (PlayerScript.instance.equippedWeapon == this.weaponData)
        {
            transform.GetComponent<Image>().color = Color.green;
            this.Name.text = $"[EQUIPPED] {this.weaponData.weaponName}";
            this.itemDetailPanel.transform.Find("Action Button").GetChild(0).GetComponent<TMP_Text>().text = "UNEQUIP";
        }
    }

    public void Select()
    {
        this.itemDetailPanel.transform.Find("Action Button").GetComponent<Button>().onClick.RemoveAllListeners();
        this.itemDetailPanel.transform.Find("Item Name").GetComponent<TMP_Text>().text = this.weaponName;
        this.itemDetailPanel.transform.Find("Item Description").GetComponent<TMP_Text>().text = this.description;
        this.itemDetailPanel.transform.Find("Item Thumbnail").GetComponent<Image>().sprite = Resources.Load<Sprite>(this.thumbnailPath);

        foreach (Transform child in this.itemDetailPanel.transform)
        {
            if (child.name.Contains("Weapon")) child.gameObject.SetActive(false);
        }

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

        this.itemDetailPanel.SetActive(true);
    }

    public void UseWeapon()
    {
        if (PlayerScript.instance.equippedWeapon == this.weaponData)
        {
            PlayerScript.instance.UnequipWeapon();
            this.itemDetailPanel.transform.Find("Action Button").GetChild(0).GetComponent<TMP_Text>().text = "EQUIP";
            this.itemDetailPanel.transform.Find("Item Name").GetComponent<TMP_Text>().text = this.weaponName;
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
                    child.GetComponent<Image>().color = Color.white;
                }
            }
        }
    }
}
