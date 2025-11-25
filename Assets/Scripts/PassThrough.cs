using UnityEngine;
using System.Collections;

public class PassThrough : MonoBehaviour
{

    private Collider2D _collider;
    private bool _playerOnPlataform;

    private void Update()
    {
        if(_playerOnPlataform && Input.GetAxisRaw("Vertical") < 0)
        {
            _collider.enabled = false;
            StartCoroutine(EnableCollider());

        }
    }

    private void Start()
    {
        _collider = GetComponent<Collider2D>();
    }    

    private IEnumerator EnableCollider()
    {
        yield return new WaitForSeconds(0.5f);
        _collider.enabled = true;
    }

    private void SetPlayerOnPlataform(Collision2D other, bool value)
    {
        var player = other.gameObject.GetComponent<scrPlayer>();
        if(player != null)
        {
            _playerOnPlataform = value;
        }
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        SetPlayerOnPlataform(other, true);
    }

     private void OnCollisionExit2D(Collision2D other)
    {
        SetPlayerOnPlataform(other, false);
    }

}
