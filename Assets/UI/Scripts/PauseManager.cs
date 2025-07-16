using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class PauseManager : MonoBehaviour
{
    public GameObject pausePanel;         
    public Button pauseButton;            
    public Button resumeButton;
    public Button restartButton;

    private bool isPaused = false;

    void Start()
    {
        // Panel esté oculto al iniciar
        pausePanel.SetActive(false);

        // Asignar funciones a los botones
        pauseButton.onClick.AddListener(TogglePause);
        resumeButton.onClick.AddListener(ResumeGame);
        restartButton.onClick.AddListener(RestartGame);
    }

    void TogglePause()
    {
        isPaused = !isPaused;

        if (isPaused)
        {
            Time.timeScale = 0f;
            pausePanel.SetActive(true);
        }
        else
        {
            Time.timeScale = 1f;
            pausePanel.SetActive(false);
        }
    }

    void ResumeGame()
    {
        isPaused = false;
        Time.timeScale = 1f;
        pausePanel.SetActive(false);
    }

    void RestartGame()
    {
        Time.timeScale = 1f; // Asegura que el tiempo esté corriendo
        //SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        SceneManager.LoadScene(0);
    }
}
