using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    public AudioClip clickSound;
    public AudioClip playSound; //// <-- sonido especial para el botón Jugar
    public AudioClip menuMusic;

    private AudioSource audioSource; // Para efectos de sonido
    private AudioSource musicSource; // Para música de fondo en el menú

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Opcional si cambia de escena
        }
        else
        {
            Destroy(gameObject);
        }


        // Creamos dos fuentes de audio: una para SFX y otra para música
        audioSource = gameObject.AddComponent<AudioSource>();
        musicSource = gameObject.AddComponent<AudioSource>();

        musicSource.loop = true; // La música del menú debe repetirse
        musicSource.volume = 0.5f; // Ajustar el volumen
    }

    public void PlayClick()
    {
        if (clickSound != null)
            audioSource.PlayOneShot(clickSound);
    }

    public void PlayPlaySound()  // <-- método específico para el botón Jugar
    {
        if (playSound != null)
            audioSource.PlayOneShot(playSound);
    }

    public void PlayMenuMusic()
    {
        if (menuMusic != null && !musicSource.isPlaying)
        {
            musicSource.clip = menuMusic;
            musicSource.Play();
        }
    }

    public void StopMusic()
    {
        musicSource.Stop();
    }
}
