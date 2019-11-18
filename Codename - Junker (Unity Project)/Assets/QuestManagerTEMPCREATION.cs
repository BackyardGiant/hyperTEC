using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class QuestManagerTEMPCREATION : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        if (PlayerPrefs.GetInt("CreatedQuest") == 0)
        {
            DontDestroyOnLoad(this);
            PlayerPrefs.SetInt("CreatedQuest", 1);
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
