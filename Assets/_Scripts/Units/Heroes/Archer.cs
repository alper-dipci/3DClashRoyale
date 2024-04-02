using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
public class Archer : HeroUnitBase
{
    [SerializeField] Arrow arrowPrefab;
    [SerializeField] Transform arrowSpawnPos;
    public override void HitTarget()
    {
        base.HitTarget();
        if (!IsServer) return;
        Arrow arrow = Instantiate(arrowPrefab, arrowSpawnPos.position,transform.rotation);
        NetworkObject arrowNetworkObject = arrow.GetComponent<NetworkObject>();
        arrowNetworkObject.Spawn();
        setArrowTargetClientRpc(arrowNetworkObject, Stats.AttackPower);
        
    }
    [ClientRpc]
    public void setArrowTargetClientRpc(NetworkObjectReference networkObjReference, int dmg)
    {
        networkObjReference.TryGet(out NetworkObject networkObject);
        Arrow arrow = networkObject.GetComponent<Arrow>();

        arrow.Target = target;
        arrow.projectileDamage = dmg;
    }

}
