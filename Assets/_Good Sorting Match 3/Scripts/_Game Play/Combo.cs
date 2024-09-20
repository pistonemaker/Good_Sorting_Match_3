using System.Collections;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Combo : Singleton<Combo>
{
    public Image comboProcess;
    public TextMeshProUGUI comboText;

    public float initialComboTime = 20f;
    public float timeReductionPerCombo = 0.5f;
    public float minimumComboTime = 1.5f;
    public int currentCombo;

    [SerializeField] private float currentComboTime;
    private bool isComboActive = false;
    private Coroutine comboCoroutine;
    private bool isPaused = false;
    private bool canCountdown = false;
    private float remainingComboTime;
    private float remainingFillAmount;

    private void OnEnable()
    {
        comboProcess = transform.Find("Combo Process").GetComponent<Image>();
        comboText = comboProcess.transform.Find("Combo Text").GetComponent<TextMeshProUGUI>();
        this.RegisterListener(EventID.On_Complete_A_Match_3, param => OnMatch3((int)param));
        EventDispatcher.Instance.RegisterListener(EventID.On_Pause_Game, PauseGame);
        EventDispatcher.Instance.RegisterListener(EventID.On_Resume_Game, ResumeGame);
        EventDispatcher.Instance.RegisterListener(EventID.On_Start_Countdown_Time, StartCountdown);
    }

    private void OnDisable()
    {
        this.RemoveListener(EventID.On_Complete_A_Match_3, param => OnMatch3((int)param));
        EventDispatcher.Instance.RemoveListener(EventID.On_Pause_Game, PauseGame);
        EventDispatcher.Instance.RemoveListener(EventID.On_Resume_Game, ResumeGame);
        EventDispatcher.Instance.RemoveListener(EventID.On_Start_Countdown_Time, StartCountdown);
    }

    private void StartCountdown(object param)
    {
        if (canCountdown == false)
        {
            canCountdown = true;
            StartCoroutine(ComboCountdownNormal());
        }
    }

    public void OnMatch3(int boxID)
    {
        if (isComboActive)
        {
            currentCombo++;
            currentComboTime = Mathf.Max(initialComboTime - (currentCombo - 1) * timeReductionPerCombo, minimumComboTime);
            comboProcess.fillAmount = 1f;

            if (comboCoroutine != null)
            {
                StopCoroutine(comboCoroutine);
            }
        }
        else
        {
            isComboActive = true;
            currentCombo = 1;
            currentComboTime = initialComboTime;
            comboText.gameObject.SetActive(true);
        }

        comboCoroutine = StartCoroutine(ComboCountdownNormal());
        SpawnStars(boxID);
    }

    private IEnumerator ComboCountdownNormal()
    {
        float elapsedTime = 0f;
        comboText.text = "Combo x" + currentCombo;
        ScaleComboText();
        comboProcess.DOKill();
        comboProcess.fillAmount = 1f;

        if (canCountdown)
        {
            comboProcess.DOFillAmount(0f, currentComboTime);

            while (elapsedTime < currentComboTime)
            {
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            EndCombo();
        }
    }

    private IEnumerator ResumeComboCountdown()
    {
        float elapsedTime = 0f;

        comboText.text = "Combo x" + currentCombo;
        comboProcess.DOKill();
        comboProcess.fillAmount = remainingFillAmount;
        comboProcess.DOFillAmount(0f, currentComboTime);

        while (elapsedTime < currentComboTime)
        {
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        EndCombo();
    }

    private void EndCombo()
    {
        isComboActive = false;
        comboText.gameObject.SetActive(false);
        ResetCombo();
    }

    private void ResetCombo()
    {
        currentCombo = 0;
        currentComboTime = initialComboTime;
    }

    private void ScaleComboText()
    {
        comboText.transform.DOScale(Vector3.one * 1.2f, 0.25f).OnComplete(() => { comboText.transform.DOScale(Vector3.one, 0.25f); });
    }

    private void PauseGame(object param)
    {
        if (isComboActive)
        {
            isPaused = true;
            StopCoroutine(comboCoroutine); // 10 - 0.6*10
            remainingComboTime = comboProcess.fillAmount * currentComboTime;
            remainingFillAmount = comboProcess.fillAmount;
            comboProcess.DOKill();
        }
    }

    private void ResumeGame(object param)
    {
        if (isPaused)
        {
            isPaused = false;
            comboCoroutine = StartCoroutine(ResumeComboCountdown());
        }
    }

    public void SpawnStars(int boxID)
    {
        int amount = currentCombo / 2 + 1;
        this.PostEvent(EventID.On_Update_Star, amount);

        for (int i = 0; i < amount; i++)
        {
            Vector3 randomOffset = new Vector3(Random.Range(-0.5f, 0.5f), Random.Range(-0.5f, 0.5f), 0);
            Vector3 spawnPosition;
            
            if (boxID != -1)
            {
                spawnPosition = GameController.Instance.boxes[boxID].transform.position + randomOffset;
            }
            else
            {
                spawnPosition = Vector3.zero + randomOffset;
            }

            var star = PoolingManager.Spawn(GameManager.Instance.star, spawnPosition, Quaternion.identity);
            star.MoveStarToUI(UIManager.Instance.uiStar.transform);
        }
    }
}