using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;

public class PauseMenuManagement : MonoBehaviour
{

    [SerializeField]
    private EndPlaysessionController m_playsessionController;

    [SerializeField, Tooltip("Settings Menu Object")]
    private GameObject m_settingsMenu;
    [SerializeField, Tooltip("LoadGame Menu Object")]
    private GameObject m_loadGameMenu;
    [SerializeField, Tooltip("All of the items on the menu to sort through")]
    private GameObject[] m_menuOptions;
    [SerializeField, Tooltip("UI sounds. First should be the scroll, second should be select")]
    private AudioClip[] m_sounds;
    [SerializeField, Tooltip("Audio Mixers")]
    private AudioMixer m_mixer;
    [SerializeField, Tooltip("Game Saved Animator")]
    private Animator m_gameSaved;

    private bool m_inputAllowed; //Tracks if the initial logo animation is done.
    private bool m_onMenu; //Tracks whether the player had progressed through from the logo to the menu yet.
    private bool m_readyForInput;
    private int m_selectedIndex;
    private AudioSource m_UIAudio;

    private void Start()
    {
        m_mixer.SetFloat("masterVol", -80 + PlayerPrefs.GetInt("masterVolume"));
        m_mixer.SetFloat("musicVol", -80 + PlayerPrefs.GetInt("musicVolume"));
        m_mixer.SetFloat("fxVol", -80 + PlayerPrefs.GetInt("fxVolume"));
        m_mixer.SetFloat("uiVol", -80 + PlayerPrefs.GetInt("uiVolume"));


        m_UIAudio = this.GetComponent<AudioSource>();
        m_selectedIndex = 0;
        m_inputAllowed = false;
    }

    private void Update()
    {
        if (Input.GetButtonDown("XboxB"))
        {
            this.GetComponentInParent<PauseGame>().CloseMenu();
        }
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
                        resumeGame();
                        break;
                    case 1:
                        saveGame();
                        break;
                    case 2:
                        loadGame();
                        break;
                    case 3:
                        settings();
                        break;
                    case 4:
                        m_playsessionController.SaveValues();
                        mainMenu();
                        break;
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


    private void resumeGame()
    {
        //Load most recent save state and move into Main Scene
        this.GetComponentInParent<PauseGame>().CloseMenu();
        Debug.Log("Continue");
    }

    private void saveGame()
    {
        Debug.Log("saveGame");
        m_gameSaved.Play("GameSavedAnim");
        string _saveIndex = PlayerPrefs.GetString("CurrentSave");

        GameManager.Instance.SaveGame();
    }
    private void loadGame()
    {
        m_loadGameMenu.SetActive(true);
        this.gameObject.SetActive(false);
    }
    private void settings()
    {
        Debug.Log("Settings");
        m_settingsMenu.SetActive(true);
        this.gameObject.SetActive(false);
    }
    private void mainMenu()
    {
        GameManager.Instance.SaveGame();
        SceneManager.LoadScene("MainMenu");
        Debug.Log("Quit");
    }

}