using UnityEngine;
using System.Collections;

public class ActivadorDialogo : MonoBehaviour
{
    public DialogoBase dialogo;

    public string[] lineas;

    IEnumerator Start()
    {
        yield return new WaitForSeconds(0.5f);
        dialogo.MostrarDialogo(lineas);


    }
}

