using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManagement : MonoBehaviour
{

    //public GameObject ControlSchemeMenu;
    //public GameObject EngineChoiceMenu;
    //public GameObject Engines;
    //public Animator engineWarning;
    //public GameObject StartGameMenu;

    //private int m_chosenEngine;

    //private void Start()
    //{
    //    m_chosenEngine = 0;
    //    ControlSchemeMenu.SetActive(true);
    //    EngineChoiceMenu.SetActive(false);
    //    Engines.SetActive(false);
    //    StartGameMenu.SetActive(false);

    //    InstantiatePlayerPrefs();

    //}



    //private void InstantiatePlayerPrefs()
    //{
    //    PlayerPrefs.SetString("PlaysessionTime","ERROR");
    //    PlayerPrefs.SetInt("RecordedSession",0);
    //    PlayerPrefs.SetInt("ControlScheme", 0);
    //    PlayerPrefs.SetInt("ChosenEngine", 0);
    //    PlayerPrefs.SetInt("ChosenEngineType", 0);
    //    PlayerPrefs.SetInt("EnemiesKilled", 0);
    //    PlayerPrefs.SetInt("BoostedCount", 0);
    //    PlayerPrefs.SetInt("WeaponsCollected", 0);
    //    PlayerPrefs.SetInt("WeaponsScanned", 0);
    //    PlayerPrefs.SetInt("WeaponsDestroyed", 0);
    //}
    //public void ChooseControlScheme(int _choice)
    //{
    //    if(_choice == 1)
    //    {
    //        Debug.Log("Control Scheme 1");
    //        PlayerPrefs.SetInt("ControlScheme",1);
    //    }
    //    else
    //    {
    //        Debug.Log("Control Scheme 2");
    //        PlayerPrefs.SetInt("ControlScheme",2);
    //    }
    //    ControlSchemeMenu.SetActive(false);
    //    EngineChoiceMenu.SetActive(true);
    //    Engines.SetActive(true);
    //}
    //public void ChooseEngine(int _choice)
    //{
    //    m_chosenEngine = _choice;
    //    PlayerPrefs.SetInt("ChosenEngine", _choice);
    //}
    //public void ChooseEngineType(int _choice)
    //{
    //    if (m_chosenEngine != 0)
    //    {
    //        if(_choice == 1)
    //        {
    //            PlayerPrefs.SetInt("ChosenEngineType", 1);
    //            EngineChoiceMenu.SetActive(false);
    //            Engines.SetActive(false);
    //            StartGameMenu.SetActive(true);
    //        }
    //        else
    //        {
    //            PlayerPrefs.SetInt("ChosenEngineType", 2);
    //            EngineChoiceMenu.SetActive(false);
    //            Engines.SetActive(false);
    //            StartGameMenu.SetActive(true);
    //        }
    //    }
    //    else
    //    {
    //        engineWarning.Play("SelectAnEngine");
    //    }
    //}
}
