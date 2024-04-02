using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class Knight : HeroUnitBase
{
    [SerializeField] private ParticleSystem hitParticle;
    // will be called from animaton when it hits on hitting frame
    public override void HitTarget()
    {
        base.HitTarget();
        HitTargetClientRpc(NetworkObject,Stats.AttackPower);
        
    }
    [ClientRpc]
    public void HitTargetClientRpc(NetworkObjectReference networkObjReference, int dmg)
    {
        networkObjReference.TryGet(out NetworkObject networkObject);
        Knight heroUnit = networkObject.GetComponent<Knight>();

        heroUnit.target.TakeDamage(Stats.AttackPower);
        heroUnit.hitParticle.Play();
    }
}
