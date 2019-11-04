using UnityEngine;
using System;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public Sound[] sounds;

    public static AudioManager Instance;

    //[SerializeField]
    //private AudioMixerGroup m_mixerGroup, m_mixerMusicGroup;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }

        //DontDestroyOnLoad(gameObject);

        foreach (Sound s in sounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;

            s.source.volume = s.volume;
            s.source.pitch = s.pitch;

            s.source.loop = s.loop;
        }
    }

    #region Audio to play no matter what

    private void Start()
    {
        // all the time
    }

    #endregion

    public void Play(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);

        if (s == null)
        {
            Debug.LogWarning("Failed Playing. Sound: " + name + " not found! maybe you misspelled it dummy");
        }

        s.source.Play();
    }

    public void Pause(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);

        if (s == null)
        {
            Debug.LogWarning("Falied Pausing. Sound: " + name + " not found! maybe you misspelled it dummy");
        }

        s.source.Pause();
    }

    public void Volume(string name,float value)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);

        if (s == null)
        {
            Debug.LogWarning("Falied Pausing. Sound: " + name + " not found! maybe you misspelled it dummy");
        }

        s.source.volume = value;
    }
    public void Pitch(string name, float value)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);

        if (s == null)
        {
            Debug.LogWarning("Falied Pausing. Sound: " + name + " not found! maybe you misspelled it dummy");
        }

        s.source.pitch = value;
    }

    public float getVolume(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);

        if (s == null)
        {
            Debug.LogWarning("Falied Pausing. Sound: " + name + " not found! maybe you misspelled it dummy");
        }

        float volume = s.source.volume;
        return volume;
    }
    public float getPitch(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);

        if (s == null)
        {
            Debug.LogWarning("Falied Pausing. Sound: " + name + " not found! maybe you misspelled it dummy");
        }

        float pitch = s.source.pitch;
        return pitch;
    }
}