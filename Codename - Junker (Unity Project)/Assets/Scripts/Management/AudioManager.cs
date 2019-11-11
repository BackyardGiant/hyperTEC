﻿using UnityEngine;
using System;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public AudioMixerGroup mixer;
    public enum WeaponSounds {Short,Medium,Long};

    [Header("Sound Lists")]
    [Tooltip("Sounds to be played locally. E.G Music, UI Effects. Usually non-diegetic audio.")]
    public Sound[] localSounds;
    [Tooltip("Sounds to be played on a certain position. E.G gunshots, explosions. Usually diegetic audio."),Space(20)]
    public Sound[] worldSounds;
    [Header("Weapon Specific Sounds"),Tooltip("Sounds to be played on a certain position. E.G gunshots, explosions. Usually diegetic audio."), Space(20)]
    public Sound[] shortWeaponSounds;
    [Tooltip("Sounds to be played on a certain position. E.G gunshots, explosions. Usually diegetic audio.")]
    public Sound[] mediumWeaponSounds;
    [Tooltip("Sounds to be played on a certain position. E.G gunshots, explosions. Usually diegetic audio.")]
    public Sound[] longWeaponSounds;
    [Tooltip("Time Delays before Long Sounds Play.")]
    public float[] longWeaponDelays;

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

        foreach (Sound s in localSounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;

            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
            s.source.outputAudioMixerGroup = mixer;
            s.source.loop = s.loop;
        }
    }
    private void Start()
    {
    }

    #region LocalSoundControls
    /// <summary>
    /// Plays local sounds defined in the audio manager.
    /// </summary>
    /// <param name="name"></param>
    public void Play(string name)
    {
        Sound s = Array.Find(localSounds, sound => sound.name == name);

        if (s == null)
        {
            Debug.LogWarning("Failed Playing. Sound: " + name + " not found! maybe you misspelled it dummy");
        }

        s.source.Play();
    }
    public void Pause(string name)
    {
        Sound s = Array.Find(localSounds, sound => sound.name == name);

        if (s == null)
        {
            Debug.LogWarning("Falied Pausing. Sound: " + name + " not found! maybe you misspelled it dummy");
        }

        s.source.Pause();
    }
    public void Volume(string name,float value)
    {
        Sound s = Array.Find(localSounds, sound => sound.name == name);

        if (s == null)
        {
            Debug.LogWarning("Falied Pausing. Sound: " + name + " not found! maybe you misspelled it dummy");
        }

        s.source.volume = value;
    }
    public void Pitch(string name, float value)
    {
        Sound s = Array.Find(localSounds, sound => sound.name == name);

        if (s == null)
        {
            Debug.LogWarning("Falied Pausing. Sound: " + name + " not found! maybe you misspelled it dummy");
        }

        s.source.pitch = value;
    }
    public float getVolume(string name)
    {
        Sound s = Array.Find(localSounds, sound => sound.name == name);

        if (s == null)
        {
            Debug.LogWarning("Falied Pausing. Sound: " + name + " not found! maybe you misspelled it dummy");
        }

        float volume = s.source.volume;
        return volume;
    }
    public float getPitch(string name)
    {
        Sound s = Array.Find(localSounds, sound => sound.name == name);

        if (s == null)
        {
            Debug.LogWarning("Falied Pausing. Sound: " + name + " not found! maybe you misspelled it dummy");
        }

        float pitch = s.source.pitch;
        return pitch;
    }
    #endregion

    #region 3DSoundControls
    /// <summary>
    /// Creates an AudioSource on the target object and plays the clip.
    /// </summary>
    /// <param name="name">Name of the Audio Clip in AudioManager World Sound.</param>
    /// <param name="target">Target gameObject to create AudioSource on</param>
    /// <param name="seperateObject">"Should the sound be placed on a seperate object at the same position as the target? Good for explosions.</param>
    /// <param name="destroy">Pass in true if the audio source should delete itself once done.</param>
    public void PlayWorld(string name,GameObject target,bool seperateObject,bool destroy)
    {
        Sound s = Array.Find(worldSounds, sound => sound.name == name);
        if (seperateObject == true)
        {
            GameObject _targetAudio = new GameObject(target.name + " Audio Source. Playing - " + name);
            _targetAudio.transform.position = target.transform.position;
            target = _targetAudio;
        }
        AudioSource _source = target.AddComponent<AudioSource>();
        _source.clip = s.clip;
        _source.volume = s.volume;
        _source.pitch = s.pitch;
        _source.loop = s.loop;
        _source.spatialBlend = 0.99f;
        _source.outputAudioMixerGroup = mixer;
        _source.Play();
        if (destroy == true)
        {
            if(seperateObject == true)
            {
                Destroy(target, _source.clip.length);
            }
            else
            {
                Destroy(_source, _source.clip.length);
            }
        }
    }
    #endregion

    #region WeaponSoundControls
    public float PlayWeapon(WeaponSounds weapon, GameObject target, bool seperateObject, bool destroy)
    {
        float returnVal = 0.0f;
        string name = "";
        if (weapon == WeaponSounds.Short)
        {
            int randomVal = UnityEngine.Random.Range(1,shortWeaponSounds.Length);
            name = "ShortWeapon" + randomVal;
            returnVal = 0.0f;
        }
        else if(weapon == WeaponSounds.Medium)
        {
            int randomVal = UnityEngine.Random.Range(1, mediumWeaponSounds.Length);
            name = "MediumWeapon" + randomVal;
            returnVal = 0.0f;
        }
        else if(weapon == WeaponSounds.Long)
        {
            int randomVal = UnityEngine.Random.Range(1, longWeaponSounds.Length);
            name = "LongWeapon" + randomVal;
            returnVal = longWeaponDelays[randomVal];
        }

        Sound s = Array.Find(worldSounds, sound => sound.name == name);
        if (seperateObject == true)
        {
            GameObject _targetAudio = new GameObject(target.name + " Audio Source. Playing - " + name);
            _targetAudio.transform.position = target.transform.position;
            target = _targetAudio;
        }
        AudioSource _source = target.AddComponent<AudioSource>();
        _source.clip = s.clip;
        _source.volume = s.volume;
        _source.pitch = s.pitch;
        _source.loop = s.loop;
        _source.spatialBlend = 0.99f;
        _source.outputAudioMixerGroup = mixer;
        _source.Play();
        if (destroy == true)
        {
            if (seperateObject == true)
            {
                Destroy(target, _source.clip.length);
            }
            else
            {
                Destroy(_source, _source.clip.length);
            }
        }

        return returnVal;
    }
    #endregion
}