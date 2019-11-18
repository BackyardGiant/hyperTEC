using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor;

public class QuestManagerTEMPCREATION : MonoBehaviour
{
    [SerializeField]
    private bool m_inEditor;
    // Start is called before the first frame update
    void Start()
    {
        if (PlayerPrefs.GetInt("CreatedQuest") == 0|| m_inEditor == true)
        {
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
}
