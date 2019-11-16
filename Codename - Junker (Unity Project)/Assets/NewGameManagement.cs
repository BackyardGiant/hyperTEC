using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;
using TMPro;

public class NewGameManagement : MonoBehaviour
{
    [SerializeField, Tooltip("Main Menu Object")]
    private GameObject m_mainMenu;
    [SerializeField, Tooltip("Stats Object")]
    private GameObject m_saveStats;
    [SerializeField]
    private TextMeshProUGUI m_titleText, m_chosenFaction, m_enemiesKilled, m_lastSaved, m_saveOverrideTitle;


    [SerializeField, Tooltip("All of the items on the menu to sort through"), Space(10)]
    private GameObject[] m_menuOptions;
    [SerializeField, Tooltip("UI sounds. First should be the scroll, second should be select")]
    private AudioClip[] m_sounds;
    [SerializeField, Tooltip("Audio Mixers")]
    private AudioMixer m_mixer;
    [SerializeField, Tooltip("Animation")]
    private Animator m_overrideScreen;

    private bool m_readyForInput;
    private int m_selectedIndex;
    private int m_saveIndex;
    private AudioSource m_UIAudio;
    private bool m_overrideCheck;

    private void Start()
    {
        m_selectedIndex = 0;
        m_mainMenu.SetActive(false);
        m_saveIndex += 1;
        m_overrideCheck = false;

        displaySaveStats(false);

        m_UIAudio = this.GetComponent<AudioSource>();
        m_selectedIndex = 0;
    }
    private void Update()
    {
        if(m_overrideCheck == true)
        {
            if (Input.GetButtonUp("XboxB"))
            {
                m_overrideScreen.Play("OverrideOut");
                m_overrideCheck = false;
            }
            if (Input.GetButtonUp("XboxX"))
            {
                overrideSave();
            }
        }

        if (Input.GetButtonDown("XboxB") && m_overrideCheck == false)
        {
            m_mainMenu.SetActive(true);
            this.gameObject.SetActive(false);
        }
        if ((Input.GetAxis("MacroEngine") == 0) && (Input.GetAxis("Vertical") == 0))
        {
            m_readyForInput = true;
        }
        if (m_readyForInput && m_overrideCheck == false)
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
                if (m_selectedIndex != 4)
                {
                    m_saveIndex = m_selectedIndex + 1;
                    displaySaveStats(true);
                }
                else
                {
                    m_saveStats.GetComponent<Animator>().Play("FadeGameDisplayFadeOut");
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
                if (m_selectedIndex != 4)
                {
                    m_saveIndex = m_selectedIndex + 1;
                    displaySaveStats(true);
                }
                else
                {
                    m_saveStats.GetComponent<Animator>().Play("FadeGameDisplayFadeOut");
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
                        //SAVE GAME 1
                        showOverride(1);
                        break;
                    case 1:
                        //SAVE GAME 2
                        showOverride(2);
                        break;
                    case 2:
                        //SAVE GAME 2
                        showOverride(3);
                        break;
                    case 3:
                        //SAVE GAME 2
                        showOverride(4);
                        break;
                    case 4:
                        mainMenu();
                        break;
                }
            }
        }
    }


    private void overrideSave()
    {
        PlayerPrefs.SetString("CurrentSave","Save" + m_saveIndex);
        SceneManager.LoadScene("MainScene");
    }
    private void newSave()
    {
        PlayerPrefs.SetString("CurrentSave", "Save" + m_saveIndex);
        SceneManager.LoadScene("MainScene");
    }

    private void showOverride(int m_saveIndex)
    {
        string _chosenFaction = PlayerPrefs.GetString("ChosenFaction" + m_saveIndex);
        if(_chosenFaction == null || _chosenFaction == "" || _chosenFaction == " ")
        {
            newSave();
        }
        else
        {
            m_overrideCheck = true;
            m_saveOverrideTitle.text = "Override Save File " + m_saveIndex + "?";
            m_overrideScreen.Play("OverrideIn");
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
    private void mainMenu()
    {
        m_mainMenu.SetActive(true);
        gameObject.SetActive(false);
    }
    private void displaySaveStats(bool _animate)
    {
        m_saveStats.SetActive(false);
        m_saveStats.SetActive(true);
        if (_animate == true)
        {
            Invoke("populateStats", 0.2f);
        }
        else
        {
            populateStats();
        }

    }
    private void populateStats()
    {
        m_titleText.text = "Save Game " + m_saveIndex;

        //CHOSEN FACTION
        string _faction = PlayerPrefs.GetString("ChosenFaction" + m_saveIndex); //Get the chosen faction from the save file
        if (_faction == "trader")
        {
            m_chosenFaction.color = Color.cyan;
            m_chosenFaction.text = "trader";//Change to the fancy name later;
        }
        else if (_faction == "exploratory")
        {
            m_chosenFaction.color = new Color32(239, 50, 40, 255);
            m_chosenFaction.text = "exploratory";//Change to the fancy name later;

        }
        else if (_faction == "construction")
        {
            m_chosenFaction.color = new Color32(238, 168, 11, 255);
            m_chosenFaction.text = "construction";//Change to the fancy name later;
        }
        else
        {
            m_chosenFaction.color = Color.red;
            m_chosenFaction.text = "EMPTY";//Change to the fancy name later;
        }


        //ENEMIES KILLED
        string _enemiesKilled = PlayerPrefs.GetInt("EnemiesKilled" + m_saveIndex).ToString();
        if (_enemiesKilled == null || _enemiesKilled == "0")
        {
            m_enemiesKilled.text = "EMPTY";
        }
        else
        {
            m_enemiesKilled.text = PlayerPrefs.GetInt("EnemiesKilled" + m_saveIndex).ToString();
        }


        //OTHER STAT
        string _lastSave = PlayerPrefs.GetString("LastSave" + m_saveIndex);
        if (_lastSave == null || _lastSave == "")
        {
            m_lastSaved.text = "EMPTY";
        }
        else
        {
            m_lastSaved.text = PlayerPrefs.GetString("LastSave" + m_saveIndex);
        }
    }
}