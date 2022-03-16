using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

[Flags] public enum BattleState 
{ 
    Start = 0, 
    PlayerTurn = 1, 
    EnemyTurn = 2, 
    Win = 4, 
    Lose = 8, 
    Ally01Turn = 16, 
    Ally02Turn = 32
} // enumerator that stores battle states
public enum StationState { NONE, LEFT, CENTER, RIGHT } // enumerator that stores player's station states


//https://www.youtube.com/watch?v=MLF9bOBCeqg&list=PLXV-vjyZiT4b7WGjgiqMy422AVyMaigl1&index=4&ab_channel=Etredal check this out

public class BattleManagerScript : MonoBehaviour
{
    public BattleState state; // variable used for setting and changing battle states
    public StationState station; // variable used fot setting and changing station states

    public GameObject playerPrefab; // get player prefab
    public GameObject Ally01Prefab; // get ally01 prefab
    public GameObject Ally02Prefab; // get ally 02 prefab
    public GameObject enemyPrefab; // get enemy prefab

    public Transform playerCenterStation; // get location of player station
    public Transform playerRightStation; // get location of player station
    public Transform playerLeftStation; // get location of player station
    public Transform enemyStation; // get location of enemy station

    private UnitScript playerUnit; // store player data
    private UnitScript enemyUnit; // store enemy data
    private UnitScript ally01Unit; // store ally 01 data
    private UnitScript ally02Unit; // store ally 02 data
    private UnitScript targetUnit; //used for unit management

    public BattleHUDScript playerHUD; // reference to player HUD
    public BattleHUDScript enemyHUD; // reference to enemy HUD
    public BattleHUDScript ally01HUD; // reference to ally 01 HUD
    public BattleHUDScript ally02HUD; // reference to ally 02 HUD
    private BattleHUDScript targetHUD; // used for hud management

    // private GameObject[,] positions = new GameObject[8, 8]; // array to store player's positions

    void Start()
    {
        state = BattleState.Start; // setting battle state as start
        StartCoroutine(SetupBattle()); // starting the setup process
    }

    IEnumerator SetupBattle()
    {
        GameObject playerGO = Instantiate(playerPrefab, playerCenterStation); // spawn player prefab on top of the station
        playerUnit = playerGO.GetComponent<UnitScript>(); // get data about player

        GameObject ally01GO = Instantiate(Ally01Prefab, playerRightStation); // spawn ally prefab on top of the station
        ally01Unit = ally01GO.GetComponent<UnitScript>(); // get data about ally
        
        GameObject ally02GO = Instantiate(Ally02Prefab, playerLeftStation); // spawn ally prefab on top of the station
        ally02Unit = ally02GO.GetComponent<UnitScript>(); // get data about ally

        GameObject enemyGO = Instantiate(enemyPrefab, enemyStation); // spawn enemy on top of station
        enemyUnit = enemyGO.GetComponent<UnitScript>(); // get data about enemy

        playerHUD.SetHUD(playerUnit); // pass player to HUD
        ally01HUD.SetHUD(ally01Unit); // pass ally01 to HUD
        ally02HUD.SetHUD(ally02Unit); // pass ally02 to HUD
        enemyHUD.SetHUD(enemyUnit); // pass enemy to HUD

        yield return new WaitForSeconds(1f); // wait for 1 second before starting player's turn

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
            switch (state) // check the status and change it accordingly
            {
                case BattleState.PlayerTurn: state = BattleState.Ally01Turn; break;
                case BattleState.Ally01Turn: state = BattleState.Ally02Turn; break;
                case BattleState.Ally02Turn: state = BattleState.EnemyTurn; StartCoroutine(EnemyTurn()); break;
            }
        }
    }

    IEnumerator PlayerHeal()
    {
        switch (state) // check whose turn is this and update them to the target
        {
            case BattleState.PlayerTurn: targetUnit = playerUnit; targetHUD = playerHUD; break;
            case BattleState.Ally01Turn: targetUnit = ally01Unit; targetHUD = ally01HUD; break;
            case BattleState.Ally02Turn: targetUnit = ally02Unit; targetHUD = ally02HUD; break;
        }

        targetUnit.Heal(5); // heal by 5 point, need to modify and set healing points through unitscript
        targetHUD.SetHP(targetUnit.currentHP); // updating unit's hp

        yield return new WaitForSeconds(1f); // wait for 1 second

        switch (state) // check status and change it accordingly
        {
            case BattleState.PlayerTurn: state = BattleState.Ally01Turn; break;
            case BattleState.Ally01Turn: state = BattleState.Ally02Turn; break;
            case BattleState.Ally02Turn: state = BattleState.EnemyTurn; StartCoroutine(EnemyTurn()); break;
        }

    }

    IEnumerator MoveRight() // not ready
    {
        station += 1;

        // playerGO.transform.SetParent(playerRightStation);

        yield return new WaitForSeconds(1f); // wait for 1 second

        state = BattleState.EnemyTurn; // change battle state to enemy's turn
        StartCoroutine(EnemyTurn()); // start enemy's turn
    }

    IEnumerator EnemyTurn()
    {
        yield return new WaitForSeconds(1f); // waiting for 1 second

        int randomNumber = Random.Range(0, 3); // get a random number of the party member

        switch(randomNumber)
        {
            case 0: // player gets attacked
                targetUnit = playerUnit;
                targetHUD = playerHUD;
                break;
            case 1: //ally 01 gets attacked
                targetUnit = ally01Unit;
                targetHUD = ally01HUD;
                break;
            case 2: // ally 02 gets attacked
                targetUnit = ally02Unit;
                targetHUD = ally02HUD;
                break;
            default: break;
        }
 
        bool isDead = targetUnit.TakeDamage(enemyUnit.unitDP); // add damage to the target

        targetHUD.SetHP(targetUnit.currentHP); // updating target hp 

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
        switch (state) // only player and ally can attack
        {
            case BattleState.EnemyTurn: return;
            case BattleState.Start: return;
            case BattleState.Win: return;
            case BattleState.Lose: return;
            default: StartCoroutine(PlayerAttack()); break;
        }

    }

    public void OnHealButton()
    {
        switch (state) // only player an ally can heal
        {
            case BattleState.EnemyTurn: return;
            case BattleState.Start: return;
            case BattleState.Win: return;
            case BattleState.Lose: return;
            default: StartCoroutine(PlayerHeal()); break;
        }
    }

    public void OnMoveRightButton()
    {
        switch (state) // only player and allies can move right
        {
            case BattleState.EnemyTurn: return;
            case BattleState.Start: return;
            case BattleState.Win: return;
            case BattleState.Lose: return;
            default: StartCoroutine(MoveRight()); break;
        }
    }

    public void OnMoveLeftButton()
    {
        switch (state) // only player and allies can move left
        {
            case BattleState.EnemyTurn: return;
            case BattleState.Start: return;
            case BattleState.Win: return;
            case BattleState.Lose: return;
            default: break;
        }

        // get current position
        // math
        // move to left one
        // if occupied, move behind (check for child objects in a parent i guess)

    }

    public void OnFleeButton()
    {
        switch (state) // only player and allies can flee
        {
            case BattleState.EnemyTurn: return;
            case BattleState.Start: return;
            case BattleState.Win: return;
            case BattleState.Lose: return;
            default: SceneManager.LoadScene("SampleScene"); break;
        }
            // if it's not player's turn, button won't work
         // go back to the previous scene
    }

}
