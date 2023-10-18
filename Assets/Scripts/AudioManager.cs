using System.Collections;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance { get; private set; }

    [Header("Global Audio Sources")]
    [SerializeField] AudioSource musicSource;
    [SerializeField] AudioSource SFXSource;

    [Header("Global Audio Clips")]
    public AudioClip backgroundMusic;
    public AudioClip backgroundMusic2;
    public AudioClip menuOpen;
    public AudioClip menuClose;
    public AudioClip shopEntered;
    public AudioClip shopPurchase;
    public AudioClip shopSell;
    public AudioClip buffPressed;
    public AudioClip buffReroll;
    public AudioClip actionFailed;
    public AudioClip levelUp;
    public AudioClip zoneUnlocked;

    public int threatLevel;
    private enum Threat
    {
        None,
        Low,
        Medium,
        High
    }
    private Threat threat;
    private bool isPlayingCombat = false;
    public AudioClip lastPlayedBGM;

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(this.gameObject);
    }

    private void Start()
    {
        musicSource.clip = backgroundMusic;
        musicSource.Play();
    }

    private void Update()
    {
        switch (threatLevel)
        {
            case int n when (n <= 0):
                threat = Threat.None;
                if (isPlayingCombat)
                {
                    isPlayingCombat = false;
                    CrossFadeBGM(lastPlayedBGM);
                }
                break;
            case int n when (n > 0 && n < 5):
                threat = Threat.Low;
                if (!isPlayingCombat)
                {
                    isPlayingCombat = true;
                    /*CrossFadeBGM(lowCombat);*/
                }
                break;
            case int n when (n >= 5 && n < 8):
                threat = Threat.Medium;
                if (!isPlayingCombat)
                {
                    isPlayingCombat = true;
                    /*CrossFadeBGM(mediumCombat);*/
                }
                break;
            case int n when (n >= 8):
                threat = Threat.High;
                if (!isPlayingCombat)
                {
                    isPlayingCombat = true;
                    /*CrossFadeBGM(highCombat);*/
                }
                break;
            default:
                break;
        }
    }

    public void DetermineLastPlayedBGM(AudioClip clip)
    {
        lastPlayedBGM = clip;
    }

    public void PlaySFX(AudioClip clip)
    {
        SFXSource.PlayOneShot(clip);
    }

    /** Experimental **/
    public void CrossFadeBGM(AudioClip clip)
    {
        Fade(clip, 1f);
        if(!isPlayingCombat)
        {
            DetermineLastPlayedBGM(clip);
        }
    }

    public void Fade(AudioClip clip, float volume)
    {
        StartCoroutine(FadeIt(clip, volume));
    }
    IEnumerator FadeIt(AudioClip clip, float volume)
    {

        ///Add new audiosource and set it to all parameters of original audiosource
        AudioSource fadeOutSource = gameObject.AddComponent<AudioSource>();
        fadeOutSource.clip = gameObject.transform.Find("BGM").gameObject.GetComponent<AudioSource>().clip;
        fadeOutSource.time = gameObject.transform.Find("BGM").gameObject.GetComponent<AudioSource>().time;
        fadeOutSource.volume = gameObject.transform.Find("BGM").gameObject.GetComponent<AudioSource>().volume;
        fadeOutSource.outputAudioMixerGroup = gameObject.transform.Find("BGM").gameObject.GetComponent<AudioSource>().outputAudioMixerGroup;

        //make it start playing
        fadeOutSource.Play();

        //set original audiosource volume and clip
        gameObject.transform.Find("BGM").gameObject.GetComponent<AudioSource>().volume = 0f;
        gameObject.transform.Find("BGM").gameObject.GetComponent<AudioSource>().clip = clip;
        float t = 0;
        float v = fadeOutSource.volume;
        gameObject.transform.Find("BGM").gameObject.GetComponent<AudioSource>().Play();

        //begin fading in original audiosource with new clip as we fade out new audiosource with old clip
        while (t < 0.98f)
        {
            t = Mathf.Lerp(t, 1f, Time.deltaTime * 0.2f);
            fadeOutSource.volume = Mathf.Lerp(v, 0f, t);
            gameObject.transform.Find("BGM").gameObject.GetComponent<AudioSource>().volume = Mathf.Lerp(0f, volume, t);
            yield return null;
        }
        gameObject.transform.Find("BGM").gameObject.GetComponent<AudioSource>().volume = volume;
        //destroy the fading audiosource
        Destroy(fadeOutSource);
        yield break;
    }
    /** End Experimental **/
}
