using System;
using UnityEngine;

/// <summary>
/// Create a scriptable hero 
/// </summary>
[CreateAssetMenu(fileName = "New Scriptable Example")]
public class ScriptableExampleHero : ScriptableExampleUnitBase {
    public ExampleHeroType HeroType;
    public Sprite HeroSprite;
    public GameObject GhostPrefab;
    public int ElixirCost;
}

[Serializable]
public enum ExampleHeroType {
    Archer=0,
    Dog=1,
    Cowboy=2,
    MaceKnight=3,
    ArcherTower=4,
}

