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

    [SerializeField]
    private Sprite unEquipped;
    [SerializeField]
    private Sprite equipped;

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
        transform.GetComponent<Image>().sprite = unEquipped;
        this.Name.text = this.weaponData.weaponName;
        this.Name.color = new Color32(1, 253, 189, 255);
        this.itemDetailPanel.transform.Find("Action Button").GetChild(0).GetComponent<TMP_Text>().text = "EQUIP";
        if (PlayerScript.instance.equippedWeapon == this.weaponData)
        {
            transform.GetComponent<Image>().sprite = equipped;
            this.Name.text = $"[EQUIPPED] {this.weaponData.weaponName}";
            this.Name.color = Color.black;
            this.itemDetailPanel.transform.Find("Action Button").GetChild(0).GetComponent<TMP_Text>().text = "UNEQUIP";
        }
    }

    public void Select()
    {
        this.itemDetailPanel.transform.Find("Action Button").GetComponent<Button>().onClick.RemoveAllListeners();
        this.itemDetailPanel.transform.Find("Item Name").GetComponent<TMP_Text>().text = this.weaponName;
        this.itemDetailPanel.transform.Find("Item Description").GetComponent<TMP_Text>().text = this.description;
        this.itemDetailPanel.transform.Find("Item Thumbnail").GetComponent<Image>().sprite = Resources.Load<Sprite>(this.thumbnailPath);

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

        this.itemDetailPanel.transform.Find("Weapon AP").GetComponent<TMP_Text>().text =  this.attackPower + "dmg";
        this.itemDetailPanel.transform.Find("Weapon CD").GetComponent<TMP_Text>().text = this.cooldown + "/s";
        this.itemDetailPanel.transform.Find("Weapon Range").GetComponent<TMP_Text>().text = this.range + "m";

        foreach (Transform child in this.itemDetailPanel.transform)
        {
            child.gameObject.SetActive(true);
        }
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
    }
}
