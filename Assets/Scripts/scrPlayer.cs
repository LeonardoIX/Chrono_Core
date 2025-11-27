using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class scrPlayer : MonoBehaviour
{
    [Header("Movement Properties")]
    public float Speed = 5f;
    public float JumpForce = 10f;
    public int maxJumps = 1;

    [Header("Detection")]
    public LayerMask groundLayer; // Configure isso no Inspector!

    [Header("Health & Damage")]
    public int maxHealth = 5;
    private int currentHealth;
    public bool isDead = false;

    [Header("Sons do Jogador")]
    public AudioClip jumpSound;
    public AudioClip walkSound;
    public AudioClip damageSound;
    public AudioClip deathSound;
    
    private AudioSource audioSource;
    private AudioSource walkAudioSource; // AudioSource separado para o som de andar

    // Variáveis internas
    private Rigidbody2D rig;
    private Animator anim;
    private BoxCollider2D boxCollider; // Referência ao colisor do corpo
    private int currentJumpCount = 0;
    private bool isGrounded;
    private float horizontalInput;
    private bool wasWalking = false; // Para controlar o som de andar

    void Start()
    {
        rig = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        boxCollider = GetComponent<BoxCollider2D>(); // Pega o colisor automaticamente
        currentHealth = maxHealth;

        // Configura AudioSource principal
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        // Cria um segundo AudioSource para o som de andar (loop)
        walkAudioSource = gameObject.AddComponent<AudioSource>();
        walkAudioSource.loop = true;
        walkAudioSource.clip = walkSound;
    }

    void Update()
    {
        if (isDead) return;

        horizontalInput = Input.GetAxisRaw("Horizontal"); 

        if (Input.GetButtonDown("Jump"))
        {
            Jump();
        }

        // Controla som de andar
        HandleWalkSound();

        // Atualiza Animator
        anim.SetBool("isGrounded", isGrounded);
        anim.SetBool("walk", horizontalInput != 0 && isGrounded);
        anim.SetBool("jump", !isGrounded && rig.linearVelocity.y > 0.1f); 
        anim.SetFloat("velocityY", rig.linearVelocity.y);

        // Vira o personagem
        if (horizontalInput > 0) transform.eulerAngles = Vector3.zero;
        else if (horizontalInput < 0) transform.eulerAngles = new Vector3(0, 180, 0);
    }

    void FixedUpdate()
    {
        if (isDead)
        {
            rig.linearVelocity = Vector2.zero;
            return;
        }

        CheckGround();
        Move();
    }

    void CheckGround()
    {
        // Cria uma caixa do tamanho do colisor do player e projeta ela levemente para baixo
        // Parâmetros: Centro, Tamanho, Angulo, Direção, Distância Extra, Layer
        RaycastHit2D hit = Physics2D.BoxCast(
            boxCollider.bounds.center, 
            boxCollider.bounds.size, 
            0f, 
            Vector2.down, 
            0.1f, 
            groundLayer
        );

        isGrounded = hit.collider != null;

        if (isGrounded)
        {
            currentJumpCount = 0;
        }
    }

    void Move()
    {
        rig.linearVelocity = new Vector2(horizontalInput * Speed, rig.linearVelocity.y);
    }

    void Jump()
    {
        if (currentJumpCount < maxJumps)
        {
            rig.linearVelocity = new Vector2(rig.linearVelocity.x, 0f);
            rig.AddForce(Vector2.up * JumpForce, ForceMode2D.Impulse);
            currentJumpCount++;

            // Toca som de pulo
            PlaySound(jumpSound);
        }
    }

    // Controla o som de andar (loop)
    void HandleWalkSound()
    {
        bool isWalking = horizontalInput != 0 && isGrounded;

        if (isWalking && !wasWalking)
        {
            // Começou a andar
            if (walkSound != null && walkAudioSource != null)
            {
                walkAudioSource.Play();
            }
        }
        else if (!isWalking && wasWalking)
        {
            // Parou de andar
            if (walkAudioSource != null)
            {
                walkAudioSource.Stop();
            }
        }

        wasWalking = isWalking;
    }

    public void TakeDamage(int damage)
    {
        if (isDead) return;

        currentHealth -= damage;
        anim.SetTrigger("takeDamage");

        // Toca som de dano
        PlaySound(damageSound);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        isDead = true;
        anim.SetTrigger("death");
        
        rig.linearVelocity = Vector2.zero;
        rig.gravityScale = 0f;
        boxCollider.enabled = false;
        this.enabled = false;

        // Para o som de andar se estiver tocando
        if (walkAudioSource != null)
        {
            walkAudioSource.Stop();
        }

        // Toca som de morte
        PlaySound(deathSound);

        StartCoroutine(HandleDeath(2f));
    }

    private IEnumerator HandleDeath(float delay)
    {
        yield return new WaitForSeconds(delay);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    // Método auxiliar para tocar sons
    private void PlaySound(AudioClip clip)
    {
        if (clip != null && audioSource != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }

    // Visualização para debug (aparece no editor quando seleciona o player)
    void OnDrawGizmos()
    {
        if (boxCollider != null)
        {
            Gizmos.color = Color.green;
            // Desenha onde o BoxCast está testando o chão
            Gizmos.DrawWireCube(boxCollider.bounds.center + Vector3.down * 0.1f, boxCollider.bounds.size);
        }
    }
}
