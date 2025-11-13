using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement; // Necessário para recarregar a cena

public class scrPlayer : MonoBehaviour
{
    // Movement Properties
    [Header("Movement Properties")]
    public float Speed = 5f;
    public float JumpForce = 10f;
    public int maxJumps = 2;

    //Health Properties
    [Header("Health & Damage")]
    public int maxHealth = 5;
    private int currentHealth;
    private bool isDead = false;

    // State Variables
    private Rigidbody2D rig;
    private Animator anim;
    private int currentJumpCount = 0;
    
    // Variável para a Layer do chão
    public LayerMask groundLayer; 

    // Start é chamado uma vez antes do primeiro frame
    void Start()
    {
        rig = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();

        if (rig == null)
        {
            Debug.LogError("Rigidbody2D not found!");
            enabled = false;
        }

        currentHealth = maxHealth;
        isDead = false;
    }

    // FixedUpdate
    void FixedUpdate()
    {
        // Não se mova se estiver morto
        if (isDead)
        {
            rig.linearVelocity = new Vector2(0, rig.linearVelocity.y);
            anim.SetBool("walk", false);
            return;
        }

        Move();
    }

    // Update
    void Update()
    {
        // Não pule se estiver morto
        if (isDead) return; 

        JumpInput();
        anim.SetFloat("velocityY", rig.linearVelocity.y);
    }

    void Move()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        rig.linearVelocity = new Vector2(horizontalInput * Speed, rig.linearVelocity.y);

        if (horizontalInput != 0f)
        {
            anim.SetBool("walk", true);
            
            if (horizontalInput > 0f)
            {
                transform.eulerAngles = new Vector3(0f, 0f, 0f); // Direita
            }
            else // horizontalInput < 0f
            {
                transform.eulerAngles = new Vector3(0f, 180f, 0f); // Esquerda
            }
        }
        else
        {
            anim.SetBool("walk", false);
        }
    }

    void JumpInput()
    {
        if (Input.GetButtonDown("Jump"))
        {
            if (currentJumpCount < maxJumps)
            {
                rig.linearVelocity = new Vector2(rig.linearVelocity.x, 0f);
                rig.AddForce(new Vector2(0f, JumpForce), ForceMode2D.Impulse);
                currentJumpCount++;
                anim.SetBool("jump", true);
            }
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        // Checagem de colisão com o chão
        if(collision.gameObject.layer == 6) // Layer 6 deve ser "Ground"
        {
            currentJumpCount = 0;
            anim.SetBool("jump", false);
        }
    }

    // DANO E VIDA
    public void TakeDamage(int damage)
    {
        // Se já estiver morto, não faça nada
        if (isDead) return;

        // Reduz a vida
        currentHealth -= damage;
        
        Debug.Log("Player Health: " + currentHealth);

        if (currentHealth <= 0)
        {
            currentHealth = 0;
            Die();
        }
        else
        {
            // Dispara o Trigger da sua animação de dano
            anim.SetTrigger("takeDamage"); // <-- MUDE "takeDamage" para o nome do seu trigger
        }
    }

    private void Die()
    {
        isDead = true;
        anim.SetTrigger("death"); // Dispara a animação de morte

        // Desativa o colisor do jogador
        GetComponent<Collider2D>().enabled = false;
        
        // Para o jogador completamente
        rig.linearVelocity = Vector2.zero;
        rig.gravityScale = 0f; // Impede que ele caia após morrer

        // Inicia a rotina para reiniciar o nível após um atraso
        StartCoroutine(HandleDeath(2f)); // Espera 2 segundos
    }


    /// Corotina para esperar um tempo e depois reiniciar a cena.

    private IEnumerator HandleDeath(float delay)
    {
        yield return new WaitForSeconds(delay);
        
        // Recarrega a cena atual
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}