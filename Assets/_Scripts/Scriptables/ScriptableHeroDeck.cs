using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerDeck")]
public class ScriptableHeroDeck : ScriptableObject {
    public List<ScriptableExampleHero> Heroes;
}
