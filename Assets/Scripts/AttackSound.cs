using UnityEngine;

public class AttackSound : MonoBehaviour
{
    public AudioSource audioSource;     // Arraste o AudioSource do Player aqui
    public AudioClip somDeAtaque;       // Arraste o áudio do ataque aqui
    public float volumeAtaque = 1f;     // Ajuste o volume no Inspector

    void Update()
    {
        // Detecta o comando de ataque (exemplo: botão esquerdo do mouse ou "Fire1")
        if (Input.GetButtonDown("Fire1"))
        {
            audioSource.PlayOneShot(somDeAtaque, volumeAtaque);
        }
    }
}

