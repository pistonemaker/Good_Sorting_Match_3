using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;
    public Sound[] musicSound, sfxSound;
    public AudioSource musicSource, sFXSource;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void PlayMusic(string mName, float volume = 1f)
    {
        Sound sound = Array.Find(musicSound, x => x.name == mName);
        musicSource.volume = volume;

        if (sound == null)
        {
            Debug.Log("Music Not Exist");
        }
        else
        {
            int random = Random.Range(0, sound.clips.Count);
            musicSource.clip = sound.clips[random];
            musicSource.Play();
        }
    }
    
    public void PlaySFX(string sName, float volume = 1f)
    {
        Sound sound = Array.Find(sfxSound, x => x.name == sName);
        sFXSource.volume = volume;

        if (sound == null)
        {
            Debug.Log("SFX Not Exist");
        }
        else
        {
            
            int random = Random.Range(0, sound.clips.Count);
            sFXSource.PlayOneShot(sound.clips[random]);
        }
    }

    public void ToggleMusic()
    {
        musicSource.mute = !musicSource.mute;
    }

    public void ToggleSFX()
    {
        sFXSource.mute = !sFXSource.mute;
    }
}

[Serializable]
public class Sound
{
    public string name;
    public List<AudioClip> clips;
}