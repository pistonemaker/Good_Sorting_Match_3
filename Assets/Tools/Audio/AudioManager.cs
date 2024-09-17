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

    public void PlayMusic(string musicName, float volume = 1f)
    {
        Sound sound = Array.Find(musicSound, x => x.name == musicName);
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
    
    public void PlaySFX(string soundName, float volume = 1f)
    {
        Sound sound = Array.Find(sfxSound, x => x.name == soundName);
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

    public void ToggleMusic(bool mute)
    {
        musicSource.mute = mute;
    }

    public void ToggleSFX(bool mute)
    {
        sFXSource.mute = mute;
    }
}

[Serializable]
public class Sound
{
    public string name;
    public List<AudioClip> clips;
}