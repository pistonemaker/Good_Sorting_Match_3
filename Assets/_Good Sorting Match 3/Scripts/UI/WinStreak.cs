using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class WinStreak : MonoBehaviour
{
    public Image process;
    public int processMaxWidth = 624;
    public Button helpButton;

    private void OnEnable()
    {
        helpButton = transform.Find("Help Button").GetComponent<Button>();
        helpButton.onClick.AddListener(() =>
        {
            if (UIManager.Instance != null)
            {
                UIManager.Instance.winStreakPanel.gameObject.SetActive(true);
            }
            else
            {
                HomeManager.Instance.winStreakPanel.gameObject.SetActive(true);
            }
        });
    }

    public void ShowWinStreak()
    {
        int treak = PlayerPrefs.GetInt(DataKey.Win_Streak);
        process.rectTransform.DOSizeDelta(new Vector2(processMaxWidth * treak / 3f, 
            process.rectTransform.rect.height), 1.5f * treak / 3f);
    }

    public void SetWinStreak()
    {
        int treak = PlayerPrefs.GetInt(DataKey.Win_Streak);
        Debug.Log("WinStreak: " + treak);
        process.rectTransform.sizeDelta = new Vector2(processMaxWidth * treak / 3f, process.rectTransform.rect.height);
    }

    public void ResetWinStreak()
    {
        int treak = PlayerPrefs.GetInt(DataKey.Win_Streak);
        Debug.Log("WinStreak: " + treak);
        process.rectTransform.sizeDelta = new Vector2(processMaxWidth * treak / 3f, process.rectTransform.rect.height);
        process.rectTransform.DOSizeDelta(new Vector2(0, process.rectTransform.rect.height), 1.5f * treak / 3f);
    }

    private void OnDisable()
    {
        helpButton.onClick.RemoveAllListeners();
    }
}
