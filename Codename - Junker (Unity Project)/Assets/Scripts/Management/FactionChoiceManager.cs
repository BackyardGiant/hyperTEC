using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FactionChoiceManager : MonoBehaviour
{
    [SerializeField] private TutorialManagement m_tutorial;
    [SerializeField]
    private GameObject m_player, m_traderHighlight, m_explorationHighlight, m_constructionHighlight;
    [SerializeField]
    private AudioClip[] m_sounds;
    [SerializeField]
    private EngineData traderEngine, explorerEngine, constructionEngine;
    [SerializeField]
    private WeaponData weaponData;
    [SerializeField]
    private GameObject m_weaponLootParent;
    [SerializeField]
    private GameObject m_engineLootParent;

    private bool m_readyForInput;
    private int m_selectedIndex;
    [SerializeField]
    private int m_saveIndex = 0;
    [SerializeField]
    private AudioSource m_UIAudio;

    [SerializeField]
    GameEvent factionChosen;


    // Update is called once per frame
    void Update()
    {
        //Input Management
        if ((Input.GetAxis("MacroEngine") == 0) && (Input.GetAxis("Horizontal") == 0))
        {
            m_readyForInput = true;
        }
        if (m_readyForInput)
        {
            //If you've added an item to the menu and it's not selecting, it's because it doesn't exist here.
            switch (m_selectedIndex)
            {
                case 0:
                    //None
                    m_traderHighlight.SetActive(false);
                    m_explorationHighlight.SetActive(false);
                    m_constructionHighlight.SetActive(false);
                    break;
                case 1:
                    //Trader
                    m_traderHighlight.SetActive(true);
                    m_explorationHighlight.SetActive(false);
                    m_constructionHighlight.SetActive(false);
                    break;
                case 2:
                    //Explorer
                    m_traderHighlight.SetActive(false);
                    m_explorationHighlight.SetActive(true);
                    m_constructionHighlight.SetActive(false);
                    break;
                case 3:
                    //Construction
                    m_traderHighlight.SetActive(false);
                    m_explorationHighlight.SetActive(false);
                    m_constructionHighlight.SetActive(true);
                    break;
            }
            if ((Input.GetAxis("MacroEngine") < -0.3) || (Input.GetAxis("Horizontal") < -0.3))
            {
                if (m_selectedIndex == 1 || m_selectedIndex == 0)
                {
                    m_selectedIndex = 3;
                }

                else
                {
                    m_selectedIndex--;
                }
                m_readyForInput = false;
                m_UIAudio.clip = m_sounds[0];
                m_UIAudio.Play();
            }
            if ((Input.GetAxis("MacroEngine") > 0.3) || (Input.GetAxis("Horizontal") > 0.3))
            {
                if (m_selectedIndex == 3 || m_selectedIndex == 0)
                {
                    m_selectedIndex = 1;
                }
                else
                {
                    m_selectedIndex++;
                }
                m_readyForInput = false;
                m_UIAudio.clip = m_sounds[0];
                m_UIAudio.Play();
            }
            if (Input.GetButtonDown("XboxA"))
            {
                string _fileName = PlayerPrefs.GetString("CurrentSave", "NoSave");
                m_saveIndex = int.Parse(_fileName[4].ToString());
                m_UIAudio.clip = m_sounds[1];
                m_UIAudio.Play();
                switch (m_selectedIndex)
                {
                    case 0:
                        break;
                    case 1:
                        Debug.Log("Chosen Trader");
                        factionChosen.Raise();
                        PlayerPrefs.SetString("ChosenFaction" + m_saveIndex,"trader");
                        m_tutorial.chosenFaction = "Trader";
                        this.gameObject.SetActive(false);
                        break;
                    case 2:
                        Debug.Log("Exploration");
                        factionChosen.Raise();
                        PlayerPrefs.SetString("ChosenFaction" + m_saveIndex, "exploratory");
                        m_tutorial.chosenFaction = "Exploration";
                        this.gameObject.SetActive(false);
                        break;
                    case 3:
                        Debug.Log("Construction");
                        factionChosen.Raise();
                        PlayerPrefs.SetString("ChosenFaction" + m_saveIndex, "construction");
                        m_tutorial.chosenFaction = "Construction";
                        this.gameObject.SetActive(false);
                        break;
                }
            }
        }
    }


}
