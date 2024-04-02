using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.UI;

public class EscapeUi : MonoBehaviour
{

    [SerializeField] private Button quitBtn;
    [SerializeField] private Button exitBtn;
    [SerializeField] private GameObject Parent;


    private void Awake()
    {
        quitBtn.onClick.AddListener(() =>
        {
            NetworkManager.Singleton.Shutdown();
            LevelManager.LoadScene(SceneNames.MainMenuScene);
        });
        exitBtn.onClick.AddListener(() =>
        {
            ChangeCursorState();
            Parent.SetActive(false);
        });
    }
    public void ChangeCursorState()
    {
        Cursor.lockState =  CursorLockMode.Locked;
    }
}
