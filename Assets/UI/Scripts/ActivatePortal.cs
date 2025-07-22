using UnityEngine;
using TMPro;

public class ActivatePortal : MonoBehaviour
{
    public DialogoBase notaController;
    public static ActivatePortal Instance;
    public GameObject portal;

    public DialogoDespuesTurmalina dialogoFinal;

    public string[] notaFrases = {
        "Sé que existe el destino, por lo que confío esta turmalina morada tomará el camino correcto. Así pues, con su poder, dotará de valentía a quien la merezca.",
        "No es solo una gema, es una prueba. Quien la posea deberá enfrentarse a sus propios temores, recorrer senderos desconocidos y recolectar los fragmentos que fortalecen el alma: la confianza, la compasión, la perseverancia y la sabiduría.",
        "Solo entonces, la luz interior brillará con tal fuerza que ningún obstáculo podrá apagarla. Una fuerza misteriosa te guía hacia el portal... Debes continuar tu camino sin dudar."
    };

    private bool yaActivado = false;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !yaActivado)
        {
            yaActivado = true;

            // Mostramos el diálogo de la nota y luego llamamos a AbrirPortal cuando termine
            notaController.MostrarDialogo(notaFrases, AbrirPortal);

            gameObject.SetActive(false); // Esconde la turmalina
        }
    }

    void Awake()
    {
        Instance = this;

        // Asegura que el portal esté oculto al inicio
        if (portal != null)
            portal.SetActive(false);
    }

    public void AbrirPortal()
    {
        if (portal != null)
        {
            portal.SetActive(true); // Activa el objeto del portal


            // Si el portal tiene un sistema de partículas, lo iniciamos manualmente
            ParticleSystem ps = portal.GetComponent<ParticleSystem>();
            if (ps != null)
            {
                ps.Play();
                Debug.Log("Portal activado y partículas reproducidas.");
            }
            else
            {
                Debug.LogWarning("El objeto 'portal' no tiene un componente ParticleSystem.");
            }
            dialogoFinal.ActivarDialogo();

            if (dialogoFinal != null)
            {
                dialogoFinal.gameObject.SetActive(true); // ACTIVAMOS el objeto aquí
                dialogoFinal.ActivarDialogo(); // Llamamos después de activarlo
            }

        }

    }


}
