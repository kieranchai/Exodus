using UnityEngine;

public class CollectorScript : MonoBehaviour
{
    private AudioSource SFXSource;

    [Header("Orb Audio Clips")]
    public AudioClip cashCollect;
    public AudioClip healthCollect;

    private void Awake()
    {
        SFXSource = GetComponent<AudioSource>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("CashOrb"))
        {
            PlayerScript.instance.UpdateCash(collision.gameObject.GetComponent<OrbScript>().value);
            SFXSource.PlayOneShot(cashCollect);
            Destroy(collision.gameObject);
        }

        if (collision.gameObject.CompareTag("HpOrb"))
        {
            if (PlayerScript.instance.currentHealth >= PlayerScript.instance.maxHealth) return;
            PlayerScript.instance.UpdateHealth(collision.gameObject.GetComponent<OrbScript>().value);
            SFXSource.PlayOneShot(healthCollect);
            Destroy(collision.gameObject);
        }
    }
}
