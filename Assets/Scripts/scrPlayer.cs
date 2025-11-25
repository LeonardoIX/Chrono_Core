using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class scrPlayer : MonoBehaviour
{
    [Header("Movement Properties")]
    public float Speed = 5f;
    public float JumpForce = 10f;
    public int maxJumps = 2;

    [Header("Detection")]
    public LayerMask groundLayer;

    [Header("Health & Damage")]
    public int maxHealth = 5;
    private int currentHealth;
    public bool isDead = false;

    [Header("Sons do Player")]
    public AudioClip somPulo;
    public AudioClip somDano;
    public AudioClip somMorte;
    [Range(0f, 1f)] public float volumePulo = 0.6f;
    [Range(0f, 1f)] public float volumeDano = 0.8f;
    [Range(0f, 1f)] public float volumeMorte = 1f;

    // Variáveis internas
    private Rigidbody2D rig;
    private Animator anim;
    private BoxCollider2D boxCollider;
    private int currentJumpCount = 0;
    private bool isGrounded;
    private float horizontalInput;

    // Para o som de passos
    private FootstepSimples footstepScript;

    void Start()
    {
        rig = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        boxCollider = GetComponent<BoxCollider2D>();
        currentHealth = maxHealth;

        // Pega o script de passos se existir
        footstepScript = GetComponent<FootstepSimples>();
    }

    void Update()
    {
        if (isDead) return;

        horizontalInput = Input.GetAxisRaw("Horizontal"); 

        if (Input.GetButtonDown("Jump"))
        {
            Jump();
        }

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

            // SOM DE PULO
            if (somPulo != null)
            {
                AudioSource.PlayClipAtPoint(somPulo, transform.position, volumePulo);
            }
        }
    }

    public void TakeDamage(int damage)
    {
        if (isDead) return;

        currentHealth -= damage;
        anim.SetTrigger("takeDamage");

        // SOM DE DANO
        if (somDano != null)
        {
            AudioSource.PlayClipAtPoint(somDano, transform.position, volumeDano);
        }

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        isDead = true;
        anim.SetTrigger("death");
        
        // SOM DE MORTE
        if (somMorte != null)
        {
            AudioSource.PlayClipAtPoint(somMorte, transform.position, volumeMorte);
        }

        rig.linearVelocity = Vector2.zero;
        rig.gravityScale = 0f;
        rig.bodyType = RigidbodyType2D.Kinematic;
        boxCollider.enabled = false;
        this.enabled = false;

        StartCoroutine(HandleDeath(2f));
    }

    private IEnumerator HandleDeath(float delay)
    {
        yield return new WaitForSeconds(delay);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    void OnDrawGizmos()
    {
        if (boxCollider != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(boxCollider.bounds.center + Vector3.down * 0.1f, boxCollider.bounds.size);
        }
    }
}
