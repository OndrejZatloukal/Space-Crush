using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance = null;

    public float masterVolume;
    public AudioClip[] backgroundMusicClips;
    public float backgroundMusicOffset;

    public AudioClip playerShotClip;
    public float playerShotVolumeOffset;
    public float turretShotPitchOffset;

    public AudioClip playerExplosionClip;

    public AudioClip enemyShotClip;
    public float enemyShotVolumeOffset;

    private AudioSource backgroundMusic;

    private AudioSource playerShot;
    private AudioSource turretShot;

    private AudioSource[] enemySounds = new AudioSource[3];
    private AudioSource[] explosions = new AudioSource[3];
    private AudioSource[] powerupSounds = new AudioSource[2];

    private AudioSource[] allAudioSources;

    private int enemySoundSource;
    private int explosionSoundSource;

    private IEnumerator coroutine;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject);

        // add AudioSources
        backgroundMusic = gameObject.AddComponent<AudioSource>();

        playerShot = gameObject.AddComponent<AudioSource>();
        turretShot = gameObject.AddComponent<AudioSource>();

        for (int i = 0; i < enemySounds.Length; ++i)
        {
            enemySounds[i] = gameObject.AddComponent<AudioSource>();
        }

        for (int i = 0; i < explosions.Length; ++i)
        {
            explosions[i] = gameObject.AddComponent<AudioSource>();
        }

        powerupSounds[0] = gameObject.AddComponent<AudioSource>();
        powerupSounds[1] = gameObject.AddComponent<AudioSource>();

        allAudioSources = GetComponents<AudioSource>();

        // initialize AudioSources
        backgroundMusic.loop = true;

        foreach (AudioSource audioSource in allAudioSources)
        {
            audioSource.volume = masterVolume;
            audioSource.playOnAwake = false;
        }

        backgroundMusic.volume *= backgroundMusicOffset;

        playerShot.clip = playerShotClip;
        playerShot.volume *= playerShotVolumeOffset;

        turretShot.clip = playerShotClip;
        turretShot.volume *= playerShotVolumeOffset;
        turretShot.pitch += turretShotPitchOffset;

        foreach (AudioSource enemySound in enemySounds)
        {
            enemySound.clip = enemyShotClip;
            enemySound.volume *= enemyShotVolumeOffset;
        }
        enemySoundSource = 0;
    }

    void OnEnable()
    {
        // subscribe to delegate
        SceneManager.sceneLoaded += OnLevelLoaded;
    }

    void OnDisable()
    {
        // unsubscribe from delegate
        SceneManager.sceneLoaded += OnLevelLoaded;
    }
	
    // subscribes to delegate
	void OnLevelLoaded(Scene scene, LoadSceneMode mode)
    {
        if (coroutine != null)
        {
            StopCoroutine(coroutine);
        }

        int levelType = System.Convert.ToInt32(scene.name.Substring(0, 2));

        AudioClip thisLevelMusic = backgroundMusicClips[levelType];
        Debug.Log("Playing clip: " + thisLevelMusic);

        if (thisLevelMusic != null)
        {
            // if not playing the menu background music or not in the menus
            if (thisLevelMusic != backgroundMusic.clip || levelType != 1)
            {
                backgroundMusic.volume = masterVolume * backgroundMusicOffset;
                backgroundMusic.clip = thisLevelMusic;
                backgroundMusic.Play();
            }
        }
    }

    public void SetMasterVolume(float volume)
    {
        masterVolume = volume;

        foreach (AudioSource audioSource in allAudioSources)
        {
            audioSource.volume = masterVolume;
        }

        backgroundMusic.volume *= backgroundMusicOffset;
        playerShot.volume *= playerShotVolumeOffset;
        turretShot.volume *= playerShotVolumeOffset;

        foreach (AudioSource enemySound in enemySounds)
        {
            enemySound.volume *= enemyShotVolumeOffset;
        }
    }

    IEnumerator FadeOutMusic(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);

        while (backgroundMusic.volume > 0)
        {
            backgroundMusic.volume -= masterVolume / 20;
            yield return new WaitForSeconds(waitTime);
        }

        Debug.Log("Stopping music.");
        backgroundMusic.Stop();
    }

    // public functions
    public void StopBackgroundMusic()
    {
        coroutine = FadeOutMusic(0.2f);
        StartCoroutine(coroutine);
    }

    public void PlayPlayerShot()
    {
        playerShot.Play();
    }

    public void PlayTurretShot()
    {
        turretShot.Play();
    }

    public void PlayEnemyShot()
    {
        enemySounds[enemySoundSource].Play();

        enemySoundSource = (enemySoundSource + 1) % enemySounds.Length;
    }

    public void PlayPlayerExplosion()
    {
        explosions[explosionSoundSource].clip = playerExplosionClip;
        explosions[explosionSoundSource].Play();

        explosionSoundSource = (explosionSoundSource + 1) % explosions.Length;
    }

    public void PlayExplosion(AudioClip explosionClip)
    {
        explosions[explosionSoundSource].clip = explosionClip;
        explosions[explosionSoundSource].Play();

        explosionSoundSource = (explosionSoundSource + 1) % explosions.Length;
    }
}