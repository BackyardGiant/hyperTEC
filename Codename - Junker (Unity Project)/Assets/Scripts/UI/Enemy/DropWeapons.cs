using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropWeapons : MonoBehaviour
{
    public Transform leftSnap, rightSnap;

    private WeaponData m_weapon1, m_weapon2;

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
                GameObject _temp = ModuleManager.Instance.GenerateWeapon(m_weapon1);
                _temp.transform.position = leftSnap.position;
                _temp.transform.rotation = leftSnap.rotation;
                _temp.transform.localScale = new Vector3(2, 2, 2);
            }
            else
            {
                GameObject _temp = ModuleManager.Instance.GenerateWeapon(m_weapon2);
                _temp.transform.position = rightSnap.position;
                _temp.transform.rotation = rightSnap.rotation;
                _temp.transform.localScale = new Vector3(2, 2, 2);
            }

            Debug.Log(_randomFloat + "Dropped One");
        }
        else
        {

            GameObject _temp = ModuleManager.Instance.GenerateWeapon(m_weapon1);
            _temp.transform.position = leftSnap.position;
            _temp.transform.rotation = leftSnap.rotation;
            _temp.transform.localScale = new Vector3(2, 2, 2);

            _temp = ModuleManager.Instance.GenerateWeapon(m_weapon2);
            _temp.transform.position = rightSnap.position;
            _temp.transform.rotation = rightSnap.rotation;
            _temp.transform.localScale = new Vector3(2, 2, 2);

            Debug.Log(_randomFloat + "Dropped Two");
        }
    }
}
