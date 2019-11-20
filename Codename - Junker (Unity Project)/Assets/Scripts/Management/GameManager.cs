﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    private static GameManager s_instance;

    public GameEvent preSlowDown;
    public GameEvent postSlowDown;
    public GameEvent normalSpeed;
    public TempEnemyInstantiate spawner;
    public GameObject lootTemplate;
    public Image blackOut;
    public Animator fadeAnimator;
    
    private int m_enemiesKilledSoFar;

    private Vector3 m_defaultScale = new Vector3(1, 1, 1);
    private Vector3 m_defaultOffset = new Vector3(0.278f, -0.226f, -0.802f); // taken from eyeballing the offset in the inspector

    private Vector3 m_scale = new Vector3(1, 1, 1);

    [SerializeField, Range(0,1), Tooltip("The speed of objects in the game on a scale of 0-1")]
    private float m_gameSpeed;

    private float m_passedSpeed;

    public static GameManager Instance { get => s_instance; set => s_instance = value; }
    public float GameSpeed { get => m_gameSpeed; private set => m_gameSpeed = value; }
    public int EnemiesKilledSoFar { get => m_enemiesKilledSoFar; set => m_enemiesKilledSoFar = value; }

    private void Awake()
    {
        if(s_instance == null)
        {
            s_instance = this;
        }
        else
        {
            Destroy(s_instance.gameObject);
            s_instance = this;
        }
    }

    private void Start()
    {
        Scene _currentScene = SceneManager.GetActiveScene();
        string _sceneName = _currentScene.name;

        if (_sceneName == "MainScene" && PlayerPrefs.GetInt("LoadFromSave", 0) == 1)
        {
            LoadInto();
        }
        else if(_sceneName == "MainScene")
        {
            LoadInto();
        }

        Debug.Log("<color=red>CURRENT SAVE: </color>" + PlayerPrefs.GetString("CurrentSave"));
        Debug.Log("<color=red>LATEST SAVE: </color>" + PlayerPrefs.GetString("LatestSave"));
    }

    private void LoadInto()
    {
        PlayerPrefs.SetInt("LoadFromSave", 0);
        spawner = GameObject.FindGameObjectWithTag("Spawner").GetComponent<TempEnemyInstantiate>();
        Invoke("LoadGame", 0.6f);
        fadeAnimator.Play("Fade");
    }

    private void LoadOut()
    {
        SaveGame();
        PlayerPrefs.SetInt("LoadFromSave", 1);
        SceneManager.LoadScene("ModularShip");
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F1))
        {
            PlayerPrefs.DeleteAll();
        }

        Scene _currentScene = SceneManager.GetActiveScene();
        string _sceneName = _currentScene.name;

        if (Input.GetButtonDown("Inventory") && _sceneName == "MainScene")
        {
            fadeAnimator.Play("FadeOut");
            Invoke("LoadOut",1.3f);
        }
        else if(Input.GetButtonDown("Inventory") && _sceneName == "ModularShip")
        {
            SaveWeapons();
            SceneManager.LoadScene("MainScene");
        }

        if (Input.GetKeyDown(KeyCode.F2))
        {
            SaveGame();
        }
        if (Input.GetKeyDown(KeyCode.F3))
        {
            LoadGame();
        }
    }

    public void SetSlowMo(float _newSpeed)
    {
        m_passedSpeed = _newSpeed;
        if (m_gameSpeed == 1f)
        {
            preSlowDown.Raise();
            StartCoroutine("waitForStuff");
        }
        else
        {
            SetNormalSpeed();
            SetSlowMo(_newSpeed);
        }
    }

    public void SetNormalSpeed()
    {
        m_gameSpeed = 1f;
        normalSpeed.Raise();
    }

    private IEnumerator waitForStuff()
    {
        yield return new WaitForSeconds(0.1f);

        m_gameSpeed = m_passedSpeed;
        postSlowDown.Raise();
    }

    public void ResetScene()
    {
        MovementUsabilityTestingManager.Instance.SaveValues();
        SceneManager.LoadScene("UserTesting");
    }

    public void EnemyKilled()
    {
        m_enemiesKilledSoFar++;
    }

    public void SaveWeapons()
    {
        string _fileName = PlayerPrefs.GetString("LatestSave", "NoSave");

        if (_fileName == "NoSave")
        {
            return;
        }

        try
        {
            string[] _lines = File.ReadAllLines(_fileName);

            PlayerSavingObject _savedPlayer = JsonUtility.FromJson<PlayerSavingObject>(_lines[0]);

            GameObject _player = GameObject.FindGameObjectWithTag("Player");

            string _saveLine = "";

            int _numberOfEnemies = int.Parse(_lines[1]);
            int _numberOfItems = int.Parse(_lines[2 + _numberOfEnemies]);

            PlayerSavingObject playerSave = new PlayerSavingObject(new Vector3(float.Parse(_savedPlayer.positionX), float.Parse(_savedPlayer.positionY), float.Parse(_savedPlayer.positionZ)), new Quaternion(float.Parse(_savedPlayer.rotationX), float.Parse(_savedPlayer.rotationY), float.Parse(_savedPlayer.rotationZ), float.Parse(_savedPlayer.rotationW)), PlayerInventoryManager.Instance.EquippedRightWeapon.Seed, PlayerInventoryManager.Instance.EquippedLeftWeapon.Seed, PlayerInventoryManager.Instance.EquippedEngine.Seed);

            List<string> _weaponSeeds = new List<string>();
            List<string> _engineSeeds = new List<string>();

            foreach (WeaponData _weapon in PlayerInventoryManager.Instance.AvailableWeapons)
            {
                _weaponSeeds.Add(_weapon.Seed);
            }
            foreach (EngineData _engine in PlayerInventoryManager.Instance.AvailableEngines)
            {
                _engineSeeds.Add(_engine.Seed);
            }

            InventorySavingObject _inventory = new InventorySavingObject(_weaponSeeds, _engineSeeds, PlayerInventoryManager.Instance.EquippedEngineIndex.ToString(), PlayerInventoryManager.Instance.EquippedLeftIndex.ToString(), PlayerInventoryManager.Instance.EquippedRightIndex.ToString());

            string _inventorySave = JsonUtility.ToJson(_inventory);

            _saveLine = JsonUtility.ToJson(playerSave);

            using (StreamWriter writer = new StreamWriter(_fileName))
            {
                for (int currentLine = 0; currentLine < _lines.Length; currentLine++)
                {
                    if (currentLine == 0)
                    {
                        writer.WriteLine(_saveLine);
                    }
                    else if (currentLine == 3 + _numberOfEnemies + _numberOfItems)
                    {
                        writer.WriteLine(_inventorySave);
                    }
                    else
                    {
                        writer.WriteLine(_lines[currentLine]);
                    }
                }
            }

        }
        catch
        {
            return;
        }
    }

    public void SaveGame()
    {
        string _saveLine = "";
        string _enemySaveLine = "";

        GameObject _player = GameObject.FindGameObjectWithTag("Player");

        string _rightWeaponSeed = "-1";
        string _leftWeaponSeed = "-1";
        string _engineSeed = "-1";

        if(PlayerInventoryManager.Instance.EquippedLeftWeapon != null)
        {
            _leftWeaponSeed = PlayerInventoryManager.Instance.EquippedLeftWeapon.Seed;
        }
        if(PlayerInventoryManager.Instance.EquippedRightWeapon != null)
        {
            _rightWeaponSeed = PlayerInventoryManager.Instance.EquippedRightWeapon.Seed;
        }
        if (PlayerInventoryManager.Instance.EquippedEngine != null)
        {
            _engineSeed = PlayerInventoryManager.Instance.EquippedEngine.Seed;
        }

        PlayerSavingObject playerSave = new PlayerSavingObject(_player.transform.position, _player.transform.rotation, _rightWeaponSeed, _leftWeaponSeed, _engineSeed);

        GameObject[] _enemies = GameObject.FindGameObjectsWithTag("Enemy");
        List<EnemySavingObject> _enemySaves = new List<EnemySavingObject>();

        foreach (GameObject _enemy in _enemies)
        {
            WeaponData _enemyWeaponRight = _enemy.transform.GetChild(0).Find("RightSnap").GetChild(0).GetComponent<WeaponGenerator>().statBlock;
            WeaponData _enemyWeaponLeft = _enemy.transform.GetChild(0).Find("LeftSnap").GetChild(0).GetComponent<WeaponGenerator>().statBlock;
            _enemySaves.Add(new EnemySavingObject(_enemy.transform.position, _enemy.transform.rotation, _enemyWeaponRight.Seed, _enemyWeaponLeft.Seed));
        }

        string amountOfEnemies = _enemies.Length.ToString();

        GameObject[] _targets = GameObject.FindGameObjectsWithTag("Component");
        List<LootSavingObject> _lootSaves = new List<LootSavingObject>();

        foreach (GameObject _target in _targets)
        {
            int _type = -1;
            switch (_target.GetComponent<LootDetection>().LootType)
            {
                case LootDetection.m_lootTypes.Weapon:
                    _type = 0;
                    break;
                case LootDetection.m_lootTypes.Engine:
                    _type = 1;
                    break;
                case LootDetection.m_lootTypes.Shield:
                    _type = 2;
                    break;
            }
            if (_type == -1)
            {
                Debug.Log("Loot has no type");
                break;
            }
            else if(_type == 0)
            {
                try
                {
                    string _seed = _target.transform.GetChild(0).GetComponent<WeaponGenerator>().statBlock.Seed;
                    _lootSaves.Add(new LootSavingObject(_target.transform.position, _target.transform.rotation, _seed, _type.ToString()));
                }
                catch
                {
                    Debug.Log("<color=red> Did not find stat block on engine </color>");
                }
            }
            else if(_type == 1)
            {
                try
                {
                    string _seed = _target.transform.GetChild(0).GetComponent<EngineGenerator>().engineStatBlock.Seed;
                    _lootSaves.Add(new LootSavingObject(_target.transform.position, _target.transform.rotation, _seed, _type.ToString()));
                }
                catch
                {
                    Debug.Log("<color=red> Did not find stat block on engine </color>");
                }
            }
        }

        List<string> _weaponSeeds = new List<string>();
        List<string> _engineSeeds = new List<string>();

        foreach (WeaponData _weapon in PlayerInventoryManager.Instance.AvailableWeapons)
        {
            _weaponSeeds.Add(_weapon.Seed);
        }
        foreach (EngineData _engine in PlayerInventoryManager.Instance.AvailableEngines)
        {
            _engineSeeds.Add(_engine.Seed);
        }

        InventorySavingObject _inventory = new InventorySavingObject(_weaponSeeds, _engineSeeds, PlayerInventoryManager.Instance.EquippedEngineIndex.ToString(), PlayerInventoryManager.Instance.EquippedLeftIndex.ToString(), PlayerInventoryManager.Instance.EquippedRightIndex.ToString());

        string _amountOfLoot = _lootSaves.Count.ToString();

        _saveLine = JsonUtility.ToJson(playerSave);

        string _inventorySave = JsonUtility.ToJson(_inventory);

        //byte[] _saveLineBytes = System.Text.Encoding.UTF8.GetBytes(_saveLine);

        string _fileName = PlayerPrefs.GetString("CurrentSave") + ".giant";

        _fileName = _fileName.Replace("/", "_");
        _fileName = _fileName.Replace(" ", "_");
        _fileName = _fileName.Replace(":", "_");

        File.Open(_fileName, FileMode.OpenOrCreate, FileAccess.Write).Dispose();

        File.WriteAllText(_fileName, "");

        File.AppendAllText(_fileName, _saveLine + System.Environment.NewLine);

        File.AppendAllText(_fileName, amountOfEnemies + System.Environment.NewLine);

        foreach(EnemySavingObject _enemySave in _enemySaves)
        {
            _enemySaveLine = JsonUtility.ToJson(_enemySave);
            File.AppendAllText(_fileName, _enemySaveLine + System.Environment.NewLine);
        }

        File.AppendAllText(_fileName, _amountOfLoot + System.Environment.NewLine);

        foreach (LootSavingObject _lootSave in _lootSaves)
        {
            _enemySaveLine = JsonUtility.ToJson(_lootSave);
            File.AppendAllText(_fileName, _enemySaveLine + System.Environment.NewLine);
        }

        File.AppendAllText(_fileName, _inventorySave + System.Environment.NewLine);

        PlayerPrefs.SetString("LastSave" + _fileName[4], System.DateTime.Now.ToString());

        PlayerPrefs.SetInt("EnemiesKilled" + _fileName[4], m_enemiesKilledSoFar);

        PlayerPrefs.SetString("ChosenFaction" + _fileName[4], "initial");

        PlayerPrefs.SetString("LatestSave", _fileName);
    }

    public void LoadGame()
    {
        string _fileName = PlayerPrefs.GetString("CurrentSave", "NoSave") + ".giant";
        GameObject _player = GameObject.FindGameObjectWithTag("Player");

        if (_fileName == "NoSave")
        {
            return;
        }

        try
        {
            File.Open(_fileName, FileMode.Open, FileAccess.Read).Dispose();
        }
        catch
        {
            _player.GetComponent<PlayerHealth>().ResetHealth(int.Parse(_fileName[4].ToString()));
            return;
        }

        string[] _loadLines = File.ReadAllLines(_fileName);

        if(_loadLines.Length < 1)
        {
            return;
        }


        PlayerSavingObject _savedPlayer = JsonUtility.FromJson<PlayerSavingObject>(_loadLines[0]);

        GameObject[] _enemies = GameObject.FindGameObjectsWithTag("Enemy");
        GameObject[] _targets = GameObject.FindGameObjectsWithTag("TargetUI");

        foreach(GameObject _enemy in _enemies)
        {
            Destroy(_enemy);
        }

        foreach (GameObject _target in _targets)
        {
            Destroy(_target);
        }

        int _numberOfEnemies = int.Parse(_loadLines[1]);

        for (int i = 0; i < _numberOfEnemies; i++)
        {
            EnemySavingObject _savedEnemy = JsonUtility.FromJson<EnemySavingObject>(_loadLines[i + 2]);
            Vector3 _enemyPos = new Vector3(float.Parse(_savedEnemy.positionX), float.Parse(_savedEnemy.positionY), float.Parse(_savedEnemy.positionZ));
            Quaternion _enemyRot = new Quaternion(float.Parse(_savedEnemy.rotationX), float.Parse(_savedEnemy.rotationY), float.Parse(_savedEnemy.rotationZ), float.Parse(_savedEnemy.rotationW));
            GameObject _newEnemy = Instantiate(spawner.enemyPrefab, _enemyPos, _enemyRot);

            Transform _leftSnap = _newEnemy.transform.Find("ConstructionShip#1").Find("LeftSnap");
            Transform _rightSnap = _newEnemy.transform.Find("ConstructionShip#1").Find("RightSnap");

            WeaponData _temp1 = ModuleManager.Instance.CreateStatBlock(_savedEnemy.rightWeaponSeed);
            GameObject _tempLeftGun = ModuleManager.Instance.GenerateWeapon(_temp1); //Instantiate(weaponBodies[Random.Range(0, weaponBodies.Length - 1)], _leftSnap);
            _tempLeftGun.GetComponent<WeaponGenerator>().statBlock = _temp1;
            _tempLeftGun.transform.SetParent(_leftSnap);
            _tempLeftGun.transform.localPosition = Vector3.zero;
            _tempLeftGun.transform.localRotation = Quaternion.identity;
            _tempLeftGun.transform.localScale = m_scale;

            _temp1 = ModuleManager.Instance.CreateStatBlock(_savedEnemy.leftWeaponSeed);
            GameObject _tempRightGun = ModuleManager.Instance.GenerateWeapon(_temp1);
            _tempRightGun.GetComponent<WeaponGenerator>().statBlock = _temp1;
            _tempRightGun.transform.SetParent(_rightSnap);
            _tempRightGun.transform.localPosition = Vector3.zero;
            _tempRightGun.transform.localRotation = Quaternion.identity;
            _tempRightGun.transform.localScale = m_scale;
        }

        GameObject[] _lootObjects = GameObject.FindGameObjectsWithTag("Component");

        foreach(GameObject _loot in _lootObjects)
        {
            Destroy(_loot);
        }

        int _numberOfItems = int.Parse(_loadLines[2 + _numberOfEnemies]);

        for(int i = 0; i < _numberOfItems; i++)
        {
            LootSavingObject _savedLoot = JsonUtility.FromJson<LootSavingObject>(_loadLines[i + 3 + _numberOfEnemies]);
            Vector3 _lootPos = new Vector3(float.Parse(_savedLoot.positionX), float.Parse(_savedLoot.positionY), float.Parse(_savedLoot.positionZ));
            Quaternion _lootRot = new Quaternion(float.Parse(_savedLoot.rotationX), float.Parse(_savedLoot.rotationY), float.Parse(_savedLoot.rotationZ), float.Parse(_savedLoot.rotationW));
            GameObject _newLoot = Instantiate(lootTemplate, _lootPos, _lootRot);
            _newLoot.GetComponent<LootDetection>().LootType = (LootDetection.m_lootTypes)int.Parse(_savedLoot.type);

            GameObject _temp = new GameObject();

            if (_newLoot.GetComponent<LootDetection>().LootType == LootDetection.m_lootTypes.Weapon)
            {
                WeaponData _temp1 = ModuleManager.Instance.CreateStatBlock(_savedLoot.seed);
                _temp = ModuleManager.Instance.GenerateWeapon(_temp1);
                _temp.GetComponent<WeaponGenerator>().statBlock = _temp1;
            }
            else if(_newLoot.GetComponent<LootDetection>().LootType == LootDetection.m_lootTypes.Engine)
            {
                EngineData _temp1 = ModuleManager.Instance.CreateEngineBlock(_savedLoot.seed);
                _temp = ModuleManager.Instance.GenerateEngine(_temp1);
                _temp.GetComponent<EngineGenerator>().engineStatBlock = _temp1;
            }

            _temp.transform.SetParent(_newLoot.transform);

            _temp.transform.localPosition = m_defaultOffset;
            _temp.transform.localRotation = Quaternion.identity;
            _temp.transform.localScale = m_defaultScale;

            _newLoot.transform.localScale = m_scale;
        }

        _player.transform.position = new Vector3(float.Parse(_savedPlayer.positionX), float.Parse(_savedPlayer.positionY), float.Parse(_savedPlayer.positionZ));
        _player.transform.rotation = new Quaternion(float.Parse(_savedPlayer.rotationX), float.Parse(_savedPlayer.rotationY), float.Parse(_savedPlayer.rotationZ), float.Parse(_savedPlayer.rotationW));

        if (_savedPlayer.rightWeaponSeed != "" && _savedPlayer.rightWeaponSeed != "-1")
        {
            Transform _rightSnapPoint = _player.transform.Find("WeaponRight");

            try
            {
                Destroy(_rightSnapPoint.GetChild(0).gameObject);
            }
            catch { }

            WeaponData _temp1 = ModuleManager.Instance.CreateStatBlock(_savedPlayer.rightWeaponSeed);
            GameObject _tempRightGun = ModuleManager.Instance.GenerateWeapon(_temp1);
            _tempRightGun.GetComponent<WeaponGenerator>().statBlock = _temp1;
            _tempRightGun.transform.SetParent(_rightSnapPoint);
            _tempRightGun.transform.localPosition = Vector3.zero;
            _tempRightGun.transform.localRotation = Quaternion.identity;
            _tempRightGun.transform.localScale = m_scale;
        }
        if (_savedPlayer.leftWeaponSeed != "" && _savedPlayer.leftWeaponSeed != "-1")
        {
            Transform _leftSnapPoint = _player.transform.Find("WeaponLeft");

            try
            {
                Destroy(_leftSnapPoint.GetChild(0).gameObject);
            }
            catch { }

            WeaponData _temp1 = ModuleManager.Instance.CreateStatBlock(_savedPlayer.leftWeaponSeed);
            GameObject _tempLeftGun = ModuleManager.Instance.GenerateWeapon(_temp1);
            _tempLeftGun.GetComponent<WeaponGenerator>().statBlock = _temp1;
            _tempLeftGun.transform.SetParent(_leftSnapPoint);
            _tempLeftGun.transform.localPosition = Vector3.zero;
            _tempLeftGun.transform.localRotation = Quaternion.identity;
            _tempLeftGun.transform.localScale = m_scale;
        }
        if(_savedPlayer.engineSeed != "" && _savedPlayer.engineSeed != "-1")
        {
            Transform _engineSnapPoint = _player.transform.Find("EngineShip");

            try
            {
                Destroy(_engineSnapPoint.GetChild(0).gameObject);
            }
            catch { }

            EngineData _temp1 = ModuleManager.Instance.CreateEngineBlock(_savedPlayer.engineSeed);
            GameObject _tempEngine = ModuleManager.Instance.GenerateEngine(_temp1);
            _tempEngine.transform.GetChild(0).GetComponent<ThrustEffectController>().player = _player.GetComponent<PlayerMovement>();
            _tempEngine.GetComponent<EngineGenerator>().engineStatBlock = _temp1;
            _tempEngine.transform.SetParent(_engineSnapPoint);
            _tempEngine.transform.localPosition = Vector3.zero;
            _tempEngine.transform.localRotation = Quaternion.identity;
            _tempEngine.transform.localScale = m_scale;
        }

        //PlayerInventoryManager.Instance.AvailableWeapons.Add(m_currentLoot.transform.GetChild(0).GetComponent<WeaponGenerator>().statBlock);

        InventorySavingObject _inventory = JsonUtility.FromJson<InventorySavingObject>(_loadLines[3 + _numberOfEnemies + _numberOfItems]);

        PlayerInventoryManager.Instance.WipeInventory();
        
        foreach(string _weapon in _inventory.availableWeapons)
        {
            if (_weapon.Length > 0)
            {
                WeaponData _tempWeapon = ModuleManager.Instance.CreateStatBlock(_weapon);
                PlayerInventoryManager.Instance.AvailableWeapons.Add(_tempWeapon);
            }
        }
        foreach(string _engine in _inventory.availableEngines)
        {
            if (_engine.Length > 0)
            {
                EngineData _tempEngine = ModuleManager.Instance.CreateEngineBlock(_engine);
                PlayerInventoryManager.Instance.AvailableEngines.Add(_tempEngine);
            }
        }

        PlayerInventoryManager.Instance.EquippedLeftIndex = int.Parse(_inventory.equippedLeftIndex);
        PlayerInventoryManager.Instance.EquippedRightIndex = int.Parse(_inventory.equippedRightIndex);
        PlayerInventoryManager.Instance.EquippedEngineIndex = int.Parse(_inventory.equippedEngineIndex);

        if (_savedPlayer.rightWeaponSeed == "")
        {
            Transform _RightSnap = _player.transform.Find("WeaponRight");

            try
            {
                Destroy(_RightSnap.GetChild(0).gameObject);

                GameObject _rightGun = ModuleManager.Instance.GenerateWeapon(PlayerInventoryManager.Instance.EquippedRightWeapon);
                _rightGun.transform.SetParent(_RightSnap.transform);
                _rightGun.transform.position = Vector3.zero;
                _rightGun.transform.localPosition = Vector3.zero;
                _rightGun.transform.localRotation = Quaternion.identity;
                _rightGun.transform.localScale = new Vector3(1, 1, 1);

            }
            catch { }
        }
        else
        {
            try
            {
                WeaponData _temp1 = ModuleManager.Instance.CreateStatBlock(_savedPlayer.rightWeaponSeed);

                PlayerInventoryManager.Instance.EquippedRightWeapon = _temp1;
            }
            catch { }
        }
        if (_savedPlayer.leftWeaponSeed == "")
        {
            Transform _LeftSnap = _player.transform.Find("WeaponLeft");

            try
            { 
                Destroy(_LeftSnap.GetChild(0).gameObject);

                GameObject _leftGun = ModuleManager.Instance.GenerateWeapon(PlayerInventoryManager.Instance.EquippedLeftWeapon);
                _leftGun.transform.SetParent(_LeftSnap.transform);
                _leftGun.transform.localPosition = Vector3.zero;
                _leftGun.transform.localRotation = Quaternion.identity;
                _leftGun.transform.localScale = new Vector3(1, 1, 1);
            }
            catch { }
        }
        else
        {
            try
            {
                WeaponData _temp1 = ModuleManager.Instance.CreateStatBlock(_savedPlayer.leftWeaponSeed);

                PlayerInventoryManager.Instance.EquippedLeftWeapon = _temp1;
            }
            catch { }
        }

        PlayerInventoryManager.Instance.EquippedEngine = PlayerInventoryManager.Instance.AvailableEngines[int.Parse(_inventory.equippedEngineIndex)];
        PlayerInventoryManager.Instance.EquippedEngineIndex = int.Parse(_inventory.equippedEngineIndex);

        _player.GetComponent<PlayerShooting>().buildWeapons();

        HUDManager.Instance.ClearAllDisplays();
    }
}
