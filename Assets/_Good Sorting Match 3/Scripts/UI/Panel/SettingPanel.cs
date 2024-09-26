using UnityEngine;
using UnityEngine.UI;

public class SettingPanel : BasePanel
{
    public Button closeButton;
    public Button soundButton;
    public Button vibrateButton;
    public Button musicButton;

    public Image soundImage;
    public Image vibrateImage;
    public Image musicImage;
    
    public Sprite sound;
    public Sprite nonSound;
    public Sprite vibrate;
    public Sprite nonVibrate;
    public Sprite music;
    public Sprite nonMusic;
    
    protected override void OnEnable()
    {
        base.OnEnable();
        ShowBox();
    }

    protected override void ClosePanel(float time, bool resume = true)
    {
        base.ClosePanel(time, resume);
    }

    protected override void LoadButtonAndImage()
    {
        closeButton = transform.Find("Ribbon").Find("Close Button").GetComponent<Button>();
        soundButton = transform.Find("Sound Button").GetComponent<Button>();
        vibrateButton = transform.Find("Vibrate Button").GetComponent<Button>();
        musicButton = transform.Find("Music Button").GetComponent<Button>();
        
        soundImage = soundButton.transform.GetChild(0).GetComponent<Image>();
        vibrateImage = vibrateButton.transform.GetChild(0).GetComponent<Image>();
        musicImage = musicButton.transform.GetChild(0).GetComponent<Image>();
    }

    private void ShowBox()
    {
        LoadBoxData();
    }

    private void LoadBoxData()
    {
        if (DataKey.IsUseVibrate())
        {
            vibrateImage.sprite = vibrate;
        }
        else
        {
            vibrateImage.sprite = nonVibrate;
        }
        
        if (DataKey.IsUseMusic())
        {
            musicImage.sprite = music;
            AudioManager.Instance.ToggleMusic(false);
        }
        else
        {
            musicImage.sprite = nonMusic;
            AudioManager.Instance.ToggleMusic(true);
        }
        
        if (DataKey.IsUseSound())
        {
            soundImage.sprite = sound;
            AudioManager.Instance.ToggleSFX(false);
        }
        else
        {
            soundImage.sprite = nonSound;
            AudioManager.Instance.ToggleSFX(true);
        }
    }

    protected override void SetListener()
    {
        vibrateButton.onClick.AddListener(() =>
        {
            if (vibrateImage.sprite == vibrate)
            {
                vibrateImage.sprite = nonVibrate;
                PlayerPrefs.SetInt(DataKey.Use_Vibrate, 0);
            }
            else if (vibrateImage.sprite == nonVibrate)
            {
                vibrateImage.sprite = vibrate;
                PlayerPrefs.SetInt(DataKey.Use_Vibrate, 1);
            }
        });
        
        musicButton.onClick.AddListener(() =>
        {
            if (musicImage.sprite == music)
            {
                musicImage.sprite = nonMusic;
                PlayerPrefs.SetInt(DataKey.Use_Music, 0);
                AudioManager.Instance.ToggleMusic(true);
            }
            else if (musicImage.sprite == nonMusic)
            {
                musicImage.sprite = music;
                PlayerPrefs.SetInt(DataKey.Use_Music, 1);
                AudioManager.Instance.ToggleMusic(false);
            }
        });
        
        soundButton.onClick.AddListener(() =>
        {
            if (soundImage.sprite == sound)
            {
                soundImage.sprite = nonSound;
                PlayerPrefs.SetInt(DataKey.Use_SFX, 0);
                AudioManager.Instance.ToggleSFX(true);
            }
            else if (soundImage.sprite == nonSound)
            {
                soundImage.sprite = sound;
                PlayerPrefs.SetInt(DataKey.Use_SFX, 1);
                AudioManager.Instance.ToggleSFX(false);
            }
        });
        
        closeButton.onClick.AddListener(() =>
        {
            ClosePanel(0f);
        });
    }

    private void OnDisable()
    {
        closeButton.onClick.RemoveAllListeners();
        soundButton.onClick.RemoveAllListeners();
        vibrateButton.onClick.RemoveAllListeners();
        musicButton.onClick.RemoveAllListeners();
    }
}