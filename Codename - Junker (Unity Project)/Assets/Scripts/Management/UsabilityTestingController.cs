using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;
using System;

public class UsabilityTestingController : MonoBehaviour
{
    private enum computer{Computer1,Computer2,Computer3,Computer4};

    [SerializeField, Tooltip("The interval between sending the player position to the sheet")] private float m_playerPosInterval;
    [SerializeField] private Transform m_player;
    [SerializeField] private computer m_buildPC;
    [SerializeField] private bool m_sendPlayerPositionData;

    private Vector3 m_prevPlayerPosition;


    private string PLAYSESSION_URL = "https://docs.google.com/forms/u/1/d/e/1FAIpQLScBrxZm9Tb4MTqtnwyIic9fP4CN_7QamPPjJDnxNFvw946cXw/formResponse";
    private string PLAYERPOS_URL;
    private string PlayerPosType_Entry;
    private string PlayerPosition_Entry;

    private string COMPUTER1_URL = "https://docs.google.com/forms/u/2/d/e/1FAIpQLSdXzfNZEeoi2ThDHywfMp3C-MnATGl3RasfdDEu2PB75i-gZg/formResponse";
    private string Comp1PosTypeEntry = "entry.2142212131";
    private string Comp1PositionEntry = "entry.811631872";

    private string COMPUTER2_URL = "https://docs.google.com/forms/u/2/d/e/1FAIpQLSetNLwvnQDr9PxBd8Ez03VaeD4aJ9afvvkVJ7dGkFUUm5CCZA/formResponse";
    private string Comp2PosTypeEntry = "entry.2142212131";
    private string Comp2PositionEntry = "entry.811631872";

    private string COMPUTER3_URL = "https://docs.google.com/forms/u/2/d/e/1FAIpQLScqtLKR3mkET6yi5ZW3aPokyQV_aFfTwnTO0njQqUVywct0yw/formResponse";
    private string Comp3PosTypeEntry = "entry.2142212131";
    private string Comp3PositionEntry = "entry.811631872";

    private string COMPUTER4_URL = "https://docs.google.com/forms/u/2/d/e/1FAIpQLScoVTyAZn2N08v4lIuUhNsY99wTVD6xeNa5szophdWBYdVD9A/formResponse";
    private string Comp4PosTypeEntry = "entry.2142212131";
    private string Comp4PositionEntry = "entry.811631872";



    private void Start()
    {
        //Reset Everything when you die.
        if (PlayerPrefs.GetInt("LoadFromSave", 0) == 0)
        {
            PlayerPrefs.SetString("PlaysessionTime", DateTime.Now.ToString());
            PlayerPrefs.SetInt("EnemiesKilled", 0);
        }

        switch (m_buildPC)
        {
            case computer.Computer1:
                PLAYERPOS_URL = COMPUTER1_URL;
                PlayerPosType_Entry = Comp1PosTypeEntry;
                PlayerPosition_Entry = Comp1PositionEntry;
                break;
            case computer.Computer2:
                PLAYERPOS_URL = COMPUTER2_URL;
                PlayerPosType_Entry = Comp2PosTypeEntry;
                PlayerPosition_Entry = Comp2PositionEntry;
                break;
            case computer.Computer3:
                PLAYERPOS_URL = COMPUTER3_URL;
                PlayerPosType_Entry = Comp3PosTypeEntry;
                PlayerPosition_Entry = Comp3PositionEntry;
                break;
            case computer.Computer4:
                PLAYERPOS_URL = COMPUTER4_URL;
                PlayerPosType_Entry = Comp4PosTypeEntry;
                PlayerPosition_Entry = Comp4PositionEntry;
                break;
        }

        if(m_sendPlayerPositionData == true)
        {
            InvokeRepeating("retrievePlayerPos", 0.01f, m_playerPosInterval);
        }
    }

    #region Playsession Data 
    public void SavePlaysession()
    {
        Destroy(PlayerInventoryManager.Instance.gameObject);
        float _time = (float)(DateTime.Now - DateTime.Parse(PlayerPrefs.GetString("PlaysessionTime"))).TotalSeconds;
        StartCoroutine(PostPlaysession(_time,PlayerPrefs.GetInt("EnemiesKilled")));
    }
    IEnumerator PostPlaysession(float _time, int _enemiesKilled)
    {
        WWWForm form = new WWWForm();
        form.AddField("entry.2021506379", _time.ToString("F2"));
        form.AddField("entry.483301909", _enemiesKilled.ToString("F2"));
        byte[] rawData = form.data;
        WWW www = new WWW(PLAYSESSION_URL, rawData);
        yield return www;

    }
    #endregion


    #region PlayerPosition Data
    private void retrievePlayerPos()
    {
        Vector3 _playerPos = m_player.position;
        if (_playerPos != m_prevPlayerPosition)
        {
            StartCoroutine(PostPlayerPos("position", m_player.position.ToString()));
            m_prevPlayerPosition = _playerPos;
        }
        
    }
    public void sendCombatPos()
    {
        Vector3 _playerPos = m_player.position;
        if (_playerPos != m_prevPlayerPosition)
        {
            StartCoroutine(PostPlayerPos("combat", m_player.position.ToString()));
            m_prevPlayerPosition = _playerPos;
        }
    }

    IEnumerator PostPlayerPos(string _type, string _position)
    {
        WWWForm form = new WWWForm();
        form.AddField(PlayerPosType_Entry, _type);
        form.AddField(PlayerPosition_Entry, _position + ",");
        byte[] rawData = form.data;
        WWW www = new WWW(PLAYERPOS_URL, rawData);
        yield return www;
    }



    #endregion

}
