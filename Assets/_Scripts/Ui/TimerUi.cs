using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TimerUi : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI gamePlayingTimerText;
    private void Update()
    {
        updateTimer(ExampleGameManager.Instance.gamePlayingTimer.Value);
    }
    private void updateTimer(float gamePlayingTimer)
    {
        int minutes = Mathf.FloorToInt(gamePlayingTimer / 60);
        int seconds = Mathf.FloorToInt(gamePlayingTimer % 60);
        gamePlayingTimerText.text = String.Format("{0:00}:{1:00}", minutes, seconds);
    }
}
