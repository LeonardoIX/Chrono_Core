using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    public Animator animator;
    public Transform attackPoint;
    public float attackRange = 0.5f;
    public LayerMask enemyLayers;
    public int attackDamage = 40;
    
    // Variáveis de Combo
    private int comboStep = 0; // 0 = parado, 1 = atk1, 2 = atk2
    private float lastClickedTime = 0f;
    public float maxComboDelay = 1f; // Tempo máximo entre cliques para contar como combo

    private scrPlayer playerScript;

    void Start()
    {
        playerScript = GetComponent<scrPlayer>();
    }

    void Update()
    {
        if (playerScript != null && playerScript.isDead) return;

        // Se clicar no botão de ataque
        if(Input.GetMouseButtonDown(0))
        {
            lastClickedTime = Time.time;
            comboStep++; // Aumenta o passo do combo

            // Limita o combo a 2 passos (já que você tem Attack1 e Attack2)
            // Se quiser 3 ataques, mude para 3
            if (comboStep > 2) 
            {
                comboStep = 1; // Reseta para o primeiro ataque se spammar
            }

            animator.SetInteger("ComboStep", comboStep);
        }

        // Opcional: Resetar se passar muito tempo sem atacar (segurança)
        if (Time.time - lastClickedTime > maxComboDelay && comboStep != 0)
        {
           // Nota: Geralmente preferimos resetar via Animation Event (veja passo 3),
           // mas isso aqui previne bugs se a animação travar.
           // EndCombo(); 
        }
    }

    // --- EVENTOS DE ANIMAÇÃO ---

    // ESSA FUNÇÃO DEVE SER CHAMADA NO FIM DA ANIMAÇÃO (Animation Event)
    public void EndCombo()
    {
        comboStep = 0;
        animator.SetInteger("ComboStep", 0);
    }

    // ESSA FUNÇÃO JÁ EXISTIA (Dano)
    public void TriggerAttackDamage()
    {
        if (attackPoint == null) return;
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayers);
        foreach(Collider2D enemy in hitEnemies)
        {
            // Verifique se o script do inimigo se chama "Enemy" mesmo, ou ajuste aqui
            if(enemy.GetComponent<Enemy>() != null)
            {
                enemy.GetComponent<Enemy>().TakeDamage(attackDamage);
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        if(attackPoint == null) return;
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
}