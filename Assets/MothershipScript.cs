using System.Collections;
using UnityEngine;

public class MothershipScript : MonoBehaviour
{
    public static MothershipScript instance { get; private set; }
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
    public bool isHoming = false;
    public bool hasShotHoming = false;
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
    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
    }

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
            case BOSS_STATE.DEAD:
                Dead();
                break;
            default:
                break;
        }
    }

    public void Dead()
    {
        //Killed Boss
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
            StopAllCoroutines();
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
            StartCoroutine(HealForceField(BOSS_STATE.STAGE2));
        }
    }

    private void Stage2()
    {
        if (!hasStartedStage2)
        {
            hasStartedStage2 = true;
            StartCoroutine(Stage2Cycle());
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
            leftTurret.GetComponent<MothershipTurretScript>().DoAttack("normal", 5, 10, 0.5f);
            rightTurret.GetComponent<MothershipTurretScript>().DoAttack("normal", 5, 10, 0.5f);
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
            leftTurret.GetComponent<MothershipTurretScript>().DoAttack("normal", 5, 10, 0.3f);
            rightTurret.GetComponent<MothershipTurretScript>().DoAttack("normal", 5, 10, 0.3f);
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
            leftTurret.GetComponent<MothershipTurretScript>().DoAttack("normal", 5, 10, 0.3f);
            rightTurret.GetComponent<MothershipTurretScript>().DoAttack("normal", 5, 10, 0.3f);
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
            leftTurret.GetComponent<MothershipTurretScript>().DoAttack("burst", 5, 10, 0.5f);
            rightTurret.GetComponent<MothershipTurretScript>().DoAttack("burst", 5, 10, 0.5f);
            yield return null;
        }
        isTargeted = false;
        newLeftTurretPos = initialRightTurretPos - new Vector3(rightTurret.GetComponent<Collider2D>().bounds.size.x + turretOffset, 0);
        newRightTurretPos = initialRightTurretPos;

        //Wave Burst Attack from Right to Left
        waveTime = 0;
        waveDuration = 5f;
        while (waveTime < waveDuration)
        {
            isTargeted = true;
            waveTime += Time.deltaTime;
            leftTurret.transform.position = Vector3.Lerp(newLeftTurretPos, initialLeftTurretPos, waveTime / waveDuration);
            rightTurret.transform.position = Vector3.Lerp(newRightTurretPos, initialLeftTurretPos + new Vector3(leftTurret.GetComponent<Collider2D>().bounds.size.x + turretOffset, 0), waveTime / waveDuration);
            leftTurret.GetComponent<MothershipTurretScript>().DoAttack("burst", 5, 10, 0.3f);
            rightTurret.GetComponent<MothershipTurretScript>().DoAttack("burst", 5, 10, 0.3f);
            yield return null;
        }
        isTargeted = false;
        newLeftTurretPos = initialLeftTurretPos;
        newRightTurretPos = initialLeftTurretPos + new Vector3(leftTurret.GetComponent<Collider2D>().bounds.size.x + turretOffset, 0);

        //Reset Turrets back to Initial Pos
        moveTime = 0;
        moveDuration = 1f;
        while (moveTime < moveDuration)
        {
            moveTime += Time.deltaTime;
            leftTurret.transform.position = Vector3.Lerp(newLeftTurretPos, initialLeftTurretPos, moveTime / moveDuration);
            rightTurret.transform.position = Vector3.Lerp(newRightTurretPos, initialRightTurretPos, moveTime / moveDuration);
            yield return null;
        }
        newLeftTurretPos = initialLeftTurretPos;
        newRightTurretPos = initialRightTurretPos;

        yield return StartCoroutine(Stage1Cycle());
    }

    IEnumerator HealForceField(BOSS_STATE nextState)
    {
        isTargeted = false;
        float healTime = 0;
        float healDuration = 5f;
        while (healTime < healDuration)
        {
            forceFieldUp = true;
            forceField.SetActive(true);
            healTime += Time.deltaTime;
            currentHealth += 5f * Time.deltaTime;

            leftTurret.transform.position = Vector3.Lerp(leftTurret.transform.position, initialLeftTurretPos, healTime / healDuration);
            rightTurret.transform.position = Vector3.Lerp(rightTurret.transform.position, initialRightTurretPos, healTime / healDuration);
            yield return null;
        }
        if (currentHealth >= maxHealth) currentHealth = maxHealth;
        newLeftTurretPos = initialLeftTurretPos;
        newRightTurretPos = initialRightTurretPos;
        forceField.SetActive(false);
        forceFieldUp = false;
        currentState = nextState;
    }

    IEnumerator Stage2Cycle()
    {
        //Guided Rocket Attack
        isTargeted = true;
        isHoming = true;
        leftTurret.GetComponent<MothershipTurretScript>().limitAttack = false;
        rightTurret.GetComponent<MothershipTurretScript>().limitAttack = false;
        while (isHoming)
        {
            if(!hasShotHoming)
            {
                hasShotHoming = true;
                leftTurret.GetComponent<MothershipTurretScript>().DoAttack("homingrocket", 10, 0, 0);
            }
            yield return null;
        }
        hasShotHoming = false;
        isTargeted = false;

        //Guided Rocket Attack
        isTargeted = true;
        isHoming = true;
        leftTurret.GetComponent<MothershipTurretScript>().limitAttack = false;
        rightTurret.GetComponent<MothershipTurretScript>().limitAttack = false;
        while (isHoming)
        {
            if (!hasShotHoming)
            {
                hasShotHoming = true;
                rightTurret.GetComponent<MothershipTurretScript>().DoAttack("homingrocket", 10, 0, 0);
            }
            yield return null;
        }
        hasShotHoming = false;
        isTargeted = false;

        //Guided Rocket Attack
        isTargeted = true;
        isHoming = true;
        leftTurret.GetComponent<MothershipTurretScript>().limitAttack = false;
        rightTurret.GetComponent<MothershipTurretScript>().limitAttack = false;
        while (isHoming)
        {
            if (!hasShotHoming)
            {
                hasShotHoming = true;
                leftTurret.GetComponent<MothershipTurretScript>().DoAttack("homingrocket", 10, 0, 0);
            }
            yield return null;
        }
        hasShotHoming = false;
        isTargeted = false;

        //Slow Rocket Attack
        float targetTime = 0;
        float targetDuration = 5f;
        while (targetTime < targetDuration)
        {
            isTargeted = true;
            targetTime += Time.deltaTime;
            leftTurret.GetComponent<MothershipTurretScript>().DoAttack("rocket", 10, 10, 0.8f);
            rightTurret.GetComponent<MothershipTurretScript>().DoAttack("rocket", 10, 10, 0.8f);
            yield return null;
        }
        isTargeted = false;

        //Fast Rocket Attack
        targetTime = 0;
        targetDuration = 5f;
        while (targetTime < targetDuration)
        {
            isTargeted = true;
            targetTime += Time.deltaTime;
            leftTurret.GetComponent<MothershipTurretScript>().DoAttack("rocket", 10, 10, 0.6f);
            rightTurret.GetComponent<MothershipTurretScript>().DoAttack("rocket", 10, 10, 0.6f);
            yield return null;
        }
        isTargeted = false;

        //Faster Rocket Attack
        targetTime = 0;
        targetDuration = 5f;
        while (targetTime < targetDuration)
        {
            isTargeted = true;
            targetTime += Time.deltaTime;
            leftTurret.GetComponent<MothershipTurretScript>().DoAttack("rocket", 10, 10, 0.3f);
            rightTurret.GetComponent<MothershipTurretScript>().DoAttack("rocket", 10, 10, 0.3f);
            yield return null;
        }
        isTargeted = false;

        yield return new WaitForSeconds(1);
        yield return StartCoroutine(HealForceField(BOSS_STATE.STAGE2));
        yield return StartCoroutine(Stage2Cycle());
    }
}
