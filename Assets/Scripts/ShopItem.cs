using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShopItem : MonoBehaviour
{
    [SerializeField] private TMP_Text Name;

    private float cost;
    private float duration;
    private float slowSpeed;

    public void Initialise()
    {
        this.Name.text = "SLOW DOWN GAS";
        /*        gameObject.transform.GetChild(0).GetComponent<Image>().sprite = Resources.Load<Sprite>(weaponData.thumbnailPath);*/
        this.cost = 100;
        this.duration = 5f;
        this.slowSpeed = 0.5f;

        gameObject.transform.GetComponent<Button>().onClick.AddListener(() => Buy());
    }

    public void Buy()
    {
        if (PlayerScript.instance.cash < this.cost) return;
        PlayerScript.instance.cash -= this.cost;

        StartCoroutine(UseItemEffect());
    }

    IEnumerator UseItemEffect()
    {
        float initialSpeed = PoisonGas.instance.expandSpeed;
        PoisonGas.instance.expandSpeed = this.slowSpeed;

        yield return new WaitForSeconds(this.duration);
        PoisonGas.instance.expandSpeed = initialSpeed;
    }
}
