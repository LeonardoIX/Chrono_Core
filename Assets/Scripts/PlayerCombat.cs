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
    public float attackRate = 2f;
    
    private float nextAttackTime = 0f;
    private scrPlayer playerScript; // Referência para verificar se está morto

    void Start()
    {
        playerScript = GetComponent<scrPlayer>();
    }

    void Update()
    {
        // Não ataca se estiver morto
        if (playerScript != null && playerScript.isDead) return;

        if(Time.time >= nextAttackTime)
        {
            if(Input.GetMouseButtonDown(0))
            {
                StartAttackAnimation();
                nextAttackTime = Time.time + 1f / attackRate;
            }
        }
    }

    void StartAttackAnimation()
    {
        // Apenas toca a animação. O dano não é aplicado aqui.
        animator.SetTrigger("Attack");
    }

    // ESSA FUNÇÃO DEVE SER CHAMADA PELA ANIMAÇÃO (Animation Event)
    public void TriggerAttackDamage()
    {
        if (attackPoint == null) return;

        // Detectar inimigos na hitbox do ataque
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayers);

        // Dar dano
        foreach(Collider2D enemy in hitEnemies)
        {
            // Verifica se o inimigo tem script de vida (ajuste "Enemy" para o nome do seu script)
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