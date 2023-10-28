using System.Collections;
using UnityEngine;
using System;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.Audio;

public class GameController : MonoBehaviour
{
    public static GameController instance { get; private set; }
    public bool isOverUI = false;
    public Collider2D[] Zones;
    public AudioMixer AudioMixer;

    public enum GAME_STATE
    {
        PANNING,
        PAUSED,
        PLAYING,
        DEAD,
        WIN
    }

    public GAME_STATE currentState = GAME_STATE.PLAYING;

    public Animator animator;

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

    private bool hasLoadedEndCutscene = false;

    private bool isBuffOpen = false;

    private float timer = 0;
    private int timePlayed = 0;
    public bool flameThrowerEquipped = false;
    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;

        foreach (Equipment equipment in Resources.LoadAll<Equipment>("ScriptableObjects/Equipment"))
        {
            equipment.currentCooldown = 0;
        }
    }

    private void Start()
    {
        if (AudioManager.instance.musicSource.clip != AudioManager.instance.ambientMusic) AudioManager.instance.PlayBGM(AudioManager.instance.ambientMusic);
    }

    private void Update()
    {
        switch (currentState)
        {
            case GAME_STATE.PANNING:
                break;
            case GAME_STATE.PLAYING:
                HandleInput();
                timer += Time.deltaTime;
                timePlayed = (int)timer;
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
        AudioManager.instance.LowerMixerVol();
        AudioManager.instance.musicSource.Pause();
        if (AudioManager.instance.GetComponent<AudioSource>())
        {
            AudioManager.instance.GetComponent<AudioSource>().Pause();
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ResumeGame();
            AudioManager.instance.PlaySFX(AudioManager.instance.menuClose);
        }
    }

    private void Choosing()
    {
        AudioManager.instance.PlaySFX(AudioManager.instance.menuOpen);
        isBuffOpen = !isBuffOpen;
        buffSelectionPanel.SetActive(isBuffOpen);
        if (isBuffOpen == true) {
            BuffController.instance.UpdatePlayerTokensDisplay();
            PlayerScript.instance.playerPanel.transform.Find("Equipped Weapon").gameObject.SetActive(false);
            PlayerScript.instance.playerPanel.transform.Find("Equipment").gameObject.SetActive(false);
        }
        if (isBuffOpen == false) {
            PlayerScript.instance.playerPanel.transform.Find("Equipped Weapon").gameObject.SetActive(true);
            PlayerScript.instance.playerPanel.transform.Find("Equipment").gameObject.SetActive(true);
            BuffController.instance.HideBuffDetails();
        }
    }

    public void ResumeGame()
    {
        AudioManager.instance.SetInitialMixerVol();
        AudioManager.instance.musicSource.Play();
        if (AudioManager.instance.GetComponent<AudioSource>())
        {
            AudioManager.instance.GetComponent<AudioSource>().Play();
        }
        CloseControls();
        pauseScreen.SetActive(false);
        Time.timeScale = 1.0f;
        currentState = GAME_STATE.PLAYING;
    }

    public void ShowControls()
    {
        AudioManager.instance.PlaySFX(AudioManager.instance.buttonPressed);
        pauseScreen.transform.Find("Pause Menu").gameObject.SetActive(false);
        pauseScreen.transform.Find("Controls Menu").gameObject.SetActive(true);
    }

    public void CloseControls()
    {
        AudioManager.instance.PlaySFX(AudioManager.instance.buttonPressed);
        pauseScreen.transform.Find("Controls Menu").gameObject.SetActive(false);
        pauseScreen.transform.Find("Pause Menu").gameObject.SetActive(true);
    }

    private void HandleInput()
    {
        PlayerScript.instance.LookAtMouse();

        //Movement Input WASD+LEFT SHIFT is in PlayerScript Update/FixedUpdate

        if (Input.GetKeyDown(KeyCode.Escape)) Exit();
        if (Input.GetMouseButton(0) && !isOverUI && PlayerScript.instance.currentState != PlayerScript.PLAYER_STATE.ROLLING)
        {
            if (flameThrowerEquipped)
            {
                PlayerScript.instance.weaponSlot.TryFlameActive();
            }
            else
            {
                PlayerScript.instance.weaponSlot.TryAttack();
            }
        }

        if (Input.GetMouseButtonUp(0) && !isOverUI && PlayerScript.instance.currentState != PlayerScript.PLAYER_STATE.ROLLING && flameThrowerEquipped)
        {
            PlayerScript.instance.weaponSlot.flameDisable();
        }

        if (Input.GetKeyDown(KeyCode.F) && PlayerScript.instance.equippedEquipment)
        {
            PlayerScript.instance.UseEquipment();
        }

        if (Input.GetKeyDown(KeyCode.R) && PlayerScript.instance.weaponSlot.weaponType != "melee" && PlayerScript.instance.equippedWeapon.currentAmmoCount < PlayerScript.instance.weaponSlot.clipSize && PlayerScript.instance.currentState != PlayerScript.PLAYER_STATE.ROLLING)
        {
            PlayerScript.instance.weaponSlot.isReloading = true;
            PlayerScript.instance.weaponSlot.StartCoroutine(PlayerScript.instance.weaponSlot.Reload());
            PlayerScript.instance.weaponSlot.flameDisable();
        }

        if (Input.GetAxis("Mouse ScrollWheel") != 0 && !isOverUI && PlayerScript.instance.inventory.Count > 0)
        {
            if (PlayerScript.instance.inventory.Count == 1 && PlayerScript.instance.weaponNumber == PlayerScript.instance.inventory.Count - 1) return;
            if (Input.GetAxis("Mouse ScrollWheel") > 0)
            {
                PlayerScript.instance.weaponSlot.flameDisable();
                PlayerScript.instance.weaponNumber++;
                if (PlayerScript.instance.weaponNumber >= PlayerScript.instance.inventory.Count) PlayerScript.instance.weaponNumber = 0;
            }
            if (Input.GetAxis("Mouse ScrollWheel") < 0)
            {
                PlayerScript.instance.weaponSlot.flameDisable();
                PlayerScript.instance.weaponNumber--;
                if (PlayerScript.instance.weaponNumber < 0) PlayerScript.instance.weaponNumber = PlayerScript.instance.inventory.Count - 1;
            }
            PlayerScript.instance.EquipWeapon(PlayerScript.instance.inventory[PlayerScript.instance.weaponNumber]);
            PlayerScript.instance.HideInventoryItemDetailUI();
        }

        if (Input.GetKeyDown(KeyCode.Tab)) PlayerScript.instance.ToggleInventoryView();
        if (Input.GetKeyDown(KeyCode.E)) Interact();
        if (Input.GetKeyDown(KeyCode.Q)) Choosing();
        if (isBuffOpen)
        {
            if (Input.GetKeyDown(KeyCode.Space)) BuffController.instance.Reroll();
            if (Input.GetKeyDown(KeyCode.Alpha1)) BuffController.instance.SelectBuff(BuffController.instance.availableBuffs[0]);
            if (Input.GetKeyDown(KeyCode.Alpha2)) BuffController.instance.SelectBuff(BuffController.instance.availableBuffs[1]);
            if (Input.GetKeyDown(KeyCode.Alpha3)) BuffController.instance.SelectBuff(BuffController.instance.availableBuffs[2]);
        }


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
        int minutes = Mathf.FloorToInt(timePlayed / 60F);
        int seconds = Mathf.FloorToInt(timePlayed - minutes * 60);
        pauseScreen.transform.Find("Pause Menu").Find("Playtime").GetComponent<TMP_Text>().text = string.Format("{0:0}:{1:00}", minutes, seconds); ;
        pauseScreen.transform.Find("Pause Menu").Find("Level").GetComponent<TMP_Text>().text = PlayerScript.instance.level.ToString();
        pauseScreen.transform.Find("Pause Menu").Find("Kills").GetComponent<TMP_Text>().text = PlayerScript.instance.kills.ToString();
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
        AudioManager.instance.LowerMixerVol();
        int minutes = Mathf.FloorToInt(timePlayed / 60F);
        int seconds = Mathf.FloorToInt(timePlayed - minutes * 60);
        deathScreen.transform.Find("Playtime").GetComponent<TMP_Text>().text = string.Format("{0:0}:{1:00}", minutes, seconds); ;
        deathScreen.transform.Find("Level").GetComponent<TMP_Text>().text = PlayerScript.instance.level.ToString();
        deathScreen.transform.Find("Kills").GetComponent<TMP_Text>().text = PlayerScript.instance.kills.ToString();
        deathScreen.SetActive(true);
    }

    public void Quit()
    {
        AudioManager.instance.PlaySFX(AudioManager.instance.buttonPressed);
        StartCoroutine(LoadTransition());
    }

    IEnumerator LoadTransition()
    {
        animator.SetTrigger("Start");
        yield return new WaitForSecondsRealtime(1f);
        AudioManager.instance.SetInitialMixerVol();
        SceneManager.LoadScene("MainMenu");
    }

    public void Retry()
    {
        AudioManager.instance.PlaySFX(AudioManager.instance.buttonPressed);
        AudioManager.instance.SetInitialMixerVol();
        Time.timeScale = 1.0f;
        currentState = GAME_STATE.PLAYING;
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
        var waitTimer = new WaitForSeconds(.03f);
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
}
