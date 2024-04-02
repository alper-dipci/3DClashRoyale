using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using System.Linq;
public class PlayerLogic : NetworkBehaviour
{
    public static PlayerLogic LocalInstance;

    public TeamType teamType;
    public ScriptableHeroDeck playerDeckSO;
    public static EventHandler onAnyPlayerSpawned;

    private ScriptableExampleHero currentChooseHero;   
    private List<ScriptableExampleHero> heroDeck;
    private GameObject ghostPrefab;
    private int heroIndexinList;
    private PlayerDeckUi playerDeckUi;
    private ElixirBarUi playerElixirBarUi;

    [SerializeField] private LayerMask groundLayer;
    public GameObject selectableAreasParent;

    [SerializeField] private Vector3 RedTeamSpawnPos;
    [SerializeField] private Vector3 BlueTeamSpawnPos;

    private const String playerDeckUi_Tag = "PlayerDeckUi";
    private const String ElixirUi_Tag = "ElixirUi";
    private const String SelectableGroundParent_Tag = "SelectableGroundParent";
    


    public int currentElixir;
    private float ElixirTimer;

    public override void OnNetworkSpawn()
    {
        TeamSelectionUi.onTeamSelected += TeamSelectionUi_onTeamSelected;
        onAnyPlayerSpawned?.Invoke(this, EventArgs.Empty);
        if (!IsOwner) return;
        heroDeck = playerDeckSO.Heroes.OrderBy(x=>Guid.NewGuid()).ToList();
        selectableAreasParent = GameObject.FindGameObjectWithTag(SelectableGroundParent_Tag);
        playerDeckUi = GameObject.FindGameObjectWithTag(playerDeckUi_Tag).GetComponent<PlayerDeckUi>();
        playerElixirBarUi = GameObject.FindGameObjectWithTag(ElixirUi_Tag).GetComponent<ElixirBarUi>();

        playerDeckUi.updateList(heroDeck);

        if (LocalInstance == null)
            LocalInstance = this;
        else
        {
            Destroy(LocalInstance);
            LocalInstance = this;
        }
        base.OnNetworkSpawn();
    }



    private void Start()
    {
        if (!IsOwner) return;
        playerElixirBarUi.UpdateBarElixir(currentElixir);        
    }

    private void TeamSelectionUi_onTeamSelected(object sender, TeamSelectedEventArgs e)
    {
        teamType = e.teamType;
        transform.position = teamType == TeamType.Red ? RedTeamSpawnPos : BlueTeamSpawnPos;
        transform.forward = teamType == TeamType.Red ? Vector3.forward : Vector3.back;
    }

    private void Update()
    {
        if (!IsOwner) return;
        
        updateElixir();
        //check if we are clicked on a card and spawn ghost
        checkSpawningHero();

        //if we have spawned ghost 
        if (ghostPrefab  != null)
        {
            //show selectable Areas
            ShowSelectableAreas(true);

            //then move it to cursor point
            moveGhostToMousePos();

            //if we click left spawn the hero
            if (Input.GetMouseButtonDown(0))
            {
                //if we have enought elixir
                if (currentElixir >= currentChooseHero.ElixirCost)
                    spawnHero();
                //else 
                    //AudioSystem.Instance.PlaySound(AudioSystem.Instance.elixirNotEnough);
            }
            //if we click right deSpawn the Ghost
            if (Input.GetMouseButtonDown(1))
                destroyGhost();
        }


    }

    private void ShowSelectableAreas(bool show)
    {
        for(int i =0; i<selectableAreasParent.transform.childCount ; i++)
        {
            GameObject go = selectableAreasParent.transform.GetChild(i).gameObject;
            TeamType groundTeamType = go.GetComponent<GroundTeamType>().teamType;
            if(groundTeamType==teamType || groundTeamType == TeamType.Both)
                go.GetComponent<MeshRenderer>().enabled=show;
        }
    }

    private void updateElixir()
    {
        if (ExampleGameManager.Instance.State.Value != GameState.Battle) return;

        ElixirTimer += Time.deltaTime*ExampleGameManager.Instance.elixirSpeed;
        playerElixirBarUi.UpdateBarFiller(ElixirTimer/10);

        if (ElixirTimer > 1f)
        {
            if (currentElixir < 10)
            {
                currentElixir += (int)ElixirTimer / 1;
                playerElixirBarUi.UpdateBarElixir(currentElixir);
            }
            ElixirTimer %= 1f;
        }
    }

    private void spawnHero()
    {
        currentElixir-=currentChooseHero.ElixirCost;
        ghostPrefab.SetActive(false);
        ShowSelectableAreas(false);
        ExampleUnitManager.Instance.SpawnUnitServerRpc(currentChooseHero.HeroType, ghostPrefab.transform.position, teamType);        
        heroDeck.RemoveAt(heroIndexinList);
        heroDeck.Insert(heroIndexinList, heroDeck[3]);
        heroDeck.RemoveAt(4);
        heroDeck.Add(currentChooseHero);
        destroyGhost();
        StartCoroutine(playerDeckUi.updateList(heroDeck, heroIndexinList));
        playerElixirBarUi.UpdateBarElixir(currentElixir);
    }
    private void destroyGhost()
    {
        currentChooseHero = null;
        Destroy(ghostPrefab);
    }

    private void checkSpawningHero()
    {
        if(Input.GetKeyDown(KeyCode.Alpha1) || Input.GetKeyDown(KeyCode.Alpha2) || Input.GetKeyDown(KeyCode.Alpha3) || Input.GetKeyDown(KeyCode.Alpha4))
            if(ghostPrefab != null)
            {
                destroyGhost();
            }
        if (getMousePos() == Vector3.one * 10) return;      
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            ghostPrefab=Instantiate(heroDeck[0].GhostPrefab,getMousePos(),transform.rotation);
            currentChooseHero = heroDeck[0];
            heroIndexinList = 0;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            ghostPrefab = Instantiate(heroDeck[1].GhostPrefab, getMousePos(), transform.rotation);
            currentChooseHero = heroDeck[1];
            heroIndexinList = 1;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            ghostPrefab = Instantiate(heroDeck[2].GhostPrefab, getMousePos(), transform.rotation);
            currentChooseHero = heroDeck[2];
            heroIndexinList = 2;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            ghostPrefab = Instantiate(heroDeck[3].GhostPrefab, getMousePos(), transform.rotation);
            currentChooseHero = heroDeck[3];
            heroIndexinList = 3;
        }
    }

    private void moveGhostToMousePos()
    {
        if(getMousePos() != Vector3.one*10)
        ghostPrefab.transform.position = getMousePos();
    }
    private Vector3 getMousePos()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, groundLayer))
        {
            if (hit.collider.gameObject.TryGetComponent(out GroundTeamType groundTeamType) && (groundTeamType.teamType == teamType || groundTeamType.teamType == TeamType.Both))
                return hit.point;
            else
                return Vector3.one*10;
        }
        else return Vector3.one * 10;
    }
    public override void OnNetworkDespawn()
    {
        base.OnNetworkDespawn();
        if (!IsOwner) return;
        TeamSelectionUi.onTeamSelected -= TeamSelectionUi_onTeamSelected;
        Destroy(gameObject);        
    }
}


public enum TeamType {
    Red =0,
    Blue =1,
    Both =2
}
