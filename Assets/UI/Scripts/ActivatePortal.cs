using UnityEngine;

public class ActivatePortal : MonoBehaviour
{
    public GameObject portalToActivate; // Asigna el portal desde el Inspector

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Desactiva la esfera al recogerla
            gameObject.SetActive(false);

            // Activa el portal
            if (portalToActivate != null)
            {
                portalToActivate.SetActive(true);
            }
        }
    }
}
