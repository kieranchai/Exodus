using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public Animator animator;

    [SerializeField]
    private GameObject mainMenu;

    [SerializeField]
    private GameObject controlsMenu;

    private void Start()
    {
        AudioManager.instance.PlayBGM(AudioManager.instance.mainMenuMusic);
    }

    public void LoadGame()
    {
        Time.timeScale = 1.0f;
        AudioManager.instance.PlaySFX(AudioManager.instance.buttonPressed);
        StartCoroutine(LoadTransition());
    }

    IEnumerator LoadTransition()
    {
        animator.SetTrigger("Start");
        yield return new WaitForSeconds(1f);
        SceneManager.LoadScene("StartCutscene");
        AudioManager.instance.musicSource.Pause();
    }

    public void ShowControls()
    {
        AudioManager.instance.PlaySFX(AudioManager.instance.buttonPressed);
        mainMenu.SetActive(false);
        controlsMenu.SetActive(true);
    }

    public void HideControls()
    {
        AudioManager.instance.PlaySFX(AudioManager.instance.buttonPressed);
        controlsMenu.SetActive(false);
        mainMenu.SetActive(true);
    }

    public void Quit()
    {
        AudioManager.instance.PlaySFX(AudioManager.instance.buttonPressed);
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
        Application.Quit();
    }
}
