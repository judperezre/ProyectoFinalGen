using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FlowerBehaviour : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        Destroy(gameObject);
        StartCoroutine(CargarYActivarCreditos());
    }

    private IEnumerator CargarYActivarCreditos()
    {
        // Cargar escena
        SceneManager.LoadScene("MainMenuScene");

        // Esperar un frame para que la escena cargue completamente
        yield return null;

        // Buscar el panel por nombre (asegúrate de que el GameObject se llame "PanelCreditos")
        GameObject panel = GameObject.Find("PanelCreditos");

        if (panel != null)
        {
            panel.SetActive(true); // Mostrar el panel
        }
        else
        {
            Debug.LogWarning("No se encontró el PanelCreditos en la escena.");
        }
    }
}
