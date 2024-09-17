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
    [SerializeField] private bool isComboActive = false; 
    private Coroutine comboCoroutine;
    private bool isPaused = false; 
    private float remainingComboTime; // Thời gian còn lại khi bị tạm dừng
    private float initialFillAmount; // Giá trị fillAmount ban đầu

    private void OnEnable()
    {
        comboProcess = transform.Find("Combo Process").GetComponent<Image>();
        comboText = comboProcess.transform.Find("Combo Text").GetComponent<TextMeshProUGUI>();
        this.RegisterListener(EventID.On_Complete_A_Match_3, param => OnMatch3((int) param));
        EventDispatcher.Instance.RegisterListener(EventID.On_Pause_Game, PauseGame);
        EventDispatcher.Instance.RegisterListener(EventID.On_Resume_Game, ResumeGame);
    }

    private void OnDisable()
    {
        this.RemoveListener(EventID.On_Complete_A_Match_3, param => OnMatch3((int) param));
        EventDispatcher.Instance.RemoveListener(EventID.On_Pause_Game, PauseGame);
        EventDispatcher.Instance.RemoveListener(EventID.On_Resume_Game, ResumeGame);
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
            comboCoroutine = StartCoroutine(ComboCountdown());
        }
        else
        {
            isComboActive = true;
            currentCombo = 1;
            currentComboTime = initialComboTime;
            comboText.gameObject.SetActive(true);
            comboCoroutine = StartCoroutine(ComboCountdown());
        }

        SpawnStars(boxID);
    }

    private IEnumerator ComboCountdown()
    {
        float elapsedTime = 0f;

        comboText.text = "Combo x" + currentCombo;
        ScaleComboText();
        comboProcess.DOKill(); // Dừng tất cả các tween trước đó

        // Đảm bảo giá trị fillAmount và thời gian còn lại được khôi phục khi resume
        if (isPaused)
        {
            comboProcess.fillAmount = initialFillAmount;
            elapsedTime = currentComboTime - remainingComboTime;
        }
        else
        {
            comboProcess.fillAmount = 1f;
        }

        // Khởi tạo tween mới nếu chưa bị pause
        if (!isPaused)
        {
            comboProcess.DOFillAmount(0f, currentComboTime).OnKill(() => { /* Handle when the tween is killed */ });
        }
        else
        {
            comboProcess.DOFillAmount(0f, remainingComboTime);
        }

        while (elapsedTime < currentComboTime)
        {
            if (!isPaused)
            {
                elapsedTime += Time.deltaTime;
            }
            else
            {
                remainingComboTime -= Time.deltaTime;
            }
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
        comboText.transform.DOScale(Vector3.one * 1.2f, 0.25f).OnComplete(() =>
        {
            comboText.transform.DOScale(Vector3.one, 0.25f);
        });
    }

    private void PauseGame(object param)
    {
        if (isComboActive)
        {
            isPaused = true;
            StopCoroutine(comboCoroutine); // Dừng Coroutine hiện tại
            remainingComboTime = currentComboTime - (comboProcess.fillAmount * currentComboTime); // Tính thời gian còn lại
            initialFillAmount = comboProcess.fillAmount; // Lưu trạng thái fillAmount
            comboProcess.DOKill(); // Dừng tween
        }
    }

    private void ResumeGame(object param)
    {
        if (isPaused)
        {
            isPaused = false;
            comboCoroutine = StartCoroutine(ComboCountdown()); // Tiếp tục Coroutine từ trạng thái hiện tại
        }
    }

    public void SpawnStars(int boxID)
    {
        int amount = currentCombo / 2 + 1;
        this.PostEvent(EventID.On_Update_Star, amount);

        for (int i = 0; i < amount; i++)
        {
            Vector3 randomOffset = new Vector3(Random.Range(-0.5f, 0.5f), Random.Range(-0.5f, 0.5f), 0);
            Vector3 spawnPosition = GameController.Instance.boxes[boxID].transform.position + randomOffset;
            var star = PoolingManager.Spawn(GameManager.Instance.star, spawnPosition, Quaternion.identity);
            star.MoveStarToUI(UIManager.Instance.uiStar);
        }
    }
}
