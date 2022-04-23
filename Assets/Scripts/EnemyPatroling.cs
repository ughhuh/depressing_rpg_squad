using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EnemyPatroling : MonoBehaviour
{
    [SerializeField] Animator transition;
    [SerializeField] SFXManager SFXManager;
    [SerializeField] AudioSource backgroundMusic;

    private void OnTriggerStay2D(Collider2D col)
    {
        if (col.tag == "Player") // check if collision happened with a player
        {
            StartCoroutine(WaitTrigger());
        }
    }

    private void OnTriggerExit2D(Collider2D col)
    {
        if (col.tag == "Player")
        {
            StopAllCoroutines();   
        }
    }
    IEnumerator WaitTrigger() // if player remains in trigger zone, wait fro a second and then trigger a fight
    {
        yield return new WaitForSeconds(1f);
        StartCoroutine(LoadScene());
    }
    IEnumerator LoadScene()
    {
        transition.SetTrigger("Start");
        backgroundMusic.Stop();
        SFXManager.PlaySound("battleTransition");

        yield return new WaitForSeconds(1f);

        // Destroy(this.gameObject); // destroy enemy
        SceneManager.LoadScene("SampleBattleScene"); // load battle scene
    }
}
