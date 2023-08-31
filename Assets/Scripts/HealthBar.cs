using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public Slider slider;
    public void UpdateHealthBar()
    {
        slider.value = PlayerScript.instance.currentHealth;
        slider.maxValue = PlayerScript.instance.maxHealth;
        slider.value = PlayerScript.instance.currentHealth;
    }
}