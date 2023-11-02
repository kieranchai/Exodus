using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

public class CutsceneController : MonoBehaviour
{
    [SerializeField]
    private bool startCutscene;

    [SerializeField]
    private bool endCutscene;

    VideoPlayer video;

    void Awake()
    {
        video = GetComponent<VideoPlayer>();
        video.Play();
        video.loopPointReached += CheckOver;
    }

    void CheckOver(UnityEngine.Video.VideoPlayer vp)
    {
        if (startCutscene)
        {
            StartCoroutine(LoadStartTransition());
        }
        else if (endCutscene)
        {
            StartCoroutine(LoadEndTransition());
        }
    }

    IEnumerator LoadStartTransition()
    {
        yield return new WaitForSeconds(1f);
        SceneManager.LoadScene("Game");
    }

    IEnumerator LoadEndTransition()
    {
        yield return new WaitForSeconds(1f);
        SceneManager.LoadScene("Game");
        AudioManager.instance.SetInitialMixerVol();
        SceneManager.LoadScene("MainMenu");
    }
}
