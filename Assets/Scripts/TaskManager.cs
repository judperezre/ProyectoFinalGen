using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TaskManager : MonoBehaviour
{
    public static TaskManager Instance;

    [Header("UI")]
    public TextMeshProUGUI tasksText;        // Arrastra aquí tu Text de UI
    public int startTasks = 3;    // Número inicial de tareas

    private int remaining;

    void Awake()
    {
        // Singleton sencillo
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        remaining = startTasks;
        UpdateUI();
    }

    /// <summary>
    /// Llamar cuando se complete una tarea
    /// </summary>
    public void CompleteTask()
    {
        remaining = Mathf.Max(0, remaining - 1);
        UpdateUI();
    }

    private void UpdateUI()
    {
        tasksText.text = $"Tareas restantes: {remaining}";
    }
}

