using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModuleManager : MonoBehaviour
{
    private static ModuleManager s_instance;

    public List<GameObject> explorerBarrels, explorerBatteries, explorerTargeting, explorerMag;
    public List<GameObject> defaultBarrels, defaultBatteries, defaultTargeting, defaultMag;
    public List<GameObject> traderBarrels, traderBatteries, traderTargeting, traderMag;
    public List<GameObject> constructBarrels, constructBatteries, constructTargeting, constructMag;

    public List<GameObject> explorerEngines, defaultEngines, traderEngines, constructEngine;

    public GameObject goExplorerGunBase, goInitialGunBase, goConstructionGunBase, goTraderGunBase;

    private Vector3 m_defaultVector = new Vector3(1, 1, 1);
    private Vector3 m_magRotation = new Vector3(0, 90, 0);

    public static ModuleManager Instance { get => s_instance; set => s_instance = value; }

    private void Awake()
    {
        if (s_instance == null)
        {
            s_instance = this;
        }
        else
        {
            Destroy(s_instance.gameObject);
            s_instance = this;
        }
    }

    public GameObject GenerateEngine(EngineData data)
    {
        switch(data.CurrentFaction)
        {
            case EngineData.faction.trader:
                return traderEngines[data.EngineId];
            case EngineData.faction.construction:
                return constructEngine[data.EngineId];
            case EngineData.faction.explorer:
                return explorerEngines[data.EngineId];
            case EngineData.faction.initial:
                return defaultEngines[data.EngineId];
            default:
                return null;
        }
    }

    public GameObject GenerateWeapon(WeaponData data)
    {
        GameObject _goTempGun;
        Transform _barrelSnap, _batterySnap, _magSnap, _targetSnap;

        switch (data.CurrentFaction)
        {
            case WeaponData.faction.trader:
                _goTempGun = Instantiate(goTraderGunBase);

                _barrelSnap = _goTempGun.transform.Find("BarrelSnap");
                _magSnap = _goTempGun.transform.Find("MagSnap");
                _batterySnap = _goTempGun.transform.Find("BatterySnap");
                _targetSnap = _goTempGun.transform.Find("TargetSnap");

                InstantiateBarrel(traderBarrels[data.BarrelId], _barrelSnap);
                InstantiateBattery(traderBatteries[data.BatteryId], _batterySnap);
                InstantiateMagazine(traderMag[data.MagazineId], _magSnap);
                InstantiateTargeting(traderTargeting[data.TargetId], _targetSnap);

                return _goTempGun;

            case WeaponData.faction.construction:
                _goTempGun = Instantiate(goConstructionGunBase);

                _barrelSnap = _goTempGun.transform.Find("BarrelSnap");
                _magSnap = _goTempGun.transform.Find("MagSnap");
                _batterySnap = _goTempGun.transform.Find("BatterySnap");
                _targetSnap = _goTempGun.transform.Find("TargetSnap");

                InstantiateBarrel(constructBarrels[data.BarrelId], _barrelSnap);
                InstantiateBattery(constructBatteries[data.BatteryId], _batterySnap);
                InstantiateMagazine(constructMag[data.MagazineId], _magSnap);
                InstantiateTargeting(constructTargeting[data.TargetId], _targetSnap);

                return _goTempGun;

            case WeaponData.faction.explorer:
                _goTempGun = Instantiate(goExplorerGunBase);

                _barrelSnap = _goTempGun.transform.Find("BarrelSnap");
                _magSnap = _goTempGun.transform.Find("MagSnap");
                _batterySnap = _goTempGun.transform.Find("BatterySnap");
                _targetSnap = _goTempGun.transform.Find("TargetSnap");

                InstantiateBarrel(explorerBarrels[data.BarrelId], _barrelSnap);
                InstantiateBattery(explorerBatteries[data.BatteryId], _batterySnap);
                InstantiateMagazine(explorerMag[data.MagazineId], _magSnap);
                InstantiateTargeting(explorerTargeting[data.TargetId], _targetSnap);

                return _goTempGun;

            case WeaponData.faction.initial:
                _goTempGun = Instantiate(goInitialGunBase);

                _barrelSnap = _goTempGun.transform.Find("BarrelSnap");
                _magSnap = _goTempGun.transform.Find("MagSnap");
                _batterySnap = _goTempGun.transform.Find("BatterySnap");
                _targetSnap = _goTempGun.transform.Find("TargetSnap");

                InstantiateBarrel(defaultBarrels[data.BarrelId], _barrelSnap);
                InstantiateBattery(defaultBatteries[data.BatteryId], _batterySnap);
                InstantiateMagazine(defaultMag[data.MagazineId], _magSnap);
                InstantiateTargeting(defaultTargeting[data.TargetId], _targetSnap);

                return _goTempGun;

            default:
                return null;
        }
    }

    #region ComponentInstantiation

    private void InstantiateBarrel(GameObject _barrel, Transform _barrelSnap)
    {
        GameObject _tempBarrel = Instantiate(_barrel, _barrelSnap);
        _tempBarrel.transform.localScale = m_defaultVector;
        _tempBarrel.transform.localPosition = Vector3.zero;
    }

    private void InstantiateMagazine(GameObject _mag, Transform _magSnap)
    {
        GameObject _tempMagazine = Instantiate(_mag, _magSnap);
        _tempMagazine.transform.localScale = m_defaultVector;
        _tempMagazine.transform.localPosition = Vector3.zero;
        _tempMagazine.transform.localEulerAngles = m_magRotation;
    }

    private void InstantiateTargeting(GameObject _target, Transform _targetSnap)
    {
        GameObject _tempTargeting = Instantiate(_target, _targetSnap);
        _tempTargeting.transform.localScale = m_defaultVector;
        _tempTargeting.transform.localPosition = Vector3.zero;
    }

    private void InstantiateBattery(GameObject _battery, Transform _batterySnap)
    {
        GameObject _tempBattery = Instantiate(_battery, _batterySnap);
        _tempBattery.transform.localScale = m_defaultVector;
        _tempBattery.transform.localPosition = Vector3.zero;
    }



    #endregion




}
