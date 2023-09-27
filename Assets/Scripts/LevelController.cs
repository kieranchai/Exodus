using UnityEngine;
using System;
public class LevelController : MonoBehaviour
{
    public static LevelController instance { get; private set; }
    public int xpNeeded;

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
        this.xpNeeded = 0;
        CalculateXpNeeded();
    }

    public void CalculateXpNeeded()
    {
        //xp = 100(1+0.1)^level
        this.xpNeeded = (int)(100 * Math.Pow((1 + 0.1), PlayerScript.instance.level));
    }
}
 