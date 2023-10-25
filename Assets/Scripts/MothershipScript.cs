using System.Collections;
using UnityEngine;

public class MothershipScript : MonoBehaviour
{
    public static MothershipScript instance { get; private set; }
    public float maxHealth;
    public float currentHealth;

    private SpriteRenderer motherShipSprite;

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
    public bool forceFieldUp = false;
    private float turretOffset = 0.1f;
    public bool victory = false;
    private Material originalMaterial;
    public Material flashMaterial;
    private Transform popupPrefab;

    public enum BOSS_STATE
    {
        STAGE1,
        STAGE2,
        DEAD,
        PAUSED
    }

    public BOSS_STATE currentState;

    public bool hasStartedStage1 = false;
    public bool hasStartedStage2 = false;
    public bool hasStartedPause = false;

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
        maxHealth = 1000f;
        currentHealth = maxHealth;
        popupPrefab = Resources.Load<RectTransform>("Prefabs/Popup");
        motherShipSprite = GetComponent<SpriteRenderer>();
        initialLeftTurretPos = leftTurret.transform.position;
        initialRightTurretPos = rightTurret.transform.position;
        newLeftTurretPos = initialLeftTurretPos;
        newRightTurretPos = initialRightTurretPos;
        currentState = BOSS_STATE.PAUSED;
        originalMaterial = motherShipSprite.material;
    }

    void Update()
    {
        //Set the turret orientation to face downwards and only rotate turrets when boss is alive
        if (currentState != BOSS_STATE.DEAD)
        {
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
            case BOSS_STATE.PAUSED:
                Paused();
                break;
            default:
                break;
        }
    }

    public void Dead()
    {
        //Killed Boss
        motherShipSprite.material = originalMaterial;
        victory = true;
    }

    public void TakeDamage(float damage, bool crit = false, bool explosion = false, bool lightning = false)
    {
        if (forceFieldUp) return;
        if (this.currentHealth <= 0) return;

        Transform dmgNumber = Instantiate(popupPrefab, transform.position, Quaternion.identity);
        if (crit)
        {
            dmgNumber.GetComponent<Popup>().SetCrit(damage);
        }
        else if (explosion)
        {
            dmgNumber.GetComponent<Popup>().SetExplosion(damage);
        }
        else if (lightning)
        {
            dmgNumber.GetComponent<Popup>().SetLightning(damage);
        }
        else
        {
            dmgNumber.GetComponent<Popup>().SetDamageNumber(damage);
        }

        if (this.currentHealth - damage > 0)
        {
            this.currentHealth -= damage;
            /*SFXSource.PlayOneShot(enemyHit);*/
        }
        else
        {
            currentState = BOSS_STATE.DEAD;
        }
    }

    private void Stage1()
    {
        if (!hasStartedStage1)
        {
            hasStartedStage1 = true;
            StopAllCoroutines();
            StartCoroutine(Stage1Cycle());
        }

        if (currentHealth <= maxHealth / 3)
        {
            StopAllCoroutines();
            motherShipSprite.material = originalMaterial;
            StartCoroutine(HealForceField(BOSS_STATE.STAGE2));
        }
    }

    private void Stage2()
    {
        if (!hasStartedStage2)
        {
            hasStartedStage2 = true;
        }
    }

    private void Paused()
    {
        if (!hasStartedPause)
        {
            hasStartedPause = true;
            StopAllCoroutines();
            StartCoroutine(PauseHeal());
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

    public IEnumerator PauseHeal()
    {
        isTargeted = false;
        //Reset Turrets back to Initial Pos
        float moveTime = 0;
        float moveDuration = 1f;
        while (moveTime < moveDuration)
        {
            moveTime += Time.deltaTime;
            leftTurret.transform.position = Vector3.Lerp(newLeftTurretPos, initialLeftTurretPos, moveTime / moveDuration);
            rightTurret.transform.position = Vector3.Lerp(newRightTurretPos, initialRightTurretPos, moveTime / moveDuration);
            yield return null;
        }
        newLeftTurretPos = initialLeftTurretPos;
        newRightTurretPos = initialRightTurretPos;

        while (true)
        {
            forceFieldUp = true;
            forceField.SetActive(true);
            currentHealth += 100f * Time.deltaTime;
            if (currentHealth >= maxHealth) currentHealth = maxHealth;
            yield return null;
        }
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
            currentHealth += 50f * Time.deltaTime;

            leftTurret.transform.position = Vector3.Lerp(leftTurret.transform.position, initialLeftTurretPos, healTime / healDuration);
            rightTurret.transform.position = Vector3.Lerp(rightTurret.transform.position, initialRightTurretPos, healTime / healDuration);
            yield return null;
        }
        if (currentHealth >= maxHealth) currentHealth = maxHealth;
        newLeftTurretPos = initialLeftTurretPos;
        newRightTurretPos = initialRightTurretPos;
        yield return FlickerForcefield();
        forceFieldUp = false;
        currentState = nextState;
    }

    IEnumerator FlickerForcefield()
    {
        forceField.SetActive(false);
        yield return new WaitForSeconds(0.1f);
        forceField.SetActive(true);
        yield return new WaitForSeconds(0.1f);
        forceField.SetActive(false);
        yield return new WaitForSeconds(0.1f);
        forceField.SetActive(true);
        yield return new WaitForSeconds(0.1f);
        forceField.SetActive(false);
        yield return new WaitForSeconds(0.1f);
        forceField.SetActive(true);
        yield return new WaitForSeconds(0.1f);
        forceField.SetActive(false);
    }

    public IEnumerator Bleed(float damage)
    {
        Debug.Log("Bleed applied");
        for (int i = 0; i < 5; i++)
        {
            //TODO: Play particle/vfx etc
            if (this.currentHealth - damage > 0)
            {
                this.currentHealth -= damage;
                Transform bleedDmgNumber = Instantiate(popupPrefab, transform.position, Quaternion.identity);
                bleedDmgNumber.GetComponent<Popup>().SetBleed(damage);
            }
            else
            {
                currentState = BOSS_STATE.DEAD;
            }
            yield return new WaitForSeconds(1);
        }
        Debug.Log("Bleed ended");
        yield return null;
    }
}
