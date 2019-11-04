﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropWeapons : MonoBehaviour
{
    public Transform leftSnap, rightSnap;

    public GameObject lootItemParent;

    private WeaponData m_weapon1, m_weapon2;

    private Vector3 m_defaultScale = new Vector3(1, 1, 1);

    private void Start()
    {
        m_weapon1 = leftSnap.GetChild(0).GetComponent<WeaponGenerator>().statBlock;
        m_weapon2 = rightSnap.GetChild(0).GetComponent<WeaponGenerator>().statBlock;
    }

    public void Drop()
    {
        float _randomFloat = Random.Range(0f, 1f);

        if(_randomFloat <= 0.5f)
        {
            Debug.Log(_randomFloat + "Dropped Nothing");
        }
        else if(_randomFloat > 0.5f && _randomFloat <= 0.9f)
        {
            if(Random.Range(0, 1) < 0.5f)
            {
                GameObject _lootItem = Instantiate(lootItemParent, leftSnap.position, leftSnap.rotation);
                GameObject _temp = ModuleManager.Instance.GenerateWeapon(m_weapon1);
                _temp.transform.SetParent(_lootItem.transform);

                _temp.transform.localPosition = Vector3.zero;
                _temp.transform.localRotation = Quaternion.identity;

                _temp.transform.localScale = m_defaultScale;
            }
            else
            {
                GameObject _lootItem = Instantiate(lootItemParent, rightSnap.position, rightSnap.rotation);
                GameObject _temp = ModuleManager.Instance.GenerateWeapon(m_weapon2);

                _temp.transform.localPosition = Vector3.zero;
                _temp.transform.localRotation = Quaternion.identity;

                _temp.transform.localScale = m_defaultScale;
            }

            Debug.Log(_randomFloat + "Dropped One");
        }
        else
        {
            GameObject _lootItem = Instantiate(lootItemParent, leftSnap.position, leftSnap.rotation);
            GameObject _temp = ModuleManager.Instance.GenerateWeapon(m_weapon1);
            _temp.transform.localPosition = Vector3.zero;
            _temp.transform.localRotation = Quaternion.identity;

            _temp.transform.localScale = m_defaultScale;

            _lootItem = Instantiate(lootItemParent, rightSnap.position, rightSnap.rotation);
            _temp = ModuleManager.Instance.GenerateWeapon(m_weapon2);

            _temp.transform.localPosition = Vector3.zero;
            _temp.transform.localRotation = Quaternion.identity;
            _temp.transform.localScale = m_defaultScale;

            Debug.Log(_randomFloat + "Dropped Two");
        }
    }
}
