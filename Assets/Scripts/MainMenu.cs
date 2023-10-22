using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{

    [SerializeField]
    private GameObject mainMenu;

    [SerializeField]
    private GameObject controlsMenu;

    public void LoadGame()
    {
        Time.timeScale = 1.0f;
        AudioManager.instance.PlaySFX(AudioManager.instance.buttonPressed);
        SceneManager.LoadScene("Game");
        AudioManager.instance.PlayBGM(AudioManager.instance.ambientMusic);
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
