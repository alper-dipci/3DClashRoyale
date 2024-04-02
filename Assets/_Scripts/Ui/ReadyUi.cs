using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class ReadyUi : NetworkBehaviour
{
    [SerializeField] private Button readyButton;
    private void Awake()
    {
        readyButton.onClick.AddListener(() =>
        {
            ExampleGameManager.Instance.setLocalPlayerReady();
        });
    }


}
