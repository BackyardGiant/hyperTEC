using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;
using System;

public class EndPlaysessionController : MonoBehaviour
{
    public GameObject QtoQuit;
    public GameObject PtoQuit;

    public Inventory playerInv;
    public WeaponData default1, default2;

    private string BASE_URL = "https://docs.google.com/forms/u/2/d/e/1FAIpQLSciZFR9V7bROx3Rp_qYcxTFKPyAruDc3Vu_jrc4JCekBiHNLg/formResponse";

    private void Start()
    {
        QtoQuit.SetActive(false);
        PtoQuit.SetActive(false);

        if (PlayerPrefs.GetInt("RecordedSession") == 1)
        {
            QtoQuit.SetActive(true);
        }
        else
        {
            PtoQuit.SetActive(true);
        } 
    }
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            SaveValues();
            SceneManager.LoadScene("MainMenu");
        }
        else if (Input.GetKeyDown(KeyCode.Q))
        {
            SaveValues();
            SceneManager.LoadScene("MainMenu");
        }
    }

    private void SaveValues()
    {
        playerInv.AvailableWeapons.Clear();
        playerInv.AvailableWeapons.Add(default1);
        playerInv.AvailableWeapons.Add(default2);
        playerInv.EquippedLeftWeapon = default1;
        playerInv.EquippedRightWeapon = default2;
        playerInv.EquippedLeftIndex = 0;
        playerInv.EquippedRightIndex = 1;

        float _time = (float)(DateTime.Now - DateTime.Parse(PlayerPrefs.GetString("PlaysessionTime"))).TotalSeconds;
        StartCoroutine(Post(_time, PlayerPrefs.GetInt("RecordedSession"),PlayerPrefs.GetInt("ControlScheme"),PlayerPrefs.GetInt("ChosenEngine"),PlayerPrefs.GetInt("ChosenEngineType"),PlayerPrefs.GetInt("EnemiesKilled"),PlayerPrefs.GetInt("BoostedCount"),PlayerPrefs.GetInt("WeaponsCollected"),PlayerPrefs.GetInt("WeaponsScanned"),PlayerPrefs.GetInt("WeaponsDestroyed")));
    }


    IEnumerator Post(float _time, int _recorded, int _controlScheme, int _chosenEngine, int _chosenEngineType, int _enemiesKilled, int _boostedCount, int _weaponsCollected, int _weaponsScanned, int _weaponsDestroyed)
    {
        WWWForm form = new WWWForm();
        form.AddField("entry.801154399", _time.ToString("F2"));
        form.AddField("entry.1715930", _recorded.ToString("F2"));
        form.AddField("entry.885755587", _controlScheme.ToString("F2"));
        form.AddField("entry.535324631", _chosenEngine.ToString("F2"));
        form.AddField("entry.873187473", _chosenEngineType.ToString("F2"));
        form.AddField("entry.527017224", _enemiesKilled.ToString("F2"));
        form.AddField("entry.82752934", _boostedCount.ToString("F2"));
        form.AddField("entry.1280276683", _weaponsCollected.ToString("F2"));
        form.AddField("entry.473311432", _weaponsScanned.ToString("F2"));
        form.AddField("entry.770706336", _weaponsDestroyed.ToString("F2"));
        byte[] rawData = form.data;
        WWW www = new WWW(BASE_URL, rawData);
        yield return www;

    }
}
