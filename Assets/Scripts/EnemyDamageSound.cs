using UnityEngine;

public class EnemyDamageSound : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip somDeDano;
    public float volume = 1f;

    public void PlayDamageSound()
    {
        if (somDeDano != null && audioSource != null)
        {
            if (!audioSource.isPlaying) // impede sons empilhados
                audioSource.PlayOneShot(somDeDano, volume);
        }
    }
}

