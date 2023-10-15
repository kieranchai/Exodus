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
    public AudioClip menuOpen;
    public AudioClip shopEntered;
    public AudioClip shopPurchase;
    public AudioClip buffPressed;
    public AudioClip buffReroll;
    public AudioClip actionFailed;
    public AudioClip levelUp;

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
    }

    private void Start()
    {
        musicSource.clip = backgroundMusic;
        musicSource.Play();
    }

    public void PlaySFX(AudioClip clip)
    {
        SFXSource.PlayOneShot(clip);
    }

    /** Experimental **/
    public void CrossFadeBGM(AudioClip clip)
    {
        Fade(clip, 1f);
    }

    public void Fade(AudioClip clip, float volume)
    {
        StartCoroutine(FadeIt(clip, volume));
    }
    IEnumerator FadeIt(AudioClip clip, float volume)
    {

        ///Add new audiosource and set it to all parameters of original audiosource
        AudioSource fadeOutSource = gameObject.transform.Find("BGM").gameObject.AddComponent<AudioSource>();
        fadeOutSource.clip = GetComponent<AudioSource>().clip;
        fadeOutSource.time = GetComponent<AudioSource>().time;
        fadeOutSource.volume = GetComponent<AudioSource>().volume;
        fadeOutSource.outputAudioMixerGroup = GetComponent<AudioSource>().outputAudioMixerGroup;

        //make it start playing
        fadeOutSource.Play();

        //set original audiosource volume and clip
        GetComponent<AudioSource>().volume = 0f;
        GetComponent<AudioSource>().clip = clip;
        float t = 0;
        float v = fadeOutSource.volume;
        GetComponent<AudioSource>().Play();

        //begin fading in original audiosource with new clip as we fade out new audiosource with old clip
        while (t < 0.98f)
        {
            t = Mathf.Lerp(t, 1f, Time.deltaTime * 0.2f);
            fadeOutSource.volume = Mathf.Lerp(v, 0f, t);
            GetComponent<AudioSource>().volume = Mathf.Lerp(0f, volume, t);
            yield return null;
        }
        GetComponent<AudioSource>().volume = volume;
        //destroy the fading audiosource
        Destroy(fadeOutSource);
        yield break;
    }
    /** End Experimental **/
}
