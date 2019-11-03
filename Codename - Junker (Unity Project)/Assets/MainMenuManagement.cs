using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManagement : MonoBehaviour
{

    public GameObject ControlSchemeMenu;
    public GameObject EngineChoiceMenu;
    public GameObject Engines;

    private int m_chosenEngine;

    private void Start()
    {
        m_chosenEngine = 0;
        ControlSchemeMenu.SetActive(true);
        EngineChoiceMenu.SetActive(false);
        Engines.SetActive(false);

    }

    public void ChooseControlScheme(int _choice)
    {
        if(_choice == 1)
        {
            Debug.Log("Control Scheme 1");
            PlayerPrefs.SetInt("ControlScheme",1);
        }
        else
        {
            Debug.Log("Control Scheme 2");
            PlayerPrefs.SetInt("ControlScheme",2);
        }
        ControlSchemeMenu.SetActive(false);
        EngineChoiceMenu.SetActive(true);
        Engines.SetActive(true);
    }
    public void ChooseEngine(int _choice)
    {
        m_chosenEngine = _choice;
        Debug.Log(m_chosenEngine);
    }
    public void ChooseEngineType(int _choice)
    {
        if (m_chosenEngine != 0)
        {
            if(_choice == 1)
            {
                PlayerPrefs.SetInt("ChosenEngineType", 1);
            }
            else
            {
                PlayerPrefs.SetInt("ChosenEngineType", 2);
            }
        }
    }
}
