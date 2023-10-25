using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class HealthBar : MonoBehaviour
{
    [SerializeField]
    private Image healthBar;

    private float lerpSpeed;

    private bool isFlashing = false;

    public Material originalMaterial;
    public Material flashMaterial;

    [SerializeField]
    private Volume lowHealthVolume;

    private Vignette lowHealthVignette;

    private AudioSource SFXSource;
    [Header("Health Audio Clip")]
    public AudioClip heartbeat;

    private void Start()
    {
        SFXSource = GetComponent<AudioSource>();
        originalMaterial = healthBar.material;
        lowHealthVolume.profile.TryGet(out lowHealthVignette);
    }

    private void Update()
    {
        lerpSpeed = 3f * Time.deltaTime;
        HealthBarFiller();
        if (PlayerScript.instance.currentHealth <= PlayerScript.instance.maxHealth * 0.3)
        {
            if (!isFlashing)
            {
                isFlashing = true;
                SFXSource.clip = heartbeat;
                SFXSource.Play();
                StartCoroutine(FlashHealth());
            }
        }
        HealthVignette();
    }

    private void HealthBarFiller()
    {
        healthBar.fillAmount = Mathf.Lerp(healthBar.fillAmount, PlayerScript.instance.currentHealth / PlayerScript.instance.maxHealth, lerpSpeed);
    }

    private void HealthVignette()
    {
        if (PlayerScript.instance.currentHealth <= PlayerScript.instance.maxHealth * 0.3)
        {
            lowHealthVignette.intensity.value = Mathf.Lerp(lowHealthVignette.intensity.value, 0.6f, lerpSpeed);
            return;
        }

        lowHealthVignette.intensity.value = Mathf.Lerp(lowHealthVignette.intensity.value, 0f, lerpSpeed);
    }

    IEnumerator FlashHealth()
    {
        while (isFlashing)
        {
            healthBar.material = flashMaterial;
            yield return new WaitForSeconds(0.1f);
            healthBar.material = originalMaterial;
            yield return new WaitForSeconds(0.1f);
            if (PlayerScript.instance.currentHealth > PlayerScript.instance.maxHealth * 0.3)
            {
                isFlashing = false;
                SFXSource.Stop();
            }
        }
        healthBar.material = originalMaterial;
        yield return null;
    }
}