using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class MeleeEnemiesBehaviour : MonoBehaviour
{
    public NavMeshAgent agent;
    public Transform player;
    public LayerMask whatIsGround, whatIsPlayer;
    public float health;

    public int damage = 10;
    public bool canDamage;

    //Animations
    [SerializeField]
    Animator meleeAttackAnimator;
    public float rotationSpeed = 5f;

    //Patroll

    public Vector3 walkPoint;
    bool walkPointSet;
    public float walkPointRange;

    //Attacks

    public float timeBetweenAttacks;
    bool alreadyAttacked;

    //States

    public float sightRange, attackRange;
    public bool isPlayerInSightRange, isPlayerInAttackRange;

    private void Awake()
    {
        player = GameObject.Find("Player").transform;
        agent = GetComponent<NavMeshAgent>();
    }

    private void Update()
    {
        float speed = agent.velocity.magnitude;
        meleeAttackAnimator.SetFloat("MoveSpeed", speed);

        isPlayerInSightRange = Physics.CheckSphere(transform.position, sightRange, whatIsPlayer);
        isPlayerInAttackRange = Physics.CheckSphere(transform.position, attackRange, whatIsPlayer);

        if (!isPlayerInSightRange && !isPlayerInAttackRange)
        {
            meleeAttackAnimator.SetBool("isPlayerInAttackRange", false);
            Patrolling();
        }

        if (isPlayerInSightRange && !isPlayerInAttackRange)
        {
            meleeAttackAnimator.SetBool("isPlayerInAttackRange", false);
            ChasePlayer();
        }
        if (isPlayerInSightRange && isPlayerInAttackRange)
        {
            meleeAttackAnimator.SetBool("isPlayerInAttackRange", true);
            AttackPlayer();
        }
    }

    private void Patrolling()
    {
        agent.isStopped = false;

        Vector3 dir = agent.steeringTarget - transform.position;

        if (dir.sqrMagnitude > 0.01f)
        {
            Quaternion lookRotation = Quaternion.LookRotation(dir);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * rotationSpeed);
        }

        if (!walkPointSet)
        {
            SearchWalkPoint();
        }

        if (walkPointSet)
        {
            agent.SetDestination(walkPoint);
        }

        Vector3 distanceToWalkPoint = transform.position - walkPoint;

        if (distanceToWalkPoint.magnitude < 1f)
        {
            walkPointSet = false;
        }
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
    }
    private void AttackPlayer()
    {
        //Make sure enemy doesn't move
        agent.isStopped = true;
        agent.velocity = Vector3.zero;
        meleeAttackAnimator.SetFloat("MoveSpeed", 0f);


        if (!alreadyAttacked)
        {
            StartDamage();
            meleeAttackAnimator.SetBool("canAttack", true);
            alreadyAttacked = true;
            Invoke(nameof(ResetAttack), timeBetweenAttacks);
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
            Invoke(nameof(DestroyEnemy), 0.5f);
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
    }

    private void StartDamage()
    {
        canDamage = true;
        GetComponent<Collider>().enabled = true;
    }
    private void StopDamage()
    {
        canDamage = false;
        GetComponent<Collider>().enabled = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (canDamage && other.CompareTag("Player"))
        {
            PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damage);
            }
            StopDamage();
        }
    }

    private void DamageToPlayer() 
    {
        OnTriggerEnter(player.GetComponent<Collider>());
    }

}
