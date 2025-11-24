using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerDeath : MonoBehaviour
{
    [Header("Configuração de Som")]
    public AudioSource audioSource;
    public AudioClip SomDeMorte;
    [Range(0f, 1f)] public float VolumeMorte = 1f;

    [Header("Configuração de Cena")]
    public float TempoParaReiniciar = 2f;

    // Variáveis internas
    private scrPlayer player;
    private Animator anim;
    private Rigidbody2D rig;
    private Collider2D coll;
    private bool jaMorreu = false;

    void Awake()
    {
        player = GetComponent<scrPlayer>();
        anim = GetComponent<Animator>();
        rig = GetComponent<Rigidbody2D>();
        coll = GetComponent<Collider2D>();

        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();
    }

    public void Die()
    {
        if (jaMorreu) return; 
        jaMorreu = true;

        // 1. Desliga o controle do jogador IMEDIATAMENTE
        if (player != null)
        {
            player.isDead = true;
            player.enabled = false; 
        }

        // 2. Toca o som (Método garantido)
        if (SomDeMorte != null)
        {
            AudioSource.PlayClipAtPoint(SomDeMorte, transform.position, VolumeMorte);
        }

        // 3. Toca a Animação (Usando o Trigger original)
        if (anim != null)
        {
            // Força a parada das outras animações
            anim.SetBool("walk", false);
            anim.SetBool("jump", false);
            anim.SetBool("isGrounded", true);
            
            // Aciona o gatilho de morte
            anim.SetTrigger("death");
        }

        // 4. Trava a física (Kinematic permite animação, Static às vezes trava)
        if (rig != null)
        {
            rig.linearVelocity = Vector2.zero;
            rig.gravityScale = 0f; 
            rig.bodyType = RigidbodyType2D.Kinematic; // Kinematic segura o player no ar mas deixa animar
        }
        
        if (coll != null) coll.enabled = false;

        // 5. Reinicia
        Invoke("ReiniciarCena", TempoParaReiniciar);
    }

    void ReiniciarCena()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
