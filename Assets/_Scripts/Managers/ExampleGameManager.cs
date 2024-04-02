using System;
using UnityEngine;
using Unity.Netcode;
using System.Collections.Generic;
using TMPro;


public class ExampleGameManager :NetworkSingleton<ExampleGameManager> {

    [SerializeField] private Transform playerPrefab;

    [SerializeField] private List<Vector3> RedTeamSpawnList;
    private int redTeamSpawnIndex;
    [SerializeField] private List<Vector3> BlueTeamSpawnList;
    private int blueTeamSpawnIndex;

    public NetworkVariable<GameState> State= new NetworkVariable<GameState>(GameState.WaitingToStart);

    private Dictionary<ulong, bool> playerReadyDictionary;
    public event EventHandler onLocalPlayerReadyChanged;
    private bool isLocalPlayerReady;
    public float elixirSpeed;

    public NetworkVariable<float> gamePlayingTimer=new NetworkVariable<float>(120f);

    public NetworkList<PlayerData> playerDataList;

    protected override void Awake()
    {
        playerDataList = new NetworkList<PlayerData>();
        DontDestroyOnLoad(gameObject);
        base.Awake();
    }
    public void startHost()
    {
        NetworkManager.Singleton.OnClientConnectedCallback += Singleton_OnClientConnectedCallback;
        NetworkManager.Singleton.StartHost();
    }

    private void Singleton_OnClientConnectedCallback(ulong clientId)
    {
        playerDataList.Add(new PlayerData
        {
            clientId = clientId,
            teamType = TeamType.Blue
        });
    }
    public void changePlayerTeam(ulong clientId,TeamType teamType)
    {
        changePlayerTeamServerRpc(clientId,teamType);
    }
    [ServerRpc(RequireOwnership =false)]
    public void changePlayerTeamServerRpc(ulong clientId, TeamType teamType)
    {
        PlayerData playerData = getPlayerDataFromClientId(clientId);
        int index = playerDataList.IndexOf(playerData);
        PlayerData playerDataNew = playerData;
        playerDataNew.teamType = teamType;
        playerDataList[index] = playerDataNew;
    }
    private PlayerData getPlayerDataFromClientId(ulong clientId)
    {
        foreach (PlayerData playerData in playerDataList)
        {
            if (playerData.clientId == clientId)
            {
                return playerData;
            }
        }
        return default;
    }

    public void startClient()
    {
        NetworkManager.Singleton.StartClient();
    }
    public void shutDownNetwork()
    {
        NetworkManager.Singleton.Shutdown();
    }

    public override void OnNetworkSpawn()
    {
        State.OnValueChanged += gameStateChanged;
        playerReadyDictionary = new Dictionary<ulong, bool>();
        if (IsServer)
        {
            NetworkManager.Singleton.SceneManager.OnLoadEventCompleted += SceneManager_OnLoadEventCompleted;
        }
        base.OnNetworkSpawn();
    }

    private void SceneManager_OnLoadEventCompleted(string sceneName, UnityEngine.SceneManagement.LoadSceneMode loadSceneMode, List<ulong> clientsCompleted, List<ulong> clientsTimedOut)
    {
        if (sceneName == SceneNames.Main.ToString()) { 
            foreach(ulong clientId in NetworkManager.Singleton.ConnectedClientsIds)
            {
                Transform playerTransform = Instantiate(playerPrefab);
                playerTransform.GetComponent<NetworkObject>().SpawnAsPlayerObject(clientId, true);
                TeamType playerTeamType = getPlayerDataFromClientId(clientId).teamType;               
                playerTransform.forward = playerTeamType == TeamType.Red ? Vector3.forward : Vector3.back;

                Vector3 pos;
                if (playerTeamType == TeamType.Red)
                {
                    Debug.Log("set to red pos");
                    pos = RedTeamSpawnList[redTeamSpawnIndex];
                    redTeamSpawnIndex++;
                }
                else
                {
                    Debug.Log("set to blue pos");
                    pos = BlueTeamSpawnList[blueTeamSpawnIndex];
                    blueTeamSpawnIndex++;
                }
                setPlayerClientRpc(playerTransform.GetComponent<NetworkObject>(), playerTeamType,pos);
            }
        }
    }
    [ClientRpc]
    public void setPlayerClientRpc(NetworkObjectReference networkObjectReference,TeamType teamType,Vector3 pos)
    {
        networkObjectReference.TryGet(out NetworkObject networkObject);
        PlayerLogic playerLogic = networkObject.GetComponent<PlayerLogic>();
        networkObject.transform.position = pos;
        if (teamType == TeamType.Red)
        {
            playerLogic.teamType = TeamType.Red;
        }
        else
        {
            playerLogic.teamType = TeamType.Blue;
        }
    }

    private void gameStateChanged(GameState oldState, GameState newState)
    {

    }
    private void Update()
    {       
        switch (State.Value)
        {
            case GameState.WaitingToStart:
                break;
            case GameState.Battle:
                handleTimer();
                break;
            case GameState.EndScreen:
                break;
        }
    }

    private void handleTimer()
    {
        if (!IsServer) return;
        gamePlayingTimer.Value -= Time.deltaTime;
        if (gamePlayingTimer.Value <= 0)
            ChangeState(GameState.EndScreen);
    }

    

    public void kingTowerDestroyed(TeamType teamType)
    {
        ChangeState(GameState.EndScreen);
        GameOverUi.Instance.showGameOverUi(teamType);
    }

    public void ChangeState(GameState newState) {
        if (!IsServer) return;
        State.Value = newState;
    }

    public bool getLocalPlayerReady()
    {
        return isLocalPlayerReady;
    }
    public void setLocalPlayerReady()
    {
        isLocalPlayerReady = true;
        setPlayerReadyServerRpc();
        onLocalPlayerReadyChanged?.Invoke(this, EventArgs.Empty);
    }
    [ServerRpc(RequireOwnership =false)]
    private void setPlayerReadyServerRpc(ServerRpcParams serverRpcParams=default)
    {
        playerReadyDictionary[serverRpcParams.Receive.SenderClientId] = true;
        bool isAllClientsReady = true;
        foreach(ulong clientId in NetworkManager.ConnectedClientsIds)
        {
            if (!playerReadyDictionary.ContainsKey(clientId) || !playerReadyDictionary[clientId])
            {
                isAllClientsReady = false;
            }
        }
        if (isAllClientsReady) // !!!!!!!!!!!ADD && playerReadyDictionary.Count=2
            ChangeState(GameState.Battle);
    }

}

[Serializable]
public enum GameState {
    WaitingToStart = 0,
    Battle=1,
    EndScreen=2
}