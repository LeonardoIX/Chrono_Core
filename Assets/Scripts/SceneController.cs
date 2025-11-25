using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;


public class SceneController : MonoBehaviour
{
    public static SceneController instance;
    [SerializeField] Animator transitionAnim;

    private void Awake(){
        if(instance == null){
            instance = this;
            DontDestroyOnLoad(gameObject);
        }else{
            Destroy(gameObject);
        }
    }

    //Script para trocar de cena "level"
    public void NextLevel(){
        StartCoroutine(LoadLevel());
    }

    IEnumerator LoadLevel()
    {
        transitionAnim.SetTrigger("End");
        yield return new WaitForSeconds(1);
        SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex + 1);  
        transitionAnim.SetTrigger("Start");
    }
}
