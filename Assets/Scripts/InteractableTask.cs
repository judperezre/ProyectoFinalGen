using UnityEngine;
using System.Collections;

public class InteractableTask : MonoBehaviour
{
    [Header("Ajustes de la tarea")]
    [Tooltip("Duración en segundos de la tarea")]
    public float taskDuration = 15f;

    private bool isTaskRunning = false;
    private TaskUIManager uiManager;

    void Awake()
    {
        // Referencia al gestor de UI
        uiManager = Object.FindAnyObjectByType<TaskUIManager>();
        if (uiManager == null)
            Debug.LogError($"[{name}] ¡No encuentro ningún TaskUIManager en la escena!");

        // Forzar collider como trigger
        var col = GetComponent<Collider>();
        if (col == null)
            Debug.LogError($"[{name}] No veo Collider!");
        else
            col.isTrigger = true;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            StartTask();
    }

    private void StartTask()
    {
        if (isTaskRunning) return;
        isTaskRunning = true;

        Debug.Log($"[{name}] StartTask: duración={taskDuration}s");

        // Arrancamos el temporizador en la UI
        if (uiManager != null)
            uiManager.StartTaskTimer(taskDuration);
        else
            Debug.LogWarning($"[{name}] No se pudo arrancar StartTaskTimer (uiManager es null)");

        // Iniciamos la lógica interna (sin volver a descontar tareas)
        StartCoroutine(RunTask());
    }

    private IEnumerator RunTask()
    {
        // Espera la duración de la tarea
        yield return new WaitForSeconds(taskDuration);
        Debug.Log($"[{name}] RunTask terminado tras {taskDuration}s");

        // Desactivamos el collider para no volver a disparar la misma tarea
        var col = GetComponent<Collider>();
        if (col != null)
            col.enabled = false;
    }
}
