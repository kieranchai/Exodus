using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MothershipFightController : MonoBehaviour
{
    [SerializeField]
    private GameObject motherShip;

    [SerializeField]
    private GameObject motherShipHealthBar;

    [SerializeField]
    private TMP_Text gameWinText;

    private float lerpSpeed;

    private bool fightStarted = false;
    private bool fightEnded = false;

    private void Update()
    {
        lerpSpeed = 3f * Time.deltaTime;
        if (fightStarted) HealthBarFiller();
        if (motherShip.GetComponent<MothershipScript>().victory && !fightEnded)
        {
            fightEnded = true;
            StartCoroutine(EndGame());
        }
    }

    private void HealthBarFiller()
    {
        motherShipHealthBar.transform.Find("Fill").GetComponent<Image>().fillAmount = Mathf.Lerp(motherShipHealthBar.transform.Find("Fill").GetComponent<Image>().fillAmount, MothershipScript.instance.currentHealth / MothershipScript.instance.maxHealth, lerpSpeed);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            StartCoroutine(StartFight());
        }
    }

    IEnumerator StartFight()
    {
        yield return ZoomOutCamera();
        yield return FlickerForcefield();
        motherShip.GetComponent<MothershipScript>().enabled = true;
        motherShipHealthBar.SetActive(true);
        fightStarted = true;
    }

    IEnumerator FlickerForcefield()
    {
        motherShip.transform.Find("Forcefield").gameObject.SetActive(false);
        yield return new WaitForSeconds(0.1f);
        motherShip.transform.Find("Forcefield").gameObject.SetActive(true);
        yield return new WaitForSeconds(0.1f);
        motherShip.transform.Find("Forcefield").gameObject.SetActive(false);
        yield return new WaitForSeconds(0.1f);
        motherShip.transform.Find("Forcefield").gameObject.SetActive(true);
        yield return new WaitForSeconds(0.1f);
        motherShip.transform.Find("Forcefield").gameObject.SetActive(false);
        yield return new WaitForSeconds(0.1f);
        motherShip.transform.Find("Forcefield").gameObject.SetActive(true);
        yield return new WaitForSeconds(0.1f);
        motherShip.transform.Find("Forcefield").gameObject.SetActive(false);
    }

    IEnumerator ZoomOutCamera()
    {
        float elapsed = 0;
        float initialZoom = Camera.main.orthographicSize;

        while (elapsed <= 1)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / 1);
            Camera.main.orthographicSize = Mathf.Lerp(initialZoom, 7, t);
            yield return null;
        }
    }

    IEnumerator EndGame()
    {
        motherShipHealthBar.SetActive(false);
        gameWinText.gameObject.SetActive(true);
        GameController.instance.currentState = GameController.GAME_STATE.WIN;
        yield return Typewriter("You've defeated the mothership and saved Earth!");
    }

    IEnumerator Typewriter(string text)
    {
        gameWinText.text = "";
        var waitTimer = new WaitForSeconds(.05f);
        foreach (char c in text)
        {
            gameWinText.text += c;
            yield return waitTimer;
        }
        yield return new WaitForSeconds(0.5f);
    }
}
