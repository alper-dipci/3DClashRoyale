using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
/// <summary>
/// This will share logic for any unit on the field. Could be friend or foe, controlled or not.
/// Things like taking damage, dying, animation triggers etc
/// </summary>
public class UnitBase : NetworkBehaviour {
    public TeamType teamType;
    [SerializeField] public Animator animator;
    public Stats Stats { get; private set; }
    public CardType CardType;

    [SerializeField] protected HealthBarUi healthBarUi;
    public int health;

    [HideInInspector]
    public bool isDead;

    [HideInInspector]
    public bool isHittable=false;

    public UnitBase target;
    public Transform UnitHitPosition;
    [SerializeField] private GameObject deathParticle;


    protected float attackTimer;
    public static EventHandler onUnitDead;

    public virtual void SetCardType(CardType cardType) => CardType = cardType;
    public virtual void SetStats(Stats stats) => Stats = stats;
    
    public virtual void SetTarget(UnitBase Target) => target = Target;
    
    //this one will be called from still server but from the other heros in servers scene
    public virtual void TakeDamage(int dmg) {
        if (!IsServer) return;
        TakeDamageClientRpc(NetworkObject, dmg);
    }

    [ClientRpc]
    public void TakeDamageClientRpc(NetworkObjectReference networkObjReference, int dmg)
    {
        networkObjReference.TryGet(out NetworkObject networkObject);
        UnitBase heroUnit = networkObject.GetComponent<UnitBase>();

        heroUnit.health -= dmg;
        heroUnit.healthBarUi.updateHealthBar(health, Stats.Health);
        if (heroUnit.health <= 0)
            heroUnit.Die();
    }


    public virtual void Die()
    {
        isDead = true;
        Instantiate(deathParticle, transform.position, Quaternion.identity);
        if (!IsServer) return;
        Destroy(gameObject);
    }
    public virtual void ExecuteMove()
    {
        attackTimer = Stats.TimeBetweenAttacks;
    }
    public virtual void HitTarget()
    {
        if(!IsServer) return;
        attackTimer = Stats.TimeBetweenAttacks;
    }
    
    protected virtual void Update()
    {
        
        if(attackTimer >=0)
            attackTimer -= Time.deltaTime;
    }
}
