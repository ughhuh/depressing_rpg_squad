using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EnemyPatroling : MonoBehaviour
{
    [SerializeField] Animator transition;

    [SerializeField] SFXManager SFXManager;

    private void OnTriggerEnter2D(Collider2D col)
    {

        if (col.tag == "Player") // check if collision happened with a player
        {
            StartCoroutine(LoadScene());
        }
    }

    IEnumerator LoadScene()
    {
        transition.SetTrigger("Start");
        SFXManager.PlaySound("battleTransition");

        yield return new WaitForSeconds(1f);

        // Destroy(this.gameObject); // destroy enemy
        SceneManager.LoadScene("SampleBattleScene"); // load battle scene
    }
}
