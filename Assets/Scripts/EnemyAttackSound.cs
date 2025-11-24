using UnityEngine;

public class EnemyAttackSound : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip somDeAtaque;
    public float volume = 1f;

    // Chamado no momento do ataque
    public void PlayAttackSound()
    {
        if (audioSource != null && somDeAtaque != null)
        {
            audioSource.PlayOneShot(somDeAtaque, volume);
        }
    }
}

