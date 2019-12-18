using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialManagement : MonoBehaviour
{
    [HideInInspector]
    public string chosenFaction = "none";


    [SerializeField] private GameObject m_player;
    [SerializeField] private GameObject m_explosion;
    [SerializeField, Space(20)] private GameObject m_factionChoice,m_skipPrompt;
    [SerializeField] private GameObject m_prompt1, m_prompt2, m_prompt3, m_prompt4, m_prompt5, m_prompt6, m_prompt7, m_prompt8, m_prompt9;

    [SerializeField]private EngineData traderEngine, explorerEngine, constructionEngine;
    [SerializeField]private WeaponData weaponData;
    [SerializeField]private GameObject m_weaponLootParent;
    [SerializeField]private GameObject m_engineLootParent;
    [SerializeField] private GameObject m_devNote;

    private int m_saveIndex;
    private bool m_spawnedGuns;
    private bool m_spawnedEngine;
    private bool m_allowSkip = false;
    private bool m_continued = false;
    

    // Start is called before the first frame update
    public void onLoad()
    {
        if (PlayerPrefs.GetString("CurrentSave") == "Save1") { m_saveIndex = 1; }
        else if (PlayerPrefs.GetString("CurrentSave") == "Save2") { m_saveIndex = 2; }
        else if (PlayerPrefs.GetString("CurrentSave") == "Save3") { m_saveIndex = 3; }
        else if (PlayerPrefs.GetString("CurrentSave") == "Save4") { m_saveIndex = 4; }
        else { Debug.LogWarning("Faction Selection : Save not properly configured. Did you go through the Main Menu?"); }
        m_prompt1.SetActive(false);
        m_prompt2.SetActive(false);
        m_prompt3.SetActive(false);
        m_prompt4.SetActive(false);
        m_prompt5.SetActive(false);
        m_prompt6.SetActive(false);
        m_prompt7.SetActive(false);
        m_prompt8.SetActive(false);
        m_prompt9.SetActive(false);
        m_factionChoice.SetActive(false);
        m_skipPrompt.SetActive(false);
        int _currentTutorialProgress = PlayerPrefs.GetInt("TutorialProgress" + m_saveIndex, 0);
        GameManager.Instance.InTutorial = true;
        if (_currentTutorialProgress == 9)
        {
            this.gameObject.SetActive(false);
            GameManager.Instance.InTutorial = false;
        }

#if UNITY_EDITOR
        m_devNote.SetActive(true);
#endif

        if (_currentTutorialProgress == 0){m_prompt1.SetActive(true); }
        if (_currentTutorialProgress == 1) { m_prompt2.SetActive(true); }
        if (_currentTutorialProgress == 2) { m_prompt3.SetActive(true); }
        if (_currentTutorialProgress == 3) { m_prompt4.SetActive(true); }
        if (_currentTutorialProgress == 4) { m_prompt5.SetActive(true); }
        if (_currentTutorialProgress == 5) { m_prompt6.SetActive(true); }
        if (_currentTutorialProgress == 6) { m_prompt7.SetActive(true); }
        if (_currentTutorialProgress == 7) { m_prompt8.SetActive(true); }
        if (_currentTutorialProgress == 8) { m_prompt9.SetActive(true); }




    }

    // Update is called once per frame
    void Update()
    {
        int _currentTutorialProgress = PlayerPrefs.GetInt("TutorialProgress" + m_saveIndex, 0);
        if (_currentTutorialProgress == 0 && !m_continued)
        {
            GameManager.Instance.SetSlowMo(0);
            //Welcome to Hypertec
            m_prompt1.SetActive(true);
        }
        if (_currentTutorialProgress == 0 && Input.GetButtonDown("Throttle Up"))
        {
            GameManager.Instance.SetNormalSpeed();
            //Start off with guns
            m_prompt1.SetActive(false);
            m_prompt2.SetActive(true);
            if(m_spawnedGuns == false)
            {
                m_spawnedGuns = true;
                SpawnStartingGuns();
            }

            m_continued = true;
            PlayerPrefs.SetInt("TutorialProgress" + m_saveIndex, 1);
        }
        if (_currentTutorialProgress == 1 && Input.GetButtonDown("XboxX"))
        {
            GameManager.Instance.SetNormalSpeed();
            //Head to Inventory Prompt
            m_prompt2.SetActive(false);
            m_prompt3.SetActive(true);
            PlayerPrefs.SetInt("TutorialProgress" + m_saveIndex, 2);
        }
        if (_currentTutorialProgress == 2 && (PlayerInventoryManager.Instance.EquippedLeftWeapon != null || PlayerInventoryManager.Instance.EquippedRightWeapon != null))
        {
            //Try shooting with LT and RT
            m_prompt3.SetActive(false);
            m_prompt4.SetActive(true);
        }
        if(_currentTutorialProgress == 2 && (PlayerInventoryManager.Instance.EquippedLeftWeapon != null || PlayerInventoryManager.Instance.EquippedRightWeapon != null))
        {
            PlayerPrefs.SetInt("TutorialProgress" + m_saveIndex, 3);
        }
        if (_currentTutorialProgress == 3 && (Input.GetButtonDown("Throttle Up")|| (Input.GetAxis("LeftTrigger")>0.1f) || (Input.GetAxis("RightTrigger")>0.1f)))
        {
            //Choose A Faction!
            m_prompt4.SetActive(false);
            m_prompt5.SetActive(true);
            PlayerPrefs.SetInt("TutorialProgress" + m_saveIndex, 4);
            Invoke("ShowFactionChoice", 2f);
        }
        if(_currentTutorialProgress == 4 && chosenFaction != "none")
        {
            GameManager.Instance.SetNormalSpeed();
            //Equip Engine!
            m_prompt5.SetActive(false);
            m_prompt6.SetActive(true);

            if(chosenFaction == "Trader" && m_spawnedEngine == false)
            {
                m_spawnedEngine = true;
                SpawnTraderEngine();
            }
            if (chosenFaction == "Exploration" && m_spawnedEngine == false)
            {
                m_spawnedEngine = true;
                SpawnExplorationEngine();
            }
            if (chosenFaction == "Construction" && m_spawnedEngine == false)
            {
                m_spawnedEngine = true;
                SpawnConstructionEngine();
            }
            m_allowSkip = true;
            m_skipPrompt.SetActive(true);
            PlayerPrefs.SetInt("TutorialProgress" + m_saveIndex, 5);
        }
        if(_currentTutorialProgress == 5 && PlayerInventoryManager.Instance.EquippedEngine != null )
        {
            //Press A to Go forward
            m_prompt6.SetActive(false);
            m_prompt7.SetActive(true);
        }
        if (_currentTutorialProgress == 5 && PlayerInventoryManager.Instance.EquippedEngine != null)
        {
            //Press A to Go forward
            PlayerPrefs.SetInt("TutorialProgress" + m_saveIndex, 6);
        }
        if (_currentTutorialProgress == 6 && Input.GetButtonDown("Throttle Up"))
        {
            //Press B to slow down
            m_prompt7.SetActive(false);
            m_prompt8.SetActive(true);
            PlayerPrefs.SetInt("TutorialProgress" + m_saveIndex, 7);
        }
        if (_currentTutorialProgress == 7 && (Input.GetButtonDown("Throttle Down") || Input.GetButtonDown("Throttle Up")))
        {
            //Go check out light
            m_prompt8.SetActive(false);
            m_prompt9.SetActive(true);
            PlayerPrefs.SetInt("TutorialProgress" + m_saveIndex, 8);
        }
        if (_currentTutorialProgress == 8 && Input.GetButtonDown("Throttle Up"))
        {
            m_prompt9.SetActive(false);
            this.gameObject.SetActive(false);
            PlayerPrefs.SetInt("TutorialProgress" + m_saveIndex, 9);
        }
        if(_currentTutorialProgress == 9)
        {
            GameManager.Instance.InTutorial = false;
            this.gameObject.SetActive(false);
        }
        Debug.Log(Input.GetAxis("RightLeftDPad"));
        if (Input.GetAxis("RightLeftDPad") > 0.3f && m_allowSkip == true)
        {
            PlayerPrefs.SetInt("TutorialProgress" + m_saveIndex, 9);
            GameManager.Instance.InTutorial = false;
            this.gameObject.SetActive(false);
        }

#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.F11))
        {
            SpawnExplorationEngine();
            SpawnStartingGuns();
            PlayerPrefs.SetString("ChosenFaction" + m_saveIndex, "exploratory");
            PlayerPrefs.SetInt("TutorialProgress" + m_saveIndex, 9);
            this.gameObject.SetActive(false);
        }
#endif

        if (_currentTutorialProgress == 1 || _currentTutorialProgress == 2 || _currentTutorialProgress == 3 || _currentTutorialProgress == 5 || _currentTutorialProgress == 6)
        {
            GameManager.Instance.SetNormalSpeed();
        }

    }


    void ShowFactionChoice()
    {
        GameManager.Instance.SetSlowMo(0);
        //Choose A Faction!
        m_prompt5.SetActive(false);
        m_factionChoice.SetActive(true);
    }
    #region Spawn Starting Loot
    void SpawnTraderEngine()
    {
        GameObject _lootParent = Instantiate(m_engineLootParent);
        _lootParent.transform.localPosition = m_player.transform.position + (Vector3.forward * 20);

        GameObject _tempEngine = ModuleManager.Instance.GenerateEngine(traderEngine);
        _tempEngine.transform.SetParent(_lootParent.transform);
        _tempEngine.transform.localPosition = Vector3.zero;
        GameObject _explosionLeft = Instantiate(m_explosion);
        _explosionLeft.transform.position = _tempEngine.transform.position;
    }
    void SpawnExplorationEngine()
    {
        GameObject _lootParent = Instantiate(m_engineLootParent);
        _lootParent.transform.localPosition = m_player.transform.position + (Vector3.forward * 20);

        GameObject _tempEngine = ModuleManager.Instance.GenerateEngine(explorerEngine);
        _tempEngine.transform.SetParent(_lootParent.transform);
        _tempEngine.transform.localPosition = Vector3.zero;
        GameObject _explosionLeft = Instantiate(m_explosion);
        _explosionLeft.transform.position = _tempEngine.transform.position;
    }
    void SpawnConstructionEngine()
    {
        GameObject _lootParent = Instantiate(m_engineLootParent);
        _lootParent.transform.localPosition = m_player.transform.position + (Vector3.forward * 20);

        GameObject _tempEngine = ModuleManager.Instance.GenerateEngine(constructionEngine);
        _tempEngine.transform.SetParent(_lootParent.transform);
        _tempEngine.transform.localPosition = Vector3.zero;
        GameObject _explosionLeft = Instantiate(m_explosion);
        _explosionLeft.transform.position = _tempEngine.transform.position;
    }
    void SpawnStartingGuns()
    {
        PromptSystemManagement.Instance.CustomPrompt("Welcome To The Quarry Sector");
        GameObject _lootParent = Instantiate(m_weaponLootParent);
        _lootParent.transform.localPosition = m_player.transform.position + (Vector3.forward * 25) + (Vector3.right * 10);
        GameObject _leftGun = ModuleManager.Instance.GenerateWeapon(weaponData);
        _leftGun.GetComponent<WeaponGenerator>().statBlock = weaponData;
        _leftGun.transform.SetParent(_lootParent.transform);
        _leftGun.transform.localPosition = Vector3.zero;
        _leftGun.transform.localRotation = Quaternion.identity;
        _leftGun.transform.localScale = new Vector3(1, 1, 1);
        GameObject _explosionLeft = Instantiate(m_explosion);
        _explosionLeft.transform.position = _leftGun.transform.position;

        _lootParent = Instantiate(m_weaponLootParent);
        _lootParent.transform.localPosition = m_player.transform.position + (Vector3.forward * 20) - (Vector3.right * 10);
        GameObject _rightGun = ModuleManager.Instance.GenerateWeapon(weaponData);
        _rightGun.GetComponent<WeaponGenerator>().statBlock = weaponData;
        _rightGun.transform.SetParent(_lootParent.transform);
        _rightGun.transform.position = Vector3.zero;
        _rightGun.transform.localPosition = Vector3.zero;
        _rightGun.transform.localRotation = Quaternion.identity;
        _rightGun.transform.localScale = new Vector3(-1, 1, 1);
        GameObject _explosionRight = Instantiate(m_explosion);
        _explosionRight.transform.position = _rightGun.transform.position;
    }
    #endregion
}
