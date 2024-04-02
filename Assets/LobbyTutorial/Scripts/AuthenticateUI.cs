using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;

public class AuthenticateUI : MonoBehaviour
{


    [SerializeField] private Button authenticateButton;
    [SerializeField] private GameObject nameIsNotValidUi;

    const string k_ProfileRegex = @"^[a-zA-Z0-9_-]{1,30}$";
    private void Awake()
    {
        nameIsNotValidUi.SetActive(false);
        authenticateButton.onClick.AddListener(() =>
        {
            Debug.Log("auth");
            string playername = EditPlayerName.Instance.GetPlayerName();
            if (string.IsNullOrEmpty(playername) || !Regex.Match(playername, k_ProfileRegex).Success)
            {
                Debug.Log("name is not valid");
                nameIsNotValidUi.SetActive(true);
                return;
            }
            LobbyManager.Instance.Authenticate(EditPlayerName.Instance.GetPlayerName());
            Hide();
        });
    }

    private void Hide()
    {
        gameObject.SetActive(false);
    }

}