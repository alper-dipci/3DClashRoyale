using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class GameOverUi : Singleton<GameOverUi>
{
    [SerializeField] private Button closeButton;
    [SerializeField] private GameObject gameOverUi;
    [SerializeField] private TextMeshProUGUI gameOverText;

    private void Start()
    {
        closeButton.onClick.AddListener(() =>
        {
            LevelManager.LoadScene(SceneNames.MainMenuScene);
        });
    }
    public  void showGameOverUi(TeamType teamType)
    {
        gameOverText.text = teamType == TeamType.Red ? "Red Team Won" : "Blue Team Won";
        gameOverUi.SetActive(true);
    }
}
