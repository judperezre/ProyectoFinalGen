
using Assets.Scripts;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class psychicEnemy : MonoBehaviour, IDamageable
{
    public NavMeshAgent agent;
    public Transform player;
    public LayerMask whatIsGround, whatIsPlayer;
    public GameObject projectile;
    public Transform arrowPoint;
    public float health;
    private Coroutine walkPointTimeoutCoroutine;
    private bool isIdleDone;
    public GameObject dangerZone;
    public bool IsAlive => health > 0;

    //animations

    [SerializeField]
    Animator shootingAnimator;
    public float rotationSpeed = 5f;

    //Patroll

    public Vector3 walkPoint;
    bool walkPointSet;
    public float walkPointRange;

    //Attacks
    private Vector3 previousPlayerPos;
    private Vector3 estimatedPlayerVelocity;
    public float timeBetweenAttacks;
    bool alreadyAttacked;

    //States

    public float sightRange, attackRange;
    public bool isPlayerInSightRange, isPlayerInAttackRange;

    private void Awake()
    {
        player = GameObject.Find("Player").transform;
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
    }

    private void FixedUpdate()
    {
        Vector3 currentPlayerPos = player.transform.position;
        estimatedPlayerVelocity = (currentPlayerPos - previousPlayerPos) / Time.deltaTime;
        previousPlayerPos = currentPlayerPos;
    }
    private void Update()
    {
        float speed = agent.velocity.magnitude;
        shootingAnimator.SetFloat("MoveSpeed", speed);

        isPlayerInSightRange = Physics.CheckSphere(transform.position, sightRange, whatIsPlayer);
        isPlayerInAttackRange = Physics.CheckSphere(transform.position, attackRange, whatIsPlayer);

        if (!isPlayerInSightRange && !isPlayerInAttackRange)
        {
            //Idle
            if (speed < 0.5f && isIdleDone == false)
            {
                StartCoroutine("IdleTimer");
            }

            shootingAnimator.SetBool("isPlayerInAttackRange", false);
            Patrolling();
            isIdleDone = false;
        }

        if (isPlayerInSightRange && !isPlayerInAttackRange)
        {
            shootingAnimator.SetBool("isPlayerInAttackRange", false);
            ChasePlayer();
        }
        if (isPlayerInSightRange && isPlayerInAttackRange)
        {
            AttackPlayer();
        }

        AlignToGround();

    }

    private void Patrolling()
    {
        if (!walkPointSet)
        {
            SearchWalkPoint();
        }

        if (walkPointSet)
        {
            agent.SetDestination(walkPoint);

            Vector3 dir = agent.steeringTarget - transform.position;
            if (dir.sqrMagnitude > 0.01f)
            {
                Debug.DrawRay(transform.position + Vector3.up * 0.5f, Vector3.down * 2f, Color.red);

                Ray ray = new Ray(transform.position + Vector3.up * 0.5f, Vector3.down);
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit, 2f, whatIsGround))
                {

                    Vector3 normal = hit.normal;
                    Quaternion lookRotation = Quaternion.LookRotation(dir, normal);
                    transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * rotationSpeed);
                }
                else
                {
                    Debug.LogWarning("Raycast no golpea nada bajo el enemigo");
                }
            }

            // Si no hay una corutina de timeout activa, arrancarla
            if (walkPointTimeoutCoroutine == null)
            {
                walkPointTimeoutCoroutine = StartCoroutine(WalkPointTimeout());
            }
        }

        Vector3 distanceToWalkPoint = transform.position - walkPoint;

        if (distanceToWalkPoint.magnitude < 1f)
        {
            walkPointSet = false;

            // Cancelar la corutina si llegó bien
            if (walkPointTimeoutCoroutine != null)
            {
                StopCoroutine(walkPointTimeoutCoroutine);
                walkPointTimeoutCoroutine = null;
            }
        }

        AlignToGround();
    }

    private void SearchWalkPoint()
    {
        float randomZ = Random.Range(-walkPointRange, walkPointRange);
        float randomX = Random.Range(-walkPointRange, walkPointRange);

        walkPoint = new Vector3(transform.position.x + randomX, transform.position.y, transform.position.z + randomZ);

        if (Physics.Raycast(walkPoint, -transform.up, 2f, whatIsGround))
        {
            walkPointSet = true;
        }

    }

    private void ChasePlayer()
    {

        agent.isStopped = false;
        agent.SetDestination(player.position);
        AlignToGround();

    }
    private void AttackPlayer()
    {
        agent.isStopped = true;
        agent.velocity = Vector3.zero;

        Vector3 playerPos = player.transform.position + Vector3.up * 1.2f;

        // Estimación del tiempo para que el proyectil llegue
        float projectileSpeed = 15f;
        float distanceToPlayer = Vector3.Distance(transform.position, playerPos);
        float predictionTime = distanceToPlayer / projectileSpeed;

        Vector3 predictedPos = player.position + estimatedPlayerVelocity * predictionTime;
        Vector3 directionToTarget = predictedPos - transform.position;
        Quaternion lookRotation = Quaternion.LookRotation(directionToTarget);
        float rotationSpeed = 5f;

        transform.rotation = Quaternion.Lerp(transform.rotation, lookRotation, Time.deltaTime * rotationSpeed);

        if (!alreadyAttacked)
        {
            shootingAnimator.SetBool("isPlayerInAttackRange", true);
            shootingAnimator.SetFloat("MoveSpeed", 0f);

        }
    }

    private void ResetAttack()
    {
        alreadyAttacked = false;
    }

    public void TakeDamage(int damage)
    {
        health -= damage;

        if (health <= 0)
        {
            shootingAnimator.SetBool("isdDead", true);
            Invoke(nameof(DestroyEnemy), 10f);
        }
    }
    public void DestroyEnemy()
    {
        Destroy(gameObject);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, sightRange);

        if (walkPoint != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(walkPoint, 0.25f);
        }
    }

    public void ShootingArrow()
    {
        GameObject arrow = Instantiate(projectile, arrowPoint.position, Quaternion.identity, arrowPoint);
        arrow.transform.parent = null;
        arrow.GetComponent<Rigidbody>().AddForce(transform.forward * 45f, ForceMode.Impulse);
    }

    private void AlignToGround()
    {

        RaycastHit hit;
        Vector3 rayOrigin = transform.position + Vector3.up * 1f;
        if (Physics.Raycast(rayOrigin, Vector3.down, out hit, 3f, whatIsGround))
        {
            // Debug del punto de impacto
            Debug.DrawRay(hit.point, hit.normal, Color.green);

            Vector3 forwardProjected = Vector3.ProjectOnPlane(transform.forward, hit.normal).normalized;
            if (forwardProjected.sqrMagnitude > 0.001f)
            {
                Quaternion targetRotation = Quaternion.LookRotation(forwardProjected, hit.normal);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
            }
        }
    }

    private IEnumerator IdleTimer()
    {
        agent.isStopped = true;
        agent.velocity = Vector3.zero;
        shootingAnimator.SetFloat("MoveSpeed", 0f);

        yield return new WaitForSeconds(10f);
        isIdleDone = true;
        agent.isStopped = false;
    }

    private IEnumerator WalkPointTimeout()
    {
        yield return new WaitForSeconds(5f);

        Vector3 distanceToWalkPoint = transform.position - walkPoint;

        if (distanceToWalkPoint.magnitude >= 1f)
        {
            walkPointSet = false;
        }

        walkPointTimeoutCoroutine = null;
    }

    private void LaunchParabolicProjectile(Vector3 targetPos)
    {
        GameObject proj = Instantiate(projectile, arrowPoint.position, Quaternion.identity);
        Rigidbody rb = proj.GetComponent<Rigidbody>();

        Vector3 velocity = CalculateParabolicVelocity(arrowPoint.position, targetPos, 3f); // 3 segundos
        rb.linearVelocity = velocity;
    }

    private Vector3 CalculateParabolicVelocity(Vector3 startPoint, Vector3 endPoint, float timeToTarget)
    {
        Vector3 toTarget = endPoint - startPoint;
        Vector3 toTargetXZ = new Vector3(toTarget.x, 0, toTarget.z);

        float yOffset = toTarget.y;
        float xzDistance = toTargetXZ.magnitude;

        float t = timeToTarget;
        float vY = yOffset / t + 0.5f * Mathf.Abs(Physics.gravity.y) * t;
        float vXZ = xzDistance / t;

        Vector3 result = toTargetXZ.normalized * vXZ;
        result.y = vY;
        return result;
    }

    private Vector3 GetGroundPosition(Vector3 pos)
    {
        RaycastHit hit;
        if (Physics.Raycast(pos + Vector3.up * 5f, Vector3.down, out hit, 10f, whatIsGround))
        {
            return hit.point;
        }
        return new Vector3(pos.x, 0f, pos.z);
    }

    private void SpawnDangerZone(Vector3 position)
    {
        Instantiate(dangerZone, position, Quaternion.identity);
    }

    public void PerformParabolicAttack()
    {
        float predictionTime = 3f;
        Vector3 predictedPos = player.position + estimatedPlayerVelocity * predictionTime;

        Vector3 direction = (predictedPos - transform.position).normalized;

        Vector3 rightOffset = Quaternion.Euler(0, 45, 0) * direction;
        Vector3 leftOffset = Quaternion.Euler(0, -45, 0) * direction;

        float radius = 3f;

        Vector3 centerImpact = GetGroundPosition(predictedPos);
        Vector3 rightImpact = GetGroundPosition(predictedPos + rightOffset * radius);
        Vector3 leftImpact = GetGroundPosition(predictedPos + leftOffset * radius);

        SpawnDangerZone(centerImpact);
        SpawnDangerZone(rightImpact);
        SpawnDangerZone(leftImpact);

        LaunchParabolicProjectile(centerImpact);
        LaunchParabolicProjectile(rightImpact);
        LaunchParabolicProjectile(leftImpact);

        alreadyAttacked = true;
        Invoke(nameof(ResetAttack), timeBetweenAttacks);
    }

}
