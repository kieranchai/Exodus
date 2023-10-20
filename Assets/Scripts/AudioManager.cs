using System.Collections;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance { get; private set; }

    [Header("Global Audio Sources")]
    [SerializeField] AudioSource musicSource;
    [SerializeField] AudioSource SFXSource;

    [Header("Global Audio Clips")]
    public AudioClip mainMenuMusic;
    public AudioClip beachZoneMusic;
    public AudioClip forestZoneMusic;
    public AudioClip cityZoneMusic;
    public AudioClip lowCombatMusic;
    public AudioClip mediumCombatMusic;
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
        Low,
        Medium,
        High
    }
    private Threat threat;
    public bool isPlayingCombat = false;
    public AudioClip lastPlayedBGM;

    public AudioMixer audioMixer;
    private float BGMVol;
    private float EnemyWeaponVol;
    private float EffectsVol;

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
        GetInitialMixerVol();
        musicSource.clip = mainMenuMusic;
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
                    CrossFadeBGM(lastPlayedBGM);
                    isPlayingCombat = false;
                }
                break;
            case int n when (n > 0 && n < 8):
                threat = Threat.Low;
                if (!isPlayingCombat)
                {
                    CrossFadeBGM(lowCombatMusic);
                    isPlayingCombat = true;
                }
                else if (musicSource.clip != lowCombatMusic)
                {
                    CrossFadeBGM(lowCombatMusic);
                }
                break;
            case int n when (n >= 8):
                threat = Threat.Medium;
                if (!isPlayingCombat)
                {
                    isPlayingCombat = true;
                    CrossFadeBGM(mediumCombatMusic);
                }
                else if (musicSource.clip != mediumCombatMusic)
                {
                    CrossFadeBGM(mediumCombatMusic);
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

    public void CrossFadeBGM(AudioClip clip)
    {
        if (!isPlayingCombat)
        {
            DetermineLastPlayedBGM(musicSource.clip);
        }
        musicSource.clip = clip;
        musicSource.Play();
    }
}
