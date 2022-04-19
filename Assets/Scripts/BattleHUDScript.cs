using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleHUDScript : MonoBehaviour
{
    public Slider hpBar; // slider for HP bar
    public Slider gaugeBar; // slider for gauge bar
    public Button ultraButton; // reference to ultra button


    public void SetHUD(Unit_Stats unitStats)
    {
        hpBar.maxValue = unitStats.maxHP; // set max hp as max in hp bar
        hpBar.value = unitStats.currentHP; // set current hp as current in hp bar
    }

    public void SetHP(int hp)
    {
        hpBar.value = hp;
    }

    public void SetGauge(int DP)
    {
        gaugeBar.value += DP;

        if (gaugeBar.value > gaugeBar.maxValue)
        {
            gaugeBar.value = gaugeBar.maxValue;
        }

        if (gaugeBar.value == gaugeBar.maxValue)
        {
            ultraButton.gameObject.SetActive(true);
        }
    }

    public void ResetGauge()
    {
        gaugeBar.value = 0;
        ultraButton.gameObject.SetActive(false);
    }
}
