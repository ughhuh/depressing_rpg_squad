using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SFXManager : MonoBehaviour
{
    [SerializeField] AudioSource playerAttackSound;
    [SerializeField] AudioSource enemyAttackSound;
    [SerializeField] AudioSource playerHealSound;
    [SerializeField] AudioSource playerFleeSound;

    [SerializeField] AudioSource playerWinSound;
    [SerializeField] AudioSource playerLoseSound;

    [SerializeField] AudioSource menuClickSound;
    [SerializeField] AudioSource battleTransitionSound;


    // add walking sound

    public void PlaySound(string sound)
    {
        switch (sound)
        {
            case "playerAttack":
                playerAttackSound.Play();
                break;
            case "enemyAttack":
                enemyAttackSound.Play();
                break;
            case "playerHeal":
                playerHealSound.Play();
                break;
            case "playerFlee":
                playerFleeSound.Play();
                break;
            case "playerWin":
                playerWinSound.Play();
                break;
            case "playerLose":
                playerLoseSound.Play();
                break;
            case "click":
                menuClickSound.Play();
                break;
            case "battleTransition":
                battleTransitionSound.Play();
                break;

        }
    }
}
