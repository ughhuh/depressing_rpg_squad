using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SFXManager : MonoBehaviour
{
    // references to audio sources
    [SerializeField] AudioSource playerAttackSound;
    [SerializeField] AudioSource enemyAttackSound;
    [SerializeField] AudioSource playerHealSound;
    [SerializeField] AudioSource playerFleeSound;
    [SerializeField] AudioSource playerUltraAttack;

    [SerializeField] AudioSource playerWinSound;
    [SerializeField] AudioSource playerLoseSound;

    [SerializeField] AudioSource menuClickSound;
    [SerializeField] AudioSource menuClickSound1;
    [SerializeField] AudioSource battleTransitionSound;

    [SerializeField] public AudioSource stepsSound;

    public void PlaySound(string sound) // get a name of sound, play a sound 
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
            case "click1":
                menuClickSound1.Play();
                break;
            case "battleTransition":
                battleTransitionSound.Play();
                break;
            case "ultraAttack":
                playerUltraAttack.Play();
                break;
            case "steps":
                stepsSound.PlayDelayed(stepsSound.time);
                stepsSound.Play();
                break;
        }
    }
}
