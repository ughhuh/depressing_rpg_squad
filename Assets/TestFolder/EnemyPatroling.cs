using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EnemyPatroling : MonoBehaviour
{
    /* what to make:
     * trigger fight
     * patrol a limited zone
     * when player is in the area, find shortest path and go to player
     */

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.tag == "Player")
        {
            Destroy(gameObject);
            SceneManager.LoadScene("SampleBattleScene");
        }
    }
}
