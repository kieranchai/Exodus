using System.Collections;
using UnityEngine;
using System;

public class GameController : MonoBehaviour
{
    public static GameController instance { get; private set; }
    public bool isOverUI = false;
    public BoxCollider2D[] Zones;
    public enum GAME_STATE
    {
        PAUSED,
        PLAYING,
        TUTORIAL,
        DEAD,
        WIN
    }

    public GAME_STATE currentState = GAME_STATE.PLAYING;

    [SerializeField]
    private GameObject pauseScreen;

    [SerializeField]
    private GameObject deathScreen;

    private bool stimCD = false;

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        // Application.targetFrameRate = 60;
    }

    private void Update()
    {
        switch (currentState)
        {
            case GAME_STATE.PLAYING:
                HandleInput();
                break;
            case GAME_STATE.PAUSED:
                PauseGame();
                break;
            case GAME_STATE.TUTORIAL:
                break;
            case GAME_STATE.DEAD:
                GameOver();
                break;
            case GAME_STATE.WIN:
                break;
            default:
                break;
        }
    }

    private void PauseGame()
    {
        Time.timeScale = 0.0f;
        pauseScreen.SetActive(true);

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            pauseScreen.SetActive(false);
            Time.timeScale = 1.0f;
            currentState = GAME_STATE.PLAYING;
        }
    }

    private void HandleInput()
    {
        PlayerScript.instance.LookAtMouse();

        //Movement Input WASD+LEFT SHIFT is in PlayerScript Update/FixedUpdate

        if (Input.GetKeyDown(KeyCode.Escape)) Exit();
        if (Input.GetMouseButton(0) && !isOverUI && PlayerScript.instance.currentState != PlayerScript.PLAYER_STATE.ROLLING)
        {
            PlayerScript.instance.weaponSlot.TryAttack();
            if (PlayerScript.instance.equippedWeapon && PlayerScript.instance.equippedWeapon.weaponType == "akimbo")
            {
                PlayerScript.instance.akimboSlot.TryAttack();
            }
        }
        if (Input.GetKeyDown(KeyCode.Tab)) PlayerScript.instance.ToggleInventoryView();
        if (Input.GetKeyDown(KeyCode.E)) Interact();

        //cheat
        if (Input.GetKeyDown(KeyCode.L)) { PlayerScript.instance.UpdateExperience(10); }
        if (Input.GetKeyDown(KeyCode.H)) { PlayerScript.instance.currentHealth = PlayerScript.instance.maxHealth; }

    }

    private void Exit()
    {
        if (PlayerScript.instance.CanSeeShop || PlayerScript.instance.CanSeeInventory)
        {
            PlayerScript.instance.TurnOffAllViews();
            return;
        }

        //If no panel is open, then PAUSE game when ESC
        currentState = GAME_STATE.PAUSED;
    }

    private void Interact()
    {
        if (PlayerScript.instance.isInShop)
        {
            PlayerScript.instance.ToggleShopView();
            return;
        }

        //Other Interactable Scenarios
    }

    private void GameOver()
    {
        PlayerScript.instance.coll.enabled = false;
        PlayerScript.instance.anim.SetBool("isWalking", false);

        Time.timeScale = 0.0f;
        deathScreen.SetActive(true);
    }

    public void CursorIsOverUI()
    {
        isOverUI = true;
    }

    public void CursorNotOverUI()
    {
        isOverUI = false;
    }

    public bool UseItem(Item itemData)
    {
        switch (itemData.type)
        {
            case "SMALL HEALTH":
            case "BIG HEALTH":
                if (PlayerScript.instance.currentHealth == PlayerScript.instance.maxHealth) {
                    PlayerScript.instance.AlertPopup("health");
                    return false;
                }
                StartCoroutine(ItemHealth(itemData));
                break;
            case "MOVEMENT SPEED":
                if (this.stimCD) {
                    PlayerScript.instance.AlertPopup("speed");
                    return false;
                }
                StartCoroutine(ItemStim(itemData));
                break;
            case "GAS":
                if (PoisonGas.instance.hasReached) {
                    PlayerScript.instance.AlertPopup("gasEnded");
                    return false;
                } else if (PoisonGas.instance.itemUsed)
                {
                    PlayerScript.instance.AlertPopup("gas");
                    return false;
                }
                StartCoroutine(ItemGas(itemData));
                break;
            default:
                break;
        }
        return true;
    }

    IEnumerator ItemHealth(Item itemData)
    {
        PlayerScript.instance.currentHealth += float.Parse(itemData.primaryValue);
        if (PlayerScript.instance.currentHealth >= PlayerScript.instance.maxHealth) PlayerScript.instance.currentHealth = PlayerScript.instance.maxHealth;
        yield return null;
    }

    IEnumerator ItemStim(Item itemData)
    {
        this.stimCD = true;
        PlayerScript.instance.movementSpeed += float.Parse(itemData.primaryValue);
        float stimTimer = 0;
        while(stimTimer < float.Parse(itemData.secondaryValue)) {
            stimTimer += Time.deltaTime;
            PlayerScript.instance.UpdateStimTimerUI(stimTimer, float.Parse(itemData.secondaryValue));
            yield return null;
        }
        PlayerScript.instance.HideStimTimerUI();
        PlayerScript.instance.movementSpeed = PlayerScript.instance.initialMovementSpeed;
        this.stimCD = false;
    }

    IEnumerator ItemGas(Item itemData)
    {
        PoisonGas.instance.itemUsed = true;
        yield return new WaitForSeconds(float.Parse(itemData.primaryValue));
        PoisonGas.instance.itemUsed = false;
    }
}
