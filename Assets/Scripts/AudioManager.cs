using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    [Header("Music")]
    public AudioClip[] musicTracks;
    private AudioSource musicSource;

    [Header("SFX")]
    public AudioClip gunshot;
    public AudioClip explosion;
    public AudioClip enemyHit;
    public AudioClip dash;
    public AudioClip gameOver;
    
    private AudioSource sfxSource;

    void Awake()
    {
        instance = this;
        
        // Music source
        musicSource = gameObject.AddComponent<AudioSource>();
        musicSource.loop = true;
        musicSource.volume = 0.5f;

        // SFX source
        sfxSource = gameObject.AddComponent<AudioSource>();
        sfxSource.volume = 1f;
    }

    void Start()
    {
        PlayRandomMusic();
    }
    
    void Update()
    {
        if (!musicSource.isPlaying)
        {
            PlayRandomMusic();
        }
    }
    
    public void PlayRandomMusic()
    {
        if (musicTracks.Length == 0) return;
        musicSource.clip = musicTracks[Random.Range(0, musicTracks.Length)];
        musicSource.Play();
    }

    public void PlayGunshot()
    {
        sfxSource.PlayOneShot(gunshot);
    }

    public void PlayExplosion()
    {
        sfxSource.PlayOneShot(explosion);
    }

    public void PlayEnemyHit()
    {
        sfxSource.PlayOneShot(enemyHit);
    }
    
    public void PlayDash()
    {
        sfxSource.PlayOneShot(dash);
    }

    public void PlayGameOver()
    {
        musicSource.Stop();
        sfxSource.PlayOneShot(gameOver);
    }
}