using UnityEngine;

public class FootstepSimples : MonoBehaviour
{
    public AudioSource audioSource;      
    public AudioClip somDePasso;

    public float intervaloEntrePassos = 0.3f;

    private Rigidbody2D rb;
    private float timer = 0f;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        timer = intervaloEntrePassos;
    }

    void Update()
    {
        bool andando = Mathf.Abs(rb.linearVelocity.x) > 0.1f;

        if (andando)
        {
            timer -= Time.deltaTime;

            if (timer <= 0f)
            {
                audioSource.PlayOneShot(somDePasso);
                timer = intervaloEntrePassos;
            }
        }
        else
        {
            // ❗ Zeramos completamente o timer
            // isso garante que nenhum som será tocado após parar
            timer = intervaloEntrePassos;
	    audioSource.Stop();

            // Opcional: se quiser garantir que nenhum som sobreposto continue:
            // audioSource.Stop();
        }
    }
}

