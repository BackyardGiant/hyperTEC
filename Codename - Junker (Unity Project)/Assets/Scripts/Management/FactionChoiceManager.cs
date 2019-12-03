using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FactionChoiceManager : MonoBehaviour
{
    [SerializeField]
    private GameObject m_player, m_traderHighlight, m_explorationHighlight, m_constructionHighlight;
    [SerializeField]
    private AudioClip[] m_sounds;
    [SerializeField]
    private EngineData traderEngine, explorerEngine, constructionEngine;

    private bool m_readyForInput;
    private int m_selectedIndex;
    private int m_saveIndex;
    [SerializeField]
    private AudioSource m_UIAudio;
 
    //// Start is called before the first frame update
    //void Start()
    //{
        

    //}

    public void onLoad()
    {
        Debug.Log(PlayerPrefs.GetString("CurrentSave"));
        m_selectedIndex = 0;
        if (PlayerPrefs.GetString("CurrentSave") == "Save1") { m_saveIndex = 1; }
        else if (PlayerPrefs.GetString("CurrentSave") == "Save2") { m_saveIndex = 2; }
        else if (PlayerPrefs.GetString("CurrentSave") == "Save3") { m_saveIndex = 3; }
        else if (PlayerPrefs.GetString("CurrentSave") == "Save4") { m_saveIndex = 4; }
        else { Debug.LogWarning("Faction Selection : Save not properly configured. Did you go through the Main Menu?"); }

        Debug.Log("Save index is" + m_saveIndex);
        Debug.LogWarning(PlayerPrefs.GetString("ChosenFaction" + m_saveIndex));
        if (PlayerPrefs.GetString("ChosenFaction" + m_saveIndex) != "initial") { GameManager.Instance.SetNormalSpeed(); this.gameObject.SetActive(false); }
        else
        {
            GameManager.Instance.SetSlowMo(0);
        }
    }

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
                m_UIAudio.clip = m_sounds[1];
                m_UIAudio.Play();
                switch (m_selectedIndex)
                {
                    case 0:
                        break;
                    case 1:
                        Debug.Log("Chosen Trader");
                        SpawnTraderEngine();
                        PlayerPrefs.SetString("ChosenFaction" + m_saveIndex,"Trader");
                        GameManager.Instance.SetNormalSpeed();
                        this.gameObject.SetActive(false);
                        break;
                    case 2:
                        Debug.Log("Exploration");
                        SpawnExplorationEngine();
                        PlayerPrefs.SetString("ChosenFaction" + m_saveIndex, "Exploration");
                        GameManager.Instance.SetNormalSpeed();
                        this.gameObject.SetActive(false);
                        break;
                    case 3:
                        Debug.Log("Construction");
                        SpawnConstructionEngine();
                        PlayerPrefs.SetString("ChosenFaction" + m_saveIndex, "Construction");
                        GameManager.Instance.SetNormalSpeed();
                        this.gameObject.SetActive(false);
                        break;
                }
            }
        }
    }


    void SpawnTraderEngine()
    {
        GameObject _tempEngine = ModuleManager.Instance.GenerateEngine(traderEngine);
    }

    void SpawnExplorationEngine()
    {
        //This is a script that's on a canvas. m_player is a reference within this script that can be used for a spawn position.
    }

    void SpawnConstructionEngine()
    {
        //This is a script that's on a canvas. m_player is a reference within this script that can be used for a spawn position.
    }
}
