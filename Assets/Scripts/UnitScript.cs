using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitScript : MonoBehaviour
{
    [SerializeField] Unit_Stats unitStats;
    
    public bool TakeDamage(int DP)
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

    public void Heal(int healP)
    {
        unitStats.currentHP += healP;

        if (unitStats.currentHP > unitStats.maxHP)
            unitStats.currentHP = unitStats.maxHP;
        
    }

    public void Care()
    {
        Debug.Log("Paige ultra attack!");
        // perform attack
        // set gauge to 0
    }
    
    public void Fury()
    {
        Debug.Log("Sage ultra attack!");
        // perform attack
        // set gauge to 0
    }

    public void Fear()
    {
        Debug.Log("Vance ultra attack!");
        // perform attack
        // set gauge to 0
    }

    public void Omen()
    {
        Debug.Log("Glen ultra attack!");
        // perform attack
        // set gauge to 0
    }

    public void Rush()
    {
        Debug.Log("Ronnie ultra attack!");
        // perform attack
        // set gauge to 0
    }
}
