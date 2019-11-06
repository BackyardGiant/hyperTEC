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

    private GameObject _tempComponent;

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

                _barrelSnap = _goTempGun.transform.Find("Body").Find("BarrelSnap");
                _magSnap = _goTempGun.transform.Find("Body").Find("MagSnap");
                _batterySnap = _goTempGun.transform.Find("Body").Find("BatterySnap");
                _targetSnap = _goTempGun.transform.Find("Body").Find("TargetSnap");

                InstantiateBarrel(traderBarrels[data.BarrelId], _barrelSnap);
                InstantiateBattery(traderBatteries[data.BatteryId], _batterySnap);
                InstantiateMagazine(traderMag[data.MagazineId], _magSnap);
                InstantiateTargeting(traderTargeting[data.TargetId], _targetSnap);

                return _goTempGun;

            case WeaponData.faction.construction:
                _goTempGun = Instantiate(goConstructionGunBase);

                _barrelSnap = _goTempGun.transform.Find("Body").Find("BarrelSnap");
                _magSnap = _goTempGun.transform.Find("Body").Find("MagSnap");
                _batterySnap = _goTempGun.transform.Find("Body").Find("BatterySnap");
                _targetSnap = _goTempGun.transform.Find("Body").Find("TargetSnap");

                InstantiateBarrel(constructBarrels[data.BarrelId], _barrelSnap);
                InstantiateBattery(constructBatteries[data.BatteryId], _batterySnap);
                InstantiateMagazine(constructMag[data.MagazineId], _magSnap);
                InstantiateTargeting(constructTargeting[data.TargetId], _targetSnap);

                return _goTempGun;

            case WeaponData.faction.explorer:
                _goTempGun = Instantiate(goExplorerGunBase);

                _barrelSnap = _goTempGun.transform.Find("Body").Find("BarrelSnap");
                _magSnap = _goTempGun.transform.Find("Body").Find("MagSnap");
                _batterySnap = _goTempGun.transform.Find("Body").Find("BatterySnap");
                _targetSnap = _goTempGun.transform.Find("Body").Find("TargetSnap");

                InstantiateBarrel(explorerBarrels[data.BarrelId], _barrelSnap);
                InstantiateBattery(explorerBatteries[data.BatteryId], _batterySnap);
                InstantiateMagazine(explorerMag[data.MagazineId], _magSnap);
                InstantiateTargeting(explorerTargeting[data.TargetId], _targetSnap);

                return _goTempGun;

            case WeaponData.faction.initial:
                _goTempGun = Instantiate(goInitialGunBase);

                _barrelSnap = _goTempGun.transform.Find("Body").Find("BarrelSnap");
                _magSnap = _goTempGun.transform.Find("Body").Find("MagSnap");
                _batterySnap = _goTempGun.transform.Find("Body").Find("BatterySnap");
                _targetSnap = _goTempGun.transform.Find("Body").Find("TargetSnap");

                InstantiateBarrel(defaultBarrels[data.BarrelId], _barrelSnap);
                InstantiateBattery(defaultBatteries[data.BatteryId], _batterySnap);
                InstantiateMagazine(defaultMag[data.MagazineId], _magSnap);
                InstantiateTargeting(defaultTargeting[data.TargetId], _targetSnap);

                return _goTempGun;

            default:
                return null;
        }
    }

    public WeaponData CreateStatBlock()
    {
        WeaponData _tempWeaponData = ScriptableObject.CreateInstance<WeaponData>();
  
        _tempWeaponData.Description = ""; // Desc gen here

        _tempWeaponData.CurrentFaction = (WeaponData.faction)Random.Range(0, 3);

        _tempWeaponData.FireRate = Random.Range(0, 100);

        _tempWeaponData.Accuracy = 100;
        _tempWeaponData.Damage = 100;
        _tempWeaponData.ReloadTime = 100;
        _tempWeaponData.Value = 100;

        switch(_tempWeaponData.CurrentFaction)
        {
            case WeaponData.faction.explorer:
                _tempWeaponData.Name = "Explorer Gun";

                _tempWeaponData.BarrelId = Random.Range(0, explorerBarrels.Count);
                _tempWeaponData.BatteryId = Random.Range(0, explorerBatteries.Count);
                _tempWeaponData.MagazineId = Random.Range(0, explorerMag.Count);
                _tempWeaponData.TargetId = Random.Range(0, explorerTargeting.Count);

                break;
            case WeaponData.faction.construction:
                _tempWeaponData.Name = "Construction Gun";

                _tempWeaponData.BarrelId = Random.Range(0, constructBarrels.Count);
                _tempWeaponData.BatteryId = Random.Range(0, constructBatteries.Count);
                _tempWeaponData.MagazineId = Random.Range(0, constructMag.Count);
                _tempWeaponData.TargetId = Random.Range(0, constructTargeting.Count);

                break;
            case WeaponData.faction.initial:
                _tempWeaponData.Name = "Default Gun";

                _tempWeaponData.BarrelId = Random.Range(0, defaultBarrels.Count);
                _tempWeaponData.BatteryId = Random.Range(0, defaultBatteries.Count);
                _tempWeaponData.MagazineId = Random.Range(0, defaultMag.Count);
                _tempWeaponData.TargetId = Random.Range(0, defaultTargeting.Count);

                break;
            case WeaponData.faction.trader:
                _tempWeaponData.Name = "Trader Gun";

                _tempWeaponData.BarrelId = Random.Range(0, traderBarrels.Count);
                _tempWeaponData.BatteryId = Random.Range(0, traderBatteries.Count);
                _tempWeaponData.MagazineId = Random.Range(0, traderMag.Count);
                _tempWeaponData.TargetId = Random.Range(0, traderTargeting.Count);

                break;
        }

        _tempWeaponData.name = "Weapon";
        return _tempWeaponData;
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

    #region ComponentSelection

    public GameObject SelectTargeting(bool _random, WeaponData _statBlock)
    {
        if (!_random)
        {
            switch (_statBlock.CurrentFaction)
            {
                case WeaponData.faction.construction:
                    _tempComponent = constructTargeting[_statBlock.TargetId];
                    break;
                case WeaponData.faction.explorer:
                    _tempComponent = explorerTargeting[_statBlock.TargetId];
                    break;
                case WeaponData.faction.initial:
                    _tempComponent = defaultTargeting[_statBlock.TargetId];
                    break;
                case WeaponData.faction.trader:
                    _tempComponent = traderTargeting[_statBlock.TargetId];
                    break;
            }
        }
        else
        {
            switch (_statBlock.CurrentFaction)
            {
                case WeaponData.faction.construction:
                    _tempComponent = constructTargeting[Random.Range(0, constructTargeting.Count)];
                    break;
                case WeaponData.faction.explorer:
                    _tempComponent = explorerTargeting[Random.Range(0, explorerTargeting.Count)];
                    break;
                case WeaponData.faction.initial:
                    _tempComponent = defaultTargeting[Random.Range(0, defaultTargeting.Count)];
                    break;
                case WeaponData.faction.trader:
                    _tempComponent = traderTargeting[Random.Range(0, traderTargeting.Count)];
                    break;
            }
        }

        return _tempComponent;
    }

    public GameObject SelectBarrel(bool _random, WeaponData _statBlock)
    {
        if (!_random)
        {
            switch (_statBlock.CurrentFaction)
            {
                case WeaponData.faction.construction:
                    _tempComponent = constructBarrels[_statBlock.BarrelId];
                    break;
                case WeaponData.faction.explorer:
                    _tempComponent = explorerBarrels[_statBlock.BarrelId];
                    break;
                case WeaponData.faction.initial:
                    _tempComponent = defaultBarrels[_statBlock.BarrelId];
                    break;
                case WeaponData.faction.trader:
                    _tempComponent = traderBarrels[_statBlock.BarrelId];
                    break;
            }
        }
        else
        {
            switch (_statBlock.CurrentFaction)
            {
                case WeaponData.faction.construction:
                    _tempComponent = constructBarrels[Random.Range(0, constructBarrels.Count)];
                    break;
                case WeaponData.faction.explorer:
                    _tempComponent = explorerBarrels[Random.Range(0, explorerBarrels.Count)];
                    break;
                case WeaponData.faction.initial:
                    _tempComponent = defaultBarrels[Random.Range(0, defaultBarrels.Count)];
                    break;
                case WeaponData.faction.trader:
                    _tempComponent = traderBarrels[Random.Range(0, traderBarrels.Count)];
                    break;
            }
        }

        return _tempComponent;
    }

    public GameObject SelectBattery(bool _random, WeaponData _statBlock)
    {
        if (!_random)
        {
            switch (_statBlock.CurrentFaction)
            {
                case WeaponData.faction.construction:
                    _tempComponent = constructBatteries[_statBlock.BatteryId];
                    break;
                case WeaponData.faction.explorer:
                    _tempComponent = explorerBatteries[_statBlock.BatteryId];
                    break;
                case WeaponData.faction.initial:
                    _tempComponent = defaultBatteries[_statBlock.BatteryId];
                    break;
                case WeaponData.faction.trader:
                    _tempComponent = traderBatteries[_statBlock.BatteryId];
                    break;
            }
        }
        else
        {
            switch (_statBlock.CurrentFaction)
            {
                case WeaponData.faction.construction:
                    _tempComponent = constructBatteries[Random.Range(0, constructBatteries.Count)];
                    break;
                case WeaponData.faction.explorer:
                    _tempComponent = explorerBatteries[Random.Range(0, explorerBatteries.Count)];
                    break;
                case WeaponData.faction.initial:
                    _tempComponent = defaultBatteries[Random.Range(0, defaultBatteries.Count)];
                    break;
                case WeaponData.faction.trader:
                    _tempComponent = traderBatteries[Random.Range(0, traderBatteries.Count)];
                    break;
            }
        }

        return _tempComponent;
    }

    public GameObject SelectMagazine(bool _random, WeaponData _statBlock)
    {
        if (!_random)
        {
            switch (_statBlock.CurrentFaction)
            {
                case WeaponData.faction.construction:
                    _tempComponent = constructMag[_statBlock.MagazineId];
                    break;
                case WeaponData.faction.explorer:
                    _tempComponent = explorerMag[_statBlock.MagazineId];
                    break;
                case WeaponData.faction.initial:
                    _tempComponent = defaultMag[_statBlock.MagazineId];
                    break;
                case WeaponData.faction.trader:
                    _tempComponent = traderMag[_statBlock.MagazineId];
                    break;
            }
        }
        else
        {
            switch (_statBlock.CurrentFaction)
            {
                case WeaponData.faction.construction:
                    _tempComponent = constructMag[Random.Range(0, constructMag.Count)];
                    break;
                case WeaponData.faction.explorer:
                    _tempComponent = explorerMag[Random.Range(0, explorerMag.Count)];
                    break;
                case WeaponData.faction.initial:
                    _tempComponent = defaultMag[Random.Range(0, defaultMag.Count)];
                    break;
                case WeaponData.faction.trader:
                    _tempComponent = traderMag[Random.Range(0, traderMag.Count)];
                    break;
            }
        }

        return _tempComponent;
    }

    #endregion




}
