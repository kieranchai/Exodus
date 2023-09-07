using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MothershipScript : MonoBehaviour
{
    public float maxHealth;
    public float currentHealth;

    [SerializeField]
    private GameObject leftTurret;

    [SerializeField]
    private GameObject rightTurret;

    private Vector3 initialLeftTurretPos;
    private Vector3 initialRightTurretPos;

    private Vector3 newLeftTurretPos;
    private Vector3 newRightTurretPos;

    private bool isTargeted = false;

    private enum BOSS_STATE
    {
        STAGE1,
        STAGE2,
        STAGE3,
        DEAD
    }

    private BOSS_STATE currentState = BOSS_STATE.STAGE1;

    private bool hasStartedStage1 = false;

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
        if(!isTargeted)
        {
            leftTurret.transform.up = -this.transform.up.normalized;
            rightTurret.transform.up = -this.transform.up.normalized;
        }

        switch (currentState)
        {
            case BOSS_STATE.STAGE1:
                Stage1();
                break;
            case BOSS_STATE.STAGE2:
                break;
            case BOSS_STATE.STAGE3:
                break;
            default:
                break;
        }
    }

    private void Stage1()
    {
        if (!hasStartedStage1)
        {
            hasStartedStage1 = true;
            StartCoroutine(Stage1Cycle());
        }


        if (currentHealth == maxHealth / 2)
        {
            currentState = BOSS_STATE.STAGE2;
        }
    }

    IEnumerator Stage1Cycle()
    {
        //Target Player
        float targetTime = 0;
        float targetDuration = 5f;
        while (targetTime < targetDuration)
        {
            isTargeted = true;
            targetTime += Time.deltaTime;
            leftTurret.transform.up = (PlayerScript.instance.transform.position - new Vector3(leftTurret.transform.position.x, leftTurret.transform.position.y));
            rightTurret.transform.up = (PlayerScript.instance.transform.position - new Vector3(rightTurret.transform.position.x, rightTurret.transform.position.y));
            leftTurret.GetComponent<MothershipTurretScript>().DoAttack("normal", 5, 5, 0.5f);
            rightTurret.GetComponent<MothershipTurretScript>().DoAttack("normal", 5, 5, 0.5f);
            yield return null;
        }
        isTargeted = false;

        //Initiate Moving Right Turret to be beside Left Turret from Initial Positions
        float moveTime = 0;
        float moveDuration = 1f;
        while (moveTime < moveDuration)
        {
            moveTime += Time.deltaTime;
            rightTurret.transform.position = Vector3.Lerp(initialRightTurretPos, initialLeftTurretPos + new Vector3(leftTurret.GetComponent<Collider2D>().bounds.size.x + 0.1f, 0), moveTime / moveDuration);
            rightTurret.GetComponent<MothershipTurretScript>().DoAttack("normal", 5, 5, 0.1f);
            yield return null;
        }
        newRightTurretPos = initialLeftTurretPos + new Vector3((float)(leftTurret.GetComponent<Collider2D>().bounds.size.x + 0.1), 0);
        newLeftTurretPos = initialLeftTurretPos;

        //Wave Attack Left to Right
        float waveTime = 0;
        float waveDuration = 1f;
        while (waveTime < waveDuration)
        {
            waveTime += Time.deltaTime;
            leftTurret.transform.position = Vector3.Lerp(newLeftTurretPos, initialRightTurretPos - new Vector3(rightTurret.GetComponent<Collider2D>().bounds.size.x + 0.1f, 0), waveTime / waveDuration);
            rightTurret.transform.position = Vector3.Lerp(newRightTurretPos, initialRightTurretPos, waveTime / waveDuration);
            leftTurret.GetComponent<MothershipTurretScript>().DoAttack("normal", 5, 5, 0.1f);
            rightTurret.GetComponent<MothershipTurretScript>().DoAttack("normal", 5, 5, 0.1f);
            yield return null;
        }
        newLeftTurretPos = initialRightTurretPos - new Vector3(rightTurret.GetComponent<Collider2D>().bounds.size.x + 0.1f, 0);
        newRightTurretPos = initialRightTurretPos;

        //Initiate Moving Turrets back to Initial Positions
        moveTime = 0;
        moveDuration = 1f;
        while (moveTime < moveDuration)
        {
            moveTime += Time.deltaTime;
            leftTurret.transform.position = Vector3.Lerp(newLeftTurretPos, initialLeftTurretPos, moveTime / moveDuration);
            rightTurret.transform.position = Vector3.Lerp(newRightTurretPos, initialRightTurretPos, moveTime / moveDuration);
            leftTurret.GetComponent<MothershipTurretScript>().DoAttack("normal", 5, 5, 0.1f);
            yield return null;
        }
        newLeftTurretPos = initialLeftTurretPos;
        newRightTurretPos = initialRightTurretPos;

        //Sweeping Attack LEFT->RIGHT RIGHT<-LEFT
        float sweepTime = 0;
        float sweepDuration = 5f;
        while (sweepTime < sweepDuration)
        {
            sweepTime += Time.deltaTime;
            leftTurret.transform.position = Vector3.Lerp(newLeftTurretPos, initialRightTurretPos, sweepTime / sweepDuration);
            rightTurret.transform.position = Vector3.Lerp(newRightTurretPos, initialLeftTurretPos, sweepTime / sweepDuration);
            leftTurret.GetComponent<MothershipTurretScript>().DoAttack("normal", 5, 5, 0.5f);
            rightTurret.GetComponent<MothershipTurretScript>().DoAttack("normal", 5, 5, 0.5f);
            yield return null;
        }
        newLeftTurretPos = initialRightTurretPos;
        newRightTurretPos = initialLeftTurretPos;

        //Sweeping Attack RIGHT->LEFT LEFT<-RIGHT
        sweepTime = 0;
        sweepDuration = 1f;
        while (sweepTime < sweepDuration)
        {
            sweepTime += Time.deltaTime;
            leftTurret.transform.position = Vector3.Lerp(newLeftTurretPos, initialLeftTurretPos, sweepTime / sweepDuration);
            rightTurret.transform.position = Vector3.Lerp(newRightTurretPos, initialRightTurretPos, sweepTime / sweepDuration);
            leftTurret.GetComponent<MothershipTurretScript>().DoAttack("normal", 5, 5, 0.1f);
            rightTurret.GetComponent<MothershipTurretScript>().DoAttack("normal", 5, 5, 0.1f);
            yield return null;
        }
        newLeftTurretPos = initialLeftTurretPos;
        newRightTurretPos = initialRightTurretPos;

        //Sweeping Attack LEFT->RIGHT RIGHT<-LEFT
        sweepTime = 0;
        sweepDuration = 5f;
        while (sweepTime < sweepDuration)
        {
            sweepTime += Time.deltaTime;
            leftTurret.transform.position = Vector3.Lerp(newLeftTurretPos, initialRightTurretPos, sweepTime / sweepDuration);
            rightTurret.transform.position = Vector3.Lerp(newRightTurretPos, initialLeftTurretPos, sweepTime / sweepDuration);
            leftTurret.GetComponent<MothershipTurretScript>().DoAttack("normal", 5, 5, 0.5f);
            rightTurret.GetComponent<MothershipTurretScript>().DoAttack("normal", 5, 5, 0.5f);
            yield return null;
        }
        newLeftTurretPos = initialRightTurretPos;
        newRightTurretPos = initialLeftTurretPos;

        //Sweeping Attack RIGHT->LEFT LEFT<-RIGHT
        sweepTime = 0;
        sweepDuration = 1f;
        while (sweepTime < sweepDuration)
        {
            sweepTime += Time.deltaTime;
            leftTurret.transform.position = Vector3.Lerp(newLeftTurretPos, initialLeftTurretPos, sweepTime / sweepDuration);
            rightTurret.transform.position = Vector3.Lerp(newRightTurretPos, initialRightTurretPos, sweepTime / sweepDuration);
            leftTurret.GetComponent<MothershipTurretScript>().DoAttack("normal", 5, 5, 0.1f);
            rightTurret.GetComponent<MothershipTurretScript>().DoAttack("normal", 5, 5, 0.1f);
            yield return null;
        }
        newLeftTurretPos = initialLeftTurretPos;
        newRightTurretPos = initialRightTurretPos;

        yield return StartCoroutine(Stage1Cycle());
    }
}
