using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class BotonSonido : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip soundClip;

    void Start()
    {
        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();

        Button btn = GetComponent<Button>();
        if (btn != null && audioSource != null && soundClip != null)
        {
            btn.onClick.AddListener(PlaySound);
        }
    }

    void PlaySound()
    {
        if (!audioSource.enabled)
            audioSource.enabled = true;

        if (!audioSource.isPlaying)
            audioSource.PlayOneShot(soundClip);
    }
}
