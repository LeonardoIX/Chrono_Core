using UnityEngine;

public class JumpSound : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip somDePulo;
    public float volumeDoPulo = 1.5f;

    private bool noChao = false;

    void OnCollisionEnter2D(Collision2D col)
    {
        // Assim que tocar no chão, sabemos que está no chão
        if (col.collider.CompareTag("Plataforma") || col.collider.CompareTag("Grama"))
        {
            noChao = true;
        }
    }

    void OnCollisionExit2D(Collision2D col)
    {
        // Quando sair do chão
        if (col.collider.CompareTag("Plataforma") || col.collider.CompareTag("Grama"))
        {
            noChao = false;
        }
    }

    void Update()
    {
        // Detecta o comando de pulo
        if (Input.GetButtonDown("Jump"))
        {
            audioSource.PlayOneShot(somDePulo, volumeDoPulo);
        }
    }
}

