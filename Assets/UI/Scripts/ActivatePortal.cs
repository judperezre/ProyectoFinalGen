using UnityEngine;
using TMPro;

public class ActivatePortal : MonoBehaviour
{
    public NotaController notaController;
    public static ActivatePortal Instance;
    public GameObject portal;

    public string[] notaFrases = {


        "Sé que existe el destino, por lo que confío esta turmalina morada tomará el camino correcto. Así pues, con su poder, dotará de valentía a quien la merezca.",
        "No es solo una gema, es una prueba. Quien la posea deberá enfrentarse a sus propios temores, recorrer senderos desconocidos y recolectar los fragmentos que fortalecen el alma: la confianza, la compasión, la perseverancia y la sabiduría.",
        "Solo entonces, la luz interior brillará con tal fuerza que ningún obstáculo podrá apagarla. Una fuerza misteriosa te guía hacia el portal... Debes continuar tu camino sin dudar.",
    };

    private bool yaActivado = false;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !yaActivado)
        {
            yaActivado = true;
            notaController.MostrarNota(notaFrases);
            gameObject.SetActive(false); // Esconde la turmalina
        }
    }

    void Awake()
    {
        Instance = this;
        portal.SetActive(false); // Oculto al inicio
    }

    public void AbrirPortal()
    {
        portal.SetActive(true);
    }
}
