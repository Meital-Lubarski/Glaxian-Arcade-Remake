using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance {  get; private set; }

    [SerializeField] private GameSoundLibrary library;
    [SerializeField] private AudioSource sfxSource;
    [SerializeField] private AudioSource musicSource;
    [Header("Debug Settings")]
    [SerializeField] private bool muteSFX = false;

    public void PlayPlayerShoot() => PlaySFX(library.playerShoot);
    public void PlayPlayerDeath() => PlaySFX(library.playerDeath);
    public void PlayEnemyShoot() => PlaySFX(library.enemyShoot);
    public void PlayEnemyExplosion() => PlaySFX(library.enemyExplosion); 
    public void PlayHitBoss() => PlaySFX(library.hitBoss);    
    public void PlayDivingSound() => PlaySFX(library.divingSound);
    public void PlayStartGame() => PlaySFX(library.startGame);
    public void PlayFallingStar() => PlaySFX(library.fallingStar);
    public void PlayStarExplosion() => PlaySFX(library.starExplosion);
    
    

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        Application.targetFrameRate = 30;
        QualitySettings.vSyncCount = 0;
    }
    
    public GameSoundLibrary GetLibrary() => library;
    public AudioSource GetMusicSource() => musicSource;
    
    
    public void PlayBackgroundMusic(SoundData data, bool shouldLoop = true)
    {
        if (data == null) return;

        AudioClip clip = data.GetRandomClip();
        if (clip == null) return;
    
        musicSource.clip = clip;
        musicSource.volume = data.volume;
        musicSource.pitch = data.pitch;
        musicSource.loop = shouldLoop; 
        musicSource.Play();
    }

    public void StopBackgroundMusic()
    {
        musicSource.Stop();
    }
    
    private void PlaySFX(SoundData data)
    {
        if (data == null || muteSFX) return;

        float p = data.pitch;
        if (data.useRandomPitch)
        {
            p += Random.Range(-data.pitchVariance, data.pitchVariance);
        }
        sfxSource.pitch = p;
        sfxSource.PlayOneShot(data.GetRandomClip(), data.volume);
    }
    
    public float GetStartGameDuration()
    {
        if (library.startGame != null && library.startGame.clips.Length > 0)
        {
            return library.startGame.clips[0].length;
        }
        return 0f;
    }
    
    public void StopAllMusic()
    {
        if (musicSource != null)
        {
            musicSource.Stop();
            musicSource.clip = null;
        }
    }
    
}
