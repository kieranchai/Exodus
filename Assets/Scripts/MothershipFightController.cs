using System.Collections;
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
    private bool fightEnded = false;

    [SerializeField]
    private BoxCollider2D forcefield;

    private void Update()
    {
        lerpSpeed = 3f * Time.deltaTime;
        if (fightStarted) HealthBarFiller();
        if (motherShip.GetComponent<MothershipScript>().victory && !fightEnded)
        {
            fightEnded = true;
            motherShipHealthBar.SetActive(false);
            GameController.instance.currentState = GameController.GAME_STATE.WIN;
        }
    }

    private void HealthBarFiller()
    {
        motherShipHealthBar.transform.Find("Fill").GetComponent<Image>().fillAmount = Mathf.Lerp(motherShipHealthBar.transform.Find("Fill").GetComponent<Image>().fillAmount, MothershipScript.instance.currentHealth / MothershipScript.instance.maxHealth, lerpSpeed);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && !fightStarted)
        {
            fightStarted = true;
            StopAllCoroutines();
            StartCoroutine(StartFight());
        }
    }

    IEnumerator StartFight()
    {
        forcefield.enabled = true;
        forcefield.gameObject.transform.Find("Particle System").gameObject.SetActive(true);
        yield return CameraController.instance.StartCoroutine(CameraController.instance.PanToBoss());
        PlayerScript.instance.rb.bodyType = RigidbodyType2D.Dynamic;
        motherShip.GetComponent<MothershipScript>().StopCoroutine(motherShip.GetComponent<MothershipScript>().PauseHeal());
        yield return FlickerForcefield();
        motherShip.GetComponent<MothershipScript>().forceFieldUp = false;
        motherShip.GetComponent<MothershipScript>().currentState = MothershipScript.BOSS_STATE.STAGE1;
        motherShipHealthBar.SetActive(true);
        AudioManager.instance.threatLevel += 1000;
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
}
