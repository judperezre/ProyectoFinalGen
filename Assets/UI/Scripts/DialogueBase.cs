using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using TMPro;

public class DialogoBase : MonoBehaviour
{
    public GameObject panelDialogo;
    public TextMeshProUGUI textoDialogo;
    public float velocidadEscritura = 0.05f;

    public PlayerController playerController; // ← Referencia directa

    public string[] lineas;
    private int indiceLinea;
    private bool dialogoActivo = false;
    private bool escribiendo = false;

    private Action onFinish;

    void Start()
    {
       
    }

    void Update()
    {
        if (dialogoActivo && Input.GetKeyDown(KeyCode.X))
        {
            if (escribiendo)
            {
                StopAllCoroutines();
                textoDialogo.text = lineas[indiceLinea];
                escribiendo = false;
            }
            else
            {
                indiceLinea++;
                if (indiceLinea < lineas.Length)
                {
                    StartCoroutine(EscribirTexto());
                }
                else
                {
                    CerrarDialogo();
                }
            }
        }
    }

    public void MostrarDialogo(string[] nuevasLineas, Action cuandoFinaliza = null)
    {
        onFinish = cuandoFinaliza;
        lineas = nuevasLineas;
        indiceLinea = 0;
        panelDialogo.SetActive(true);
        dialogoActivo = true;

        if (playerController != null)
            playerController.enabled = false; // ← Detiene movimiento

        StartCoroutine(EscribirTexto());
    }

    IEnumerator EscribirTexto()
    {
        escribiendo = true;
        textoDialogo.text = "";

        foreach (char letra in lineas[indiceLinea])
        {
            textoDialogo.text += letra;
            yield return new WaitForSeconds(velocidadEscritura);
        }

        escribiendo = false;
    }

    void CerrarDialogo()
    {
        panelDialogo.SetActive(false);
        dialogoActivo = false;

        if (playerController != null)
            playerController.enabled = true; // ← Reactiva movimiento

        if (onFinish != null)
            onFinish.Invoke();
    }
}
