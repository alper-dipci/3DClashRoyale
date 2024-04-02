using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class Arrow : Projectile
{
    public override void projectileReached()
    {
        base.projectileReached();

        if (!IsServer) return;

        if (Target != null)
        {
            HitTargetClientRpc(NetworkObject);            
        }        
    }
    [ClientRpc]
    public void HitTargetClientRpc(NetworkObjectReference networkObjReference)
    {
        networkObjReference.TryGet(out NetworkObject networkObject);
        Arrow arrow = networkObject.GetComponent<Arrow>();

        arrow.Target.TakeDamage(projectileDamage);
        Destroy(arrow.gameObject);
    }
}
