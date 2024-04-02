using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Services.Authentication;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.UI;
using System;

public class LobbyUI : MonoBehaviour {


    public static LobbyUI Instance { get; private set; }


    [SerializeField] private Transform playerSingleTemplate;
    [SerializeField] private Transform blueTeamContainer;
    [SerializeField] private Transform redTeamContainer;

    [SerializeField] private TextMeshProUGUI lobbyNameText;
    [SerializeField] private TextMeshProUGUI playerCountText;
    //[SerializeField] private TextMeshProUGUI gameModeText;
    //[SerializeField] private Button changeMarineButton;
    //[SerializeField] private Button changeNinjaButton;
    //[SerializeField] private Button changeZombieButton;
    [SerializeField] private Button leaveLobbyButton;
    //[SerializeField] private Button changeGameModeButton;
    [SerializeField] private Button changeBlueTeamButton;
    [SerializeField] private Button changeRedTeamButton;

    [SerializeField] private Button StartGameButton;


    private void Awake() {
        Instance = this;

        playerSingleTemplate.gameObject.SetActive(false);

        //change character

        //changeMarineButton.onClick.AddListener(() => {
        //    LobbyManager.Instance.UpdatePlayerCharacter(LobbyManager.PlayerCharacter.Marine);
        //});
        //changeNinjaButton.onClick.AddListener(() => {
        //    LobbyManager.Instance.UpdatePlayerCharacter(LobbyManager.PlayerCharacter.Ninja);
        //});
        //changeZombieButton.onClick.AddListener(() => {
        //    LobbyManager.Instance.UpdatePlayerCharacter(LobbyManager.PlayerCharacter.Zombie);
        //});

        //Change team
        StartGameButton.onClick.AddListener(() => {
            LevelManager.LoadNetworkScene(SceneNames.Main);
        });

        changeBlueTeamButton.onClick.AddListener(() => {
            LobbyManager.Instance.UpdatePlayerTeam(TeamType.Blue);
        });
        changeRedTeamButton.onClick.AddListener(() => {
            LobbyManager.Instance.UpdatePlayerTeam(TeamType.Red);
        });

        leaveLobbyButton.onClick.AddListener(() => {
            LobbyManager.Instance.LeaveLobby();
        });

        //changeGameModeButton.onClick.AddListener(() => {
        //    LobbyManager.Instance.ChangeGameMode();
        //});
    }

    private void Start() {
        LobbyManager.Instance.OnJoinedLobby += UpdateLobby_Event;
        LobbyManager.Instance.OnJoinedLobbyUpdate += UpdateLobby_Event;
        //LobbyManager.Instance.OnLobbyGameModeChanged += UpdateLobby_Event;
        LobbyManager.Instance.OnLeftLobby += LobbyManager_OnLeftLobby;
        LobbyManager.Instance.OnKickedFromLobby += LobbyManager_OnLeftLobby;

        Hide();
    }

    private void LobbyManager_OnLeftLobby(object sender, System.EventArgs e) {
        ClearLobby();
        Hide();
    }

    private void UpdateLobby_Event(object sender, LobbyManager.LobbyEventArgs e) {
        UpdateLobby();
    }

    private void UpdateLobby() {
        UpdateLobby(LobbyManager.Instance.GetJoinedLobby());
    }

    private void UpdateLobby(Lobby lobby) {
        try
        {
            ClearLobby();



            foreach (Player player in lobby.Players)
            {

                player.Data.TryGetValue(LobbyManager.KEY_PLAYER_TEAM, out PlayerDataObject playerDataObject);
                TeamType PlayerTeamType = (TeamType)Enum.Parse(typeof(TeamType), playerDataObject.Value);
                Transform playerContainer = PlayerTeamType == TeamType.Blue ? blueTeamContainer : redTeamContainer;

                Transform playerSingleTransform = Instantiate(playerSingleTemplate, playerContainer);
                playerSingleTransform.gameObject.SetActive(true);
                LobbyPlayerSingleUI lobbyPlayerSingleUI = playerSingleTransform.GetComponent<LobbyPlayerSingleUI>();

                lobbyPlayerSingleUI.SetKickPlayerButtonVisible(
                    LobbyManager.Instance.IsLobbyHost() &&
                    player.Id != AuthenticationService.Instance.PlayerId // Don't allow kick self
                );
                lobbyPlayerSingleUI.SetStartGameButtonVisible(
                    LobbyManager.Instance.IsLobbyHost()
                );

                lobbyPlayerSingleUI.UpdatePlayer(player);
            }

            //changeGameModeButton.gameObject.SetActive(LobbyManager.Instance.IsLobbyHost());

            lobbyNameText.text = lobby.Name;
            playerCountText.text = lobby.Players.Count + "/" + lobby.MaxPlayers;
            //gameModeText.text = lobby.Data[LobbyManager.KEY_GAME_MODE].Value;

            Show();
        }
        catch (Exception ex) {
            Debug.Log(ex);

        }
    }

    private void ClearLobby() {
        foreach (Transform child in blueTeamContainer)
        {
            if (child == playerSingleTemplate) continue;
            Destroy(child.gameObject);
        }
        foreach (Transform child in redTeamContainer)
        {
            if (child == playerSingleTemplate) continue;
            Destroy(child.gameObject);
        }
    }

    private void Hide() {
        gameObject.SetActive(false);
    }

    private void Show() {
        gameObject.SetActive(true);
    }

}