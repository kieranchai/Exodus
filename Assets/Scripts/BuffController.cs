using UnityEngine;
using UnityEngine.UI;
using System;
using Random = UnityEngine.Random;
using TMPro;

public class BuffController : MonoBehaviour
{
    public static BuffController instance { get; private set; }

    private Buff[] allBuffs;
    private Buff[] availableBuffs = new Buff[3];
    private GameObject[] slots;

    [SerializeField]
    private GameObject buffSelectionPanel;
    private GameObject upgradeTokens;
    private GameObject rerollPriceUI;

    private WeaponScript weaponScript;

    private int rerollPrice = 200;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;

        allBuffs = Resources.LoadAll<Buff>("ScriptableObjects/Buffs");
        slots = new GameObject[buffSelectionPanel.transform.Find("BuffList").Find("Buffs").childCount];
        for (int i = 0; i < buffSelectionPanel.transform.Find("BuffList").Find("Buffs").childCount; i++)
        {
            slots[i] = buffSelectionPanel.transform.Find("BuffList").Find("Buffs").GetChild(i).gameObject;
        }

        upgradeTokens = PlayerScript.instance.playerPanel.transform.Find("Upgrades").Find("Upgrades Image").Find("Buff Tokens").gameObject;
        rerollPriceUI = buffSelectionPanel.transform.Find("Reroll").Find("Reroll Image").Find("Reroll Price").gameObject;
    }

    private void Start()
    {
        weaponScript = PlayerScript.instance.weaponSlot;
        UpdatePlayerTokensDisplay();
        RandomiseBuffs();
        rerollPriceUI.GetComponent<TMP_Text>().text = "$" + this.rerollPrice.ToString();
        upgradeTokens.GetComponent<TMP_Text>().text = $"{PlayerScript.instance.buffTokens}";
    }

    public void UpdatePlayerTokensDisplay()
    {
        upgradeTokens.GetComponent<TMP_Text>().text = $"{PlayerScript.instance.buffTokens}";
    }

    public void RandomiseBuffs()
    {
        for (int i = 0; i < 3; i++)
        {
            availableBuffs[i] = null;

            while (!availableBuffs[i])
            {
                Buff rolledBuff = allBuffs[Random.Range(0, allBuffs.Length)];
                if (Random.Range(0f, 1f) < rolledBuff.rollChance)
                {
                    availableBuffs[i] = allBuffs[Random.Range(0, allBuffs.Length)];
                }
            }
        }

        RefreshAvailableBuffsUI();
    }

    private void RefreshAvailableBuffsUI()
    {
        for (int i = 0; i < slots.Length; i++)
        {
            slots[i].transform.GetComponent<Button>().onClick.RemoveAllListeners();
            int i2 = i;
            slots[i].transform.GetComponent<Button>().onClick.AddListener(() => SelectBuff(availableBuffs[i2]));
            slots[i].transform.Find("Category").GetComponent<TMP_Text>().text = availableBuffs[i].category.ToUpper();
            slots[i].transform.Find("Name").GetComponent<TMP_Text>().text = availableBuffs[i].buffName.ToUpper();

            string formattedDesc = FormatDescription(availableBuffs[i].description, availableBuffs[i]);
            slots[i].transform.Find("Description").GetComponent<TMP_Text>().text = formattedDesc;
        }
    }

    private string FormatDescription(string desc, Buff buff)
    {

        desc = desc.Replace("[value]", buff.value);
        desc = desc.Replace("[secValue]", buff.secValue);

        switch (buff.category)
        {
            //Player Buffs
            case "player":
                switch (buff.type)
                {
                    case "maxHealth":
                        desc = desc.Replace("[currentValue]", ((PlayerScript.instance.maxHealthMultiplier - 1) * 100).ToString("0.##") + "%");
                        break;
                    case "movementSpeed":
                        desc = desc.Replace("[currentValue]", ((PlayerScript.instance.moveSpeedMultiplier - 1) * 100).ToString("0.##") + "%");
                        break;
                    case "evasion":
                        if (PlayerScript.instance.evasionMultiplier == 0) desc = desc.Replace("[currentValue]", PlayerScript.instance.evasionMultiplier + "%");
                        desc = desc.Replace("[currentValue]", ((PlayerScript.instance.evasionMultiplier - 1) * 100).ToString("0.##") + "%");
                        break;
                    case "regeneration":
                        desc = desc.Replace("[currentValue]", PlayerScript.instance.regenValue.ToString());
                        break;
                    case "dodgeCd":
                        desc = desc.Replace("[currentValue]", ((1 - (PlayerScript.instance.rollCD / 3)) * 100).ToString("0.##") + "%");
                        break;
                    default:
                        break;
                }
                break;

            //Melee Buffs
            case "melee":
                switch (buff.type)
                {
                    case "explode":
                        desc = desc.Replace("[currentValue]", weaponScript.meleeExplodeDmg.ToString());
                        break;
                    case "lifesteal":
                        if (weaponScript.meleeLifeStealMultiplier == 0) desc = desc.Replace("[currentValue]", weaponScript.meleeLifeStealMultiplier + "%");
                        desc = desc.Replace("[currentValue]", ((weaponScript.meleeLifeStealMultiplier - 1) * 100).ToString("0.##") + "%");
                        break;
                    case "crit":
                        if (weaponScript.meleeCritChance == 0) desc = desc.Replace("[currentValue]", weaponScript.meleeCritChance + "%");
                        desc = desc.Replace("[currentValue]", ((weaponScript.meleeCritChance - 1) * 100).ToString("0.##") + "%");
                        break;
                    case "special":
                        //TODO
                        break;
                    case "fireRate":
                        desc = desc.Replace("[currentValue]", ((weaponScript.meleeFireRateMultiplier - 1) * 100).ToString("0.##") + "%");
                        break;
                    case "dmgBlock":
                        if (PlayerScript.instance.meleeDmgBlockMultiplier == 0) desc = desc.Replace("[currentValue]", PlayerScript.instance.meleeDmgBlockMultiplier + "%");
                        desc = desc.Replace("[currentValue]", ((PlayerScript.instance.meleeDmgBlockMultiplier - 1) * 100).ToString("0.##") + "%");
                        break;
                    case "bleed":
                        if (weaponScript.meleeBleedChance == 0) desc = desc.Replace("[currentValue]", weaponScript.meleeBleedChance + "%");
                        desc = desc.Replace("[currentValue]", ((weaponScript.meleeBleedChance - 1) * 100).ToString("0.##") + "%");
                        break;
                    case "damage":
                        desc = desc.Replace("[currentValue]", ((weaponScript.meleeDmgMultiplier - 1) * 100).ToString("0.##") + "%");
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
                        if (weaponScript.gunBleedChance == 0) desc = desc.Replace("[currentValue]", weaponScript.gunBleedChance + "%");
                        desc = desc.Replace("[currentValue]", ((weaponScript.gunBleedChance - 1) * 100).ToString("0.##") + "%");
                        break;
                    case "lightning":
                        if (weaponScript.lightningChance == 0) desc = desc.Replace("[currentValue]", weaponScript.lightningChance + "%");
                        desc = desc.Replace("[currentValue]", ((weaponScript.lightningChance - 1) * 100).ToString("0.##") + "%");
                        break;
                    case "reload":
                        if (weaponScript.instaReloadChance == 0) desc = desc.Replace("[currentValue]", weaponScript.instaReloadChance + "%");
                        desc = desc.Replace("[currentValue]", ((weaponScript.instaReloadChance - 1) * 100).ToString("0.##") + "%");
                        break;
                    case "fireRate":
                        desc = desc.Replace("[currentValue]", ((weaponScript.gunFireRateMultiplier - 1) * 100).ToString("0.##") + "%");
                        break;
                    case "range":
                        desc = desc.Replace("[currentValue]", ((weaponScript.rangeMultiplier - 1) * 100).ToString("0.##") + "%");
                        break;
                    case "pierce":
                        if (weaponScript.pierceChance == 0) desc = desc.Replace("[currentValue]", weaponScript.pierceChance + "%");
                        desc = desc.Replace("[currentValue]", ((weaponScript.pierceChance - 1) * 100).ToString("0.##") + "%");
                        break;
                    case "crit":
                        if (weaponScript.gunCritChance == 0) desc = desc.Replace("[currentValue]", weaponScript.gunCritChance + "%");
                        desc = desc.Replace("[currentValue]", ((weaponScript.gunCritChance - 1) * 100).ToString("0.##") + "%");
                        break;
                    default:
                        break;
                }
                break;
            default:
                break;
        }
        return desc;
    }

    private void SelectBuff(Buff buff)
    {
        if (PlayerScript.instance.buffTokens <= 0)
        {
            AudioManager.instance.PlaySFX(AudioManager.instance.actionFailed);
            return;
        };

        if (PlayerScript.instance.buffList.ContainsKey(buff))
        {
            PlayerScript.instance.buffList[buff]++;
        }
        else
        {
            PlayerScript.instance.buffList.Add(buff, 1);
        }

        --PlayerScript.instance.buffTokens;
        AudioManager.instance.PlaySFX(AudioManager.instance.buttonPressed);
        UpdatePlayerBuffs();
        UpdatePlayerTokensDisplay();
        RandomiseBuffs();
    }

    public void Reroll()
    {
        if (rerollPrice > PlayerScript.instance.cash)
        {
            AudioManager.instance.PlaySFX(AudioManager.instance.actionFailed);
            return;
        };

        PlayerScript.instance.UpdateCash(-rerollPrice);
        rerollPrice = (int)(rerollPrice * 1.05f);
        rerollPriceUI.GetComponent<TMP_Text>().text = "$" + this.rerollPrice.ToString();
        AudioManager.instance.PlaySFX(AudioManager.instance.buffReroll);
        RandomiseBuffs();
    }

    private void UpdatePlayerBuffs()
    {
        foreach (var buff in PlayerScript.instance.buffList)
        {
            switch (buff.Key.category)
            {
                //Player Buffs
                case "player":
                    switch (buff.Key.type)
                    {
                        case "maxHealth":
                            float hpVal = 1 + float.Parse(buff.Key.value.Replace("%", "")) / 100; //1.2 = 1+0.2
                            //multiplicative = 1.2 ^ count eg 1.2^3 = 1.774
                            PlayerScript.instance.maxHealthMultiplier = (float)Math.Pow(hpVal, buff.Value);
                            //max hp = max hp * multiplier (default 1)
                            PlayerScript.instance.UpdateMaxHealth();
                            break;
                        case "movementSpeed":
                            float msVal = 1 + float.Parse(buff.Key.value.Replace("%", "")) / 100;
                            PlayerScript.instance.moveSpeedMultiplier = (float)Math.Pow(msVal, buff.Value);
                            break;
                        case "evasion":
                            float evasionVal = 1 + float.Parse(buff.Key.value.Replace("%", "")) / 100;
                            PlayerScript.instance.evasionMultiplier = (float)Math.Pow(evasionVal, buff.Value);
                            break;
                        case "regeneration":
                            PlayerScript.instance.regenValue = int.Parse(buff.Key.value) * buff.Value;
                            break;
                        case "dodgeCd":
                            float dCDVal = 1 - float.Parse(buff.Key.value.Replace("%", "")) / 100; //0.9 = 1 - 0.1
                            //base rollcd = 3
                            //rollcd = 3 x 0.9 ^ count
                            PlayerScript.instance.rollCD = (float)(3 * Math.Pow(dCDVal, buff.Value));
                            break;
                        default:
                            break;
                    }
                    break;

                //Melee Buffs
                case "melee":
                    switch (buff.Key.type)
                    {
                        case "explode":
                            float mExplodeDmg = float.Parse(buff.Key.value);
                            weaponScript.meleeExplodeDmg = mExplodeDmg + (mExplodeDmg / 2 * (buff.Value - 1));
                            break;
                        case "lifesteal":
                            float mLifeSteal = 1 + float.Parse(buff.Key.value.Replace("%", "")) / 100;
                            weaponScript.meleeLifeStealMultiplier = (float)Math.Pow(mLifeSteal, buff.Value);
                            break;
                        case "crit":
                            float mCrit = 1 + float.Parse(buff.Key.value.Replace("%", "")) / 100;
                            float mCritDmg = float.Parse(buff.Key.secValue.Replace("%", "")) / 100;
                            weaponScript.meleeCritDamageMultiplier = mCritDmg;
                            weaponScript.meleeCritChance = (float)Math.Pow(mCrit, buff.Value);
                            break;
                        case "special":
                            //TODO
                            break;
                        case "fireRate":
                            float mFRate = 1 - float.Parse(buff.Key.value.Replace("%", "")) / 100;
                            weaponScript.meleeFireRateMultiplier = (float)Math.Pow(mFRate, buff.Value);
                            break;
                        case "dmgBlock":
                            float mDBlock = 1 + float.Parse(buff.Key.value.Replace("%", "")) / 100;
                            PlayerScript.instance.meleeDmgBlockMultiplier = (float)Math.Pow(mDBlock, buff.Value);
                            break;
                        case "bleed":
                            float mBleed = 1 + float.Parse(buff.Key.value.Replace("%", "")) / 100;
                            weaponScript.meleeBleedChance = (float)Math.Pow(mBleed, buff.Value);
                            weaponScript.meleeBleedDmg = float.Parse(buff.Key.secValue);
                            break;
                        case "damage":
                            float mDVal = 1 + float.Parse(buff.Key.value.Replace("%", "")) / 100;
                            weaponScript.meleeDmgMultiplier = (float)Math.Pow(mDVal, buff.Value);
                            break;
                        default:
                            break;
                    }
                    break;

                //Gun Buffs
                case "gun":
                    switch (buff.Key.type)
                    {
                        case "bleed":
                            float gBleed = 1 + float.Parse(buff.Key.value.Replace("%", "")) / 100;
                            weaponScript.gunBleedChance = (float)Math.Pow(gBleed, buff.Value);
                            weaponScript.gunBleedDmg = float.Parse(buff.Key.secValue);
                            break;
                        case "lightning":
                            float lightningChance = 1 + float.Parse(buff.Key.value.Replace("%", "")) / 100;
                            weaponScript.lightningChance = (float)Math.Pow(lightningChance, buff.Value);
                            weaponScript.lightningDmg = float.Parse(buff.Key.secValue);
                            break;
                        case "reload":
                            float instaReloadVal = 1 + float.Parse(buff.Key.value.Replace("%", "")) / 100;
                            weaponScript.instaReloadChance = (float)Math.Pow(instaReloadVal, buff.Value);
                            break;
                        case "fireRate":
                            float gFRate = 1 - float.Parse(buff.Key.value.Replace("%", "")) / 100;
                            weaponScript.gunFireRateMultiplier = (float)Math.Pow(gFRate, buff.Value);
                            break;
                        case "range":
                            float rangeVal = 1 + float.Parse(buff.Key.value.Replace("%", "")) / 100;
                            weaponScript.rangeMultiplier = (float)Math.Pow(rangeVal, buff.Value);
                            break;
                        case "pierce":
                            float pierceVal = 1 + float.Parse(buff.Key.value.Replace("%", "")) / 100;
                            weaponScript.pierceChance = (float)Math.Pow(pierceVal, buff.Value);
                            break;
                        case "crit":
                            float gCrit = 1 + float.Parse(buff.Key.value.Replace("%", "")) / 100;
                            float gCritDmg = float.Parse(buff.Key.secValue.Replace("%", "")) / 100;
                            weaponScript.gunCritDamageMultiplier = gCritDmg;
                            weaponScript.gunCritChance = (float)Math.Pow(gCrit, buff.Value);
                            break;
                        default:
                            break;
                    }
                    break;
                default:
                    break;
            }
        }
    }
}
