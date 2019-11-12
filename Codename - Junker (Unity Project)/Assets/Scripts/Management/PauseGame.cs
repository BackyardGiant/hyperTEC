using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class PauseGame : MonoBehaviour
{
    [SerializeField]
    private GameObject m_pauseMenu, m_settingsMenu,m_overlay;
    [SerializeField]
    private AudioMixer m_mixer;

    private bool m_gamePaused;

    private void Start()
    {
        m_mixer.SetFloat("masterVol", -80 + PlayerPrefs.GetInt("masterVolume"));
        m_mixer.SetFloat("musicVol", -80 + PlayerPrefs.GetInt("musicVolume"));
        m_mixer.SetFloat("fxVol", -80 + PlayerPrefs.GetInt("fxVolume"));
        m_mixer.SetFloat("uiVol", -80 + PlayerPrefs.GetInt("uiVolume"));


        m_pauseMenu.SetActive(false);
        m_settingsMenu.SetActive(false);
        m_overlay.SetActive(false);
    }
    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("XboxStart") && m_gamePaused == false)
        {
            //PAUSE GAME HERE & DISABLE INPUTS


            m_gamePaused = true;
            m_pauseMenu.SetActive(true);
            m_overlay.SetActive(true);
        }
        else if (Input.GetButtonDown("XboxStart") && m_gamePaused == true)
        {
            CloseMenu();
        }
    }

    public void CloseMenu()
    {
        m_gamePaused = false;
        m_pauseMenu.SetActive(false);
        m_settingsMenu.SetActive(false);
        m_overlay.SetActive(false);
    }
}
