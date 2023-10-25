using System.Collections;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance { get; private set; }

    [Header("Global Audio Sources")]
    [SerializeField] public AudioSource musicSource;
    [SerializeField] AudioSource SFXSource;

    [Header("Global Audio Clips")]
    public AudioClip mainMenuMusic;

    //Unused
    public AudioClip beachZoneMusic;
    public AudioClip forestZoneMusic;
    public AudioClip cityZoneMusic;
    //

    public AudioClip ambientMusic;
    public AudioClip mediumCombatMusic;
    public AudioClip endMediumCombat;
    public AudioClip menuOpen;
    public AudioClip menuClose;
    public AudioClip shopEntered;
    public AudioClip shopPurchase;
    public AudioClip shopSell;
    public AudioClip buttonPressed;
    public AudioClip buffReroll;
    public AudioClip actionFailed;
    public AudioClip levelUp;
    public AudioClip zoneUnlocked;

    [Header("Threat Level")]
    public int threatLevel;
    private enum Threat
    {
        None,
        Active
    }
    private Threat threat;
    public bool isPlayingCombat = false;
    public AudioClip lastPlayedBGM;

    public AudioMixer audioMixer;
    private float BGMVol;
    private float EnemyWeaponVol;
    private float EffectsVol;

    private float timer = 0;
    private bool isPlayingSomething = true;

    /* Experimental */
    private float bpm = 140f;
    private float bps;
    private float timeTillNextBeat;



    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(this.gameObject);

        bps = bpm / 60f;
    }

    private void Start()
    {
        GetInitialMixerVol();
    }

    private void Update()
    {
        if (timer > 0)
        {
            timer -= Time.deltaTime;
        }

        if (!isPlayingSomething && timer <= 0)
        {
            isPlayingSomething = true;
            CrossFadeBGM(ambientMusic);
        }

        switch (threatLevel)
        {
            case int n when (n <= 0):
                threat = Threat.None;
                if (isPlayingCombat)
                {
                    timeTillNextBeat = musicSource.time % bps;
                    if (timeTillNextBeat < 0.1f) timeTillNextBeat = 0;
                    if (timeTillNextBeat == 0)
                    {
                        musicSource.Stop();
                        musicSource.clip = null;
                        musicSource.PlayOneShot(endMediumCombat);
                        isPlayingCombat = false;
                        isPlayingSomething = false;
                        timer = 10;
                    }
                }
                break;
            case int n when (n >= 5):
                threat = Threat.Active;
                if (!isPlayingCombat)
                {
                    timer = 0;
                    isPlayingSomething = true;
                    CrossFadeBGM(mediumCombatMusic);
                    isPlayingCombat = true;
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

    public void PlayBGM(AudioClip clip)
    {
        musicSource.clip = clip;
        musicSource.Play();
    }

    public void GetInitialMixerVol()
    {
        audioMixer.GetFloat("BGM", out BGMVol);
        audioMixer.GetFloat("EnemyWeapon", out EnemyWeaponVol);
        audioMixer.GetFloat("Effects", out EffectsVol);
    }

    public void SetInitialMixerVol()
    {
        audioMixer.SetFloat("BGM", BGMVol);
        audioMixer.SetFloat("EnemyWeapon", EnemyWeaponVol);
        audioMixer.SetFloat("Effects", EffectsVol);

    }

    public void LowerMixerVol()
    {
        audioMixer.SetFloat("EnemyWeapon", -80);
        audioMixer.SetFloat("Effects", -80);
    }

    /** Experimental **/
    public void CrossFadeBGM(AudioClip clip)
    {
        if (!isPlayingCombat)
        {
            DetermineLastPlayedBGM(musicSource.clip);
        }
        StartCoroutine(XFade(clip, 1f));
    }

    IEnumerator XFade(AudioClip clip, float volume)
    {

        ///Add new audiosource and set it to all parameters of original audiosource
        AudioSource fadeOutSource;
        if (gameObject.GetComponent<AudioSource>())
        {
            fadeOutSource = gameObject.GetComponent<AudioSource>();
        }
        else
        {
            fadeOutSource = gameObject.AddComponent<AudioSource>();
        }
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
            t = Mathf.Lerp(t, 1f, Time.deltaTime * 0.5f);
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
