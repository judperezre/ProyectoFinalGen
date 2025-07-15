using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    public AudioClip clickSound;
    public AudioClip playSound; //// <-- sonido especial para el botón Jugar

    private AudioSource audioSource;

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

        audioSource = GetComponent<AudioSource>();
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
}
