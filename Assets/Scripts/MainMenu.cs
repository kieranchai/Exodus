using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.G)) LoadGame();
    }
    public void LoadGame()
    {
        Time.timeScale = 1.0f;
        SceneManager.LoadScene("Game");
    }

    public void LoadScene()
    {
        Time.timeScale = 1.0f;
        SceneManager.LoadScene("Tutorial");
    }

    public void ShowControls()
    {
        SceneManager.LoadScene("Controls");
    }

    public void ShowMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void Quit()
    {
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
        Application.Quit();
    }
}
