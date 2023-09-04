using System.Collections;
using UnityEngine;
using System;

public class GameController : MonoBehaviour
{
    public static GameController instance { get; private set; }
    public bool isOverUI = false;
    public BoxCollider2D LowTierZone;

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

    private bool stimCD = false;

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
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
        if (Input.GetMouseButton(0) && !isOverUI && PlayerScript.instance.currentState != PlayerScript.PLAYER_STATE.ROLLING) PlayerScript.instance.weaponSlot.TryAttack();
        if (Input.GetKeyDown(KeyCode.Tab)) PlayerScript.instance.ToggleInventoryView();
        if (Input.GetKeyDown(KeyCode.E)) Interact();
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
        PlayerScript.instance.anim.SetBool("isWalking", false);

        Debug.Log("You died");
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
                if (PlayerScript.instance.currentHealth == PlayerScript.instance.maxHealth) return false;
                StartCoroutine(ItemHealth(itemData));
                break;
            case "MOVEMENT SPEED":
                if (this.stimCD) return false;
                StartCoroutine(ItemStim(itemData));
                break;
            case "GAS":
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
        yield return new WaitForSeconds(float.Parse(itemData.secondaryValue));
        PlayerScript.instance.movementSpeed = PlayerScript.instance.initialMovementSpeed;
        this.stimCD = false;
    }
}
