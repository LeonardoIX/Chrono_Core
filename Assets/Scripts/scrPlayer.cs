using System.Collections;
using UnityEngine;

public class scrPlayer : MonoBehaviour
{
    // Movement Properties
    [Header("Movement Properties")]
    public float Speed = 5f;
    public float JumpForce = 10f;
    public int maxJumps = 2; // Número máximo de pulos (1=pulo simples, 2=pulo duplo)

    // State Variables
    private Rigidbody2D rig;
    private Animator anim;
    private int currentJumpCount = 0; // Contador de pulos usados no ar
    
    // Variável para a Layer do chão (configure no Inspector para a Layer correta)
    public LayerMask groundLayer; 

    // Start é chamado uma vez antes do primeiro frame
    void Start()
    {
        rig = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();

        // Boa prática: se o Rigidbody2D não existir, desabilita o script
        if (rig == null)
        {
            Debug.LogError("Rigidbody2D not found! Please add a Rigidbody2D component to the player.");
            enabled = false;
        }
    }

    // FixedUpdate é chamado em intervalos de tempo fixos, ideal para Física (Rigidbody)
    void FixedUpdate()
    {
        Move();
    }

    // Update é chamado uma vez por frame, ideal para Inputs
    void Update()
    {
        JumpInput();

        // --- ADIÇÃO IMPORTANTE ---
        // Envia a velocidade vertical (Y) do Rigidbody para o parâmetro "velocityY" do Animator.
        // É isso que vai controlar as transições para o "playerFall".
        anim.SetFloat("velocityY", rig.velocity.y);
    }

    void Move()
    {
        float horizontalInput = Input.GetAxis("Horizontal");

        // 1. Movimentação (usando Rigidbody2D.velocity)
        // Move o personagem horizontalmente, mas mantém a velocidade vertical atual (rig.velocity.y)
        rig.velocity = new Vector2(horizontalInput * Speed, rig.velocity.y);

        // 2. Lógica de Animação e Virar (Flip)
        if (horizontalInput != 0f)
        {
            anim.SetBool("walk", true);
            
            if (horizontalInput > 0f)
            {
                // Rotação normal (olhando para a direita)
                transform.eulerAngles = new Vector3(0f, 0f, 0f);
            }
            else // horizontalInput < 0f
            {
                // Rotação invertida em Y (olhando para a esquerda)
                transform.eulerAngles = new Vector3(0f, 180f, 0f);
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
            if (currentJumpCount < maxJumps) // Verifica se o jogador ainda tem pulos disponíveis
            {
                // Resetar a velocidade vertical é crucial para garantir que o pulo duplo
                // tenha a força total, mesmo que o jogador esteja caindo.
                rig.velocity = new Vector2(rig.velocity.x, 0f);

                // Aplica a força do pulo
                rig.AddForce(new Vector2(0f, JumpForce), ForceMode2D.Impulse);

                // Incrementa o contador de pulos usados
                currentJumpCount++;
                
                // Ativa a animação de pulo
                anim.SetBool("jump", true);
            }
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        // Para a sua lógica que usa a Layer 6:
        if(collision.gameObject.layer == 6) // Layer 6 deve ser configurada como "Ground"
        {
            // Resetar o contador de pulos e o estado da animação ao tocar o chão
            currentJumpCount = 0;
            anim.SetBool("jump", false);
        }
    }
}