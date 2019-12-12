using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PopulateStatDisplay : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI m_stat1Title, m_stat2Title, m_stat3Title, m_stat4Title, m_stat1Value, m_stat2Value, m_stat3Value, m_stat4Value,m_equippedStat1, m_equippedStat2, m_equippedStat3, m_equippedStat4;

    public void PopulateWeapon(WeaponData _data)
    {
        string outcome;
        string left;
        string right;
        m_stat1Title.text = "Damage";
        m_stat1Value.text = DisplayNiceStats(_data.Damage);
        try{ left = "L:" + DisplayNiceStats(PlayerInventoryManager.Instance.EquippedLeftWeapon.Damage); } catch { left = ""; }
        try {right = " R:" + DisplayNiceStats(PlayerInventoryManager.Instance.EquippedRightWeapon.Damage);} catch { right = ""; }
        outcome = left + right;
        m_equippedStat1.text = outcome; 


        m_stat2Title.text = "Fire Rate";
        m_stat2Value.text = DisplayNiceStats(_data.FireRate);
        try { left = "L:" + DisplayNiceStats(PlayerInventoryManager.Instance.EquippedLeftWeapon.FireRate); } catch { left = ""; }
        try { right = " R:" + DisplayNiceStats(PlayerInventoryManager.Instance.EquippedRightWeapon.FireRate); } catch { right = ""; }
        outcome = left + right;
        m_equippedStat2.text = outcome;



        m_stat3Title.text = "Accuracy";
        m_stat3Value.text = DisplayNiceStats(_data.Accuracy);
        try { left = "L:" + DisplayNiceStats(PlayerInventoryManager.Instance.EquippedLeftWeapon.Accuracy); } catch { left = ""; }
        try { right = " R:" + DisplayNiceStats(PlayerInventoryManager.Instance.EquippedRightWeapon.Accuracy); } catch { right = ""; }
        outcome = left + right;
        m_equippedStat3.text = outcome;



        m_stat4Title.text = "";
        m_stat4Value.text = "";
        m_equippedStat4.text = "";
    }
    public void PopulateEngine(EngineData _data)
    {
        m_stat1Title.text = "Top Speed";
        m_stat1Value.text = DisplayNiceStats(_data.TopSpeed);
        try { m_equippedStat1.text = DisplayNiceStats(PlayerInventoryManager.Instance.EquippedEngine.TopSpeed); } catch { m_equippedStat1.text = ""; }
        m_stat2Title.text = "Acceleration";
        m_stat2Value.text = DisplayNiceStats(_data.Acceleration);
        try { m_equippedStat2.text = DisplayNiceStats(PlayerInventoryManager.Instance.EquippedEngine.Acceleration); } catch { m_equippedStat2.text = ""; }
        m_stat3Title.text = "Boost Power";
        m_stat3Value.text = DisplayNiceStats(_data.BoostPower);
        try{ m_equippedStat3.text = DisplayNiceStats(PlayerInventoryManager.Instance.EquippedEngine.BoostPower);} catch { m_equippedStat3.text = ""; }
        m_stat4Title.text = "Handling";
        m_stat4Value.text = DisplayNiceStats(_data.Handling);
        try{m_equippedStat4.text = DisplayNiceStats(PlayerInventoryManager.Instance.EquippedEngine.Handling); } catch { m_equippedStat4.text = ""; }
    }

    public void ClearEngine()
    {
        m_equippedStat1.text = "";
        m_equippedStat2.text = "";
        m_equippedStat3.text = "";
        m_equippedStat4.text = "";
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
