using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Sonidos")]
    public AudioClip clickSound;
    public AudioClip playSound; // Sonido especial para el botón Jugar

    [Header("Música de Fondo")]
    public AudioClip menuMusic;
    public AudioClip escenaCasa;     // Agregado
    public AudioClip nivel1Music;     // Agregado
    public AudioClip finalMusic;      // Agregado

    private AudioSource audioSource; // Para efectos de sonido (SFX)
    private AudioSource musicSource; // Para música de fondo

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Permite que el AudioManager sobreviva al cambio de escenas
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        // Inicializa las fuentes de audio
        audioSource = gameObject.AddComponent<AudioSource>();
        musicSource = gameObject.AddComponent<AudioSource>();

        musicSource.loop = true;
        musicSource.volume = 0.2f;
    }


    public void PlayClick()
    {
        if (clickSound != null)
            audioSource.PlayOneShot(clickSound);
    }

    public void PlayPlaySound()
    {
        if (playSound != null)
            audioSource.PlayOneShot(playSound);
    }

    public void PlayMenuMusic()
    {
        PlayMusic(menuMusic);
    }

    //Método general para reproducir cualquier música
    public void PlayMusic(AudioClip clip)
    {
        if (clip != null)
        {
            if (musicSource.isPlaying)
                musicSource.Stop();

            musicSource.clip = clip;
            musicSource.Play();
        }
    }

    public void StopMusic()
    {
        musicSource.Stop();
    }
}

