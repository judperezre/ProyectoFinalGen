using System.Collections;
using UnityEngine;

public class DialogoDespuesTurmalina : MonoBehaviour
{
    public DialogoBase dialogoBase; // Asigna este mismo GameObject
    public string[] lineasFinales;  // Aquí escribes las líneas finales en el inspector
    public float delayAntesDialogo = 2f; // Tiempo tras aparecer el portal

    void Start()
    {
        gameObject.SetActive(false); // Este objeto debe empezar desactivado
    }

    public void ActivarDialogo()
    {
        StartCoroutine(MostrarDialogoConDelay());
    }

    IEnumerator MostrarDialogoConDelay()
    {
        yield return new WaitForSeconds(delayAntesDialogo);

        gameObject.SetActive(true); // Activa el cuadro de diálogo
        dialogoBase.lineas = lineasFinales; // Asigna las nuevas líneas
        dialogoBase.MostrarDialogo(dialogoBase.lineas); // Llama el método de inicio del diálogo
    }
}

