using System.Collections;
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

    [SerializeField] SFXManager SFXManager; // reference to sound manager
    [SerializeField] AudioSource backgroundMusic;
    [SerializeField] Text updateText; // reference to running text
    private float typingSpeed = 0.05f; // reference to typing speed
    private string activeName; // temp storage of playable character's name

    [SerializeField] Animator transition; // reference to tanimator handling transitions

    public Unit_Stats[] enemyPool; // pool of enemies available for spawning

    [SerializeField] Image playerIcon; // reference to icons
    [SerializeField] Image ally01Icon;
    [SerializeField] Image ally02Icon;

    void Start()
    {
        transition.SetTrigger("Finish");
        state = BattleState.Start; // setting battle state as start
        StartCoroutine(SetupBattle()); // starting the setup process
    }

    IEnumerator SetupBattle()
    {
        // make images disabled by default
        playerIcon.GetComponent<Image>().color = new Color32(255, 255, 225, 100);
        ally01Icon.GetComponent<Image>().color = new Color32(255, 255, 225, 100);
        ally02Icon.GetComponent<Image>().color = new Color32(255, 255, 225, 100);

        int enemy = Random.Range(0, 5); // randomly pick an enemy to spawn

        // spawn units on top of stations
        GameObject playerGO = Instantiate(playerStats.unitModel, playerCenterStation);
        GameObject ally01GO = Instantiate(ally01Stats.unitModel, playerRightStation);
        GameObject ally02GO = Instantiate(ally02Stats.unitModel, playerLeftStation);
        GameObject enemyGO = Instantiate(enemyPool[enemy].unitModel, enemyStation);

        enemyUnit = enemyGO.GetComponent<UnitScript>();
        enemyUnit.SetupEnemy(); // setup temporary stats for enemy
        enemyStats = enemyPool[enemy]; // assign enemy stats to current enemy
        
        // pass unit stats to HUD
        playerHUD.SetHUD(playerStats); 
        ally01HUD.SetHUD(ally01Stats); 
        ally02HUD.SetHUD(ally02Stats); 
        enemyHUD.SetHUD(enemyStats);

        playerGO.GetComponent<PlayerMovement>().enabled = false; // disable player movement

        StartCoroutine(TypeLine($"YOU RAN INTO A WILD {enemyStats.unitName}!"));
        yield return new WaitForSeconds(3f); // wait for 1 second before starting player's turn

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
        StartCoroutine(TypeLine($"{targetStats.unitName} ATTACKS {enemyStats.unitName}!"));

        bool isDead = enemyUnit.TakeDamage(targetStats.unitDP, false); // add damage to enemy

        SFXManager.PlaySound("playerAttack");

        enemyHUD.SetHP(enemyUnit.enemyCurrentHP); // updating enemy hp

        yield return new WaitForSeconds(3f); // wait a bit
        StartCoroutine(TypeLine($"{enemyStats.unitName} TOOK {targetStats.unitDP} DAMAGE"));
        yield return new WaitForSeconds(3f); // wait a bit more

        if (isDead)
        {
            state = BattleState.Win; // change the battle state to win
            StartCoroutine(EndBattle()); // end the battle
        }
        else
        {
            switch (state) // check the status and change it accordingly
            {
                case BattleState.PlayerTurn: playerIcon.GetComponent<Image>().color = new Color32(255, 255, 225, 100); state = BattleState.Ally01Turn; PlayerTurn(); break;
                case BattleState.Ally01Turn: ally01Icon.GetComponent<Image>().color = new Color32(255, 255, 225, 100); state = BattleState.Ally02Turn; PlayerTurn(); break;
                case BattleState.Ally02Turn: ally02Icon.GetComponent<Image>().color = new Color32(255, 255, 225, 100); state = BattleState.EnemyTurn; StartCoroutine(EnemyTurn()); break;
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
        StartCoroutine(TypeLine($"{targetStats.unitName} HEALS {targetStats.unitHP} POINTS"));
        targetUnit.Heal(targetStats.unitHP); // heal by 5 point, need to modify and set healing points through unitscript
        targetHUD.SetHP(targetStats.currentHP); // updating unit's hp

        yield return new WaitForSeconds(4f); // wait for 1 second

        switch (state) // check the status and change it accordingly
        {
            case BattleState.PlayerTurn: playerIcon.GetComponent<Image>().color = new Color32(255, 255, 225, 100); state = BattleState.Ally01Turn; PlayerTurn(); break;
            case BattleState.Ally01Turn: ally01Icon.GetComponent<Image>().color = new Color32(255, 255, 225, 100); state = BattleState.Ally02Turn; PlayerTurn(); break;
            case BattleState.Ally02Turn: ally02Icon.GetComponent<Image>().color = new Color32(255, 255, 225, 100); state = BattleState.EnemyTurn; StartCoroutine(EnemyTurn()); break;
        }
    }

    IEnumerator EnemyTurn() // right now every enemy has the same attack and just does different amount of damage
    {
        StartCoroutine(EnemyAttack());

        yield return new WaitForSeconds(6f);

        bool isDead = playerUnit.CheckHP();
        bool isDead01 = ally01Unit.CheckHP();
        bool isDead02 = ally02Unit.CheckHP();

        if (isDead && isDead01 && isDead02)
        {
            state = BattleState.Lose; // change the battle state to lose
            StartCoroutine(EndBattle()); // end the battle
        }
        else
        {
            state = BattleState.PlayerTurn; // change battle state to player's turn
            PlayerTurn(); // start player's turn
        }

    }

    IEnumerator EnemyAttack()
    {
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

        StartCoroutine(TypeLine($"{enemyStats.unitName} ATTACKS {targetStats.unitName}!"));
        targetUnit.TakeDamage(enemyUnit.enemyDP, true); // add damage to the target
        SFXManager.PlaySound("enemyAttack");

        targetHUD.SetHP(targetStats.currentHP); // updating target hp 
        gaugeHUD.SetGauge(enemyUnit.enemyDP); // update damage gauge 

        yield return new WaitForSeconds(4f);

        StartCoroutine(TypeLine($"{targetStats.unitName} TOOK {enemyUnit.enemyDP} DAMAGE"));
        yield return new WaitForSeconds(3f);
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

        SFXManager.PlaySound("ultraAttack"); // play sfx
        gaugeHUD.ResetGauge(); // nullify gauge
        
        StartCoroutine(TypeLine($"{targetStats.unitName} USES ULTRA ATTACK ON {enemyStats.unitName}!!!"));
        yield return new WaitForSeconds(3f); // wait for 1 second

        bool isDead = enemyUnit.CheckHP();

        enemyHUD.SetHP(enemyUnit.enemyCurrentHP); // updating enemy hp

        if (isDead)
        {
            state = BattleState.Win; // change the battle state to win
            StartCoroutine(EndBattle()); // end the battle
        }
        else
        {
            state = BattleState.EnemyTurn;  // proceed to enemy's turn immediately
            StartCoroutine(EnemyTurn());
        }
    } 

    IEnumerator EndBattle()
    {
        if (state == BattleState.Win)
        {
            backgroundMusic.Stop();
            SFXManager.PlaySound("playerWin");
            StartCoroutine(TypeLine("YOUR PARTY WON!"));
            yield return new WaitForSeconds(6f);
            SceneManager.LoadScene("Zone1"); // go back to the previous scene
        }
        else if (state == BattleState.Lose)
        {
            backgroundMusic.Stop();
            SFXManager.PlaySound("playerLose");
            StartCoroutine(TypeLine("YOUR PARTY WAS SLAIN..."));
            yield return new WaitForSeconds(6f);
            SceneManager.LoadScene("LoseScene");
        }
    }   
    
    private void PlayerTurn()
    {
        switch (state) // check whose turn it is and update them to the target, enable an icon and put their name into line
        {
            case BattleState.PlayerTurn: activeName = playerStats.unitName; playerIcon.GetComponent<Image>().color = new Color32(255, 255, 225, 225); break;
            case BattleState.Ally01Turn: activeName = ally01Stats.unitName; ally01Icon.GetComponent<Image>().color = new Color32(255, 255, 225, 225); break;
            case BattleState.Ally02Turn: activeName = ally02Stats.unitName; ally02Icon.GetComponent<Image>().color = new Color32(255, 255, 225, 225); break;
            default: return;
        }

        StartCoroutine(TypeLine($"IT'S {activeName}'S TURN! CHOOSE YOUR ACTION."));

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
            default: backgroundMusic.Stop(); SFXManager.PlaySound("playerFlee"); SceneManager.LoadScene("Zone1"); break;
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
