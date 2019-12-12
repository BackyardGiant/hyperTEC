﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropWeapons : MonoBehaviour
{
    public Transform leftSnap, rightSnap, engineSnap;

    public GameObject lootItemParent, lootItemParentEngine;

    private WeaponData m_weapon1, m_weapon2;
    private EngineData m_engine;

    private Vector3 m_defaultScale = new Vector3(1, 1, 1);
    private Vector3 m_defaultOffset = new Vector3(0.278f, -0.226f, -0.802f); // taken from eyeballing the offset in the inspector
    private Vector3 m_engineOffset = new Vector3(0.94f, 0.71f, 1.17f); // taken from eyeballing the offset in the inspector

    private void Start()
    {
        try
        {
            m_weapon1 = leftSnap.GetChild(0).GetComponent<WeaponGenerator>().statBlock;
            m_weapon2 = rightSnap.GetChild(0).GetComponent<WeaponGenerator>().statBlock;
            m_engine = engineSnap.GetChild(0).GetComponent<EngineGenerator>().engineStatBlock;
        }
        catch
        {

        }
    }

    public void Drop()
    {
        #region weapons
        float _randomFloat = Random.Range(0f, 1f);

        if (_randomFloat <= 0.3f)
        {
            Debug.Log(_randomFloat + "Dropped Nothing");
        }
        else if (_randomFloat > 0.3f && _randomFloat <= 0.9f)
        {
            if (Random.Range(0, 1) < 0.5f)
            {
                GameObject _lootItem = Instantiate(lootItemParent, leftSnap.position, leftSnap.rotation);
                GameObject _temp = ModuleManager.Instance.GenerateWeapon(m_weapon1);
                _temp.GetComponent<WeaponGenerator>().statBlock = m_weapon1;
                _temp.transform.SetParent(_lootItem.transform);

                _temp.transform.localPosition = m_defaultOffset;
                _temp.transform.localRotation = Quaternion.identity;

                _temp.transform.localScale = m_defaultScale;

                AddExplosionForce(_lootItem, leftSnap);
            }
            else
            {
                GameObject _lootItem = Instantiate(lootItemParent, rightSnap.position, rightSnap.rotation);
                GameObject _temp = ModuleManager.Instance.GenerateWeapon(m_weapon2);
                _temp.GetComponent<WeaponGenerator>().statBlock = m_weapon2;
                _temp.transform.SetParent(_lootItem.transform);

                _temp.transform.localPosition = m_defaultOffset;
                _temp.transform.localRotation = Quaternion.identity;

                _temp.transform.localScale = m_defaultScale;

                AddExplosionForce(_lootItem, rightSnap);
            }

            Debug.Log(_randomFloat + "Dropped One");
        }
        else
        {
            GameObject _lootItem = Instantiate(lootItemParent, leftSnap.position, leftSnap.rotation);
            GameObject _temp = ModuleManager.Instance.GenerateWeapon(m_weapon1);
            _temp.GetComponent<WeaponGenerator>().statBlock = m_weapon1;
            _temp.transform.SetParent(_lootItem.transform);
            _temp.transform.localPosition = m_defaultOffset;
            _temp.transform.localRotation = Quaternion.identity;

            _temp.transform.localScale = m_defaultScale;

            AddExplosionForce(_lootItem, leftSnap);

            _lootItem = Instantiate(lootItemParent, rightSnap.position, rightSnap.rotation);
            _temp = ModuleManager.Instance.GenerateWeapon(m_weapon2);
            _temp.GetComponent<WeaponGenerator>().statBlock = m_weapon2;
            _temp.transform.SetParent(_lootItem.transform);
            _temp.transform.localPosition = m_defaultOffset;
            _temp.transform.localRotation = Quaternion.identity;
            _temp.transform.localScale = m_defaultScale;

            AddExplosionForce(_lootItem, rightSnap);

            Debug.Log(_randomFloat + "Dropped Two");
        }
        #endregion

        #region engines
        _randomFloat = Random.Range(0f, 1f);

        if (_randomFloat <= 0.5f)
        {
            Debug.Log(_randomFloat + "Dropped Nothing");
        }
        else
        {
            GameObject _lootItem = Instantiate(lootItemParentEngine, engineSnap.position, engineSnap.rotation);
            GameObject _temp = ModuleManager.Instance.GenerateEngine(m_engine);
            _temp.GetComponent<EngineGenerator>().engineStatBlock = m_engine;
            _temp.transform.SetParent(_lootItem.transform);
            _temp.transform.localPosition = m_engineOffset;
            _temp.transform.localRotation = Quaternion.identity;

            _temp.transform.localScale = m_defaultScale;

            AddExplosionForce(_lootItem, engineSnap);

            Debug.Log(_randomFloat + "Dropped engine");
        }
        #endregion
    }

    public void DropWithoutChance()
    {
        try
        {
            m_weapon1 = leftSnap.GetChild(0).GetComponent<WeaponGenerator>().statBlock;
            m_weapon2 = rightSnap.GetChild(0).GetComponent<WeaponGenerator>().statBlock;
            m_engine = engineSnap.GetChild(0).GetComponent<EngineGenerator>().engineStatBlock;
        }
        catch
        {

        }

        try
        {
            GameObject _lootItem = Instantiate(lootItemParent, leftSnap.position, leftSnap.rotation);
            GameObject _temp = ModuleManager.Instance.GenerateWeapon(PlayerInventoryManager.Instance.EquippedLeftWeapon);
            _temp.GetComponent<WeaponGenerator>().statBlock = m_weapon1;
            _temp.transform.SetParent(_lootItem.transform);
            _temp.transform.localPosition = m_defaultOffset;
            _temp.transform.localRotation = Quaternion.identity;

            _temp.transform.localScale = m_defaultScale;

            AddExplosionForce(_lootItem, leftSnap);
        }
        catch { }
        try
        {
            GameObject _lootItem = Instantiate(lootItemParent, rightSnap.position, rightSnap.rotation);
            GameObject _temp = ModuleManager.Instance.GenerateWeapon(PlayerInventoryManager.Instance.EquippedRightWeapon);
            _temp.GetComponent<WeaponGenerator>().statBlock = m_weapon2;
            _temp.transform.SetParent(_lootItem.transform);
            _temp.transform.localPosition = m_defaultOffset;
            _temp.transform.localRotation = Quaternion.identity;
            _temp.transform.localScale = m_defaultScale;

            AddExplosionForce(_lootItem, rightSnap);
        }
        catch { }

        try
        {
            GameObject _lootItem = Instantiate(lootItemParentEngine, engineSnap.position, engineSnap.rotation);
            GameObject _temp = ModuleManager.Instance.GenerateEngine(m_engine);
            _temp.GetComponent<EngineGenerator>().engineStatBlock = m_engine;
            _temp.transform.SetParent(_lootItem.transform);
            _temp.transform.localPosition = m_engineOffset;
            _temp.transform.localRotation = Quaternion.identity;

            _temp.transform.localScale = m_defaultScale;

            AddExplosionForce(_lootItem, engineSnap);
        }
        catch { }
    }

    private void AddExplosionForce(GameObject _lootItem, Transform _snap)
    {
        _lootItem.GetComponent<Rigidbody>().AddForce(new Vector3(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f)) * Random.Range(1, 5) + (transform.forward * Random.Range(2f, 3f)), ForceMode.Impulse);
        _lootItem.GetComponent<Rigidbody>().AddTorque(new Vector3(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f)) * Random.Range(1, 5), ForceMode.Impulse);
    }
}
