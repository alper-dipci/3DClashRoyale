using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;
using System;

public class NetworkUi : MonoBehaviour
{
    [SerializeField] private Button hostBtn;
    [SerializeField] private Button clientBtn;
    [SerializeField] private GameObject ReadyUi;
    [SerializeField] private GameObject TeamSelectUi;

    private void Awake()
    {
        hostBtn.onClick.AddListener(() =>
        {
            NetworkManager.Singleton.StartHost();
            nextState();
        });
        clientBtn.onClick.AddListener(() =>
        {
            NetworkManager.Singleton.StartClient();
            nextState();
        });
    }
    private void nextState()
    {
        gameObject.SetActive(false);
        ReadyUi.SetActive(true);
        TeamSelectUi.SetActive(true);
    }
}
