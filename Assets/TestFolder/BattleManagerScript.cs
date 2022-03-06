using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum BattleState { Start, PlayerTurn, EnemyTurn, Win, Lose } // enumerator that stores battle states

public class BattleManagerScript : MonoBehaviour
{
    public BattleState state; // variable used for setting and changing battle states

    public GameObject playerPrefab; // get player prefab
    public GameObject enemyPrefab; // get enemy prefab

    public Transform playerStation; // get location of player station
    public Transform enemyStation; // get location of enemy station

    private UnitScript playerUnit; // store player data
    private UnitScript enemyUnit; // store enemy data

    public BattleHUDScript playerHUD; // reference to player HUD
    public BattleHUDScript enemyHUD; // reference to enemy HUD

    void Start()
    {
        state = BattleState.Start;
        StartCoroutine(SetupBattle());
    }

    IEnumerator SetupBattle()
    {
        GameObject playerGO = Instantiate(playerPrefab, playerStation); // spawn player prefab on top of station
        playerUnit = playerGO.GetComponent<UnitScript>(); // get data about player


        GameObject enemyGO = Instantiate(enemyPrefab, enemyStation); // spawn enemy on top of station
        enemyUnit = enemyGO.GetComponent<UnitScript>(); // get data about enemy

        playerHUD.SetHUD(playerUnit); // pass player to HUD
        enemyHUD.SetHUD(enemyUnit); // pass enemy to HUD

        yield return new WaitForSeconds(2f); // wait for 2 seconds before starting player's turn

        state = BattleState.PlayerTurn; // set battle state to player's turn
        PlayerTurn(); // start player's turn
    }

    IEnumerator PlayerAttack()
    {
        bool isDead = enemyUnit.TakeDamage(playerUnit.unitDP); // add damage to enemy

        enemyHUD.SetHP(enemyUnit.currentHP); // updating enemy hp

        yield return new WaitForSeconds(2f); // wait for 2 seconds
       
        if (isDead)
        {
            state = BattleState.Win; // change the battle state to win
            EndBattle(); // end the battle
        }
        else
        {
            state = BattleState.EnemyTurn; // change battle state to enemy's turn
            StartCoroutine(EnemyTurn()); // start enemy's turn
        }
    }

    IEnumerator PlayerHeal()
    {
        playerUnit.Heal(5); // heal by 5 point, need to modify and set healing points through unitscript
        playerHUD.SetHP(playerUnit.currentHP); // updating player's hp

        yield return new WaitForSeconds(2f); // wait for 2 seconds

        state = BattleState.EnemyTurn; // change battle state to enemy's turn
        StartCoroutine(EnemyTurn()); // start enemy's turn

    }

    IEnumerator EnemyTurn()
    {
        yield return new WaitForSeconds(1f); // waiting for 1 second
        
        bool isDead = playerUnit.TakeDamage(enemyUnit.unitDP); // add damage to player

        playerHUD.SetHP(playerUnit.currentHP); // updating player hp 

        yield return new WaitForSeconds(1f); // waiting for 1 second

        if (isDead)
        {
            state = BattleState.Lose; // change the battle state to lose
            EndBattle(); // end the battle
        }
        else
        {
            state = BattleState.PlayerTurn; // change battle state to player's turn
            PlayerTurn(); // start player's turn
        }

    }

    void EndBattle()
    {
        if (state == BattleState.Win)
        {
            // some text like "your party won this fight"
            SceneManager.LoadScene("SampleScene"); // go back to the previous scene
            // somehow destroy the enemy you just defeated in that scene
        }
        else if (state == BattleState.Lose)
        {
            // you lost the battle
            // "you're dead / try again"
        }
    }   
    
    private void PlayerTurn()
    {
        // a space for phrases like "choose your attack"
    } 
    public void OnAttackButton()
    {
        if (state != BattleState.PlayerTurn) // if it's not player's turn, button won't work
            return;

        StartCoroutine(PlayerAttack()); 
    }

    public void OnHealButton()
    {
        if (state != BattleState.PlayerTurn) // if it's not player's turn, button won't work
            return;

        StartCoroutine(PlayerHeal());
    }

    public void OnFleeButton()
    {
        if (state != BattleState.PlayerTurn) // if it's not player's turn, button won't work
            return;

        SceneManager.LoadScene("SampleScene"); // go back to the previous scene
    }


}
