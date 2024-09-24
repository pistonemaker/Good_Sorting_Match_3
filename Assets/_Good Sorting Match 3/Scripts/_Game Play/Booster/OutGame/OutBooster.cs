using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OutBooster : MonoBehaviour
{
    public string dataKey;
    public Button button;
    public Image buttonBG;
    public Image addImage;
    public Image useImage;
    public Image plusImage;
    public TextMeshProUGUI amountText;

    protected virtual void OnEnable()
    {
        
    }

    protected void Init(string key)
    {
        dataKey = key;
        button = GetComponent<Button>();
        buttonBG = GetComponent<Image>();
        addImage = transform.Find("Add").GetComponent<Image>();
        useImage = transform.Find("Use").GetComponent<Image>();
        plusImage = addImage.transform.Find("Image").GetComponent<Image>();
        amountText = addImage.transform.Find("Text").GetComponent<TextMeshProUGUI>();
        useImage.gameObject.SetActive(false);
        NoUse();
        button.onClick.AddListener(Click);
    }

    protected void NoUse()
    {
        if (HomeManager.Instance != null)
        {
            buttonBG.sprite = HomeManager.Instance.noUseSprite;
        }
        else
        {
            buttonBG.sprite = UIManager.Instance.noUseSprite;
        }
    }

    protected void Use()
    {
        if (HomeManager.Instance != null)
        {
            buttonBG.sprite = HomeManager.Instance.useSprite;
        }
        else
        {
            buttonBG.sprite = UIManager.Instance.useSprite;
        }
    }

    protected void SetUp()
    {
        int amount = PlayerPrefs.GetInt(dataKey);

        if (amount > 0)
        {
            plusImage.gameObject.SetActive(false);
            amountText.gameObject.SetActive(true);
            amountText.text = amount.ToString();
        }
        else
        {
            plusImage.gameObject.SetActive(true);
            amountText.gameObject.SetActive(false);
            amountText.text = "0";
        }
    }

    protected virtual void Click()
    {
        
    }

    protected void OnDisable()
    {
        button.onClick.RemoveAllListeners();
    }
}
