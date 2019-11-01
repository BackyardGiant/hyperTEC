﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponGenerator : MonoBehaviour
{
    [Header("Snap Points")]
    public Transform BarrelSnap;
    public Transform BatterySnap;
    public Transform TargetingSnap;
    public Transform MagazineSnap;

    [Header("Statblock to Generate")]
    public WeaponData statBlock;

    [Tooltip("Note: Ticking the 'random' checkbox will ignore any component IDs in the statblock")]
    public bool isRandom;

    private Vector3 m_defaultVector = new Vector3(1, 1, 1);
    private Vector3 m_magRotation = new Vector3(0, 90, 0);

    private GameObject m_selectedBarrel, m_selectedMagazine, m_selectedBattery, m_selectedTargeting;

    private void Start()
    {
        //if(statBlock != null)
        //{
        //    GenerateGun();
        //}     
        //else
        //{
        //    Debug.Log("No statblock assigned");
        //}
    }

    [ContextMenu("Regenerate Weapon")]
    private void RegenerateGun()
    {
        ClearCurrentGun();
        GenerateGun();
    }

    public void GenerateGun()
    {
        m_selectedBarrel = ModuleManager.Instance.SelectBarrel(isRandom, statBlock);
        m_selectedBattery = ModuleManager.Instance.SelectBattery(isRandom, statBlock);
        m_selectedMagazine = ModuleManager.Instance.SelectMagazine(isRandom, statBlock);
        m_selectedTargeting = ModuleManager.Instance.SelectTargeting(isRandom, statBlock);

        InstantiateBarrel();
        InstantiateBattery();
        InstantiateMagazine();
        InstantiateTargeting();
    }

    private void ClearCurrentGun()
    {
        int _tempChildrenCount = BarrelSnap.transform.childCount;

        if (_tempChildrenCount != 0)
        {
            for (int i = 0; i < _tempChildrenCount; i++)
            {
                Destroy(BarrelSnap.transform.GetChild(i).gameObject);
            }
        }

        _tempChildrenCount = BatterySnap.transform.childCount;

        if (_tempChildrenCount != 0)
        {
            for (int i = 0; i < _tempChildrenCount; i++)
            {
                Destroy(BatterySnap.transform.GetChild(i).gameObject);
            }
        }

        _tempChildrenCount = MagazineSnap.transform.childCount;

        if (_tempChildrenCount != 0)
        {
            for (int i = 0; i < _tempChildrenCount; i++)
            {
                Destroy(MagazineSnap.transform.GetChild(i).gameObject);
            }
        }

        _tempChildrenCount = TargetingSnap.transform.childCount;

        if (_tempChildrenCount != 0)
        {
            for (int i = 0; i < _tempChildrenCount; i++)
            {
                Destroy(TargetingSnap.transform.GetChild(i).gameObject);
            }
        }
    }

    #region ComponentInstantiation

    private void InstantiateBarrel()
    {
        GameObject _tempBarrel = Instantiate(m_selectedBarrel, BarrelSnap);
        _tempBarrel.transform.localScale = m_defaultVector;
        _tempBarrel.transform.localPosition = Vector3.zero;
    }

    private void InstantiateMagazine()
    {
        GameObject _tempMagazine = Instantiate(m_selectedMagazine, MagazineSnap);
        _tempMagazine.transform.localScale = m_defaultVector;
        _tempMagazine.transform.localPosition = Vector3.zero;
        _tempMagazine.transform.localEulerAngles = m_magRotation;
    }

    private void InstantiateTargeting()
    {
        GameObject _tempTargeting = Instantiate(m_selectedTargeting, TargetingSnap);
        _tempTargeting.transform.localScale = m_defaultVector;
        _tempTargeting.transform.localPosition = Vector3.zero;
    }

    private void InstantiateBattery()
    {
        GameObject _tempBattery = Instantiate(m_selectedBattery, BatterySnap);
        _tempBattery.transform.localScale = m_defaultVector;
        _tempBattery.transform.localPosition = Vector3.zero;
    }



    #endregion
}
