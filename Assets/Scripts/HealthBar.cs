using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [SerializeField]
    private Image healthBar;

    private float lerpSpeed;

    private void Update()
    {
        lerpSpeed = 3f * Time.deltaTime;
        HealthBarFiller();
/*        ColorChanger();*/
    }

    private void HealthBarFiller()
    {
        healthBar.fillAmount = Mathf.Lerp(healthBar.fillAmount, PlayerScript.instance.currentHealth / PlayerScript.instance.maxHealth, lerpSpeed);
    }

/*    private void ColorChanger()
    {
        Color healthColor = Color.Lerp(Color.red, Color.green, PlayerScript.instance.currentHealth / PlayerScript.instance.maxHealth);
        healthBar.color = healthColor;
    }*/
}