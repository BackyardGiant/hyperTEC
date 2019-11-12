using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;

public class MainMenuManagement : MonoBehaviour
{
    [SerializeField, Tooltip("Settings Menu Object")]
    private GameObject m_settingsMenu;
    [SerializeField, Tooltip("All of the items on the menu to sort through")]
    private GameObject[] m_menuOptions;
    [SerializeField, Tooltip("UI sounds. First should be the scroll, second should be select")]
    private AudioClip[] m_sounds;
    [SerializeField, Tooltip("Audio Mixers")]
    private AudioMixer m_mixer;
    [SerializeField, Tooltip("MainMenu Anim")]
    private Animator m_mainMenuAnimator;

    private bool m_inputAllowed; //Tracks if the initial logo animation is done.
    private bool m_onMenu; //Tracks whether the player had progressed through from the logo to the menu yet.
    private bool m_readyForInput;
    private int m_selectedIndex;
    private AudioSource m_UIAudio;

    private void Start()
    {
        if (PlayerPrefs.GetInt("FirstTime") == 0)
        {
            PlayerPrefs.SetInt("masterVolume", 80);
            PlayerPrefs.SetInt("musicVolume", 80);
            PlayerPrefs.SetInt("fxVolume", 80);
            PlayerPrefs.SetInt("uiVolume", 80);
            PlayerPrefs.SetInt("ControlScheme", 2);
            PlayerPrefs.SetInt("FirstTime", 1);
        }

        m_mixer.SetFloat("masterVol", -80 + PlayerPrefs.GetInt("masterVolume"));
        m_mixer.SetFloat("musicVol", -80 + PlayerPrefs.GetInt("musicVolume"));
        m_mixer.SetFloat("fxVol", -80 + PlayerPrefs.GetInt("fxVolume"));
        m_mixer.SetFloat("uiVol", -80 + PlayerPrefs.GetInt("uiVolume"));


        m_UIAudio = this.GetComponent<AudioSource>();
        m_selectedIndex = 0;
        m_onMenu = false;
        m_inputAllowed = false;
        if (m_onMenu == false)
        {
            Invoke("allowInputs", 4f);
        }
    }

    private void Update()
    {

        if (m_onMenu == true)
        {
            if ((Input.GetAxis("MacroEngine") == 0) && (Input.GetAxis("Vertical") == 0))
            {
                m_readyForInput = true;
            }

            if (m_readyForInput)
            {
                //If you've added an item to the menu and it's not selecting, it's because it doesn't exist here.
                switch (m_selectedIndex)
                {
                    case 0:
                        //Continue
                        AnimateBar(0);
                        CloseBar(1);
                        CloseBar(2);
                        CloseBar(3);
                        CloseBar(4);
                        break;
                    case 1:
                        AnimateBar(1);
                        CloseBar(0);
                        CloseBar(2);
                        CloseBar(3);
                        CloseBar(4);
                        //Load Game
                        break;
                    case 2:
                        AnimateBar(2);
                        CloseBar(0);
                        CloseBar(1);
                        CloseBar(3);
                        CloseBar(4);
                        //New Game
                        break;
                    case 3:
                        AnimateBar(3);
                        CloseBar(0);
                        CloseBar(1);
                        CloseBar(2);
                        CloseBar(4);
                        //Settings
                        break;
                    case 4:
                        AnimateBar(4);
                        CloseBar(0);
                        CloseBar(1);
                        CloseBar(2);
                        CloseBar(3);
                        //Quit
                        break;
                }
                if ((Input.GetAxis("MacroEngine") < -0.3) || (Input.GetAxis("Vertical") < -0.3))
                {
                    if (m_selectedIndex == m_menuOptions.Length - 1)
                    {
                        m_selectedIndex = 0;
                    }
                    else
                    {
                        m_selectedIndex++;
                    }
                    m_readyForInput = false;
                    m_UIAudio.clip = m_sounds[0];
                    m_UIAudio.Play();
                }
                if ((Input.GetAxis("MacroEngine") > 0.3) || (Input.GetAxis("Vertical") > 0.3))
                {
                    if (m_selectedIndex == 0)
                    {
                        m_selectedIndex = m_menuOptions.Length - 1;
                    }
                    else
                    {
                        m_selectedIndex--;
                    }
                    m_readyForInput = false;
                    m_UIAudio.clip = m_sounds[0];
                    m_UIAudio.Play();
                }


                if (Input.GetButtonDown("XboxA"))
                {
                    m_UIAudio.clip = m_sounds[1];
                    m_UIAudio.Play();
                    switch (m_selectedIndex)
                    {
                        case 0:
                            continueGame();
                            break;
                        case 1:
                            loadGame();
                            break;
                        case 2:
                            newGame();
                            break;
                        case 3:
                            settings();
                            break;
                        case 4:
                            quit();
                            break;
                    }
                }
            }
        }

        
    }

    private void AnimateBar(int _item)
    {
        Image _bar = m_menuOptions[_item].GetComponent<Image>();
        _bar.fillAmount += 2f * Time.deltaTime;
    }
    private void CloseBar(int _item)
    {
        Image _bar = m_menuOptions[_item].GetComponent<Image>();
        _bar.fillAmount -= 3f * Time.deltaTime;
    }
    private void InstantiatePlayerPrefs()
    {
        PlayerPrefs.SetString("PlaysessionTime", "ERROR");
        PlayerPrefs.SetInt("RecordedSession", 0);
        PlayerPrefs.SetInt("ControlScheme", 0);
        PlayerPrefs.SetInt("ChosenEngine", 0);
        PlayerPrefs.SetInt("ChosenEngineType", 0);
        PlayerPrefs.SetInt("EnemiesKilled", 0);
        PlayerPrefs.SetInt("BoostedCount", 0);
        PlayerPrefs.SetInt("WeaponsCollected", 0);
        PlayerPrefs.SetInt("WeaponsScanned", 0);
        PlayerPrefs.SetInt("WeaponsDestroyed", 0);
    }

    public void allowInputs()
    {
        m_onMenu = true;
        m_menuOptions[0].SetActive(true);
        m_menuOptions[1].SetActive(true);
        m_menuOptions[2].SetActive(true);
        m_menuOptions[3].SetActive(true);
        m_menuOptions[4].SetActive(true);
    }


    private void continueGame()
    {
        //Load most recent save state and move into Main Scene
        Debug.Log("Continue");
    }

    private void loadGame()
    {
        Debug.Log("Load Game");
        //m_onMenu = false;
    }
    private void newGame()
    {
        Debug.Log("New Game");
    }
    private void settings()
    {
        Debug.Log("Settings");
        m_settingsMenu.SetActive(true);
        this.gameObject.SetActive(false);
    }
    private void quit()
    {
        Application.Quit();
        Debug.Log("Quit");
    }

}