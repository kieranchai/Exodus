using UnityEngine;
using System;
public class LevelController : MonoBehaviour
{
    public static LevelController instance { get; private set; }
    public float xpNeeded;
    public float healthBuff;
    public Levels[] allLevels;
    public bool isMaxLvl;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
    }

    private void Start()
    {
        allLevels = Resources.LoadAll<Levels>("ScriptableObjects/Levels");
        Array.Sort(allLevels, (a, b) => a.levelId - b.levelId);
        this.xpNeeded = allLevels[0].xpNeeded;
        this.healthBuff = allLevels[0].healthBuff;
        isMaxLvl = false;
    }

    public void UpdateLevelsModifier()
    {
        for (int i = 0; i < allLevels.Length; i++)
        {
            if (PlayerScript.instance.level == allLevels[i].levelId)
            {
                this.xpNeeded = allLevels[i].xpNeeded;
                this.healthBuff = allLevels[i].healthBuff;
            } else if (PlayerScript.instance.level == allLevels[^1].levelId) {
                this.xpNeeded = allLevels[i].xpNeeded;
                this.healthBuff = allLevels[i].healthBuff;
                isMaxLvl = true;
            }
        }
    }
}
