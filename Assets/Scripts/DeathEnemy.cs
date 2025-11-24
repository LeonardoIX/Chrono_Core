using UnityEngine;

public class EnemyDeath : MonoBehaviour
{
    [Header("Configuração de Som")]
    public AudioClip SomDeMorte;
    [Range(0f, 1f)] public float VolumeMorte = 1f;

    [Header("Configuração de Cena")]
    public float TempoParaDestruir = 1.5f;

    // Variáveis internas
    private Animator anim;
    private Rigidbody2D rig;
    private Collider2D coll;
    private EnemyAI aiScript;
    private EnemyFollow followScript; // Referência extra caso use o script de seguir separado
    private bool jaMorreu = false;

    void Awake()
    {
        anim = GetComponent<Animator>();
        rig = GetComponent<Rigidbody2D>();
        coll = GetComponent<Collider2D>();
        aiScript = GetComponent<EnemyAI>();
        followScript = GetComponent<EnemyFollow>();
    }

    public void Die()
    {
        if (jaMorreu) return;
        jaMorreu = true;

        // 1. Desliga as IAs (Para de atacar e andar)
        if (aiScript != null) aiScript.enabled = false;
        if (followScript != null) followScript.enabled = false;

        // 2. Toca o som (Garantido com PlayClipAtPoint)
        if (SomDeMorte != null)
        {
            AudioSource.PlayClipAtPoint(SomDeMorte, transform.position, VolumeMorte);
        }

        // 3. Toca a Animação (CORRIGIDO AQUI)
        if (anim != null)
        {
            anim.SetBool("walk", false); // Garante que pare de andar
            
            // Troquei "SetTrigger("death")" pelo seu parâmetro original:
            anim.SetBool("IsDead", true); 
        }

        // 4. Trava a física
        if (rig != null)
        {
            rig.linearVelocity = Vector2.zero;
            rig.bodyType = RigidbodyType2D.Kinematic; 
        }

        // 5. Desliga colisão
        if (coll != null) coll.enabled = false;

        // 6. Destrói o inimigo
        Destroy(gameObject, TempoParaDestruir);
    }
}
