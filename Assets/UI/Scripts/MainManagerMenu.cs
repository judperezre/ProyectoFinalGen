using UnityEngine;
using UnityEngine.SceneManagement;

public class MainManagerMenu : MonoBehaviour
{
    public GameObject panelComoJugar;
    public GameObject panelCreditos;
    public GameObject panelHistoria;
    public GameObject canvasImage;
    public GameObject canvasMainMenu;
    public void Jugar()
    {
        SceneManager.LoadScene("DanielPrueba"); // reemplaza por el nombre de la escena del juego
    }

    public void ComoJugar()
    {
        panelComoJugar.SetActive(true);
        canvasImage.SetActive(false);
        canvasMainMenu.SetActive(false);
    }
    public void OcultarComoJugar()
    {
        panelComoJugar.SetActive(false);
        canvasImage.SetActive(true);
        canvasMainMenu.SetActive(true);
    }
    public void Historia()
    {
        panelHistoria.SetActive(true);
        canvasImage.SetActive(false);
        canvasMainMenu.SetActive(false);
    }

    public void OcultarHistoria()
    {
        panelHistoria.SetActive(false);
        canvasImage.SetActive(true);
        canvasMainMenu.SetActive(true);
    }

    public void Creditos()
    {
        panelCreditos.SetActive(true);
        canvasImage.SetActive(false);
        canvasMainMenu.SetActive(false);
    }
    public void OcultarCreditos()
    {
        panelCreditos.SetActive(false);
        canvasImage.SetActive(true);
        canvasMainMenu.SetActive(true);
    }
}
