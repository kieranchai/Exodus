using UnityEngine;
using UnityEngine.UI;
using System;
using Random = UnityEngine.Random;
using TMPro;
using System.Collections.Generic;

public class BuffController : MonoBehaviour
{
    public static BuffController instance { get; private set; }

    private Buff[] allBuffs;
    private Buff[] availableBuffs = new Buff[3];
    private GameObject[] slots;

    [SerializeField]
    private GameObject buffSelectionPanel;
    private GameObject upgradeTokens;

    private WeaponScript weaponScript;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;

        allBuffs = Resources.LoadAll<Buff>("ScriptableObjects/Buffs");
        slots = new GameObject[buffSelectionPanel.transform.Find("Buffs").childCount];
        for (int i = 0; i < buffSelectionPanel.transform.Find("Buffs").childCount; i++)
        {
            slots[i] = buffSelectionPanel.transform.Find("Buffs").GetChild(i).gameObject;
        }

        upgradeTokens = buffSelectionPanel.transform.Find("Upgrade Tokens").gameObject;
    }

    private void Start()
    {
        UpdatePlayerTokensDisplay();
        RandomiseBuffs();
        weaponScript = PlayerScript.instance.weaponSlot;
    }

    public void UpdatePlayerTokensDisplay()
    {
        upgradeTokens.GetComponent<TMP_Text>().text = $"Upgrade Tokens:{PlayerScript.instance.buffTokens}";
    }

    public void RandomiseBuffs()
    {
        for (int i = 0; i < 3; i++)
        {
            availableBuffs[i] = allBuffs[Random.Range(0, allBuffs.Length)];
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

        //TODO: Calculate and format currentValue and currentSecValue
        return desc;
    }

    private void SelectBuff(Buff buff)
    {
        if (PlayerScript.instance.buffTokens <= 0) return;

        if (PlayerScript.instance.buffList.ContainsKey(buff))
        {
            PlayerScript.instance.buffList[buff]++;
        }
        else
        {
            PlayerScript.instance.buffList.Add(buff, 1);
        }

        --PlayerScript.instance.buffTokens;
        UpdatePlayerBuffs();
        UpdatePlayerTokensDisplay();
        RandomiseBuffs();
    }

    public void Reroll()
    {
        //TODO: Reroll costs money
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
                            weaponScript.meleeExplodeDmg = mExplodeDmg;
                            break;
                        case "lifesteal":
                            //TODO
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
                        case "pickpocket":
                            //TODO
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
                            break;
                        case "burn":
                            break;
                        case "lightning":
                            break;
                        case "reload":
                            break;
                        case "fireRate":
                            break;
                        case "clipSize":
                            break;
                        case "range":
                            break;
                        case "pierce":
                            break;
                        case "crit":
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
