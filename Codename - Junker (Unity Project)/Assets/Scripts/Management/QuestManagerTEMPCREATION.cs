using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor;

public class QuestManagerTEMPCREATION : MonoBehaviour
{
    [SerializeField]
    private bool m_inEditor;

    [SerializeField]
    private GameObject m_lootTemplate;

    [SerializeField]
    private Transform[] m_engineSpawnPoints;

    [SerializeField]
    private GameObject[] m_engines;

    private Vector3 _offset = new Vector3(0.35f, 0.3f, 0f);

    // Start is called before the first frame update
    void Start()
    {
        if (PlayerPrefs.GetInt("CreatedQuest") == 0 || m_inEditor == true)
        {
            Debug.Log("Created Quest");
            DontDestroyOnLoad(this);
            PlayerPrefs.SetInt("CreatedQuest", 1);
            QuestManager.Instance.CreateKillQuest(15, "Control the Sector!", "Kill 15 Enemies to Control the Sector.");
      
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (SceneManager.GetActiveScene().name == "MainMenu")
        {
            PlayerPrefs.SetInt("CreatedQuest", 0);
            Destroy(this);
        }
    }

    void SpawnEngine()
    {
        string _saveName = PlayerPrefs.GetString("CurrentSave");
        int _saveIndex = int.Parse(_saveName[_saveName.Length - 1].ToString());

        // make thing that will be spawning relative to save idnex + spawn it   

        GameObject _lootParent0 = Instantiate(m_lootTemplate, m_engineSpawnPoints[0].position, m_engineSpawnPoints[0].rotation);

        GameObject _tempEngine0 = Instantiate(m_engines[0]);
        _tempEngine0.transform.SetParent(_lootParent0.transform);
        _tempEngine0.transform.localPosition = _offset;
        _tempEngine0.transform.GetChild(0).GetComponent<ThrustEffectController>().enabled = false;

        GameObject _lootParent1 = Instantiate(m_lootTemplate, m_engineSpawnPoints[1].position, m_engineSpawnPoints[1].rotation);

        GameObject _tempEngine1 = Instantiate(m_engines[1]);
        _tempEngine1.transform.SetParent(_lootParent1.transform);
        _tempEngine1.transform.localPosition = _offset;
        _tempEngine1.transform.GetChild(0).GetComponent<ThrustEffectController>().enabled = false;

        GameObject _lootParent2 = Instantiate(m_lootTemplate, m_engineSpawnPoints[2].position, m_engineSpawnPoints[2].rotation);

        GameObject _tempEngine2 = Instantiate(m_engines[2]);
        _tempEngine2.transform.SetParent(_lootParent2.transform);
        _tempEngine2.transform.localPosition = _offset;
        _tempEngine2.transform.GetChild(0).GetComponent<ThrustEffectController>().enabled = false;

        GameObject _lootParent3 = Instantiate(m_lootTemplate, m_engineSpawnPoints[3].position, m_engineSpawnPoints[3].rotation);

        GameObject _tempEngine3 = Instantiate(m_engines[3]);
        _tempEngine3.transform.SetParent(_lootParent3.transform);
        _tempEngine3.transform.localPosition = _offset;
        _tempEngine3.transform.GetChild(0).GetComponent<ThrustEffectController>().enabled = false;
    }
}
