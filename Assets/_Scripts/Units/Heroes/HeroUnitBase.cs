using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using Unity.Netcode;

public abstract class HeroUnitBase : UnitBase {

    
    public LayerMask targetLayerMask;
    private NavMeshAgent agent;
    private Vector3 lastPosition;
    public bool isFlying { get; private set; }
    private bool isAttacking;
    public virtual void SetFlying(bool isflying) => isFlying = isflying;
    private bool isWalkingCard => (CardType == CardType.normalHero || CardType == CardType.towerKiller);


    private void Start()
    {
        isHittable = true;
        health = Stats.Health;
        if (isWalkingCard) {
            agent = GetComponent<NavMeshAgent>();
            agent.stoppingDistance = Stats.AttackRange;
            findClosestTower();
            if(target!=null)
                agent.SetDestination(target.transform.position);
        }
        if (CardType != CardType.tower)
            //AudioSystem.Instance.PlaySound(AudioSystem.Instance.heroSpawn);
        isHittable = true;
    }

    protected override void Update()
    {
        if (isDead) return;

        //if we dont have target find one (at worst it will target the towers)
        if (target==null)
            checkForTarget();

        //we coulnd find a target
        if (target==null)
            return;

        //if target moved out from detect range reset target
        if (!isTargetInDetectRange())
        {
            target = null;
            checkForTarget();
        }


        //we checked for a target but still couldnt found so game is over or bug
        if (target == null && (isWalkingCard))
            return;

        //if (CardType == CardType.building)
            //health -= buildingDecreaseAmount;        

        //rest of it target related so return if we have no target in 
        if (target==null)
            return;

        

        //look to target
        if(CardType != CardType.tower)
            transform.LookAt(new Vector3(target.transform.position.x, transform.position.y, target.transform.position.z));

        //only normal heros and tower killers can move
        if (isWalkingCard)
            moveToTarget();

        //target moved out from our attack range ,  if there is a closer target take it
        if (!isTargetInAttackRange()) {
            checkForTarget();
            animator.SetBool("inRange", false);
            if (isWalkingCard)
                agent.isStopped = false;
            isAttacking = false;
         }

        checkAndAttackTarget();

        base.Update();       
    }

    private void checkAndAttackTarget()
    {
        int maxColliders = 40;
        Collider[] targetColliders = new Collider[maxColliders];
        int numColliders = Physics.OverlapSphereNonAlloc(transform.position, Stats.AttackRange, targetColliders, targetLayerMask);
        for (int i = 0; i < numColliders; i++)
        {
            if (targetColliders[i].TryGetComponent(out UnitBase unitBase))
            {
                if(unitBase == target)
                {
                    animator.SetBool("inRange", true);
                    if (isWalkingCard)
                        agent.isStopped=true;
                    isAttacking = true;
                    // can we hit
                    if (attackTimer <= 0 & target.isHittable)
                    {
                        if (!IsServer) return;
                        ExecuteMoveClientRpc(NetworkObject);                      
                    }
                }
            }
        }
        if (numColliders == 0)
        {
            checkForTarget();
            // set anim from walk to aim with this boolean
            animator.SetBool("inRange", false);
        }
    }
    [ClientRpc]
    public void ExecuteMoveClientRpc(NetworkObjectReference networkObjReference)
    {
        networkObjReference.TryGet(out NetworkObject networkObject);
        UnitBase unit = networkObject.GetComponent<UnitBase>();

        unit.animator.SetTrigger("attack");
        unit.ExecuteMove();
    }

    protected bool isTargetInAttackRange() { return Vector3.Distance(target.transform.position, transform.position) < Stats.AttackRange; }
    protected bool isTargetInDetectRange() { return Vector3.Distance(target.transform.position, transform.position) < Stats.AttackRange; }


    private void moveToTarget()
    {
        if (isAttacking) return;

        if (lastPosition == null) lastPosition = target.transform.position;

        if (Vector3.Distance(lastPosition , target.transform.position) > 0.01f) 
        {
            lastPosition = target.transform.position;
            agent.SetDestination(target.transform.position);
        }
    }

    private void checkForTarget()
    {
        switch (CardType)
        {
            case CardType.normalHero:
                checkForNormalHero();
                break;
            case CardType.towerKiller:
                checkForTowerKiller();
                break;
            case CardType.building:
                checkForBuilding();
                break;
            case CardType.tower:
                checkForBuilding();
                break;
        }
        // Set  max amount enemy in range for performance
        
    }

    private void checkForBuilding()
    {
        int maxColliders = 40;
        Collider[] targetColliders = new Collider[maxColliders];
        int numColliders = Physics.OverlapSphereNonAlloc(transform.position, Stats.targetDetectRange, targetColliders, targetLayerMask);
        for (int i = 0; i < numColliders; i++)
        {
            if (targetColliders[i].TryGetComponent(out UnitBase unitBase))
            {
                if (unitBase.teamType == teamType || unitBase.isDead)
                    continue;
                findTarget(unitBase);
            }
        }
    }

    private void checkForNormalHero()
    {
        int maxColliders = 40;
        Collider[] targetColliders = new Collider[maxColliders];
        int numColliders = Physics.OverlapSphereNonAlloc(transform.position, Stats.targetDetectRange, targetColliders, targetLayerMask);
        for (int i = 0; i < numColliders; i++)
        {
            if (targetColliders[i].TryGetComponent(out UnitBase unitBase)){
                if (unitBase.teamType == teamType || unitBase.isDead)
                    continue;
                findTarget(unitBase);
            } 
        }
        // if we couldnt found Target in Range
        if(target == null)
            findClosestTower();
    } 
    private void checkForTowerKiller()
    {
        int maxColliders = 40;
        Collider[] targetColliders = new Collider[maxColliders];
        int numColliders = Physics.OverlapSphereNonAlloc(transform.position, Stats.targetDetectRange, targetColliders, targetLayerMask);
        for (int i = 0; i < numColliders; i++)
        {
            if(targetColliders[i].TryGetComponent(out UnitBase unitBase))
            {
                if (unitBase.teamType == teamType)
                    continue;
                if (isWalkingCard)
                    continue;
                findTarget(targetColliders[i].gameObject.GetComponent<UnitBase>());
            }
        }
            
        // if we couldnt found Target in Range
        if (target == null)
            findClosestTower();
    }

    private void findClosestTower()
    {
        Collider[] targetColliders = Physics.OverlapSphere(transform.position, Stats.maxRangeToDetectTower, targetLayerMask);
        foreach (Collider collider in targetColliders)
        {
            if(collider.TryGetComponent(out UnitBase unitBase))
            {
                if (unitBase.teamType == teamType || unitBase.CardType != CardType.tower)
                    continue;

                findTarget(collider.gameObject.GetComponent<UnitBase>());
            }
        }
    }
    private void findTarget(UnitBase targetUnit)
    {
        if (target == null)
            target = targetUnit;
        else if (Vector3.Distance(targetUnit.transform.position, transform.position) < Vector3.Distance(target.transform.position, transform.position))
            target = targetUnit;
    }


    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, Stats.targetDetectRange);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, Stats.AttackRange);
        if (target == null) return;
        Gizmos.color = Color.green;
        Gizmos.DrawLine(transform.position, target.transform.position);
    }

}

