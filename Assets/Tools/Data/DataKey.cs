using UnityEngine;

public static class DataKey
{
    #region int
    
    public const string Use_Vibrate = "Use_Vibrate";
    public const string Use_Music = "Use_Music";
    public const string Use_SFX = "Use_Sound";
    public const string Win_Streak = "Win_Streak";
    public const string Cur_Level = "Cur_Level";
    public const string Cur_Level_Lost_Time = "Cur_Level_Lost_Time";
    
    public const string Ingame_Hammer = "Ingame_Hammer";
    public const string Ingame_Magic_Wand = "Ingame_Magic_Wand";
    public const string Ingame_Freeze = "Ingame_Freeze";
    public const string Ingame_Shuffle = "Ingame_Shuffle";
    
    public const string Outgame_Hammer = "Outgame_Hammer";
    public const string Outgame_Clock = "Outgame_Clock";
    public const string Outgame_Double_Star = "Outgame_Double_Star";
    
    public const string Heart = "Heart";
    public const string Coin = "Coin";
    public const string Star = "Star";
    
    public const string Open_App_Count = "Open_App_Count";
    public const string Show_Inter_Count = "Show_Inter_Count";
    public const string Open_App_Ads = "Open_App_Ads";
    
    #endregion
    
    
    public static bool IsUseMusic()
    {
        return PlayerPrefs.GetInt(Use_Music) == 1; 
    }
    
    public static bool IsUseVibrate()
    {
        return PlayerPrefs.GetInt(Use_Vibrate) == 1; 
    }
    
    public static bool IsUseSound()
    {
        return PlayerPrefs.GetInt(Use_SFX) == 1; 
    }
    
    public static bool IsLostCurLevelBefore()
    {
        return PlayerPrefs.GetInt(Cur_Level_Lost_Time) == 1; 
    }
}
