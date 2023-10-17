using System.Collections;
using UnityEngine;
using System;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class GameController : MonoBehaviour
{
    public static GameController instance { get; private set; }
    public bool isOverUI = false;
    public Collider2D[] Zones;

    public enum GAME_STATE
    {
        PANNING,
        PAUSED,
        PLAYING,
        DEAD,
        WIN
    }

    public GAME_STATE currentState = GAME_STATE.PLAYING;

    [SerializeField]
    private GameObject tutorialDialogue;

    [SerializeField]
    private Collider2D tutorialBarrier1;

    [SerializeField]
    private Collider2D tutorialBarrier2;

    public bool tutorialFlag1 = false;
    public bool tutorialFlag2 = false;

    [SerializeField]
    private GameObject pauseScreen;

    [SerializeField]
    private GameObject deathScreen;

    [SerializeField]
    private GameObject buffSelectionPanel;

    public Transform announcementContainer;

    /*private bool stimCD = false;*/

    private bool hasLoadedEndCutscene = false;

    private bool isBuffOpen = false;

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
            case GAME_STATE.PANNING:
                break;
            case GAME_STATE.PLAYING:
                HandleInput();
                break;
            case GAME_STATE.PAUSED:
                PauseGame();
                break;
            case GAME_STATE.DEAD:
                GameOver();
                break;
            case GAME_STATE.WIN:
                if (!hasLoadedEndCutscene) EndGame();
                break;
            default:
                break;
        }
    }

    private void EndGame()
    {
        PlayerScript.instance.coll.enabled = false;
        PlayerScript.instance.anim.SetBool("isWalking", false);
        PlayerScript.instance.currentState = PlayerScript.PLAYER_STATE.WIN;

        Debug.Log("Win");
        hasLoadedEndCutscene = true;
        // Load Scene
    }

    private void PauseGame()
    {
        Time.timeScale = 0.0f;
        pauseScreen.SetActive(true);

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ResumeGame();
        }
    }

    private void Choosing()
    {
        AudioManager.instance.PlaySFX(AudioManager.instance.menuOpen);
        isBuffOpen = !isBuffOpen;
        buffSelectionPanel.SetActive(isBuffOpen);
        if(isBuffOpen == true) BuffController.instance.UpdatePlayerTokensDisplay();
    }

    public void ResumeGame()
    {
        CloseControls();
        pauseScreen.SetActive(false);
        buffSelectionPanel.SetActive(false);
        Time.timeScale = 1.0f;
        currentState = GAME_STATE.PLAYING;
    }

    public void ShowControls()
    {
        pauseScreen.transform.Find("Paused").gameObject.SetActive(false);
        pauseScreen.transform.Find("Resume").gameObject.SetActive(false);
        pauseScreen.transform.Find("Controls").gameObject.SetActive(false);
        pauseScreen.transform.Find("Exit").gameObject.SetActive(false);
        pauseScreen.transform.Find("Controls Screen").gameObject.SetActive(true);
    }

    public void CloseControls()
    {
        pauseScreen.transform.Find("Paused").gameObject.SetActive(true);
        pauseScreen.transform.Find("Resume").gameObject.SetActive(true);
        pauseScreen.transform.Find("Controls").gameObject.SetActive(true);
        pauseScreen.transform.Find("Exit").gameObject.SetActive(true);
        pauseScreen.transform.Find("Controls Screen").gameObject.SetActive(false);
    }

    private void HandleInput()
    {
        PlayerScript.instance.LookAtMouse();

        //Movement Input WASD+LEFT SHIFT is in PlayerScript Update/FixedUpdate

        if (Input.GetKeyDown(KeyCode.Escape)) Exit();
        if (Input.GetMouseButton(0) && !isOverUI && PlayerScript.instance.currentState != PlayerScript.PLAYER_STATE.ROLLING)
        {
            PlayerScript.instance.weaponSlot.TryAttack();
        }

        if (Input.GetKeyDown(KeyCode.R) && PlayerScript.instance.weaponSlot.weaponType != "melee" && PlayerScript.instance.equippedWeapon.currentAmmoCount < PlayerScript.instance.weaponSlot.clipSize && PlayerScript.instance.currentState != PlayerScript.PLAYER_STATE.ROLLING)
        {
            PlayerScript.instance.weaponSlot.isReloading = true;
            PlayerScript.instance.weaponSlot.StartCoroutine(PlayerScript.instance.weaponSlot.Reload());
        }

        if (Input.GetAxis("Mouse ScrollWheel") != 0 && !isOverUI && PlayerScript.instance.inventory.Count > 0)
        {
            if (PlayerScript.instance.inventory.Count == 1 && PlayerScript.instance.weaponNumber == PlayerScript.instance.inventory.Count - 1) return;
            if (Input.GetAxis("Mouse ScrollWheel") > 0)
            {
                PlayerScript.instance.weaponNumber++;
                if (PlayerScript.instance.weaponNumber >= PlayerScript.instance.inventory.Count) PlayerScript.instance.weaponNumber = 0;
            }
            if (Input.GetAxis("Mouse ScrollWheel") < 0)
            {
                PlayerScript.instance.weaponNumber--;
                if (PlayerScript.instance.weaponNumber < 0) PlayerScript.instance.weaponNumber = PlayerScript.instance.inventory.Count - 1;
            }
            PlayerScript.instance.EquipWeapon(PlayerScript.instance.inventory[PlayerScript.instance.weaponNumber]);
            PlayerScript.instance.HideInventoryItemDetailUI();
        }

        if (Input.GetKeyDown(KeyCode.Tab)) PlayerScript.instance.ToggleInventoryView();
        if (Input.GetKeyDown(KeyCode.E)) Interact();
        if (Input.GetKeyDown(KeyCode.Q)) Choosing();
        if (isBuffOpen && Input.GetKeyDown(KeyCode.Space)) BuffController.instance.Reroll();


        //cheat
        if (Input.GetKeyDown(KeyCode.M)) PlayerScript.instance.UpdateCash(1000);
        if (Input.GetKeyDown(KeyCode.L)) PlayerScript.instance.UpdateExperience(50);
        if (Input.GetKeyDown(KeyCode.H)) PlayerScript.instance.UpdateHealth(PlayerScript.instance.maxHealth);
        if (Input.GetKeyDown(KeyCode.T)) SkipTutorial();
    }

    private void Exit()
    {
        AudioManager.instance.PlaySFX(AudioManager.instance.menuOpen);
        if (PlayerScript.instance.CanSeeShop || PlayerScript.instance.CanSeeInventory)
        {
            PlayerScript.instance.TurnOffAllViews();
            isOverUI = false;
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

        deathScreen.SetActive(true);
    }

    public void Quit()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void Retry()
    {
        SceneManager.LoadScene("Game");
    }

    public void CursorIsOverUI()
    {
        isOverUI = true;
    }

    public void CursorNotOverUI()
    {
        isOverUI = false;
    }

    public void StartTutorial()
    {
        StartCoroutine(Tutorial());
    }

    IEnumerator Typewriter(string text)
    {
        tutorialDialogue.GetComponent<TMP_Text>().text = "";
        var waitTimer = new WaitForSeconds(.05f);
        foreach (char c in text)
        {
            tutorialDialogue.GetComponent<TMP_Text>().text += c;
            yield return waitTimer;
        }
        yield return new WaitForSeconds(0.5f);
    }

    IEnumerator Tutorial()
    {
        tutorialDialogue.SetActive(true);
        yield return StartCoroutine(Typewriter("Good morning, soldier! Let's get you up to speed."));
        yield return StartCoroutine(Typewriter("First, try running around with WASD."));
        bool hasMoved = false;
        while (!hasMoved)
        {
            if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D))
            {
                hasMoved = true;
            }
            yield return null;
        }
        tutorialDialogue.SetActive(false);
        yield return new WaitForSeconds(1f);
        tutorialDialogue.SetActive(true);
        yield return StartCoroutine(Typewriter("Next, try pressing SHIFT to dodge."));
        bool hasDodged = false;
        while (!hasDodged)
        {
            if (PlayerScript.instance.currentState == PlayerScript.PLAYER_STATE.ROLLING)
            {
                hasDodged = true;
            }
            yield return null;
        }
        tutorialDialogue.SetActive(false);
        yield return new WaitForSeconds(1f);
        tutorialDialogue.SetActive(true);
        yield return StartCoroutine(Typewriter("You can press SHIFT in conjunction with any directional key to dodge in that direction."));
        yield return StartCoroutine(Typewriter("If timed well, dodging allows you to avoid taking enemy damage."));
        yield return StartCoroutine(Typewriter("Proceed over to the bridge."));
        tutorialBarrier1.enabled = false;
        while (!tutorialFlag1)
        {
            yield return null;
        }
        yield return StartCoroutine(Typewriter("The alien crabs on your left seem to be guarding a vending machine."));
        yield return StartCoroutine(Typewriter("You can attack them by pressing MOUSE1."));
        yield return StartCoroutine(Typewriter("Defeat the alien crabs in order to unlock access to the vending machine."));
        while (!tutorialFlag2)
        {
            yield return null;
        }
        yield return StartCoroutine(Typewriter("Amazing work!"));
        yield return StartCoroutine(Typewriter("When defeated, aliens drop cash, health, and experience points."));
        yield return StartCoroutine(Typewriter("Use the cash you've earned to purchase weapons at vending machines scattered across the map."));
        yield return StartCoroutine(Typewriter("Purchase a weapon from the vending machine."));
        while (PlayerScript.instance.inventory.Count == 0)
        {
            yield return false;
        }
        yield return StartCoroutine(Typewriter("A fine choice."));
        yield return StartCoroutine(Typewriter("Whenever you purchase a weapon, your current weapon will be moved into your inventory."));
        yield return StartCoroutine(Typewriter("You can press TAB to access your inventory."));
        bool hasPressed = false;
        while (!hasPressed)
        {
            if (PlayerScript.instance.CanSeeInventory)
            {
                hasPressed = true;
            }
            yield return null;
        }
        yield return StartCoroutine(Typewriter("Great!"));
        yield return StartCoroutine(Typewriter("It seems that you are ready to venture out into the wilderness."));
        yield return StartCoroutine(Typewriter("The aliens will only get stronger the further you journey out from here."));
        yield return StartCoroutine(Typewriter("However, there are some things you have to know before you can proceed. Pay attention!"));
        yield return StartCoroutine(Typewriter("The alien mothership has been spreading a poisonous gas throughout the planet."));
        yield return StartCoroutine(Typewriter("Once this gas covers the entire planet, it's game over."));
        yield return StartCoroutine(Typewriter("I estimate you only have about 10 minutes, so get going!"));
        GameObject announcement = Instantiate(Resources.Load<GameObject>("Prefabs/Announcement"), announcementContainer);
        announcement.GetComponent<AnnouncementScript>().SetText("GAME START!");
        tutorialBarrier2.enabled = false;
        yield return new WaitForSeconds(1f);
        tutorialDialogue.SetActive(false);
        PoisonGas.instance.StartGas();
    }

    private void SkipTutorial()
    {
        StopCoroutine(Tutorial());
        tutorialBarrier1.enabled = false;
        tutorialBarrier2.enabled = false;
        tutorialDialogue.SetActive(false);
        PoisonGas.instance.StartGas();
    }

    /*IEnumerator ItemHealth(Item itemData)
    {
        PlayerScript.instance.UpdateHealth(float.Parse(itemData.primaryValue));
        yield return null;
    }

    IEnumerator ItemStim(Item itemData)
    {
        this.stimCD = true;
        PlayerScript.instance.movementSpeed += float.Parse(itemData.primaryValue);
        float stimTimer = 0;
        while (stimTimer < float.Parse(itemData.secondaryValue))
        {
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
    }*/
}
