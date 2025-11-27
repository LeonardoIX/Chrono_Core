using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    [Header("Referências")]
    public Animator animator;
    public Transform attackPoint;
    private scrPlayer playerScript; // Para verificar se está morto

    [Header("Configuração do Ataque")]
    public LayerMask enemyLayers;
    public int attackDamage = 40;
    
    // MUDANÇA AQUI: De Raio (float) para Tamanho (Vector2 = Largura e Altura)
    public Vector2 attackSize = new Vector2(1f, 1f); 

    [Header("Configuração do Combo")]
    public float maxComboDelay = 1f;
    private int comboStep = 0; 
    private float lastClickedTime = 0f;

    [Header("Sons de Ataque")]
    public AudioClip[] attackSounds; // Array para múltiplos sons de ataque
    private AudioSource audioSource;

    void Start()
    {
        playerScript = GetComponent<scrPlayer>();
        audioSource = GetComponent<AudioSource>();
        
        // Se não tiver AudioSource, adiciona um
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    void Update()
    {
        // Não ataca se estiver morto
        if (playerScript != null && playerScript.isDead) return;

        // Input de Ataque
        if(Input.GetMouseButtonDown(0))
        {
            lastClickedTime = Time.time;
            comboStep++; 

            // Limita o combo a 2 passos (Ataque 1 e Ataque 2)
            if (comboStep > 2) 
            {
                comboStep = 1; 
            }

            animator.SetInteger("ComboStep", comboStep);
        }
    }

    // --- EVENTOS DE ANIMAÇÃO ---

    // Chamado no FINAL das animações de ataque (Animation Event)
    public void EndCombo()
    {
        comboStep = 0;
        animator.SetInteger("ComboStep", 0);
    }

    // Chamado no MOMENTO DO GOLPE (Animation Event)
    public void TriggerAttackDamage()
    {
        if (attackPoint == null) return;

        // Toca som de ataque
        PlayAttackSound();

        // MUDANÇA AQUI: Usando OverlapBox em vez de Circle
        // Parâmetros: Ponto Central, Tamanho (X, Y), Ângulo, Camada
        Collider2D[] hitEnemies = Physics2D.OverlapBoxAll(attackPoint.position, attackSize, 0f, enemyLayers);

        foreach(Collider2D enemy in hitEnemies)
        {
            // Tenta pegar o script do inimigo
            EnemyAI enemyScript = enemy.GetComponent<EnemyAI>();
            
            if(enemyScript != null)
            {
                enemyScript.TakeDamage(attackDamage);
            }
        }
    }

    // Método para tocar som de ataque
    private void PlayAttackSound()
    {
        if (attackSounds != null && attackSounds.Length > 0 && audioSource != null)
        {
            // Escolhe um som aleatório do array
            AudioClip clip = attackSounds[Random.Range(0, attackSounds.Length)];
            if (clip != null)
            {
                audioSource.PlayOneShot(clip);
            }
        }
    }

    // Desenha o quadrado vermelho na tela para você ajustar (Gizmos)
    void OnDrawGizmosSelected()
    {
        if(attackPoint == null) return;
        
        Gizmos.color = Color.red;
        // Desenha o cubo (retângulo) em vez da esfera
        Gizmos.DrawWireCube(attackPoint.position, attackSize);
    }
}
