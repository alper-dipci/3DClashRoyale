using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;

public class TeamSelectionUi : NetworkBehaviour {
    [SerializeField] private Button BlueBtn;
    [SerializeField] private Button RedtBtn;
    public static event EventHandler<TeamSelectedEventArgs> onTeamSelected;

    public override void OnNetworkSpawn()
    {
        TeamSelectedEventArgs args = new TeamSelectedEventArgs();
        BlueBtn.onClick.AddListener(() =>
        {
            args.teamType = TeamType.Blue;
            onTeamSelected?.Invoke(this, args);
        });
        RedtBtn.onClick.AddListener(() =>
        {
            args.teamType = TeamType.Red;
            onTeamSelected?.Invoke(this, args);
        });
        base.OnNetworkSpawn();
    }
}
public class TeamSelectedEventArgs : EventArgs {
    public TeamType teamType { get; set; }
}
