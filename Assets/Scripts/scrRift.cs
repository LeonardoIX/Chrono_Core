using UnityEngine;

public class EnemyFollow : MonoBehaviour
{
    [Header("Propriedades de Movimento")]
    public float speed = 3f;           // Velocidade de perseguição
    public float stopDistance = 0.5f;  // Distância mínima para parar

    [Header("Pulo Automático")]
    public float jumpForce = 10f;       // Força do pulo do inimigo
    public Transform groundCheck;      // Ponto de checagem de chão (coloque abaixo do inimigo)
    public LayerMask groundLayer;      // Layer do chão
    private bool isGrounded;

    [Header("Ataque & Dano")]
    public int damage = 1;             // Dano causado ao player
    public float attackCooldown = 1f;  // Tempo entre ataques
    private float lastAttackTime = 0f;

    [Header("Referências")]
    public Transform player;           // Player a ser seguido

    private Rigidbody2D rig;
    private Animator anim;

    void Start()
    {
        rig = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();

        // Se o player não for definido manualmente, procura pelo tag "Player"
        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
                player = playerObj.transform;
        }
    }

    void Update()
    {
        // Checa se está tocando o chão
        if (groundCheck != null)
        {
            isGrounded = Physics2D.OverlapCircle(groundCheck.position, 0.1f, groundLayer);
        }
    }

    void FixedUpdate()
    {
        if (player == null) return;

        float distance = Vector2.Distance(transform.position, player.position);

        if (distance > stopDistance)
        {
            // Calcula direção no eixo X
            Vector2 targetPosition = new Vector2(player.position.x, transform.position.y);
            Vector2 direction = (targetPosition - (Vector2)transform.position).normalized;

            // Move o inimigo no eixo X
            rig.linearVelocity = new Vector2(direction.x * speed, rig.linearVelocity.y);

            // Espelha o inimigo na direção do player
            if (direction.x > 0)
                transform.eulerAngles = new Vector3(0, 0, 0);
            else
                transform.eulerAngles = new Vector3(0, 180, 0);

            // Ativa animação de caminhada
            if (anim != null)
                anim.SetBool("walk", true);
        }
        else
        {
            // Para o inimigo
            rig.linearVelocity = new Vector2(0, rig.linearVelocity.y);
            if (anim != null)
                anim.SetBool("walk", false);
        }

        // Se o player estiver mais alto e o inimigo no chão, pula
        if (player.position.y > transform.position.y + 0.5f && isGrounded)
        {
            rig.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        }
    }

    // ====================================
    // DANO NO PLAYER POR COLISÃO
    // ====================================
    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            // Verifica cooldown
            if (Time.time >= lastAttackTime + attackCooldown)
            {
                scrPlayer playerScript = collision.gameObject.GetComponent<scrPlayer>();
                if (playerScript != null)
                {
                    playerScript.TakeDamage(damage);
                    lastAttackTime = Time.time;

                    // Animação de ataque, se houver
                    if (anim != null)
                        anim.SetTrigger("attack");
                }
            }
        }
    }

    // Gizmo opcional para ver o groundCheck
    private void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(groundCheck.position, 0.1f);
        }
    }
}

