using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitScript : MonoBehaviour
{
    [SerializeField] Unit_Stats unitStats;

    public int enemyCurrentHP;
    public int enemyMaxHP;
    public int enemyDP;
    
    public bool TakeDamage(int DP, bool isPlayer)
    {
        if (isPlayer == true) // if it's player, damage unit
        {
            unitStats.currentHP -= DP;

            if (unitStats.currentHP <= 0)
            {
                return true;
            }
            else
            {
                return false;
            } 
        }
        else // if it's enemy, damage temp points
        {
            enemyCurrentHP -= DP;

            if (enemyCurrentHP <= 0)
            {
                return true;
            }
            else
            {
                return false;
            } 
        }
    }

    public void Heal(int healP)
    {
        if (unitStats.currentHP <= 0) // can't heal the dead
            return;

        unitStats.currentHP += healP;

        if (unitStats.currentHP > unitStats.maxHP)
            unitStats.currentHP = unitStats.maxHP;    
    }

    public bool CheckHP()
    {
        if (unitStats.currentHP <= 0)
        {
            return true; // is dead
        }
        else
        {
            return false; // is alive
        }
    }

    public void Care(int healP)
    {
        Debug.Log("Paige ultra attack!");
        
        if (unitStats.currentHP <= 0) // can't heal the dead
            return;

        unitStats.currentHP += healP;

        if (unitStats.currentHP > unitStats.maxHP)
            unitStats.currentHP = unitStats.maxHP;
    }
    
    public void Fury(int DP)
    {
        Debug.Log("Sage ultra attack!");

        unitStats.currentHP -= DP * 2;
    }

    public void Fear()
    {
        Debug.Log("Vance ultra attack!");
        // perform attack

        // idk somehow skip enemy's turn and use a counter for it
        // after counter is done, end the battle

        // if enemy is tagged as boss, it doesn't succumb and survives
    }

    public void Omen()
    {
        Debug.Log("Glen ultra attack!");
        // perform attack

        // prepare a separate shield stat that starts as 0
        // update it here for everyone
        // enemy attack remains bool isDead = targetUnit.TakeDamage(enemyStats.unitDP);
        // in TakeDamage check if shield points are nonzero
        // if shield exists, unitStats.shieldHP -= DP;

        // have some kind of built in counter that counts turns. after 2 turns the shield is nullified
    }

    public void Rush()
    {
        Debug.Log("Ronnie ultra attack!");
        // this should probably be in a battlemanagerscript and use takedamage
        // perform attack

        // if there's one enemy, just attack it
        // if more than 1 enemy, make a random number generator
        // pick target according to the number
        // picked enemy takes damage
        // rinse and repeat 3-5 times (pick random number again)
    }

    public void SetupEnemy()
    {
        enemyCurrentHP = unitStats.currentHP;
        enemyMaxHP = unitStats.maxHP;
        enemyDP = unitStats.unitDP;
    }
}
