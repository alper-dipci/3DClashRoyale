using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class KingTower : HeroUnitBase
{
    [SerializeField] private ScriptableExampleUnitBase archerSO;
    private void Awake()
    {
        Stats stats = archerSO.BaseStats;
        CardType cardType = archerSO.cardType;

        SetCardType(cardType);
        SetStats(stats);
    }
    public override void ExecuteMove()
    {
        base.ExecuteMove();
        HitTarget();
    }
    public override void HitTarget()
    {
        base.HitTarget();
        if (!IsServer) return;
        HitTargetClientRpc(NetworkObject, Stats.AttackPower);
    }
    [ClientRpc]
    public void HitTargetClientRpc(NetworkObjectReference networkObjReference, int dmg)
    {
        networkObjReference.TryGet(out NetworkObject networkObject);
        HeroUnitBase heroUnit = networkObject.GetComponent<HeroUnitBase>();

        if (heroUnit.target == null) return;

        heroUnit.target.TakeDamage(Stats.AttackPower); ;
    }
    public override void Die()
    {
        ExampleGameManager.Instance.kingTowerDestroyed(teamType);
        base.Die();
    }
}
