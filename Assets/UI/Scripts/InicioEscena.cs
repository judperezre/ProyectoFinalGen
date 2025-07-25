using UnityEngine;
using System.Collections;

public class InicioEscena : MonoBehaviour
{
    public DialogoBase dialogoBase;
    public GameObject panelInstrucciones;

    void Start()
    {
        StartCoroutine(MostrarDialogoConDelay());
    }

    IEnumerator MostrarDialogoConDelay()
    {
        yield return new WaitForSeconds(0.5f); // Puedes ajustar el tiempo para evitar que inicie de inmediato

        string[] frasesInicio = {
            "Xiomara: Mis padres se fueron temprano al mercado…",
            "Xiomara: Me toca revisar la huerta, sacar los huevos y limpiar un poco el corral.",
            "Xiomara:  Aprovecharé que el día está lindo para terminar todo antes de que regresen. ¡A ver si termino antes del mediodía!."
        };

        dialogoBase.MostrarDialogo(frasesInicio, () =>
        {
            panelInstrucciones.SetActive(true);
        });
    }

    void Update()
    {
        if (panelInstrucciones.activeSelf && Input.GetKeyDown(KeyCode.X))
        {
            panelInstrucciones.SetActive(false);
        }
    }
}

