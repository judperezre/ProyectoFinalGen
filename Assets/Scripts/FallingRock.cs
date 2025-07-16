using System.Collections;
using UnityEngine;

public class FallingRock : MonoBehaviour
{
    [Header("Falling Settings")]
    [Tooltip("Retraso (en segundos) antes de que la piedra caiga")]
    public float fallDelay = 1f;
    [Tooltip("Si es true, la piedra cae automáticamente al Start()")]
    public bool autoFall = true;

    [Header("Impact Effects")]
    [Tooltip("Prefab de partículas o efecto que se instanciará al chocar con el suelo")]
    public GameObject impactEffectPrefab;
    [Tooltip("Sonido que sonará al impactar")]
    public AudioClip impactSound;

    private Rigidbody rb;
    private AudioSource audioSource;
    private bool hasFallen = false;

    void Awake()
    {
        // Nos aseguramos de que haya un Rigidbody
        rb = GetComponent<Rigidbody>();
        if (rb == null) rb = gameObject.AddComponent<Rigidbody>();
        rb.isKinematic = true;

        // Congelar posición X/Z y rotaciones
        rb.constraints = RigidbodyConstraints.FreezePositionX
                       | RigidbodyConstraints.FreezePositionZ
                       | RigidbodyConstraints.FreezeRotation;

        // Creamos o recuperamos un AudioSource
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null) audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.spatialBlend = 1f; // sonido 3D
    }

    void Start()
    {
        if (autoFall)
            StartCoroutine(FallAfterDelay());
    }

    /// <summary>
    /// Llama a este método si quieres que la piedra caiga bajo demanda.
    /// </summary>
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

        // Debug: ver con qué colisiona
        Debug.Log($"FallingRock colisionó con: {collision.collider.name} (tag: {collision.collider.tag})");

        // Reproducir partículas
        if (impactEffectPrefab != null)
        {
            Instantiate(
                impactEffectPrefab,
                collision.contacts[0].point,
                Quaternion.identity
            );
        }

        // Reproducir sonido de impacto
        if (impactSound != null)
        {
            audioSource.PlayOneShot(impactSound);
        }

        
    }
}