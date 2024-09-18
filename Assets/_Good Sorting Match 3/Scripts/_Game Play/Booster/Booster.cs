using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Booster : MonoBehaviour
{
    protected Button button;
    protected TextMeshProUGUI amountText;
    protected string dataKey;
    protected GameObject boosterPrefab;

    protected virtual void Init(string key)
    {
        dataKey = key;
        button = GetComponent<Button>();
        amountText = transform.Find("Add").GetChild(0).GetComponent<TextMeshProUGUI>();
        
        button.onClick.AddListener(() =>
        {
            OnUseBooster(dataKey);
        });
    }

    protected virtual void OnEnable()
    {
        this.RegisterListener(EventID.On_Use_Ingame_Booster, param => OnUseBooster((string)param));
        this.RegisterListener(EventID.On_Buy_Ingame_Booster, param => UseBooster((string)param));
    }

    protected virtual void OnDisable()
    {
        this.RemoveListener(EventID.On_Use_Ingame_Booster, param => OnUseBooster((string)param));
        this.RemoveListener(EventID.On_Buy_Ingame_Booster, param => UseBooster((string)param));
    }

    protected void SetUpBooster()
    {
        int amount = PlayerPrefs.GetInt(dataKey);

        if (amount > 0)
        {
            amountText.text = amount.ToString();
        }
        else
        {
            amountText.text = "+";
        }
    }

    protected void OnUseBooster(string key)
    {
        if (dataKey != key)
        {
            return;
        }
        
        int amount = PlayerPrefs.GetInt(dataKey);
        
        if (amount <= 0)
        {
            this.PostEvent(EventID.On_Show_Get_More_Panel, dataKey);
            return;
        }
        
        amount--;
        PlayerPrefs.SetInt(dataKey, amount);
        SetUpBooster();
        UseBooster(dataKey);
    }

    protected virtual void UseBooster(string key)
    {
        
    }
}
