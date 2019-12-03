using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;
using System;

public class EndPlaysessionController : MonoBehaviour
{
    private string BASE_URL = "https://docs.google.com/forms/u/1/d/e/1FAIpQLScBrxZm9Tb4MTqtnwyIic9fP4CN_7QamPPjJDnxNFvw946cXw/formResponse";

    private void Start()
    {
        if (PlayerPrefs.GetInt("LoadFromSave", 0) == 0)
        {
            PlayerPrefs.SetString("PlaysessionTime", DateTime.Now.ToString());
            PlayerPrefs.SetInt("EnemiesKilled", 0);
        }
    }

    public void SaveValues()
    {
        Destroy(PlayerInventoryManager.Instance.gameObject);
        float _time = (float)(DateTime.Now - DateTime.Parse(PlayerPrefs.GetString("PlaysessionTime"))).TotalSeconds;
        StartCoroutine(Post(_time,PlayerPrefs.GetInt("EnemiesKilled")));
    }


    IEnumerator Post(float _time, int _enemiesKilled)
    {
        WWWForm form = new WWWForm();
        form.AddField("entry.2021506379", _time.ToString("F2"));
        form.AddField("entry.483301909", _enemiesKilled.ToString("F2"));
        byte[] rawData = form.data;
        WWW www = new WWW(BASE_URL, rawData);
        yield return www;

    }
}
