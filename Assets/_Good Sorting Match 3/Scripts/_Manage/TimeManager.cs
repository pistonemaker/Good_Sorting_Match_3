using System.Collections;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TimeManager : Singleton<TimeManager>
{
    public TextMeshProUGUI timerText;
    public float totalTimeInSeconds;
    private float remainingTime;
    private Coroutine countdownRountine;
    private bool isPaused = false;
    private bool canCountdown = false;
    public Image freezeImage;
    public const int freezeTime = 10;
    public Transform timerTransform;

    private void OnEnable()
    {
        totalTimeInSeconds = 20f;
        EventDispatcher.Instance.RegisterListener(EventID.On_Pause_Game, PauseGame);
        EventDispatcher.Instance.RegisterListener(EventID.On_Resume_Game, ResumeGame);
        EventDispatcher.Instance.RegisterListener(EventID.On_Start_Countdown_Time, StartCountdown);
    }

    private void OnDisable()
    {
        EventDispatcher.Instance.RemoveListener(EventID.On_Pause_Game, PauseGame);
        EventDispatcher.Instance.RemoveListener(EventID.On_Resume_Game, ResumeGame);
        EventDispatcher.Instance.RemoveListener(EventID.On_Start_Countdown_Time, StartCountdown);
    }

    private void Start()
    {
        remainingTime = totalTimeInSeconds;
        int minutes = Mathf.FloorToInt(remainingTime / 60f);
        int seconds = Mathf.FloorToInt(remainingTime % 60f);
        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
        freezeImage = transform.Find("Freeze Image").GetComponent<Image>();
        freezeImage.fillAmount = 1;
    }

    private void StartCountdown(object param)
    {
        if (!canCountdown)
        {
            canCountdown = true;
            countdownRountine = StartCoroutine(TimerCountdown());
        }
    }

    private IEnumerator TimerCountdown()
    {
        while (remainingTime > 0)
        {
            if (!isPaused && canCountdown)
            {
                remainingTime -= Time.deltaTime;

                int minutes = Mathf.FloorToInt(remainingTime / 60f);
                int seconds = Mathf.FloorToInt(remainingTime % 60f);

                timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);

                if (remainingTime <= 0f)
                {
                    remainingTime = 0f; 
                    timerText.text = "00:00";
                }
            }

            yield return null;
        }

        GameOver();
    }

    public void BoostTime(float boostTime, GameObject booster, bool isContinue)
    {
        booster.transform.DOScale(0.1f, 1f);
        booster.transform.DOMove(timerTransform.position, 1f).OnComplete(() =>
        {
            // totalTimeInSeconds += boostTime;
            // remainingTime = totalTimeInSeconds;
            remainingTime += boostTime;
            int minutes = Mathf.FloorToInt(remainingTime / 60f);
            int seconds = Mathf.FloorToInt(remainingTime % 60f);
            timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
            PoolingManager.Despawn(booster);

            if (isContinue)
            {
                countdownRountine = StartCoroutine(TimerCountdown());
            }
        });
    }

    private void GameOver()
    {
        StopCoroutine(countdownRountine);
        UIManager.Instance.timeUpPanel.gameObject.SetActive(true);
    }

    private void PauseGame(object param)    
    {
        if (!isPaused && countdownRountine != null)
        {
            Debug.Log("Pause");
            isPaused = true;
        }
    }

    private void ResumeGame(object param)
    {
        if (isPaused)
        {
            Debug.Log("Resume");
            isPaused = false;
        }
    }

    public void Freeze()
    {
        PauseGame(null);
        freezeImage.fillAmount = 1;
        freezeImage.gameObject.SetActive(true);
        freezeImage.DOFillAmount(0f, freezeTime).SetEase(Ease.Linear).OnComplete(() =>
        {
            ResumeGame(null);
            freezeImage.gameObject.SetActive(false);
            freezeImage.fillAmount = 1;
        });
    }
}