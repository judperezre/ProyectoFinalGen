using System.Collections;
using UnityEngine;

public class DangerZoneBehaviour : MonoBehaviour
{
    public float warningTime = 3f;
    public float damage = 20f;
    private bool playerInside = false;
    private bool hasExploded = false;
    private Renderer zoneRenderer;
    public GameObject impactVFXPrefab;

    private void Start()
    {
        zoneRenderer = GetComponent<Renderer>();
        StartCoroutine(BlinkAndExplode());
    }

    private IEnumerator BlinkAndExplode()
    {
        float elapsed = 0f;
        bool visible = true;

        while (elapsed < warningTime)
        {
            visible = !visible;
            zoneRenderer.enabled = visible;
            yield return new WaitForSeconds(0.2f);
            elapsed += 0.2f;
        }

        if (playerInside && !hasExploded)
        {
            PlayerController player = GetComponent<PlayerController>();
            if (player != null)
            {
                player.TakeDamage(damage);
            }
        }

        if (impactVFXPrefab != null)
        {
            Instantiate(impactVFXPrefab, transform.position, Quaternion.Euler(-90f, 0f, 0f));
        }

        hasExploded = true;
        Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInside = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInside = false;
        }
    }
}

