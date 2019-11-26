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

    private PlayerMovement m_playerMovement;

    private bool m_gamePaused;

    private void Start()
    {
        if (PlayerPrefs.GetInt("FirstTime") == 0)
        {
            PlayerPrefs.SetInt("masterVolume", 80);
            PlayerPrefs.SetInt("musicVolume", 80);
            PlayerPrefs.SetInt("fxVolume", 80);
            PlayerPrefs.SetInt("uiVolume", 80);
            PlayerPrefs.SetInt("ControlScheme", 1);
            PlayerPrefs.SetInt("FirstTime", 1);
        }

        m_mixer.SetFloat("masterVol", -80 + PlayerPrefs.GetInt("masterVolume", 80));
        m_mixer.SetFloat("musicVol", -80 + PlayerPrefs.GetInt("musicVolume", 80));
        m_mixer.SetFloat("fxVol", -80 + PlayerPrefs.GetInt("fxVolume", 80));
        m_mixer.SetFloat("uiVol", -80 + PlayerPrefs.GetInt("uiVolume", 80));


        m_playerMovement = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>();

        m_pauseMenu.SetActive(false);
        m_settingsMenu.SetActive(false);
        m_overlay.SetActive(false);
    }
    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("XboxStart") && m_gamePaused == false && GameManager.Instance.CanLeaveScene)
        {
            //PAUSE GAME HERE & DISABLE INPUTS
            GameManager.Instance.SetSlowMo(0);

            m_gamePaused = true;
            m_pauseMenu.SetActive(true);
            m_overlay.SetActive(true);
        }
        else if (Input.GetButtonDown("XboxStart") && m_gamePaused == true && GameManager.Instance.CanLeaveScene)
        {
            CloseMenu();
        }
    }

    public void CloseMenu()
    {
        GameManager.Instance.SetNormalSpeed();
        m_playerMovement.ControlScheme = (PlayerMovement.ControlType)PlayerPrefs.GetInt("ControlScheme", 0);
        m_gamePaused = false;
        m_pauseMenu.SetActive(false);
        m_settingsMenu.SetActive(false);
        m_overlay.SetActive(false);
    }
}
