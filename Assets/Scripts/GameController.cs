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
        switch (currentState) { 
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
        bool success = PlayerScript.instance.MovePlayer(new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized);
        if (!success)
        {
            success = PlayerScript.instance.MovePlayer(new Vector2(Input.GetAxisRaw("Horizontal"), 0));
            if (!success)
            {
                success = PlayerScript.instance.MovePlayer(new Vector2(0, Input.GetAxisRaw("Vertical")));
            }
        }

        if (Input.GetAxisRaw("Horizontal") != 0 || Input.GetAxisRaw("Vertical") != 0)
        {
            PlayerScript.instance.anim.SetBool("isWalking", true);
        }
        else
        {
            PlayerScript.instance.anim.SetBool("isWalking", false);
        }

        if (Input.GetKeyDown(KeyCode.Escape)) currentState = GAME_STATE.PAUSED;
        if (Input.GetMouseButton(0) && !isOverUI) PlayerScript.instance.weaponSlot.TryAttack();
        if (Input.GetKeyDown(KeyCode.Tab)) PlayerScript.instance.ToggleInventoryView();
        if (Input.GetKeyDown(KeyCode.E)) PlayerScript.instance.ToggleShopView();
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
}
