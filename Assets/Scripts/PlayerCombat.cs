using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    [Header("Referências")]
    public Animator animator;
    public Transform attackPoint;
    private scrPlayer playerScript;

    [Header("Configuração do Ataque")]
    public LayerMask enemyLayers;
    public int attackDamage = 40;
    public Vector2 attackSize = new Vector2(1f, 1f); 

    [Header("Som de Ataque")]
    public AudioClip somAtaque;
    [Range(0f, 1f)] public float volumeAtaque = 0.7f;

    [Header("Configuração do Combo")]
    public float maxComboDelay = 1f;
    private int comboStep = 0; 
    private float lastClickedTime = 0f;

    void Start()
    {
        playerScript = GetComponent<scrPlayer>();
    }

    void Update()
    {
        if (playerScript != null && playerScript.isDead) return;

        // Input de Ataque
        if(Input.GetMouseButtonDown(0))
        {
            lastClickedTime = Time.time;
            comboStep++; 

            if (comboStep > 2) 
            {
                comboStep = 1; 
            }

            animator.SetInteger("ComboStep", comboStep);

            // SOM DE ATAQUE
            if (somAtaque != null)
            {
                AudioSource.PlayClipAtPoint(somAtaque, transform.position, volumeAtaque);
            }
        }
    }

    public void EndCombo()
    {
        comboStep = 0;
        animator.SetInteger("ComboStep", 0);
    }

    public void TriggerAttackDamage()
    {
        if (attackPoint == null) return;

        Collider2D[] hitEnemies = Physics2D.OverlapBoxAll(attackPoint.position, attackSize, 0f, enemyLayers);

        foreach(Collider2D enemy in hitEnemies)
        {
            EnemyAI enemyScript = enemy.GetComponent<EnemyAI>();
            
            if(enemyScript != null)
            {
                enemyScript.TakeDamage(attackDamage);
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        if(attackPoint == null) return;
        
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(attackPoint.position, attackSize);
    }
}
