using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleHUDScript : MonoBehaviour
{
    public Slider hpBar; // slider for HP bar
    

    public void SetHUD(UnitScript unit)
    {
        hpBar.maxValue = unit.maxHP; // set max hp as max in hp bar
        hpBar.value = unit.currentHP; // set current hp as current in hp bar
    }

    public void SetHP(int hp)
    {
        hpBar.value = hp;
    }
}
