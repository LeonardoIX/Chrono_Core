using UnityEngine;
using System.Collections;

public class ReloadScene : MonoBehaviour
{
    [Header("Configurações")]
    [Tooltip("Usar transição suave do SceneController?")]
    public bool useTransition = true;
    
    [Tooltip("Delay antes de reiniciar (segundos)")]
    public float delayBeforeReload = 0.5f;
    
    [Header("Opcional - Efeitos")]
    [Tooltip("Som que toca quando o player entra no trigger")]
    public AudioClip deathSound;
    
    private AudioSource audioSource;
    private bool hasTriggered = false; // Evita ativar múltiplas vezes
    
    void Start()
    {
        // Pega o AudioSource se tiver som configurado
        if (deathSound != null)
        {
            audioSource = GetComponent<AudioSource>();
            if (audioSource == null)
            {
                audioSource = gameObject.AddComponent<AudioSource>();
            }
        }
    }
    
    void OnTriggerEnter2D(Collider2D other)
    {
        // Verifica se é o Player e se ainda não foi ativado
        if (other.CompareTag("Player") && !hasTriggered)
        {
            hasTriggered = true;
            
            // Toca som se tiver
            if (deathSound != null && audioSource != null)
            {
                audioSource.PlayOneShot(deathSound);
            }
            
            // Reinicia a cena
            StartCoroutine(ReloadCurrentScene());
        }
    }
    
    IEnumerator ReloadCurrentScene()
    {
        // Aguarda o delay configurado
        yield return new WaitForSeconds(delayBeforeReload);
        
        // Usa o SceneController diretamente se estiver disponível
        if (useTransition && SceneController.instance != null)
        {
            // Chama o método NextLevel mas modificado para reiniciar
            StartCoroutine(ReloadWithTransition());
        }
        else
        {
            // Reinicia direto sem transição
            UnityEngine.SceneManagement.SceneManager.LoadScene(
                UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex
            );
        }
    }
    
    IEnumerator ReloadWithTransition()
    {
        // Acessa o Animator através de reflexão ou usa o SceneController
        SceneController controller = SceneController.instance;
        
        // Pega o animator através do campo serializado usando reflexão
        System.Reflection.FieldInfo field = controller.GetType().GetField("transitionAnim", 
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        
        if (field != null)
        {
            Animator anim = (Animator)field.GetValue(controller);
            if (anim != null)
            {
                anim.SetTrigger("End");
                yield return new WaitForSeconds(1);
                UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(
                    UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex
                );
                anim.SetTrigger("Start");
            }
        }
    }
    
    // Visualização no Editor (opcional)
    void OnDrawGizmos()
    {
        // Desenha o trigger em vermelho
        Gizmos.color = new Color(1f, 0f, 0f, 0.3f);
        BoxCollider2D box = GetComponent<BoxCollider2D>();
        
        if (box != null)
        {
            Gizmos.matrix = transform.localToWorldMatrix;
            Gizmos.DrawCube(box.offset, box.size);
        }
    }
}