﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField]
    private GameEvent m_playerDeath;

    [SerializeField]
    private IntVariable[] m_playerHealth = new IntVariable[4];

    [SerializeField]
    private int m_healthMax;

    [SerializeField]
    private float m_rechargeRate, m_rechargePeriod;

    private float m_timeSinceDamage;

    private int _currentSave;

    private bool _isRecharging = false;

    private void Start()
    {
        switch(PlayerPrefs.GetString("CurrentSave"))
        {
            case "Save1":
                _currentSave = 1;
                break;
            case "Save2":
                _currentSave = 2;
                break;
            case "Save3":
                _currentSave = 3;
                break;
            case "Save4":
                _currentSave = 4;
                break;
        }
    }

    private void Update()
    {
        if(m_timeSinceDamage > m_rechargePeriod && m_playerHealth[_currentSave].Value != m_healthMax)
        {
            m_playerHealth[_currentSave].Value += (int)m_rechargeRate;
        }

        m_timeSinceDamage += Time.deltaTime;
    }

    public void TakeDamage(int _dmg)
    {
        if (m_playerHealth[_currentSave].Value - _dmg > 0)
        {
            m_playerHealth[_currentSave].Value -= _dmg;
            m_timeSinceDamage = 0;
            Debug.Log("<color=green>CURRENT HEALTH : </color>" + m_playerHealth[_currentSave].Value);
        }
        else
        {
            m_playerDeath.Raise();
            Debug.Log("<color=green>DEAD</color>");
        }      
    }

    private void ResetHealth(int _save)
    {
        m_playerHealth[_save].Value = m_healthMax;
    }

    private void ResetHealth()
    {
        foreach(IntVariable healthBar in m_playerHealth)
        {
            healthBar.Value = m_healthMax;
        }
    }
}
