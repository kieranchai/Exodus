using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MothershipFightController : MonoBehaviour
{
    [SerializeField]
    private GameObject motherShip;

    [SerializeField]
    private GameObject motherShipHealthBar;

    private float lerpSpeed;

    private bool fightStarted = false;

    private void Update()
    {
        lerpSpeed = 3f * Time.deltaTime;
        if (fightStarted) HealthBarFiller();
    }

    private void HealthBarFiller()
    {
        motherShipHealthBar.transform.Find("Fill").GetComponent<Image>().fillAmount = Mathf.Lerp(motherShipHealthBar.transform.Find("Fill").GetComponent<Image>().fillAmount, MothershipScript.instance.currentHealth / MothershipScript.instance.maxHealth, lerpSpeed);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            motherShip.SetActive(true);
            motherShipHealthBar.SetActive(true);
            StartCoroutine(ZoomOutCamera());
            fightStarted = true;
        }
    }

    IEnumerator ZoomOutCamera()
    {
        float elapsed = 0;
        float initialZoom = Camera.main.orthographicSize;

        while (elapsed <= 3)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / 3);
            Camera.main.orthographicSize = Mathf.Lerp(initialZoom, 5, t);
            yield return null;
        }
    }
}
