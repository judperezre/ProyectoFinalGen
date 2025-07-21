using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class TaskUIManager : MonoBehaviour
{
    [Header("UI Elements")]
    public TextMeshProUGUI taskCounterText;   // El TMP de “Tareas: X”
    public Image timerFillImage;             // El Image de TimerBarFill

    [Header("Tarea Settings")]
    public float taskDuration = 15f;         // Duracion por defecto de cada tarea

    private int remainingTasks;
    private Coroutine timerCoroutine;

    void Start()
    {
        // Asume que al inicio hay 3 tareas
        remainingTasks = 3;
        UpdateTaskCounter();
        ResetTimerFill();
    }

    /// <summary>
    /// Llama esto cuando completes una tarea
    /// </summary>
    public void OnTaskCompleted()
    {
        // Detén cualquier timer en curso
        if (timerCoroutine != null)
            StopCoroutine(timerCoroutine);

        remainingTasks = Mathf.Max(0, remainingTasks - 1);
        UpdateTaskCounter();

        // Reinicia barra para la siguiente (o vacía si no hay mas)
        ResetTimerFill();
    }

    /// <summary>
    /// Llama esto cuando empieces a trabajar una tarea
    /// </summary>
    public void StartTaskTimer(float customDuration = -1f)
    {
        // Si ya tienes un timer corriendo, lo detienes
        if (timerCoroutine != null)
            StopCoroutine(timerCoroutine);

        float duration = (customDuration > 0f) ? customDuration : taskDuration;
        timerCoroutine = StartCoroutine(RunTimer(duration));
    }

    private IEnumerator RunTimer(float duration)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            // Llenado (1→0)
            if (timerFillImage != null)
                timerFillImage.fillAmount = 1f - (elapsed / duration);
            yield return null;
        }

        // Cuando termine el timer, marca la tarea completada en UI
        OnTaskCompleted();
    }

    private void UpdateTaskCounter()
    {
        if (taskCounterText != null)
            taskCounterText.text = $"Tareas restantes: {remainingTasks}";
    }

    private void ResetTimerFill()
    {
        if (timerFillImage != null)
            timerFillImage.fillAmount = 1f;
        
    }

    
}
