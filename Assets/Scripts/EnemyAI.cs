using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    [Header("Referências")]
    public Transform player;
    public Animator animator;
    public Transform attackPoint;

    [Header("Detecção")]
    public float detectionRange = 8f; // Distância para começar a seguir o player
    private bool playerDetected = false; // Se o player foi detectado

    [Header("Movimento")]
    public float moveSpeed = 3f;
    public float stopDistance = 1.5f; // Distância que ele para de andar
    public float attackRange = 1.0f;  // Distância que ele decide atacar

    [Header("Combate")]
    public int damage = 10;
    public float attackCooldown = 2f; // Tempo de espera entre ataques
    public Vector2 attackSize = new Vector2(1f, 1f); // Tamanho da hitbox
    public LayerMask playerLayer;

    [Header("Vida")]
    public int maxHealth = 100;
    private int currentHealth;

    // Variáveis de Controle
    private float nextAttackTime = 0f;
    private bool useSecondAttack = false; 
    private bool isDead = false;
    
    // Variável para corrigir o problema de escala (tamanho)
    private Vector3 originalScale;

    void Start()
    {
        // Pega o Animator automaticamente se você esqueceu de arrastar
        if (animator == null) animator = GetComponent<Animator>();
        
        currentHealth = maxHealth;
        
        // Tenta achar o player automaticamente pela Tag se estiver vazio
        if (player == null) 
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null) player = playerObj.transform;
        }

        // SALVA O TAMANHO QUE VOCÊ DEFINIU NA UNITY
        // Isso impede que ele fique minúsculo quando o jogo começa
        originalScale = transform.localScale;
    }

    void Update()
    {
        if (isDead || player == null) return;

        // Calcula distância até o player
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        // SISTEMA DE DETECÇÃO
        // Uma vez detectado, continua seguindo (não "desdetecta")
        if (!playerDetected && distanceToPlayer <= detectionRange)
        {
            playerDetected = true;
        }

        // Só age se o player foi detectado
        if (playerDetected)
        {
            // 1. Lógica de Movimento
            if (distanceToPlayer > stopDistance)
            {
                MoveTowardsPlayer();
                animator.SetBool("walk", true);
            }
            else
            {
                // Parar de andar
                animator.SetBool("walk", false);
                
                // 2. Lógica de Ataque
                if (distanceToPlayer <= attackRange && Time.time >= nextAttackTime)
                {
                    PerformAttack();
                    nextAttackTime = Time.time + attackCooldown;
                }
            }
            
            // Virar o sprite para o lado do jogador
            LookAtPlayer();
        }
        else
        {
            // Inimigo em estado de espera (idle)
            animator.SetBool("walk", false);
        }
    }

    void MoveTowardsPlayer()
    {
        // Move apenas no eixo X (para não voar)
        Vector2 target = new Vector2(player.position.x, transform.position.y);
        transform.position = Vector2.MoveTowards(transform.position, target, moveSpeed * Time.deltaTime);
    }

    void PerformAttack()
    {
        // Alterna entre Ataque 1 e Ataque 2
        if (!useSecondAttack)
        {
            animator.SetTrigger("Attack1");
            useSecondAttack = true; 
        }
        else
        {
            animator.SetTrigger("Attack2");
            useSecondAttack = false; 
        }
    }

    // --- EVENTO DE ANIMAÇÃO ---
    // Lembre-se de colocar esse evento nas animações de ataque do inimigo!
    public void TriggerEnemyDamage()
    {
        if (attackPoint == null) return;

        // Cria a caixa de dano
        Collider2D hitPlayer = Physics2D.OverlapBox(attackPoint.position, attackSize, 0f, playerLayer);
        
        if (hitPlayer != null)
        {
            // Procura pelo script de vida do JOGADOR (scrPlayer)
            scrPlayer playerScript = hitPlayer.GetComponent<scrPlayer>();
            
            if (playerScript != null)
            {
                playerScript.TakeDamage(damage);
            }
        }
    }

    public void TakeDamage(int dmg)
    {
        if (isDead) return;

        currentHealth -= dmg;
        animator.SetTrigger("Hurt");
        
        // QUANDO LEVAR DANO, TAMBÉM ATIVA A DETECÇÃO
        // Assim o inimigo reage mesmo se você atacar de longe
        playerDetected = true;

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        isDead = true;
        animator.SetBool("IsDead", true); 
        
        // Desliga colisão e gravidade (opcional) para não atrapalhar
        GetComponent<Collider2D>().enabled = false;
        GetComponent<Rigidbody2D>().simulated = false; // Se tiver Rigidbody
        this.enabled = false; 
    }

    void LookAtPlayer()
    {
        // Usa o tamanho original salvo no Start para não encolher o inimigo
        float sizeX = Mathf.Abs(originalScale.x);

        if (player.position.x > transform.position.x)
        {
            // Olha para direita (X positivo)
            transform.localScale = new Vector3(sizeX, originalScale.y, originalScale.z);
        }
        else if (player.position.x < transform.position.x)
        {
            // Olha para esquerda (X negativo)
            transform.localScale = new Vector3(-sizeX, originalScale.y, originalScale.z);
        }
    }

    void OnDrawGizmosSelected()
    {
        if (attackPoint == null) return;
        
        // Hitbox de ataque (amarelo)
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(attackPoint.position, attackSize);
        
        // Distância de parada (azul)
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, stopDistance);
        
        // RANGE DE DETECÇÃO (verde)
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }
}