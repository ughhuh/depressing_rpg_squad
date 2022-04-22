using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public enum BattleState // store battle states
{ 
    Start, 
    PlayerTurn, 
    EnemyTurn, 
    Win, 
    Lose, 
    Ally01Turn, 
    Ally02Turn
} 
public enum StationState { NONE, LEFT, CENTER, RIGHT } // enumerator that stores player's station states

public class BattleManagerScript : MonoBehaviour
{
    public BattleState state; // variable used for setting and changing battle states

    // get spawning locations for player and enemy
    [SerializeField] Transform playerCenterStation;
    [SerializeField] Transform playerRightStation;
    [SerializeField] Transform playerLeftStation;
    [SerializeField] Transform enemyStation;

    // used for damage and healing calculations
    [SerializeField] UnitScript playerUnit;
    [SerializeField] UnitScript enemyUnit; 
    [SerializeField] UnitScript ally01Unit; 
    [SerializeField] UnitScript ally02Unit; 
    private UnitScript targetUnit;

    public Unit_Stats playerStats; // store player data
    public Unit_Stats enemyStats; // store enemy data 
    public Unit_Stats ally01Stats; // store ally 01 data
    public Unit_Stats ally02Stats; // store ally 02 data
    private Unit_Stats targetStats; // store target data

    public BattleHUDScript playerHUD; // reference to player HUD
    public BattleHUDScript enemyHUD; // reference to enemy HUD
    public BattleHUDScript ally01HUD; // reference to ally 01 HUD
    public BattleHUDScript ally02HUD; // reference to ally 02 HUD
    private BattleHUDScript targetHUD; // used for hud management
    public BattleHUDScript gaugeHUD; // reference to gauge HUD

    GameObject[] enemies; // stores enemies in the scene
    int enemyNumber = 0;

    [SerializeField] SFXManager SFXManager;
    [SerializeField] Text updateText;
    private float typingSpeed = 0.4f;
    private string activeName;

    [SerializeField] Animator transition;

    void Start()
    {
        transition.SetTrigger("Finish");
        state = BattleState.Start; // setting battle state as start
        StartCoroutine(SetupBattle()); // starting the setup process
    }

    IEnumerator SetupBattle()
    {
        // spawn the first enemy
        // check probability, spawn one more enemy (higher level lower probability)
        
        // spawn units on top of stations
        GameObject playerGO = Instantiate(playerStats.unitModel, playerCenterStation);
        GameObject ally01GO = Instantiate(ally01Stats.unitModel, playerRightStation);
        GameObject ally02GO = Instantiate(ally02Stats.unitModel, playerLeftStation);
        GameObject enemyGO = Instantiate(enemyStats.unitModel, enemyStation);

        // pass unit stats to HUD
        playerHUD.SetHUD(playerStats); 
        ally01HUD.SetHUD(ally01Stats); 
        ally02HUD.SetHUD(ally02Stats); 
        enemyHUD.SetHUD(enemyStats);

        enemies = GameObject.FindGameObjectsWithTag("Enemy");
        enemyNumber = enemies.Length;

        playerGO.GetComponent<PlayerMovement>().enabled = false; // disable player movement

        yield return new WaitForSeconds(1f); // wait for 1 second before starting player's turn

        state = BattleState.PlayerTurn; // set battle state to player's turn
        PlayerTurn(); // start player's turn
    }

    IEnumerator PlayerAttack()
    {
        switch (state) // check whose turn is this and update them to the target
        {
            case BattleState.PlayerTurn: targetStats = playerStats; break;
            case BattleState.Ally01Turn: targetStats = ally01Stats; break;
            case BattleState.Ally02Turn: targetStats = ally02Stats; break;
        }

        bool isDead = enemyUnit.TakeDamage(targetStats.unitDP); // add damage to enemy

        SFXManager.PlaySound("playerAttack");

        enemyHUD.SetHP(enemyStats.currentHP); // updating enemy hp

        yield return new WaitForSeconds(0.5f); // wait for 1 second
       
        if (isDead)
        {
            state = BattleState.Win; // change the battle state to win
            EndBattle(); // end the battle
        }
        else
        {
            switch (state) // check the status and change it accordingly
            {
                case BattleState.PlayerTurn: state = BattleState.Ally01Turn; PlayerTurn(); break;
                case BattleState.Ally01Turn: state = BattleState.Ally02Turn; PlayerTurn(); break;
                case BattleState.Ally02Turn: state = BattleState.EnemyTurn; StartCoroutine(EnemyTurn()); break;
            }
        }
    }

    IEnumerator PlayerHeal()
    {
        switch (state) // check whose turn is this and update them to the target
        {
            case BattleState.PlayerTurn: targetUnit = playerUnit; targetHUD = playerHUD; targetStats = playerStats; break;
            case BattleState.Ally01Turn: targetUnit = ally01Unit; targetHUD = ally01HUD; targetStats = ally01Stats; break;
            case BattleState.Ally02Turn: targetUnit = ally02Unit; targetHUD = ally02HUD; targetStats = ally02Stats; break;
        }
        
        SFXManager.PlaySound("playerHeal");
        targetUnit.Heal(targetStats.unitHP); // heal by 5 point, need to modify and set healing points through unitscript
        targetHUD.SetHP(targetStats.currentHP); // updating unit's hp

        yield return new WaitForSeconds(0.5f); // wait for 1 second

        switch (state) // check status and change it accordingly
        {
            case BattleState.PlayerTurn: state = BattleState.Ally01Turn; PlayerTurn(); break;
            case BattleState.Ally01Turn: state = BattleState.Ally02Turn; PlayerTurn(); break;
            case BattleState.Ally02Turn: state = BattleState.EnemyTurn; StartCoroutine(EnemyTurn()); break;
        }
    }

    IEnumerator EnemyTurn() // right now every enemy has the same attack and just does different amount of damage
    {
        for (int i = 0; i <= enemyNumber; i++) // enemy attack loop
        {
            yield return new WaitForSeconds(1f); // waiting for 1 second

            int randomNumber = Random.Range(0, 3); // get a random number of the party member

            switch (randomNumber)
            {
                case 0: // player gets attacked
                    targetUnit = playerUnit;
                    targetStats = playerStats;
                    targetHUD = playerHUD;
                    break;
                case 1: //ally 01 gets attacked
                    targetUnit = ally01Unit;
                    targetStats = ally01Stats;
                    targetHUD = ally01HUD;
                    break;
                case 2: // ally 02 gets attacked
                    targetUnit = ally02Unit;
                    targetStats = ally02Stats;
                    targetHUD = ally02HUD;
                    break;
                default: break;
            }

            targetUnit.TakeDamage(enemyStats.unitDP); // add damage to the target
            SFXManager.PlaySound("enemyAttack");

            targetHUD.SetHP(targetStats.currentHP); // updating target hp 
            gaugeHUD.SetGauge(enemyStats.unitDP); // update damage gauge 
        }

        bool isDead = playerUnit.CheckHP();
        bool isDead01 = ally01Unit.CheckHP();
        bool isDead02 = ally02Unit.CheckHP();

        yield return new WaitForSeconds(1f); // waiting for 1 second

        if (isDead && isDead01 && isDead02)
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

    IEnumerator UltraAttack()
    {
        yield return new WaitForSeconds(1f);

        switch (state) // check whose turn is this and update them to the target
        {
            case BattleState.PlayerTurn: targetUnit = playerUnit; targetStats = playerStats; break;
            case BattleState.Ally01Turn: targetUnit = ally01Unit; targetStats = ally01Stats; break;
            case BattleState.Ally02Turn: targetUnit = ally02Unit; targetStats = ally02Stats; break;
        }

        switch (targetStats.unitName) // check status and assign attacks accordingly
        {
            case "Ronnie": targetUnit.Rush(); break;
            case "Paige": playerUnit.Care(targetStats.ultraP); ally01Unit.Care(targetStats.ultraP); ally02Unit.Care(targetStats.ultraP); break; // heal every character
            case "Sage": enemyUnit.Fury(targetStats.ultraP); break;
            case "Vance": targetUnit.Fear(); break;
            case "Glen": targetUnit.Omen(); break;
        }

        gaugeHUD.ResetGauge(); // nullify gauge
        
        bool isDead = enemyUnit.CheckHP();

        enemyHUD.SetHP(enemyStats.currentHP); // updating enemy hp

        yield return new WaitForSeconds(1f); // wait for 1 second

        if (isDead)
        {
            state = BattleState.Win; // change the battle state to win
            EndBattle(); // end the battle
        }
        else
        {
            state = BattleState.EnemyTurn;  // proceed to enemy's turn immediately
            StartCoroutine(EnemyTurn());
        }
    } 

    void EndBattle()
    {
        if (state == BattleState.Win)
        {
            SFXManager.PlaySound("playerWin");
            // some text like "your party won this fight"
            SceneManager.LoadScene("SampleScene"); // go back to the previous scene
            // somehow destroy the enemy you just defeated in that scene
        }
        else if (state == BattleState.Lose)
        {
            SFXManager.PlaySound("playerLose");
            // you lost the battle
            // "you're dead / try again"
        }
    }   
    
    private void PlayerTurn()
    {
        switch (state) // check whose turn it is and update them to the target and put their name into line
        {
            case BattleState.PlayerTurn: activeName = playerStats.unitName; break;
            case BattleState.Ally01Turn: activeName = ally01Stats.unitName; break;
            case BattleState.Ally02Turn: activeName = ally02Stats.unitName; break;
            default: return;
        }

        StartCoroutine(TypeLine($"It's {activeName}'s turn! Choose your action."));

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
        switch (state) // only player and ally can heal
        {
            case BattleState.EnemyTurn: return;
            case BattleState.Start: return;
            case BattleState.Win: return;
            case BattleState.Lose: return;
            default: StartCoroutine(PlayerHeal()); break;
        }
    }

    public void OnFleeButton()
    {
        switch (state) // only player and allies can flee
        {
            case BattleState.EnemyTurn: return;
            case BattleState.Start: return;
            case BattleState.Win: return;
            case BattleState.Lose: return;
            default: SFXManager.PlaySound("playerFlee"); SceneManager.LoadScene("SampleScene"); break;
        }
            // if it's not player's turn, button won't work
         // go back to the previous scene
    }

    public void OnUltraButton()
    {
        switch (state) // only player and ally can heal
        {
            case BattleState.EnemyTurn: return;
            case BattleState.Start: return;
            case BattleState.Win: return;
            case BattleState.Lose: return;
            default: StartCoroutine(UltraAttack()); break;
            // default: Debug.Log("ultra attack triggered"); break;
        }
        // assign this to its own button ffs
        // right now the activation button is heal
    }

    IEnumerator TypeLine(string line)
    {
        updateText.text = null;

        foreach (char k in line) // type line letter by letter
        {   
            updateText.text += k;
            yield return new WaitForSeconds(typingSpeed);
        }
    }
}
