using UnityEngine;

public class DamageSound : MonoBehaviour
{
    public AudioClip somDeDano;
    public float volumeDano = 1f;

    public void PlayDamageSound()
    {
        AudioSource.PlayClipAtPoint(somDeDano, transform.position, volumeDano);
    }
}

