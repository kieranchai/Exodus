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
    public BoxCollider2D[] Zones;

    private bool tutorialStarted = false;
    private bool allowShooting = false;
    [SerializeField] private GameObject tutorialDialogueBox;
    [SerializeField] private TMP_Text tutorialDialogueText;
    [SerializeField] private Collider2D area1Collider;
    public bool passedArea2 = false;
    public bool dummyShot = false;

    public enum GAME_STATE
    {
        PANNING,
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

    private bool hasLoadedEndCutscene = false;

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
            case GAME_STATE.TUTORIAL:
                StartTutorial();
                if (tutorialStarted) HandleTutorialInput();
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

    public void ResumeGame()
    {
        CloseControls();
        pauseScreen.SetActive(false);
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
            if (PlayerScript.instance.equippedWeapon && PlayerScript.instance.equippedWeapon.weaponType == "akimbo")
            {
                PlayerScript.instance.akimboSlot.TryAttack();
            }
        }
        if (Input.GetKeyDown(KeyCode.Tab)) PlayerScript.instance.ToggleInventoryView();
        if (Input.GetKeyDown(KeyCode.E)) Interact();

        //cheat
        if (Input.GetKeyDown(KeyCode.M)) PlayerScript.instance.UpdateCash(1000);
        if (Input.GetKeyDown(KeyCode.L)) PlayerScript.instance.UpdateExperience(10);
        if (Input.GetKeyDown(KeyCode.H)) PlayerScript.instance.UpdateHealth(PlayerScript.instance.maxHealth);
    }

    private void StartTutorial()
    {
        if (tutorialStarted) return;
        tutorialStarted = true;

        StartCoroutine(Tutorial());
    }

    private void HandleTutorialInput()
    {
        PlayerScript.instance.LookAtMouse();

        //Movement Input WASD+LEFT SHIFT is in PlayerScript Update/FixedUpdate

        if (allowShooting)
        {
            if (Input.GetMouseButton(0) && !isOverUI && PlayerScript.instance.currentState != PlayerScript.PLAYER_STATE.ROLLING)
            {
                PlayerScript.instance.weaponSlot.TryAttack();
                if (PlayerScript.instance.equippedWeapon && PlayerScript.instance.equippedWeapon.weaponType == "akimbo")
                {
                    PlayerScript.instance.akimboSlot.TryAttack();
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.Tab)) PlayerScript.instance.ToggleInventoryView();
        if (Input.GetKeyDown(KeyCode.E)) Interact();
    }

    IEnumerator Tutorial()
    {
        tutorialDialogueBox.SetActive(true);
        yield return StartCoroutine(Typewriter("Good morning, soldier! Welcome to your training."));
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
        tutorialDialogueBox.SetActive(false);
        yield return new WaitForSeconds(1f);
        tutorialDialogueBox.SetActive(true);
        yield return StartCoroutine(Typewriter("Amazing work!"));
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
        tutorialDialogueBox.SetActive(false);
        yield return new WaitForSeconds(1f);
        tutorialDialogueBox.SetActive(true);
        yield return StartCoroutine(Typewriter("Bravo!"));
        yield return StartCoroutine(Typewriter("If timed well, dodging allows you to avoid taking enemy damage."));
        yield return StartCoroutine(Typewriter("You can only dodge once every 5 seconds, so use your dodges carefully!"));
        yield return StartCoroutine(Typewriter("Proceed over to the next area."));
        area1Collider.enabled = false;
        while (!passedArea2)
        {
            yield return null;
        }
        tutorialDialogueBox.SetActive(false);
        yield return new WaitForSeconds(1f);
        tutorialDialogueBox.SetActive(true);
        yield return StartCoroutine(Typewriter("Good work."));
        PlayerScript.instance.AddToInventory(Resources.Load<Weapon>("ScriptableObjects/Weapons/Light Pistol"));
        yield return StartCoroutine(Typewriter("A weapon has been given to you."));
        yield return StartCoroutine(Typewriter("Press TAB to open your inventory."));
        bool hasPressed = false;
        while (!hasPressed)
        {
            if (PlayerScript.instance.CanSeeInventory)
            {
                hasPressed = true;
            }
            yield return null;
        }
        tutorialDialogueBox.SetActive(false);
        yield return new WaitForSeconds(1f);
        tutorialDialogueBox.SetActive(true);
        yield return StartCoroutine(Typewriter("Now, select the gun and equip it."));
        bool hasDone = false;
        while (!hasDone)
        {
            if (PlayerScript.instance.equippedWeapon == Resources.Load<Weapon>("ScriptableObjects/Weapons/Heavy Pistol"))
            {
                hasDone = true;
            }
            yield return null;
        }
        tutorialDialogueBox.SetActive(false);
        yield return new WaitForSeconds(1f);
        tutorialDialogueBox.SetActive(true);
        yield return StartCoroutine(Typewriter("You're a natural!"));
        yield return StartCoroutine(Typewriter("Oops, it seems that you need bullets."));
        yield return StartCoroutine(Typewriter("Notice that there are 3 different types of ammo below your minimap."));
        yield return StartCoroutine(Typewriter("Different weapons will require different types of ammo."));
        yield return StartCoroutine(Typewriter("You can purchase ammo from the shop."));
        yield return StartCoroutine(Typewriter("Try opening the shop panel by pressing E near the shop."));
        PlayerScript.instance.UpdateCash(150);
        hasDone = false;
        while (!hasDone)
        {
            if (PlayerScript.instance.CanSeeShop)
            {
                hasDone = true;
            }
            yield return null;
        }
        tutorialDialogueBox.SetActive(false);
        yield return new WaitForSeconds(1f);
        tutorialDialogueBox.SetActive(true);
        yield return StartCoroutine(Typewriter("You've been given some cash."));
        yield return StartCoroutine(Typewriter("Cash can only be earned by killing aliens, and can only be used at the shop to purchase weapons and items."));
        yield return StartCoroutine(Typewriter("Use the cash to purchase ammo for your weapon."));
        hasDone = false;
        while (!hasDone)
        {
            if (PlayerScript.instance.cash != 150)
            {
                hasDone = true;
            }
            yield return null;
        }
        tutorialDialogueBox.SetActive(false);
        yield return new WaitForSeconds(1f);
        tutorialDialogueBox.SetActive(true);
        yield return StartCoroutine(Typewriter("Great."));
        yield return StartCoroutine(Typewriter("Press MOUSE1 to shoot the target dummy in front of you"));
        yield return StartCoroutine(Typewriter("You can try unequipping your weapon and pressing MOUSE1 to punch as well."));
        allowShooting = true;
        hasDone = false;
        while (!hasDone)
        {
            if (this.dummyShot)
            {
                hasDone = true;
            }
            yield return null;
        }
        tutorialDialogueBox.SetActive(false);
        yield return new WaitForSeconds(1f);
        tutorialDialogueBox.SetActive(true);
        yield return StartCoroutine(Typewriter("Awesome!"));
        yield return new WaitForSeconds(2f);
        yield return StartCoroutine(Typewriter("You might be wondering what the bar on the left of your minimap represents."));
        yield return StartCoroutine(Typewriter("Well... it represents the progress of the poisonous gas."));
        yield return StartCoroutine(Typewriter("The closer the gas is to the end of the bar, the closer to Earth's doom!"));
        yield return StartCoroutine(Typewriter("Once the gas reaches the end, you will be unable to buy anything from the shop."));
        yield return StartCoroutine(Typewriter("Luckily, you can stop it, and save Earth, by finding and destroying the alien mothership responsible for spreading the poisonous gas."));
        yield return new WaitForSeconds(1f);
        yield return StartCoroutine(Typewriter("What are you waiting for? Tutorial's over, now time to save Earth!"));
        yield return new WaitForSeconds(1f);
        tutorialDialogueBox.SetActive(false);
        SceneManager.LoadScene("Game");
        yield return null;
    }

    IEnumerator Typewriter(string text)
    {
        tutorialDialogueText.text = "";
        var waitTimer = new WaitForSeconds(.05f);
        foreach (char c in text)
        {
            tutorialDialogueText.text += c;
            yield return waitTimer;
        }
        yield return new WaitForSeconds(0.5f);
    }


    private void Exit()
    {
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

    public bool UseItem(Item itemData)
    {
        switch (itemData.type)
        {
            case "SMALL HEALTH":
                if (PlayerScript.instance.currentHealth == PlayerScript.instance.maxHealth)
                {
                    PlayerScript.instance.AlertPopup("health");
                    return false;
                }
                PlayerScript.instance.audioSource.clip = Resources.Load<AudioClip>($"Audio/Bandage");
                PlayerScript.instance.audioSource.Play();
                StartCoroutine(ItemHealth(itemData));
                break;
            case "BIG HEALTH":
                if (PlayerScript.instance.currentHealth == PlayerScript.instance.maxHealth)
                {
                    PlayerScript.instance.AlertPopup("health");
                    return false;
                }
                PlayerScript.instance.audioSource.clip = Resources.Load<AudioClip>($"Audio/Med Kit");
                PlayerScript.instance.audioSource.Play();
                StartCoroutine(ItemHealth(itemData));
                break;
            case "MOVEMENT SPEED":
                if (this.stimCD)
                {
                    PlayerScript.instance.AlertPopup("speed");
                    return false;
                }
                StartCoroutine(ItemStim(itemData));
                break;
            case "GAS":
                if (PoisonGas.instance.hasReached)
                {
                    PlayerScript.instance.AlertPopup("gasEnded");
                    return false;
                }
                else if (PoisonGas.instance.itemUsed)
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
    }
}
