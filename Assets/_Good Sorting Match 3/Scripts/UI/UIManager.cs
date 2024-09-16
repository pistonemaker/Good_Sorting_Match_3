using System;
using TMPro;

public class UIManager : Singleton<UIManager>
{
    public TextMeshProUGUI levelText;

    private void SetLevelName()
    {
        var levelData = GameController.Instance.levelData;
        string levelName = levelData.name;
        string[] splitName = levelName.Split('_');
        string levelNumber = splitName[1]; 
        levelText.text = "Lv." + levelNumber;
    }

    private void OnEnable()
    {
        SetLevelName();
    }
}
