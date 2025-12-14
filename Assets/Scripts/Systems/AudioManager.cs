using UnityEngine;
using System.Collections.Generic;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }
    
    [Header("Audio Sources")]
    [Tooltip("AudioSource a háttérzenéhez")]
    public AudioSource musicSource;
    
    [Tooltip("AudioSource pool a hangeffektekhez")]
    public AudioSource sfxSource;
    
    [Header("Music")]
    [Tooltip("Háttérzene")]
    public AudioClip backgroundMusic;
    
    [Header("Sound Effects")]
    [Tooltip("Lépés hang")]
    public AudioClip footstepSound;
    
    [Tooltip("Támadás hang")]
    public AudioClip attackSound;
    
    [Tooltip("Sebzés hang")]
    public AudioClip hitSound;
    
    [Tooltip("Tárgygyűjtés hang")]
    public AudioClip collectSound;
    
    [Tooltip("Szintlépés hang")]
    public AudioClip levelUpSound;
    
    [Tooltip("Harc kezdés hang")]
    public AudioClip combatStartSound;
    
    [Tooltip("Harc vége hang")]
    public AudioClip combatEndSound;
    
    [Tooltip("UI gomb hang")]
    public AudioClip buttonClickSound;
    
    [Header("Volume Settings")]
    [Range(0f, 1f)]
    public float masterVolume = 1f;
    
    [Range(0f, 1f)]
    public float musicVolume = 1f;
    
    [Range(0f, 1f)]
    public float sfxVolume = 1f;
    
    private const string MASTER_VOLUME_KEY = "MasterVolume";
    private const string MUSIC_VOLUME_KEY = "MusicVolume";
    private const string SFX_VOLUME_KEY = "SFXVolume";
    
    private float lastFootstepTime = 0f;
    private const float FOOTSTEP_INTERVAL = 0.3f; // Lépések közötti minimális idő
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeAudioSources();
            LoadVolumeSettings();
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }
    
    private void Start()
    {
        PlayBackgroundMusic();
    }
    
    private void InitializeAudioSources()
    {
        // Music source inicializálása
        if (musicSource == null)
        {
            GameObject musicObj = new GameObject("MusicSource");
            musicObj.transform.SetParent(transform);
            musicSource = musicObj.AddComponent<AudioSource>();
            musicSource.loop = true;
            musicSource.playOnAwake = false;
        }
        
        // SFX source inicializálása
        if (sfxSource == null)
        {
            GameObject sfxObj = new GameObject("SFXSource");
            sfxObj.transform.SetParent(transform);
            sfxSource = sfxObj.AddComponent<AudioSource>();
            sfxSource.playOnAwake = false;
        }
    }
    
    private void LoadVolumeSettings()
    {
        masterVolume = PlayerPrefs.GetFloat(MASTER_VOLUME_KEY, 1f);
        musicVolume = PlayerPrefs.GetFloat(MUSIC_VOLUME_KEY, 1f);
        sfxVolume = PlayerPrefs.GetFloat(SFX_VOLUME_KEY, 1f);
        
        UpdateVolumes();
    }
    
    public void SetMasterVolume(float volume)
    {
        masterVolume = Mathf.Clamp01(volume);
        PlayerPrefs.SetFloat(MASTER_VOLUME_KEY, masterVolume);
        PlayerPrefs.Save();
        UpdateVolumes();
    }
    
    public void SetMusicVolume(float volume)
    {
        musicVolume = Mathf.Clamp01(volume);
        PlayerPrefs.SetFloat(MUSIC_VOLUME_KEY, musicVolume);
        PlayerPrefs.Save();
        UpdateVolumes();
    }
    
    public void SetSFXVolume(float volume)
    {
        sfxVolume = Mathf.Clamp01(volume);
        PlayerPrefs.SetFloat(SFX_VOLUME_KEY, sfxVolume);
        PlayerPrefs.Save();
        UpdateVolumes();
    }
    
    private void UpdateVolumes()
    {
        if (musicSource != null)
        {
            musicSource.volume = masterVolume * musicVolume;
        }
        
        if (sfxSource != null)
        {
            sfxSource.volume = masterVolume * sfxVolume;
        }
    }
    
    public void PlayBackgroundMusic()
    {
        if (musicSource != null && backgroundMusic != null)
        {
            musicSource.clip = backgroundMusic;
            musicSource.volume = masterVolume * musicVolume;
            musicSource.Play();
        }
    }
    
    public void StopBackgroundMusic()
    {
        if (musicSource != null)
        {
            musicSource.Stop();
        }
    }
    
    public void PlaySFX(AudioClip clip, float volumeMultiplier = 1f)
    {
        if (sfxSource != null && clip != null)
        {
            float volume = masterVolume * sfxVolume * volumeMultiplier;
            sfxSource.PlayOneShot(clip, volume);
        }
    }
    
    // Konkrét hangok lejátszása
    public void PlayFootstep()
    {
        // Lépéshangok gyakoriságának korlátozása
        if (Time.time - lastFootstepTime < FOOTSTEP_INTERVAL)
        {
            return;
        }
        
        lastFootstepTime = Time.time;
        PlaySFX(footstepSound, 0.5f); // Kisebb hangerő
    }
    
    public void PlayAttack()
    {
        PlaySFX(attackSound);
    }
    
    public void PlayHit()
    {
        PlaySFX(hitSound);
    }
    
    public void PlayCollect()
    {
        PlaySFX(collectSound);
    }
    
    public void PlayLevelUp()
    {
        PlaySFX(levelUpSound);
    }
    
    public void PlayCombatStart()
    {
        PlaySFX(combatStartSound);
    }
    
    public void PlayCombatEnd()
    {
        PlaySFX(combatEndSound);
    }
    
    public void PlayButtonClick()
    {
        PlaySFX(buttonClickSound, 0.7f);
    }
}

