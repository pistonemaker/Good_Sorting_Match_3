using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class TimeManager : Singleton<TimeManager>
{
    public TextMeshProUGUI timerText;
    public float totalTimeInSeconds = 300f; 
    private float remainingTime;
    private Coroutine countdownRountine;
    private bool isPaused = false;  

    private void OnEnable()
    {
        EventDispatcher.Instance.RegisterListener(EventID.On_Pause_Game, PauseGame);
        EventDispatcher.Instance.RegisterListener(EventID.On_Resume_Game, ResumeGame);
    }

    private void OnDisable()
    {
        EventDispatcher.Instance.RemoveListener(EventID.On_Pause_Game, PauseGame);
        EventDispatcher.Instance.RemoveListener(EventID.On_Resume_Game, ResumeGame);
    }

    private void Start()
    {
        remainingTime = totalTimeInSeconds;
        countdownRountine = StartCoroutine(TimerCountdown());
    }

    private IEnumerator TimerCountdown()
    {
        while (remainingTime > 0)
        {
            if (!isPaused)
            {
                remainingTime -= Time.deltaTime;

                int minutes = Mathf.FloorToInt(remainingTime / 60f);
                int seconds = Mathf.FloorToInt(remainingTime % 60f);

                timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
            }
            
            yield return null;
        }

        GameOver();
    }

    private void GameOver()
    {
        Debug.Log("Game Over! Time's up!");
    }

    private void PauseGame(object param)
    {
        if (!isPaused && countdownRountine != null)
        {
            isPaused = true;  
            Debug.Log("Game Paused");
        }
    }

    private void ResumeGame(object param)
    {
        if (isPaused)
        {
            isPaused = false;  
            Debug.Log("Game Resumed");
        }
    }
}
