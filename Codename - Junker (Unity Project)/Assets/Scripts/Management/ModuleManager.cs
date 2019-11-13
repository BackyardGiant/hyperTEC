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
        data.Seed = data.EngineId.ToString();
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
        string _seed = GenerateSeed();

        WeaponData _tempWeaponData = ScriptableObject.CreateInstance<WeaponData>();

        _tempWeaponData.Seed = _seed;
  
        _tempWeaponData.Description = ""; // Desc gen here

        _tempWeaponData.CurrentFaction = (WeaponData.faction)int.Parse(_seed[0].ToString());

        _tempWeaponData.FireRate = float.Parse(_seed.Substring(1,3));

        _tempWeaponData.CurrentFireRateType = (WeaponData.fireRateType)int.Parse(_seed[23].ToString());

        _tempWeaponData.FireRateIndex = int.Parse(_seed.Substring(20, 3));

        _tempWeaponData.Accuracy = float.Parse(_seed.Substring(4, 3));
        _tempWeaponData.Damage = float.Parse(_seed.Substring(7, 3));
        _tempWeaponData.ReloadTime = float.Parse(_seed.Substring(10, 3));
        _tempWeaponData.Value = float.Parse(_seed.Substring(13, 3));

        switch(_tempWeaponData.CurrentFaction)
        {
            case WeaponData.faction.explorer:
                _tempWeaponData.Name = "Explorer Gun";

                _tempWeaponData.BarrelId = int.Parse(_seed[16].ToString());
                _tempWeaponData.BatteryId = int.Parse(_seed[17].ToString());
                _tempWeaponData.MagazineId = int.Parse(_seed[18].ToString());
                _tempWeaponData.TargetId = int.Parse(_seed[19].ToString());

                break;
            case WeaponData.faction.construction:
                _tempWeaponData.Name = "Construction Gun";

                _tempWeaponData.BarrelId = int.Parse(_seed[16].ToString());
                _tempWeaponData.BatteryId = int.Parse(_seed[17].ToString());
                _tempWeaponData.MagazineId = int.Parse(_seed[18].ToString());
                _tempWeaponData.TargetId = int.Parse(_seed[19].ToString());

                break;
            case WeaponData.faction.initial:
                _tempWeaponData.Name = "Default Gun";

                _tempWeaponData.BarrelId = int.Parse(_seed[16].ToString());
                _tempWeaponData.BatteryId = int.Parse(_seed[17].ToString());
                _tempWeaponData.MagazineId = int.Parse(_seed[18].ToString());
                _tempWeaponData.TargetId = int.Parse(_seed[19].ToString());

                break;
            case WeaponData.faction.trader:
                _tempWeaponData.Name = "Trader Gun";

                _tempWeaponData.BarrelId = int.Parse(_seed[16].ToString());
                _tempWeaponData.BatteryId = int.Parse(_seed[17].ToString());
                _tempWeaponData.MagazineId = int.Parse(_seed[18].ToString());
                _tempWeaponData.TargetId = int.Parse(_seed[19].ToString());

                break;
        }

        _tempWeaponData.name = "Weapon";
        return _tempWeaponData;
    }

    public WeaponData CreateStatBlock(string _seed)
    {
        WeaponData _tempWeaponData = ScriptableObject.CreateInstance<WeaponData>();

        _tempWeaponData.Seed = _seed;

        _tempWeaponData.Description = ""; // Desc gen here

        _tempWeaponData.CurrentFaction = (WeaponData.faction)int.Parse(_seed[0].ToString());

        _tempWeaponData.FireRate = float.Parse(_seed.Substring(1, 3));

        _tempWeaponData.CurrentFireRateType = (WeaponData.fireRateType)int.Parse(_seed[23].ToString());

        _tempWeaponData.FireRateIndex = int.Parse(_seed.Substring(20, 3));

        _tempWeaponData.Accuracy = float.Parse(_seed.Substring(4, 3));
        _tempWeaponData.Damage = float.Parse(_seed.Substring(7, 3));
        _tempWeaponData.ReloadTime = float.Parse(_seed.Substring(10, 3));
        _tempWeaponData.Value = float.Parse(_seed.Substring(13, 3));

        switch (_tempWeaponData.CurrentFaction)
        {
            case WeaponData.faction.explorer:
                _tempWeaponData.Name = "Explorer Gun";

                _tempWeaponData.BarrelId = int.Parse(_seed[16].ToString());
                _tempWeaponData.BatteryId = int.Parse(_seed[17].ToString());
                _tempWeaponData.MagazineId = int.Parse(_seed[18].ToString());
                _tempWeaponData.TargetId = int.Parse(_seed[19].ToString());

                break;
            case WeaponData.faction.construction:
                _tempWeaponData.Name = "Construction Gun";

                _tempWeaponData.BarrelId = int.Parse(_seed[16].ToString());
                _tempWeaponData.BatteryId = int.Parse(_seed[17].ToString());
                _tempWeaponData.MagazineId = int.Parse(_seed[18].ToString());
                _tempWeaponData.TargetId = int.Parse(_seed[19].ToString());

                break;
            case WeaponData.faction.initial:
                _tempWeaponData.Name = "Default Gun";

                _tempWeaponData.BarrelId = int.Parse(_seed[16].ToString());
                _tempWeaponData.BatteryId = int.Parse(_seed[17].ToString());
                _tempWeaponData.MagazineId = int.Parse(_seed[18].ToString());
                _tempWeaponData.TargetId = int.Parse(_seed[19].ToString());

                break;
            case WeaponData.faction.trader:
                _tempWeaponData.Name = "Trader Gun";

                _tempWeaponData.BarrelId = int.Parse(_seed[16].ToString());
                _tempWeaponData.BatteryId = int.Parse(_seed[17].ToString());
                _tempWeaponData.MagazineId = int.Parse(_seed[18].ToString());
                _tempWeaponData.TargetId = int.Parse(_seed[19].ToString());

                break;
        }

        _tempWeaponData.name = "Weapon";
        return _tempWeaponData;
    }

    private string GenerateSeed()
    {
        string _seed = "";

        int _barrelId = 0;
        int _batteryId = 0;
        int _magazineId = 0;
        int _targetId = 0;

        float _fireRate = Random.Range(1, 100);
        string _fireRateString;

        int _fireRateTypeData;

        if (_fireRate <= 20)
        {
            _fireRateTypeData = 0;
        }
        else if(_fireRate > 20 && _fireRate <= 80)
        {
            _fireRateTypeData = 1;
        }
        else
        {
            _fireRateTypeData = 2;
        }

        WeaponData.fireRateType _fireRateType = (WeaponData.fireRateType)_fireRateTypeData;

        int _fireSoundIndex = 0;

        switch(_fireRateType)
        {
            case WeaponData.fireRateType.fast:
                _fireSoundIndex = Random.Range(0, AudioManager.Instance.shortWeaponSounds.Length-1);
                break;
            case WeaponData.fireRateType.medium:
                _fireSoundIndex = Random.Range(0, AudioManager.Instance.mediumWeaponSounds.Length-1);
                break;
            case WeaponData.fireRateType.slow:
                _fireSoundIndex = Random.Range(0, AudioManager.Instance.longWeaponSounds.Length-1);
                break;
        }

        string _fireSoundIndexString = _fireSoundIndex.ToString();

        float _accuracy = Random.Range(1, 100);
        string _accuracyString;
        float _damage = Random.Range(1, 100);
        string _damageString;
        float _reloadTime = Random.Range(1, 100);
        string _reloadTimeString;
        float _value = Random.Range(1, 100); 
        string _valueString;
        int _factionData = Random.Range(0, 4);
        WeaponData.faction _faction = (WeaponData.faction)_factionData;

        switch (_faction)
        {
            case WeaponData.faction.explorer:
                _barrelId = Random.Range(0, explorerBarrels.Count);
                _batteryId = Random.Range(0, explorerBatteries.Count);
                _magazineId = Random.Range(0, explorerMag.Count);
                _targetId = Random.Range(0, explorerTargeting.Count);
                break;
            case WeaponData.faction.construction:
                _barrelId = Random.Range(0, constructBarrels.Count);
                _batteryId = Random.Range(0, constructBatteries.Count);
                _magazineId = Random.Range(0, constructMag.Count);
                _targetId = Random.Range(0, constructTargeting.Count);
                break;
            case WeaponData.faction.initial:
                _barrelId = Random.Range(0, defaultBarrels.Count);
                _batteryId = Random.Range(0, defaultBatteries.Count);
                _magazineId = Random.Range(0, defaultMag.Count);
                _targetId = Random.Range(0, defaultTargeting.Count);
                break;
            case WeaponData.faction.trader:
                _barrelId = Random.Range(0, traderBarrels.Count);
                _batteryId = Random.Range(0, traderBatteries.Count);
                _magazineId = Random.Range(0, traderMag.Count);
                _targetId = Random.Range(0, traderTargeting.Count);
                break;
        }

        #region Creating corecctly formated string
        if (_fireRate < 10)
        {
            _fireRateString = "00" + _fireRate;
        }
        else if(_fireRate < 100)
        {
            _fireRateString = "0" + _fireRate;
        }
        else
        {
            _fireRateString = _fireRate.ToString();
        }

        if (_accuracy < 10)
        {
            _accuracyString = "00" + _accuracy;
        }
        else if (_accuracy < 100)
        {
            _accuracyString = "0" + _accuracy;
        }
        else
        {
            _accuracyString = _accuracy.ToString();
        }

        if (_damage < 10)
        {
            _damageString = "00" + _damage;
        }
        else if (_damage < 100)
        {
            _damageString = "0" + _damage;
        }
        else
        {
            _damageString = _damage.ToString();
        }

        if (_reloadTime < 10)
        {
            _reloadTimeString = "00" + _reloadTime;
        }
        else if (_reloadTime < 100)
        {
            _reloadTimeString = "0" + _reloadTime;
        }
        else
        {
            _reloadTimeString = _reloadTime.ToString();
        }

        if (_value < 10)
        {
            _valueString = "00" + _value;
        }
        else if (_value < 100)
        {
            _valueString = "0" + _value;
        }
        else
        {
            _valueString = _value.ToString();
        }

        if (_fireSoundIndex < 10)
        {
            _fireSoundIndexString = "00" + _fireSoundIndex;
        }
        else if (_fireSoundIndex < 100)
        {
            _fireSoundIndexString = "0" + _fireSoundIndex;
        }
        else
        {
            _fireSoundIndexString = _fireSoundIndex.ToString();
        }
        #endregion

        _seed = _factionData.ToString() + _fireRateString + _accuracyString + _damageString + _reloadTimeString + _valueString + _barrelId.ToString() + _batteryId.ToString() + _magazineId.ToString() + _targetId.ToString() + _fireSoundIndexString + _fireRateTypeData.ToString(); 

        return _seed;
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
