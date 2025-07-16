using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class DeathScreen : MonoBehaviour
{
    public GameObject deathPanel;       // Asignar el panel desde el inspector
    public Button restartButton;

    public TMP_Text mensajeMuerteText;

    private string[] frasesMuerte = new string[]
{
        "La turmalina se apaga… el viaje deberá comenzar de nuevo.",
        "El alma de Xiomara cae en sombras. ¿Volverás a intentarlo?",
        "La senda se ha oscurecido. ¿Te atreves a volver a caminarla?",
        "El miedo ha vencido por ahora… pero la esperanza aún arde.",
        "La flor de la confianza se marchita… pero puede volver a florecer."
};

    void OnEnable()
    {
        MostrarFraseAleatoria();
    }

    void MostrarFraseAleatoria()
    {
        int index = Random.Range(0, frasesMuerte.Length);
        mensajeMuerteText.text = frasesMuerte[index];
    }


    void Start()
    {
        deathPanel.SetActive(true); // Oculto al iniciar
        restartButton.onClick.AddListener(RestartGame);
    }

    public void ShowDeathScreen()
    {
        deathPanel.SetActive(true);
        Time.timeScale = 0f; // Pausa el juego
    }

    void RestartGame()
    {
        Time.timeScale = 1f; // Reactivar tiempo por si se pausó
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

}
