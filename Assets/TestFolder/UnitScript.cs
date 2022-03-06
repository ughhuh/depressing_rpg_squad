using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitScript : MonoBehaviour
{
    public int unitLevel;

    public int unitDP;

    public int maxHP;
    public int currentHP;

    public bool TakeDamage(int DP)
    {
        currentHP -= DP;

        if (currentHP <= 0)
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
        currentHP += healP;

        if (currentHP > maxHP)
            currentHP = maxHP;
        
    }
}
