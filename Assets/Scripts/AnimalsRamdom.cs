using UnityEngine;

public class AnimalsRamdom : MonoBehaviour
{
    [Header("Patrulla")]
    [Tooltip("Radio en metros alrededor de la posición inicial")]
    public float wanderRadius = 5f;
    [Tooltip("Velocidad de caminata")]
    public float walkSpeed = 1.5f;
    [Tooltip("Tiempo máximo entre paradas a comer")]
    public float wanderInterval = 5f;

    [Header("Comer")]
    [Tooltip("Cuánto dura la pausa para comer (segundos)")]
    public float eatDuration = 3f;

    // Referencias internas
    private Vector3 startPosition;
    private Vector3 targetPosition;
    private float wanderTimer;
    private float eatTimer;
    private bool isEating = false;
    private Animator animator;

    void Start()
    {
        startPosition = transform.position;
        wanderTimer = wanderInterval * Random.value; // arranca con un retardo aleatorio
        animator = GetComponent<Animator>();
        PickNewTarget();
    }

    void Update()
    {
        if (isEating)
        {
            // Estado "comer"
            eatTimer -= Time.deltaTime;
            if (eatTimer <= 0f)
            {
                // Termina de comer, vuelve a patrullar
                isEating = false;
               
                wanderTimer = wanderInterval;
                PickNewTarget();
            }
        }
        else
        {
            // Estado "patrullar"
            wanderTimer -= Time.deltaTime;

            Vector3 dir = targetPosition - transform.position;
            if (dir.magnitude < 0.2f || wanderTimer <= 0f)
            {
                // Llegó al destino o terminó el temporizador → comer
                isEating = true;
                
                
                eatTimer = eatDuration;
            }
            else
            {
                // Caminar hacia targetPosition
                Vector3 move = dir.normalized * walkSpeed * Time.deltaTime;
                transform.position += move;

                // Girar suave hacia la dirección
                Quaternion look = Quaternion.LookRotation(dir);
                transform.rotation = Quaternion.Slerp(transform.rotation, look, Time.deltaTime * 2f);

                
            }
        }
    }

    // Elige un nuevo punto aleatorio dentro del radio
    private void PickNewTarget()
    {
        Vector2 rnd = Random.insideUnitCircle * wanderRadius;
        targetPosition = startPosition + new Vector3(rnd.x, 0f, rnd.y);
    }

    // Para visualizar en el Scene view
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, wanderRadius);
    }
}

