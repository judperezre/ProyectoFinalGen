using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Assertions.Must;

public class RangedEnemiesBehaviour : MonoBehaviour
{
    public NavMeshAgent agent;
    public Transform player;
    public LayerMask whatIsGround, whatIsPlayer;
    public GameObject projectile;
    public Transform arrowPoint;
    public float health;

    //animations

    [SerializeField]
    Animator shootingAnimator;
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
        shootingAnimator.SetFloat("MoveSpeed", speed);

        isPlayerInSightRange = Physics.CheckSphere(transform.position, sightRange, whatIsPlayer);
        isPlayerInAttackRange = Physics.CheckSphere(transform.position, attackRange, whatIsPlayer);

        if (!isPlayerInSightRange && !isPlayerInAttackRange)
        {
            shootingAnimator.SetBool("isPlayerInAttackRange", false);
            Patrolling();
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
    }

    private void Patrolling()
    {
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


        if (!alreadyAttacked)
        {

            ///Ranged Attack code:


            transform.LookAt(player);
            shootingAnimator.SetBool("isPlayerInAttackRange", true);
            shootingAnimator.SetFloat("MoveSpeed", 0f);

            ///


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
        arrow.GetComponent<Rigidbody>().AddForce(transform.forward * 25f, ForceMode.Impulse);
        arrow.GetComponent<Rigidbody>().AddForce(transform.up * 8f, ForceMode.Impulse);
    }

}
