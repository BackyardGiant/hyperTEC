using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;

public class StartGameControls : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            PlayerPrefs.SetInt("RecordedSession", 1);
            PlayerPrefs.SetString("PlaysessionTime", DateTime.Now.ToString());
            SceneManager.LoadScene("TechDemo61119");    
        }
        else if (Input.GetKeyDown(KeyCode.P))
        {
            PlayerPrefs.SetInt("RecordedSession", 0);
            PlayerPrefs.SetString("PlaysessionTime", DateTime.Now.ToString());
            SceneManager.LoadScene("TechDemo61119");
        }  
    }
}
