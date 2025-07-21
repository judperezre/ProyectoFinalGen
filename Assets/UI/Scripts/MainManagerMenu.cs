using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainManagerMenu : MonoBehaviour
{
    public GameObject panelComoJugar;
    public GameObject panelCreditos;
    public GameObject panelHistoria;
    public GameObject canvasImage;
    public GameObject canvasLogo;
    public GameObject canvasMainMenu;

    void Start()
    {
        AudioManager.Instance.PlayMenuMusic();
    }

    public void Jugar()
    {
        AudioManager.Instance.PlayPlaySound(); // ← sonido especial solo para Jugar
        AudioManager.Instance.StopMusic();
        //StartCoroutine(CambiarPanelConDelay(setupPanel));
        SceneManager.LoadScene("SceneHouse"); // reemplaza por el nombre de la escena del juego
    }

    public void ComoJugar()
    {
        AudioManager.Instance.PlayClick();
        StartCoroutine(CambiarPanelConDelay(panelComoJugar));

        //panelComoJugar.SetActive(true);
        //canvasImage.SetActive(false);
        //canvasMainMenu.SetActive(false);
    }
    public void OcultarComoJugar()
    {
        panelComoJugar.SetActive(false);
        canvasImage.SetActive(true);
        canvasLogo.SetActive(true);
        canvasMainMenu.SetActive(true);
    }
    public void Historia()
    {
        AudioManager.Instance.PlayClick();
        StartCoroutine(CambiarPanelConDelay(panelHistoria));
        /*panelHistoria.SetActive(true);
        canvasImage.SetActive(false);
        canvasMainMenu.SetActive(false);*/
    }

    public void OcultarHistoria()
    {
        panelHistoria.SetActive(false);
        canvasImage.SetActive(true);
        canvasLogo.SetActive(true);
        canvasMainMenu.SetActive(true);
    }

    public void Creditos()
    {
        AudioManager.Instance.PlayClick();
        StartCoroutine(CambiarPanelConDelay(panelCreditos));
        /*panelCreditos.SetActive(true);
        canvasImage.SetActive(false);
        canvasMainMenu.SetActive(false);*/
    }
    public void OcultarCreditos()
    {
        panelCreditos.SetActive(false);
        canvasImage.SetActive(true);
        canvasLogo.SetActive(true);
        canvasMainMenu.SetActive(true);
    }

    IEnumerator CambiarPanelConDelay(GameObject panelNuevo)
    {
        yield return new WaitForSeconds(0.1f); // Espera un poco para que suene el clic
        //mainPanel.SetActive(false);
        canvasImage.SetActive(false);
        canvasLogo.SetActive(false);
        canvasMainMenu.SetActive(false);
        panelNuevo.SetActive(true);
    }
}
