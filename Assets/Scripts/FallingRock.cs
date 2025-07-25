using System.Collections;
using UnityEngine;

public class FallingRock : MonoBehaviour
{
    [Header("Falling Settings")]
    public float fallDelay = 1f;
    public bool autoFall = true;

    [Header("Impact Effects")]
    public GameObject impactEffectPrefab;
    public AudioClip impactSound;

    [Header("Diálogo")]
    public DialogoBase dialogoBase;
    public string[] dialogoAlCaer;
    public string[] dialogoAlAcercarse;

    [Header("Jugador")]
    public Transform jugador;
    public float distanciaDialogo = 5f;

    private Rigidbody rb;
    private AudioSource audioSource;
    private bool hasFallen = false;
    private bool yaMostroDialogoAlAcercarse = false;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        if (rb == null) rb = gameObject.AddComponent<Rigidbody>();
        rb.isKinematic = true;

        rb.constraints = RigidbodyConstraints.FreezePositionX
                       | RigidbodyConstraints.FreezePositionZ
                       | RigidbodyConstraints.FreezeRotation;

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null) audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.spatialBlend = 1f;
    }

    void Start()
    {
        if (autoFall)
            StartCoroutine(FallAfterDelay());
    }

    void Update()
    {
        // Segundo diálogo: cuando jugador se acerca
        if (hasFallen && !yaMostroDialogoAlAcercarse)
        {
            float distancia = Vector3.Distance(jugador.position, transform.position);

            if (distancia < distanciaDialogo)
            {
                yaMostroDialogoAlAcercarse = true;
                dialogoBase.MostrarDialogo(dialogoAlAcercarse);
            }
        }
    }

    public void TriggerFall()
    {
        if (!hasFallen)
            StartCoroutine(FallAfterDelay());
    }

    private IEnumerator FallAfterDelay()
    {
        yield return new WaitForSeconds(fallDelay);
        rb.isKinematic = false;
        hasFallen = true;
    }

    void OnCollisionEnter(Collision collision)
    {
        if (!hasFallen) return;

        Debug.Log($"FallingRock colisionó con: {collision.collider.name}");

        if (impactEffectPrefab != null)
        {
            Instantiate(impactEffectPrefab, collision.contacts[0].point, Quaternion.identity);
        }

        if (impactSound != null)
        {
            audioSource.PlayOneShot(impactSound);
        }

        // Mostrar diálogo de impacto (una vez)
        dialogoBase.MostrarDialogo(dialogoAlCaer);
    }
}
