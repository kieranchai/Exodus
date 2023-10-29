using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeUI : MonoBehaviour
{
    [SerializeField]
    private Image thumbnail;

    [SerializeField]
    private TMP_Text count;

    [SerializeField]
    private GameObject popup;

    private Buff buff;

    private string currentValue;

    public void Initialise(Buff buff)
    {
        this.buff = buff;
        thumbnail.sprite = Resources.Load<Sprite>($"{buff.thumbnailPath}");
        count.text = PlayerScript.instance.buffList[buff].ToString();
    }

    public void DisplayBuffDetail()
    {
        popup.transform.Find("Name").GetComponent<TMP_Text>().text = this.buff.buffName;
        string desc = this.buff.description;
        desc = desc.Replace("[secValue]", this.buff.secValue);
        desc = desc.Replace("[value]", FormatValue(this.buff));
        desc = desc.Replace("+", "<color=#d62000>+</color>");
        desc = desc.Replace("-", "<color=#d62000>-</color>");
        popup.transform.Find("Desc").GetComponent<TMP_Text>().text = desc;
        popup.SetActive(true);
    }

    private string FormatValue(Buff buff)
    {
        switch (buff.category)
        {
            //Player Buffs
            case "player":
                switch (buff.type)
                {
                    case "maxHealth":
                        currentValue = ((PlayerScript.instance.maxHealthMultiplier - 1) * 100).ToString("0.##") + "%";
                        break;
                    case "movementSpeed":
                        currentValue = ((PlayerScript.instance.moveSpeedMultiplier - 1) * 100).ToString("0.##") + "%";
                        break;
                    case "evasion":
                        currentValue = ((PlayerScript.instance.evasionMultiplier - 1) * 100).ToString("0.##") + "%";
                        break;
                    case "regeneration":
                        currentValue = PlayerScript.instance.regenValue.ToString();
                        break;
                    case "dodgeCd":
                        currentValue = ((1 - (PlayerScript.instance.rollCD / 3)) * 100).ToString("0.##") + "%";
                        break;
                    default:
                        break;
                }
                break;

            //Gun Buffs
            case "gun":
                switch (buff.type)
                {
                    case "bleed":
                        currentValue = ((PlayerScript.instance.weaponSlot.gunBleedChance - 1) * 100).ToString("0.##") + "%";
                        break;
                    case "lightning":
                        currentValue = ((PlayerScript.instance.weaponSlot.lightningChance - 1) * 100).ToString("0.##") + "%";
                        break;
                    case "reload":
                        currentValue = ((PlayerScript.instance.weaponSlot.instaReloadChance - 1) * 100).ToString("0.##") + "%";
                        break;
                    case "fireRate":
                        currentValue = ((1 - PlayerScript.instance.weaponSlot.gunFireRateMultiplier) * 100).ToString("0.##") + "%";
                        break;
                    case "range":
                        currentValue = ((PlayerScript.instance.weaponSlot.rangeMultiplier - 1) * 100).ToString("0.##") + "%";
                        break;
                    case "pierce":
                        currentValue = ((PlayerScript.instance.weaponSlot.pierceChance - 1) * 100).ToString("0.##") + "%";
                        break;
                    case "crit":
                        currentValue = ((PlayerScript.instance.weaponSlot.gunCritChance - 1) * 100).ToString("0.##") + "%";
                        break;
                    case "lifesteal":
                        currentValue = ((PlayerScript.instance.weaponSlot.gunLifeStealMultiplier - 1) * 100).ToString("0.##") + "%";
                        break;
                    default:
                        break;
                }
                break;
            default:
                break;
        }

        currentValue = $"<color=#d62000>{currentValue}</color>";
        return currentValue;
    }

    public void HideBuffDetail()
    {
        popup.SetActive(false);
    }
}
