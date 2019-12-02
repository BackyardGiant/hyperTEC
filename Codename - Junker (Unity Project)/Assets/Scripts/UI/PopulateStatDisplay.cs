using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PopulateStatDisplay : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI m_stat1Title, m_stat2Title, m_stat3Title, m_stat4Title, m_stat1Value, m_stat2Value, m_stat3Value, m_stat4Value;

    public void PopulateWeapon(WeaponData _data)
    {
        m_stat1Title.text = "Damage";
        m_stat1Value.text = DisplayNiceStats(_data.Damage);
        m_stat2Title.text = "Fire Rate";
        m_stat2Value.text = DisplayNiceStats(_data.FireRate);
        //m_stat3Title.text = "Reload Time";
        //m_stat3Value.text = DisplayNiceStats(_data.ReloadTime);
        m_stat4Title.text = "";
        m_stat4Value.text = "";
        m_stat3Title.text = "Accuracy";
        m_stat3Value.text = DisplayNiceStats(_data.Accuracy);
    }
    public void PopulateEngine(EngineData _data)
    {
        m_stat1Title.text = "Top Speed";
        m_stat1Value.text = DisplayNiceStats(_data.TopSpeed);
        m_stat2Title.text = "Acceleration";
        m_stat2Value.text = DisplayNiceStats(_data.Acceleration);
        m_stat3Title.text = "Boost Power";
        m_stat3Value.text = DisplayNiceStats(_data.BoostPower);
        m_stat4Title.text = "Handling";
        m_stat4Value.text = DisplayNiceStats(_data.Handling);
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
