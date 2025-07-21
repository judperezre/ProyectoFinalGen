using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NotaController : MonoBehaviour
{
    public GameObject notaPanel;
    public TextMeshProUGUI notaText;
    public string[] frases;
    public float velocidadEscritura = 0.05f;

    public GameObject dialogoReaccionPanel;
    public TextMeshProUGUI textoDialogoReaccion;

    private int indice = 0;
    private bool escribiendo = false;
    private bool puedeContinuar = false;

    void Update()
    {
        if (notaPanel.activeSelf && Input.GetKeyDown(KeyCode.X) && puedeContinuar)
        {
            if (indice < frases.Length)
            {
                StartCoroutine(MostrarTexto(frases[indice]));
                indice++;
            }
            else
            {

                notaPanel.SetActive(false);
                // Activar el portal
                ActivatePortal.Instance.AbrirPortal();


                // Mostrar cuadro de diálogo de reacción de Xiomara
                dialogoReaccionPanel.SetActive(true);
                textoDialogoReaccion.text = "";
                StartCoroutine(EscribirTextoReaccion("¿Qué fue eso...? Esta nota... y mi ropa... ¡Tengo una gema en la mano!, será mejor ir hacia el portal..."));
            }

            puedeContinuar = false;
        }

        if (dialogoReaccionPanel.activeSelf && !escribiendo && Input.GetKeyDown(KeyCode.X))
        {
            dialogoReaccionPanel.SetActive(false);
        }
    }

    public void MostrarNota(string[] lineas)
    {
        frases = lineas;
        indice = 0;
        notaPanel.SetActive(true);
        StartCoroutine(MostrarTexto(frases[indice]));
        indice++;
    }

    IEnumerator MostrarTexto(string texto)
    {
        escribiendo = true;
        notaText.text = "";

        foreach (char letra in texto.ToCharArray())
        {
            notaText.text += letra;
            yield return new WaitForSeconds(velocidadEscritura);
        }

        escribiendo = false;
        puedeContinuar = true;
    }
    IEnumerator EscribirTextoReaccion(string texto)
    {
        escribiendo = true;
        textoDialogoReaccion.text = "";

        foreach (char letra in texto)
        {
            textoDialogoReaccion.text += letra;
            yield return new WaitForSeconds(velocidadEscritura);
        }

        escribiendo = false;
    }

}

