using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;
using TMPro;

public class MainMenuManagement : MonoBehaviour
{
    [SerializeField, Tooltip("Settings Menu Object"), Header("Sub-Menu List")]
    private GameObject m_settingsMenu;
    [SerializeField, Tooltip("Load Game Menu Object")]
    private GameObject m_loadGameMenu;
    [SerializeField, Tooltip("New Game Menu Object")]
    private GameObject m_newGameMenu;


    [SerializeField, Tooltip("Continue Text - Used to display WHICH save you will be continuing")]
    private TextMeshProUGUI m_continueText;
    [SerializeField, Tooltip("All of the items on the menu to sort through")]
    private GameObject[] m_menuOptions;
    [SerializeField, Tooltip("UI sounds. First should be the scroll, second should be select")]
    private AudioClip[] m_sounds;
    [SerializeField, Tooltip("Audio Mixers")]
    private AudioMixer m_mixer;
    [SerializeField, Tooltip("MainMenu Anim")]
    private Animator m_mainMenuAnimator, m_logoAnimator;

    private bool m_inputAllowed; //Tracks if the initial logo animation is done.
    private bool m_onMenu; //Tracks whether the player had progressed through from the logo to the menu yet.
    private bool m_readyForInput;
    private string m_recentSave;
    private int m_selectedIndex;
    private AudioSource m_UIAudio;

    private void Start()
    {
        latestSave();
        m_selectedIndex = 0;
        m_onMenu = false;
        m_settingsMenu.SetActive(false);
        m_loadGameMenu.SetActive(false);
        m_newGameMenu.SetActive(false);
        if (PlayerPrefs.GetInt("FirstTime") == 0)
        {
            PlayerPrefs.SetInt("masterVolume", 80);
            PlayerPrefs.SetInt("musicVolume", 80);
            PlayerPrefs.SetInt("fxVolume", 80);
            PlayerPrefs.SetInt("uiVolume", 80);
            PlayerPrefs.SetInt("ControlScheme", 1);
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
        if (Input.anyKeyDown && m_onMenu == false)
        {
            m_mainMenuAnimator.speed = 1000;
            m_logoAnimator.speed = 1000;
            Invoke("allowInputs", 0.2f);
        }

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
                            newGame();
                            break;
                        case 2:
                            loadGame();
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
    private void latestSave()
    {
        //Retrieve Dates from Player Prefs
        string _date1 = PlayerPrefs.GetString("LastSave1");
        string _date2 = PlayerPrefs.GetString("LastSave2");
        string _date3 = PlayerPrefs.GetString("LastSave3");
        string _date4 = PlayerPrefs.GetString("LastSave4");


        //If the playerpref is empty, set it to an impossible date
        if (_date1 == null || _date1 == "" || _date1 == " "){_date1 = "01/01/2000";}
        if (_date2 == null || _date2 == "" || _date2 == " "){_date2 = "01/01/2000"; }
        if (_date3 == null || _date3 == "" || _date3 == " "){_date3 = "01/01/2000"; }
        if (_date4 == null || _date4 == "" || _date4 == " "){_date4 = "01/01/2000"; }


        //Parse all of the dates so they're easy to check
        System.DateTime save1 = System.DateTime.Parse(_date1);
        System.DateTime save2 = System.DateTime.Parse(_date2);
        System.DateTime save3 = System.DateTime.Parse(_date3);
        System.DateTime save4 = System.DateTime.Parse(_date4);

        //FailedDate to check if it's an established save
        System.DateTime failedDate = System.DateTime.Parse("01/01/2000");
        System.DateTime now = System.DateTime.Now;
        System.DateTime currentlyLatest = failedDate;
        int lowestIndex = 0;

        //Check each date to find the lowest
        if (System.DateTime.Compare(save1, currentlyLatest) > 0 && save1 != failedDate){ currentlyLatest = save1; lowestIndex = 1;}
        Debug.Log(currentlyLatest);
        if (System.DateTime.Compare(save2, currentlyLatest) > 0 && save2 != failedDate){ currentlyLatest = save2; lowestIndex = 2;}
        Debug.Log(currentlyLatest);
        if (System.DateTime.Compare(save3, currentlyLatest) > 0 && save3 != failedDate){ currentlyLatest = save3; lowestIndex = 3;}
        Debug.Log(currentlyLatest);
        if (System.DateTime.Compare(save4, currentlyLatest) > 0 && save4 != failedDate){ currentlyLatest = save4; lowestIndex = 4;}
        Debug.Log(currentlyLatest);
        //No save files currentlyLatest.
        if (lowestIndex == 0){m_continueText.text = "Play Game"; m_recentSave = "Save1"; } else { m_continueText.text = "Continue - Save " + lowestIndex; m_recentSave = "Save" + lowestIndex;}
    }


    private void continueGame()
    {
        //Load most recent save state and move into Main Scene
        Debug.Log("Continue");
        PlayerPrefs.SetString("CurrentSave", m_recentSave);
        SceneManager.LoadScene("MainScene");
    }
    private void loadGame()
    {
        Debug.Log("Load Game");
        m_loadGameMenu.SetActive(true);
        gameObject.SetActive(false);
    }
    private void newGame()
    {
        Debug.Log("New Game");
        m_newGameMenu.SetActive(true);
        this.gameObject.SetActive(false);
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