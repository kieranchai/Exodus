using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [SerializeField]
    private Image healthBar;

    private float lerpSpeed;

    private bool isFlashing = false;

    private Material originalMaterial;
    public Material flashMaterial;

    private void Start()
    {
        originalMaterial = healthBar.material;
    }

    private void Update()
    {
        lerpSpeed = 3f * Time.deltaTime;
        HealthBarFiller();

        if(PlayerScript.instance.currentHealth <= PlayerScript.instance.maxHealth * 0.3)
        {
            if (!isFlashing)
            {
                isFlashing = true;
                StartCoroutine(FlashHealth());
            }
        }
    }

    private void HealthBarFiller()
    {
        healthBar.fillAmount = Mathf.Lerp(healthBar.fillAmount, PlayerScript.instance.currentHealth / PlayerScript.instance.maxHealth, lerpSpeed);
    }

    IEnumerator FlashHealth()
    {
        while (isFlashing)
        {
            healthBar.material = flashMaterial;
            yield return new WaitForSeconds(0.1f);
            healthBar.material = originalMaterial;
            yield return new WaitForSeconds(0.1f);
            if (PlayerScript.instance.currentHealth > PlayerScript.instance.maxHealth * 0.3) isFlashing = false;
        }
        healthBar.material = originalMaterial;
        yield return null;
    }
}