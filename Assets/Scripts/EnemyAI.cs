using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    [Header("Referências")]
    public Transform player;
    public Animator animator;
    public Transform attackPoint;
    public Transform groundCheck;

    [Header("Detecção")]
    public float visionRange = 15f;
    public float losePlayerRange = 20f;
    public LayerMask groundLayer;
    public LayerMask obstacleLayer;

    [Header("Movimento")]
    public float moveSpeed = 5f;
    public float stopDistance = 1.5f;
    public float attackRange = 1.0f;

    [Header("Pulo Inteligente")]
    public float jumpForce = 16f;
    public float jumpCooldown = 0.5f;
    private float lastJumpTime = 0f;

    [Header("Combate")]
    public int damage = 10;
    public float attackCooldown = 2f;
    public Vector2 attackSize = new Vector2(1f, 1f);
    public LayerMask playerLayer;

    [Header("Vida")]
    public int maxHealth = 100;
    private int currentHealth;

    [Header("Sons do Inimigo")]
    public AudioClip somAtaque;
    public AudioClip somDano;
    public AudioClip somMorte;
    [Range(0f, 1f)] public float volumeAtaque = 0.7f;
    [Range(0f, 1f)] public float volumeDano = 0.8f;
    [Range(0f, 1f)] public float volumeMorte = 1f;

    private enum EnemyState { Idle, Chasing, Attacking, Dead }
    private EnemyState currentState = EnemyState.Idle;

    private Rigidbody2D rig;
    private float nextAttackTime = 0f;
    private bool isGrounded;
    private bool isDead = false;
    private bool needsToJump = false;

    private Vector3 originalScale;
    private float lastPosX = 0f;
    private float stuckTimer = 0f;

    void Start()
    {
        rig = GetComponent<Rigidbody2D>();
        currentHealth = maxHealth;

        if (player == null)
            player = GameObject.FindGameObjectWithTag("Player").transform;

        originalScale = transform.localScale;
        lastPosX = transform.position.x;
    }

    void Update()
    {
        if (isDead) return;

        CheckGround();

        float dist = Vector2.Distance(transform.position, player.position);

        switch (currentState)
        {
            case EnemyState.Idle:
                animator.SetBool("walk", false);

                if (dist <= visionRange)
                    currentState = EnemyState.Chasing;
                break;

            case EnemyState.Chasing:
                HandleChasing(dist);
                break;

            case EnemyState.Attacking:
                HandleAttacking(dist);
                break;
        }

        LookAtPlayer();
    }

    void FixedUpdate()
    {
        if (currentState == EnemyState.Chasing)
            DetectStuck();
    }

    void HandleChasing(float dist)
    {
        if (dist > losePlayerRange)
        {
            animator.SetBool("walk", false);
            currentState = EnemyState.Idle;
            return;
        }

        if (dist <= stopDistance)
        {
            animator.SetBool("walk", false);
            currentState = EnemyState.Attacking;
            return;
        }

        animator.SetBool("walk", true);

        MoveTowardsPlayer();
        CheckForObstacles();
    }

    void HandleAttacking(float dist)
    {
        animator.SetBool("walk", false);

        if (dist > attackRange)
        {
            currentState = EnemyState.Chasing;
            return;
        }

        if (Time.time >= nextAttackTime)
        {
            animator.SetTrigger("Attack1");
            nextAttackTime = Time.time + attackCooldown;

            // SOM DE ATAQUE
            if (somAtaque != null)
            {
                AudioSource.PlayClipAtPoint(somAtaque, transform.position, volumeAtaque);
            }
        }
    }

    void MoveTowardsPlayer()
    {
        float direction = Mathf.Sign(player.position.x - transform.position.x);

        if (IsOnSlope())
        {
            rig.linearVelocity = new Vector2(direction * moveSpeed, rig.linearVelocity.y);
        }
        else
        {
            rig.linearVelocity = new Vector2(direction * moveSpeed, rig.linearVelocity.y);
        }

        if (needsToJump && isGrounded && Time.time >= lastJumpTime + jumpCooldown)
        {
            Jump();
            needsToJump = false;
        }
    }

    void CheckForObstacles()
    {
        float direction = Mathf.Sign(player.position.x - transform.position.x);

        Vector2 origin = new Vector2(transform.position.x, transform.position.y + 0.4f);

        RaycastHit2D wallHit = Physics2D.Raycast(origin, Vector2.right * direction, 0.5f, obstacleLayer);

        Vector2 forward = new Vector2(transform.position.x + direction * 0.6f, transform.position.y);
        RaycastHit2D groundAhead = Physics2D.Raycast(forward, Vector2.down, 1f, groundLayer);

        needsToJump = false;

        if (wallHit.collider != null && !IsOnSlope())
            needsToJump = true;

        if (groundAhead.collider == null)
            needsToJump = true;
    }

    void DetectStuck()
    {
        if (Mathf.Abs(transform.position.x - lastPosX) < 0.01f)
        {
            stuckTimer += Time.fixedDeltaTime;

            if (stuckTimer > 0.5f && isGrounded)
            {
                Jump();
                stuckTimer = 0f;
            }
        }
        else
        {
            stuckTimer = 0f;
        }

        lastPosX = transform.position.x;
    }

    bool IsOnSlope()
    {
        Vector2 pos = transform.position;
        RaycastHit2D hit = Physics2D.Raycast(pos, Vector2.down, 1f, groundLayer);

        if (hit.collider == null)
            return false;

        float angle = Vector2.Angle(hit.normal, Vector2.up);

        return angle > 0f && angle <= 40f;
    }

    void Jump()
    {
        rig.linearVelocity = new Vector2(rig.linearVelocity.x, 0);
        rig.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        lastJumpTime = Time.time;
    }

    void CheckGround()
    {
        if (groundCheck != null)
            isGrounded = Physics2D.OverlapCircle(groundCheck.position, 0.2f, groundLayer);
        else
            isGrounded = Physics2D.OverlapCircle(transform.position + Vector3.down * 0.5f, 0.2f, groundLayer);
    }

    public void TriggerEnemyDamage()
    {
        Collider2D hit = Physics2D.OverlapBox(attackPoint.position, attackSize, 0f, playerLayer);

        if (hit != null)
        {
            scrPlayer p = hit.GetComponent<scrPlayer>();
            p?.TakeDamage(damage);
        }
    }

    public void TakeDamage(int dmg)
    {
        if (isDead) return;

        currentHealth -= dmg;
        animator.SetTrigger("Hurt");

        // SOM DE DANO
        if (somDano != null)
        {
            AudioSource.PlayClipAtPoint(somDano, transform.position, volumeDano);
        }

        if (currentState == EnemyState.Idle)
            currentState = EnemyState.Chasing;

        if (currentHealth <= 0)
            Die();
    }

    void Die()
    {
        isDead = true;
        currentState = EnemyState.Dead;

        animator.SetBool("IsDead", true);
        animator.SetBool("walk", false);

        // SOM DE MORTE
        if (somMorte != null)
        {
            AudioSource.PlayClipAtPoint(somMorte, transform.position, volumeMorte);
        }

        // Desliga física
        if (rig != null)
        {
            rig.linearVelocity = Vector2.zero;
            rig.bodyType = RigidbodyType2D.Kinematic;
        }

        // Desliga colisão
        Collider2D coll = GetComponent<Collider2D>();
        if (coll != null) coll.enabled = false;

        // Desliga o script
        this.enabled = false;

        // Destrói depois de um tempo
        Destroy(gameObject, 1.5f);
    }

    void LookAtPlayer()
    {
        if (player == null) return;

        float sizeX = Mathf.Abs(originalScale.x);

        if (player.position.x > transform.position.x)
            transform.localScale = new Vector3(sizeX, originalScale.y, originalScale.z);
        else
            transform.localScale = new Vector3(-sizeX, originalScale.y, originalScale.z);
    }

    void OnDrawGizmosSelected()
    {
        if (attackPoint != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(attackPoint.position, attackSize);
        }
    }
}
