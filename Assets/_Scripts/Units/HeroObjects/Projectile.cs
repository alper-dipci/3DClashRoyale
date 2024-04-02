using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Projectile : NetworkBehaviour
{
    public UnitBase Target;
    public int projectileDamage;
    [SerializeField] private float projectileSpeed;
    private Vector3 lastPositionOfTarget;


    public virtual void Update()
    {
        if(Target != null)
        {
            transform.position=Vector3.MoveTowards(transform.position, Target.UnitHitPosition.position, Time.deltaTime * projectileSpeed);
            lastPositionOfTarget= Target.UnitHitPosition.position;
        }           
        else
        {
            transform.position = Vector3.MoveTowards(transform.position, lastPositionOfTarget, Time.deltaTime * projectileSpeed);
        }

        if (Vector3.Distance(transform.position, lastPositionOfTarget) < .1f)
        {
            projectileReached();
        }
            
    }
    public virtual void projectileReached()
    {

    }
    
}
