using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class ZombieAI : MonoBehaviour
{
    public float detectionRange = 15f;
    public float attackRange = 2f;
    public float stoppingDistance = 1.5f;

    private Transform player;
    private Animator animator;
    private NavMeshAgent agent;

    private bool isAttacking = false;
    private bool isDead = false;
    private float attackCooldown = 1.5f;
    private float lastAttackTime = -999f;

    public Slider Sliderbar;
    int hp = 100;

    void Start()
    {
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
            player = playerObj.transform;
        else
            Debug.LogError("Player not found! Tag the player object as 'Player'");
    }

    void Update()
    {
        if (isDead || player == null) return;

        Sliderbar.value = hp;

        float distance = Vector3.Distance(transform.position, player.position);

        if (distance <= attackRange)
        {
            agent.isStopped = true;
            animator.SetBool("isRunning", false);
            FacePlayer();

            if (Time.time - lastAttackTime > attackCooldown)
            {
                animator.SetTrigger("Attack");
                lastAttackTime = Time.time;
            }

            isAttacking = true;
        }
        else if (distance <= detectionRange)
        {
            agent.isStopped = false;
            agent.SetDestination(player.position);
            animator.SetBool("isRunning", true);
            isAttacking = false;
        }
        else
        {
            agent.isStopped = true;
            animator.SetBool("isRunning", false);
            isAttacking = false;
        }
    }

    void FacePlayer()
    {
        Vector3 direction = (player.position - transform.position).normalized;
        direction.y = 0;

        if (direction.magnitude > 0.01f)
        {
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
        }
    }

    public void tackdamge(int damageammount)
    {
        if (isDead) return;

        hp -= damageammount;
        Sliderbar.value = hp;

        if (hp <= 0)
        {
            Die();
        }
        else
        {
            animator.SetTrigger("Hit"); // Use a separate "Hit" animation if available
        }
    }

    void Die()
    {
        isDead = true;
        animator.SetTrigger("die");
        GetComponent<Collider>().enabled = false;
        agent.isStopped = true;
        Debug.Log("Zombie died");

        // Optional: Destroy after death animation
        // Destroy(gameObject, 3f);
    }
}
