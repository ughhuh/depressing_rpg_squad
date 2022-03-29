using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Unit Data", menuName = "ScriptableObjects/Unit Data", order = 1)]
public class Unit_Stats : ScriptableObject
{
    public string unitName;
    public int unitLevel;

    public GameObject unitModel;

    public int maxHP;
    public int currentHP;

    public int unitDP;
    public int unitHP;

}
