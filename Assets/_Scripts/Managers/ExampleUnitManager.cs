using UnityEngine;
using Unity.Netcode;
using System;

/// <summary>
/// An example of a scene-specific manager grabbing resources from the resource system
/// Scene-specific managers are things like grid managers, unit managers, environment managers etc
/// </summary>
public class ExampleUnitManager : StaticNetworkInstance<ExampleUnitManager> {

    

    [ServerRpc(RequireOwnership =false)]
    public void SpawnUnitServerRpc(ExampleHeroType t, Vector3 pos,TeamType teamType) {

        ScriptableExampleHero heroSO = ResourceSystem.Instance.GetExampleHero(t);
        HeroUnitBase spawned = Instantiate(heroSO.Prefab, pos, Quaternion.identity, transform);
        NetworkObject heroNetworkObject = spawned.GetComponent<NetworkObject>();
        heroNetworkObject.Spawn(true);

        SpawnUnitClientRpc(t,spawned.NetworkObject,teamType);
    }
    [ClientRpc]
    public void SpawnUnitClientRpc(ExampleHeroType t, NetworkObjectReference networkObjectReference,TeamType teamType)
    {
        ScriptableExampleHero heroSO = ResourceSystem.Instance.GetExampleHero(t);
        networkObjectReference.TryGet(out NetworkObject networkObject);
        HeroUnitBase spawned = networkObject.GetComponent<HeroUnitBase>();
        // Apply possible modifications here such as potion boosts, team synergies, etc
        Stats stats = heroSO.BaseStats;
        CardType cardType = heroSO.cardType;
        bool isFlying = heroSO.isFlying;

        spawned.teamType = teamType;
        spawned.SetFlying(isFlying);
        spawned.SetCardType(cardType);
        spawned.SetStats(stats);
    }
    


}