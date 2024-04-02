using System;
using UnityEngine;

/// <summary>
/// Keeping all relevant information about a unit on a scriptable means we can gather and show
/// info on the menu screen, without instantiating the unit prefab.
/// </summary>
public abstract class ScriptableExampleUnitBase : ScriptableObject {
    public CardType cardType;
    public bool isFlying;

    [SerializeField] private Stats _stats;
    public Stats BaseStats => _stats;

    // Used in game
    public HeroUnitBase Prefab;
    
    // Used in menus
    public string Description;
    public Sprite MenuSprite;
}

/// <summary>
/// Keeping base stats as a struct on the scriptable keeps it flexible and easily editable.
/// We can pass this struct to the spawned prefab unit and alter them depending on conditions.
/// </summary>
[Serializable]
public struct Stats {
    public int Health;
    public int AttackPower;
    public float AttackRange;
    public float TimeBetweenAttacks;
    public float targetDetectRange;
    //just for detecting tower to check all map
    public float maxRangeToDetectTower;
    public float moveSpeed;
}
[Serializable]
public enum CardType {
    normalHero = 0,
    towerKiller = 1,
    building = 2,
    tower=3,
}


