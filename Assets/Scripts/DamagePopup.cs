using TMPro;
using UnityEngine;

public class DamagePopup : MonoBehaviour
{
    private TextMeshPro textMesh;
    private float disappearTimer = 0.3f;
    private Color textColor;

    private int currentVal;
    private bool existingCashPopup = false;

    private void Awake()
    {
        textMesh = GetComponent<TextMeshPro>();
    }

    public void Setup(float damageAmount)
    {
        textMesh.SetText(((int)damageAmount).ToString());
        textColor = textMesh.color;
    }

    public void SetupPlayerDamage(float damageAmount)
    {
        textMesh.color = Color.red;
        textMesh.SetText("-" + (int)damageAmount);
    }

    public void SetupXP(int xpAmount)
    {
        textMesh.SetText($"+{xpAmount}XP");
        textColor = textMesh.color;
    }

    public void SetupCash(int cashAmount)
    {
        textColor = textMesh.color;
        currentVal = cashAmount;
        textMesh.SetText($"+${currentVal}");
        existingCashPopup = true;
    }

    public void UpdateCash(int cashAmount)
    {
        textMesh.fontSize *= 1.02f;
        textColor = textMesh.color;
        transform.position = PlayerScript.instance.transform.position;
        currentVal += cashAmount;
        textMesh.SetText($"+${currentVal}");
        disappearTimer = 0.5f;
        textColor.a = 1;
        textMesh.color = textColor;
    }

    public void SetupLevelUp()
    {
        textMesh.SetText("<color=#F3F316>Lv. up!</color>");
    }

    public void SetupHeal(float value)
    {
        textMesh.SetText("+" + (int)value);
        textColor = textMesh.color;
    }

    public void Update()
    {
        float moveYSpeed = 1f;
        transform.position += new Vector3(0, moveYSpeed) * Time.deltaTime;

        disappearTimer -= Time.deltaTime;
        if (disappearTimer < 0)
        {
            float disappearSpeed = 2f;
            textColor.a -= disappearSpeed * Time.deltaTime;
            textMesh.color = textColor;
            if (textColor.a < 0)
            {
                if (existingCashPopup) PlayerScript.instance.currentCashPopup = null;
                Destroy(gameObject);
            }
        }
    }
}
