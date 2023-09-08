using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MothershipScript : MonoBehaviour
{
    public float maxHealth;
    public float currentHealth;

    [SerializeField] private Transform damagePopupPrefab;

    [SerializeField]
    private GameObject leftTurret;

    [SerializeField]
    private GameObject rightTurret;

    [SerializeField]
    private GameObject forceField;

    private Vector3 initialLeftTurretPos;
    private Vector3 initialRightTurretPos;

    private Vector3 newLeftTurretPos;
    private Vector3 newRightTurretPos;

    private bool isTargeted = false;
    private bool forceFieldUp = false;

    private float turretOffset = 0.1f;

    public enum BOSS_STATE
    {
        STAGE1,
        STAGE2,
        STAGE3,
        DEAD
    }

    public BOSS_STATE currentState = BOSS_STATE.STAGE1;

    private bool hasStartedStage1 = false;
    private bool hasStartedStage2 = false;

    private void Start()
    {
        maxHealth = 100f;
        currentHealth = maxHealth;

        initialLeftTurretPos = leftTurret.transform.position;
        initialRightTurretPos = rightTurret.transform.position;
    }

    void Update()
    {
        //Set the turret orientation to face downwards
        if (!isTargeted)
        {
            leftTurret.transform.up = -transform.up.normalized;
            rightTurret.transform.up = -transform.up.normalized;
        }
        else
        {
            leftTurret.transform.up = (PlayerScript.instance.transform.position - new Vector3(leftTurret.transform.position.x, leftTurret.transform.position.y));
            rightTurret.transform.up = (PlayerScript.instance.transform.position - new Vector3(rightTurret.transform.position.x, rightTurret.transform.position.y));
        }

        switch (currentState)
        {
            case BOSS_STATE.STAGE1:
                Stage1();
                break;
            case BOSS_STATE.STAGE2:
                Stage2();
                break;
            case BOSS_STATE.STAGE3:
                break;
            default:
                break;
        }
    }

    public void TakeDamage(float damage)
    {
        if (forceFieldUp) return;
        Transform damagePopupTransform = Instantiate(damagePopupPrefab, transform.position, Quaternion.identity);
        DamagePopup damagePopup = damagePopupTransform.GetComponent<DamagePopup>();
        damagePopup.Setup(damage);

        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            //DIE
            currentState = BOSS_STATE.DEAD;
        }
    }

    private void Stage1()
    {
        if (!hasStartedStage1)
        {
            hasStartedStage1 = true;
            StartCoroutine(Stage1Cycle());
        }

        if (currentHealth <= maxHealth / 2)
        {
            StopAllCoroutines();
            currentState = BOSS_STATE.STAGE2;
        }
    }

    private void Stage2()
    {
        if (!hasStartedStage2)
        {
            hasStartedStage2 = true;
            StartCoroutine(Stage2Cycle());
        }


        if (currentHealth == maxHealth / 4)
        {
            currentState = BOSS_STATE.STAGE3;
        }
    }

    IEnumerator Stage1Cycle()
    {
        //Stationary Attack
        float targetTime = 0;
        float targetDuration = 5f;
        while (targetTime < targetDuration)
        {
            isTargeted = true;
            targetTime += Time.deltaTime;
            leftTurret.GetComponent<MothershipTurretScript>().DoAttack("normal", 5, 5, 0.5f);
            rightTurret.GetComponent<MothershipTurretScript>().DoAttack("normal", 5, 5, 0.5f);
            yield return null;
        }
        isTargeted = false;
        newLeftTurretPos = initialLeftTurretPos;
        newRightTurretPos = initialRightTurretPos;

        //Sweeping Attack LEFT->RIGHT RIGHT<-LEFT
        float sweepTime = 0;
        float sweepDuration = 3f;
        while (sweepTime < sweepDuration)
        {
            sweepTime += Time.deltaTime;
            leftTurret.transform.position = Vector3.Lerp(newLeftTurretPos, initialRightTurretPos, sweepTime / sweepDuration);
            rightTurret.transform.position = Vector3.Lerp(newRightTurretPos, initialLeftTurretPos, sweepTime / sweepDuration);
            leftTurret.GetComponent<MothershipTurretScript>().DoAttack("normal", 5, 5, 0.3f);
            rightTurret.GetComponent<MothershipTurretScript>().DoAttack("normal", 5, 5, 0.3f);
            yield return null;
        }
        newLeftTurretPos = initialRightTurretPos;
        newRightTurretPos = initialLeftTurretPos;

        //Sweeping Attack RIGHT->LEFT LEFT<-RIGHT
        sweepTime = 0;
        sweepDuration = 3f;
        while (sweepTime < sweepDuration)
        {
            sweepTime += Time.deltaTime;
            leftTurret.transform.position = Vector3.Lerp(newLeftTurretPos, initialLeftTurretPos, sweepTime / sweepDuration);
            rightTurret.transform.position = Vector3.Lerp(newRightTurretPos, initialRightTurretPos, sweepTime / sweepDuration);
            leftTurret.GetComponent<MothershipTurretScript>().DoAttack("normal", 5, 5, 0.3f);
            rightTurret.GetComponent<MothershipTurretScript>().DoAttack("normal", 5, 5, 0.3f);
            yield return null;
        }
        newLeftTurretPos = initialLeftTurretPos;
        newRightTurretPos = initialRightTurretPos;

        //Move Right Turret to be beside Left Turret + Offset
        float moveTime = 0;
        float moveDuration = 1f;
        while (moveTime < moveDuration)
        {
            moveTime += Time.deltaTime;
            rightTurret.transform.position = Vector3.Lerp(newRightTurretPos, initialLeftTurretPos + new Vector3(leftTurret.GetComponent<Collider2D>().bounds.size.x + turretOffset, 0), moveTime / moveDuration);
            yield return null;
        }
        newRightTurretPos = initialLeftTurretPos + new Vector3((float)(leftTurret.GetComponent<Collider2D>().bounds.size.x + turretOffset), 0);
        newLeftTurretPos = initialLeftTurretPos;

        //Wave Burst Attack from Left to Right
        float waveTime = 0;
        float waveDuration = 5f;
        while (waveTime < waveDuration)
        {
            isTargeted = true;
            waveTime += Time.deltaTime;
            leftTurret.transform.position = Vector3.Lerp(newLeftTurretPos, initialRightTurretPos - new Vector3(rightTurret.GetComponent<Collider2D>().bounds.size.x + turretOffset, 0), waveTime / waveDuration);
            rightTurret.transform.position = Vector3.Lerp(newRightTurretPos, initialRightTurretPos, waveTime / waveDuration);
            leftTurret.GetComponent<MothershipTurretScript>().DoAttack("burst", 5, 5, 0.3f);
            rightTurret.GetComponent<MothershipTurretScript>().DoAttack("burst", 5, 5, 0.3f);
            yield return null;
        }
        isTargeted = false;
        newLeftTurretPos = initialRightTurretPos - new Vector3(rightTurret.GetComponent<Collider2D>().bounds.size.x + turretOffset, 0);
        newRightTurretPos = initialRightTurretPos;

        yield return StartCoroutine(Stage1Cycle());
    }

    IEnumerator Stage2Cycle()
    {
        //Reset Turrets back to Initial Pos and No Targetting
        isTargeted = false;
        float moveTime = 0;
        float moveDuration = 1f;
        while (moveTime < moveDuration)
        {
            moveTime += Time.deltaTime;
            leftTurret.transform.position = Vector3.Lerp(leftTurret.transform.position, initialLeftTurretPos, moveTime / moveDuration);
            rightTurret.transform.position = Vector3.Lerp(rightTurret.transform.position, initialRightTurretPos, moveTime / moveDuration);
            yield return null;
        }
        newLeftTurretPos = initialLeftTurretPos;
        newRightTurretPos = initialRightTurretPos;

        //Heal + Forcefield
        float healTime = 0;
        float healDuration = 5f;
        while (healTime < healDuration)
        {
            forceFieldUp = true;
            forceField.SetActive(true);
            healTime += Time.deltaTime;
            currentHealth += 5f * Time.deltaTime;
            yield return null;
        }
        forceField.SetActive(false);
        forceFieldUp = false;
        newLeftTurretPos = initialLeftTurretPos;
        newRightTurretPos = initialRightTurretPos;

        //Stationary Rocket Attack
        float targetTime = 0;
        float targetDuration = 10f;
        while (targetTime < targetDuration)
        {
            isTargeted = true;
            targetTime += Time.deltaTime;
            leftTurret.GetComponent<MothershipTurretScript>().DoAttack("rocket", 10, 5, 0.5f);
            rightTurret.GetComponent<MothershipTurretScript>().DoAttack("rocket", 10, 5, 0.5f);
            yield return null;
        }
        isTargeted = false;
        newLeftTurretPos = initialLeftTurretPos;
        newRightTurretPos = initialRightTurretPos;

    }
}
