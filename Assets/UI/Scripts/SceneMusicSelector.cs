using UnityEngine;

public class SceneMusicSelector : MonoBehaviour
{
    public enum MusicaEscena { Menu, EscenaCasa, Nivel1, Final }
    public MusicaEscena tipoMusica;

    void Start()
    {
        switch (tipoMusica)
        {
            case MusicaEscena.Menu:
                AudioManager.Instance.PlayMusic(AudioManager.Instance.menuMusic);
                break;
            case MusicaEscena.EscenaCasa:
                AudioManager.Instance.PlayMusic(AudioManager.Instance.escenaCasa);
                break;
            case MusicaEscena.Nivel1:
                AudioManager.Instance.PlayMusic(AudioManager.Instance.nivel1Music);
                break;
            case MusicaEscena.Final:
                AudioManager.Instance.PlayMusic(AudioManager.Instance.finalMusic);
                break;
        }
    }
}

