using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [SerializeField]
    private Image healthBar;

    private Sprite initial;

    private float lerpSpeed;

    private bool isFlashing = false;

    private void Start()
    {
        initial = healthBar.sprite;
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
            healthBar.sprite = Resources.Load<Sprite>("UISprites/Player HP Fill Flash");
            yield return new WaitForSeconds(0.1f);
            healthBar.sprite = initial;
            yield return new WaitForSeconds(0.1f);
            if (PlayerScript.instance.currentHealth > PlayerScript.instance.maxHealth * 0.3) isFlashing = false;
        }
        healthBar.sprite = initial;
        yield return null;
    }
}