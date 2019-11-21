using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class InventoryDisplayWeaponStats : MonoBehaviour
{

    [SerializeField]
    private TextMeshProUGUI m_damageAmount, m_fireAmount, m_reloadTime, m_accuracyAmount;


    // Start is called before the first frame update
    void Start()
    {
        WeaponData _dataBlock = this.GetComponent<WeaponStatManager>().Data;
        m_damageAmount.text = DisplayNiceStats(_dataBlock.Damage);
        m_fireAmount.text = DisplayNiceStats(_dataBlock.FireRate);
        m_reloadTime.text = DisplayNiceStats(_dataBlock.ReloadTime);
        m_accuracyAmount.text = DisplayNiceStats(_dataBlock.Accuracy);


    }


    private string DisplayNiceStats(float _value)
    {
        string _returnVal;
        float _percentage = _value / 100f;

        float _newValue = 100 + (899 * _percentage);
        _returnVal = ((int)_newValue).ToString();
        return _returnVal;
    }
}
