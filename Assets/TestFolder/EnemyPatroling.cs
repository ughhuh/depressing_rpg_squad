using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EnemyPatroling : MonoBehaviour
{
    /* what to make:
     * patrol a limited zone
     * when player is in the area, go to player
     */

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.tag == "Player") // check if collision happened with a player
        {
            Destroy(gameObject); // destroy enemy
            SceneManager.LoadScene("SampleBattleScene"); // load battle scene
        }
    }
}
