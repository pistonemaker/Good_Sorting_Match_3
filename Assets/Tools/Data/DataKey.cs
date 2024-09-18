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
}
