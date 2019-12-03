using System.Collections;
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
    public DisplayOptions display;
    public FactionChoiceManager choiceManager;

    public GameObject questBeacon;

    private PlayerMovement m_playerMove;

    private bool m_canLeaveScene = false;

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
    public PlayerMovement PlayerMove { get => m_playerMove; set => m_playerMove = value; }
    public bool CanLeaveScene { get => m_canLeaveScene; set => m_canLeaveScene = value; }

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

        m_playerMove = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>();
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
        else if(_sceneName == "ModularShip")
        {
            fadeAnimator.Play("Fade");
            LoadInventory();
        }

        Debug.Log("<color=green>CURRENT SAVE: </color>" + PlayerPrefs.GetString("CurrentSave"));
        Debug.Log("<color=green>LATEST SAVE: </color>" + PlayerPrefs.GetString("LatestSave"));

        if (m_playerMove != null)
        {
            if (PlayerPrefs.GetInt("Invert", 0) == 0)
            {
                m_playerMove.InvertY = false;
            }
            else if (PlayerPrefs.GetInt("Invert", 0) == 1)
            {
                m_playerMove.InvertY = true;
            }
        }
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

    private void InventoryLoadOut()
    {
        SaveWeapons();
        SceneManager.LoadScene("MainScene");
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F1))
        {
            PlayerPrefs.DeleteAll();
        }

        Scene _currentScene = SceneManager.GetActiveScene();
        string _sceneName = _currentScene.name;

        if (Input.GetButtonDown("Inventory") && _sceneName == "MainScene" && m_canLeaveScene && m_gameSpeed != 0)
        {
            fadeAnimator.Play("FadeOut");
            Invoke("LoadOut",0.5f);
        }
        else if((Input.GetButtonDown("Inventory") || Input.GetButtonDown("XboxB")) && _sceneName == "ModularShip" && m_canLeaveScene && m_gameSpeed != 0)
        {
            fadeAnimator.Play("FadeOut");
            Invoke("InventoryLoadOut", 0.5f);
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

            string _leftSeed = "";
            string _rightSeed = "";
            string _engineSeed = "";

            if(PlayerInventoryManager.Instance.EquippedRightWeapon != null)
            {
                _rightSeed = PlayerInventoryManager.Instance.EquippedRightWeapon.Seed;
            }
            if (PlayerInventoryManager.Instance.EquippedLeftWeapon != null)
            {
                _leftSeed = PlayerInventoryManager.Instance.EquippedLeftWeapon.Seed;
            }
            if (PlayerInventoryManager.Instance.EquippedEngine != null)
            {
                _engineSeed = PlayerInventoryManager.Instance.EquippedEngine.Seed;
            }
            PlayerSavingObject playerSave = new PlayerSavingObject(new Vector3(float.Parse(_savedPlayer.positionX), float.Parse(_savedPlayer.positionY), float.Parse(_savedPlayer.positionZ)), new Quaternion(float.Parse(_savedPlayer.rotationX), float.Parse(_savedPlayer.rotationY), float.Parse(_savedPlayer.rotationZ), float.Parse(_savedPlayer.rotationW)), _rightSeed, _leftSeed, _engineSeed);

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

        PlayerSavingObject _playerSave = new PlayerSavingObject(_player.transform.position, _player.transform.rotation, _rightWeaponSeed, _leftWeaponSeed, _engineSeed);

        GameObject[] _enemies = GameObject.FindGameObjectsWithTag("Enemy");
        List<EnemySavingObject> _enemySaves = new List<EnemySavingObject>();

        foreach (GameObject _enemy in _enemies)
        {
            WeaponData _enemyWeaponRight = _enemy.transform.GetChild(0).Find("RightSnap").GetChild(0).GetComponent<WeaponGenerator>().statBlock;
            WeaponData _enemyWeaponLeft = _enemy.transform.GetChild(0).Find("LeftSnap").GetChild(0).GetComponent<WeaponGenerator>().statBlock;
            EngineData _enemyEngine = _enemy.transform.GetChild(0).Find("EngineSnap").GetChild(0).GetComponent<EngineGenerator>().engineStatBlock;
            _enemySaves.Add(new EnemySavingObject(_enemy.transform.position, _enemy.transform.rotation, _enemyWeaponRight.Seed, _enemyWeaponLeft.Seed, _enemyEngine.Seed, _enemy.GetComponent<EnemyManager>().enemySpawnPointIndex.ToString(), ((int)_enemy.GetComponent<EnemyStats>().m_currentFaction).ToString()));
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

        _saveLine = JsonUtility.ToJson(_playerSave);

        string _inventorySave = JsonUtility.ToJson(_inventory);

        string _amountOfQuests = QuestManager.Instance.CurrentQuests.Count.ToString();
        string _questIndex = QuestManager.Instance.TrackingQuestIndex.ToString();

        List<QuestSavingObject> _questSavingObjects = new List<QuestSavingObject>();

        foreach (Quest _quest in QuestManager.Instance.CurrentQuests)
        {
            QuestSavingObject _savedQuest = new QuestSavingObject(_quest.Name, _quest.Description, ((int)_quest.QuestType).ToString(), _quest.PercentageComplete.ToString(), _quest.Size.ToString(), _quest.CurrentAmountCompleted.ToString());
            _questSavingObjects.Add(_savedQuest);
        }

        GameObject[] _questBeacons = GameObject.FindGameObjectsWithTag("QuestBeacon");
        List<BeaconSavingObject> _beaconSaves = new List<BeaconSavingObject>();

        foreach(GameObject _questBeacon in _questBeacons)
        {
            int _type = -1;
            Quest _quest = _questBeacon.GetComponent<QuestBeconDetection>().Quest;
            switch (_questBeacon.GetComponent<QuestBeconDetection>().QuestType)
            {
                case QuestType.kill:
                    _type = 0;
                    break;
                case QuestType.collect:
                    _type = 1;
                    break;
                case QuestType.control:
                    _type = 2;
                    break;
                case QuestType.recon:
                    _type = 3;
                    break;
                case QuestType.targets:
                    _type = 4;
                    break;
            }
            if (_type == -1)
            {
                Debug.Log("Quest has no type");
                break;
            }
            else
            {
                BeaconSavingObject _savedBeacon = new BeaconSavingObject(_questBeacon.transform.position, _questBeacon.transform.rotation, _quest.Name, _quest.Description, ((int)_quest.QuestType).ToString(), _quest.Size.ToString(), _quest.RewardName);
                _beaconSaves.Add(_savedBeacon);
            }
        }

        string _amountOfBeacons = _beaconSaves.Count.ToString();

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

        File.AppendAllText(_fileName, _amountOfQuests + System.Environment.NewLine);
        File.AppendAllText(_fileName, _questIndex + System.Environment.NewLine);

        foreach (QuestSavingObject _quest in _questSavingObjects)
        {
            string _questSaveLine = JsonUtility.ToJson(_quest);
            File.AppendAllText(_fileName, _questSaveLine + System.Environment.NewLine);
        }

        File.AppendAllText(_fileName, _amountOfBeacons + System.Environment.NewLine);

        foreach (BeaconSavingObject _beacon in _beaconSaves)
        {
            string _beaconSaveLine = JsonUtility.ToJson(_beacon);
            File.AppendAllText(_fileName, _beaconSaveLine + System.Environment.NewLine);
        }

        PlayerPrefs.SetString("LastSave" + _fileName[4], System.DateTime.Now.ToString());

        PlayerPrefs.SetInt("EnemiesKilled" + _fileName[4], m_enemiesKilledSoFar);

        PlayerPrefs.SetString("LatestSave", _fileName);
    }

    public void SaveGame(bool _isPlayerDead)
    {
        string _saveLine = "";
        string _enemySaveLine = "";

        GameObject _player = GameObject.FindGameObjectWithTag("Player");

        string _rightWeaponSeed = "-1";
        string _leftWeaponSeed = "-1";
        string _engineSeed = "-1";

        if (PlayerInventoryManager.Instance.EquippedLeftWeapon != null)
        {
            _leftWeaponSeed = PlayerInventoryManager.Instance.EquippedLeftWeapon.Seed;
        }
        if (PlayerInventoryManager.Instance.EquippedRightWeapon != null)
        {
            _rightWeaponSeed = PlayerInventoryManager.Instance.EquippedRightWeapon.Seed;
        }
        if (PlayerInventoryManager.Instance.EquippedEngine != null)
        {
            _engineSeed = PlayerInventoryManager.Instance.EquippedEngine.Seed;
        }

        PlayerSavingObject _playerSave = null;

        if (_isPlayerDead)
        {
            _playerSave = new PlayerSavingObject(new Vector3(-6.6f,0,0), Quaternion.identity, _rightWeaponSeed, _leftWeaponSeed, _engineSeed);
        }
        else
        {
            _playerSave = new PlayerSavingObject(_player.transform.position, _player.transform.rotation, _rightWeaponSeed, _leftWeaponSeed, _engineSeed);
        }

        GameObject[] _enemies = GameObject.FindGameObjectsWithTag("Enemy");
        List<EnemySavingObject> _enemySaves = new List<EnemySavingObject>();

        foreach (GameObject _enemy in _enemies)
        {
            WeaponData _enemyWeaponRight = _enemy.transform.GetChild(0).Find("RightSnap").GetChild(0).GetComponent<WeaponGenerator>().statBlock;
            WeaponData _enemyWeaponLeft = _enemy.transform.GetChild(0).Find("LeftSnap").GetChild(0).GetComponent<WeaponGenerator>().statBlock;
            EngineData _enemyEngine = _enemy.transform.GetChild(0).Find("EngineSnap").GetChild(0).GetComponent<EngineGenerator>().engineStatBlock;
            _enemySaves.Add(new EnemySavingObject(_enemy.transform.position, _enemy.transform.rotation, _enemyWeaponRight.Seed, _enemyWeaponLeft.Seed, _enemyEngine.Seed, _enemy.GetComponent<EnemyManager>().enemySpawnPointIndex.ToString(), ((int)_enemy.GetComponent<EnemyStats>().m_currentFaction).ToString()));
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
            else if (_type == 0)
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
            else if (_type == 1)
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

        _saveLine = JsonUtility.ToJson(_playerSave);

        string _inventorySave = JsonUtility.ToJson(_inventory);

        string _amountOfQuests = QuestManager.Instance.CurrentQuests.Count.ToString();
        string _questIndex = QuestManager.Instance.TrackingQuestIndex.ToString();

        List<QuestSavingObject> _questSavingObjects = new List<QuestSavingObject>();

        foreach (Quest _quest in QuestManager.Instance.CurrentQuests)
        {
            QuestSavingObject _savedQuest = new QuestSavingObject(_quest.Name, _quest.Description, ((int)_quest.QuestType).ToString(), _quest.PercentageComplete.ToString(), _quest.Size.ToString(), _quest.CurrentAmountCompleted.ToString());
            _questSavingObjects.Add(_savedQuest);
        }

        GameObject[] _questBeacons = GameObject.FindGameObjectsWithTag("QuestBeacon");
        List<BeaconSavingObject> _beaconSaves = new List<BeaconSavingObject>();

        foreach (GameObject _questBeacon in _questBeacons)
        {
            int _type = -1;
            Quest _quest = _questBeacon.GetComponent<QuestBeconDetection>().Quest;
            switch (_questBeacon.GetComponent<QuestBeconDetection>().QuestType)
            {
                case QuestType.kill:
                    _type = 0;
                    break;
                case QuestType.collect:
                    _type = 1;
                    break;
                case QuestType.control:
                    _type = 2;
                    break;
                case QuestType.recon:
                    _type = 3;
                    break;
                case QuestType.targets:
                    _type = 4;
                    break;
            }
            if (_type == -1)
            {
                Debug.Log("Quest has no type");
                break;
            }
            else
            {
                BeaconSavingObject _savedBeacon = new BeaconSavingObject(_questBeacon.transform.position, _questBeacon.transform.rotation, _quest.Name, _quest.Description, ((int)_quest.QuestType).ToString(), _quest.Size.ToString(), _quest.RewardName);
                _beaconSaves.Add(_savedBeacon);
            }
        }

        string _amountOfBeacons = _beaconSaves.Count.ToString();

        //byte[] _saveLineBytes = System.Text.Encoding.UTF8.GetBytes(_saveLine);

        string _fileName = PlayerPrefs.GetString("CurrentSave") + ".giant";

        _fileName = _fileName.Replace("/", "_");
        _fileName = _fileName.Replace(" ", "_");
        _fileName = _fileName.Replace(":", "_");

        File.Open(_fileName, FileMode.OpenOrCreate, FileAccess.Write).Dispose();

        File.WriteAllText(_fileName, "");

        File.AppendAllText(_fileName, _saveLine + System.Environment.NewLine);

        File.AppendAllText(_fileName, amountOfEnemies + System.Environment.NewLine);

        foreach (EnemySavingObject _enemySave in _enemySaves)
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

        File.AppendAllText(_fileName, _amountOfQuests + System.Environment.NewLine);
        File.AppendAllText(_fileName, _questIndex + System.Environment.NewLine);

        foreach (QuestSavingObject _quest in _questSavingObjects)
        {
            string _questSaveLine = JsonUtility.ToJson(_quest);
            File.AppendAllText(_fileName, _questSaveLine + System.Environment.NewLine);
        }

        File.AppendAllText(_fileName, _amountOfBeacons + System.Environment.NewLine);

        foreach (BeaconSavingObject _beacon in _beaconSaves)
        {
            string _beaconSaveLine = JsonUtility.ToJson(_beacon);
            File.AppendAllText(_fileName, _beaconSaveLine + System.Environment.NewLine);
        }

        PlayerPrefs.SetString("LastSave" + _fileName[4], System.DateTime.Now.ToString());

        PlayerPrefs.SetInt("EnemiesKilled" + _fileName[4], m_enemiesKilledSoFar);

        PlayerPrefs.SetString("LatestSave", _fileName);
    }

    public void LoadGame()
    {
        string _fileName = PlayerPrefs.GetString("CurrentSave", "NoSave") + ".giant";
        GameObject _player = GameObject.FindGameObjectWithTag("Player");

        if (_fileName == "NoSave")
        {
            m_canLeaveScene = true;
            return;
        }

        try
        {
            File.Open(_fileName, FileMode.Open, FileAccess.Read).Dispose();
        }
        catch
        {
            _player.GetComponent<PlayerHealth>().ResetHealth(int.Parse(_fileName[4].ToString()) - 1);
            if (PlayerInventoryManager.Instance.EquippedLeftWeapon != null)
            {
                Transform _leftSnapPoint = _player.transform.Find("WeaponLeft");

                GameObject _leftGun = ModuleManager.Instance.GenerateWeapon(PlayerInventoryManager.Instance.EquippedLeftWeapon);
                _leftGun.transform.SetParent(_leftSnapPoint.transform);
                _leftGun.transform.localPosition = Vector3.zero;
                _leftGun.transform.localRotation = Quaternion.identity;
                _leftGun.transform.localScale = new Vector3(1, 1, 1);
            }

            if (PlayerInventoryManager.Instance.EquippedRightWeapon != null)
            {
                Transform _rightSnapPoint = _player.transform.Find("WeaponRight");

                GameObject _rightGun = ModuleManager.Instance.GenerateWeapon(PlayerInventoryManager.Instance.EquippedRightWeapon);
                _rightGun.transform.SetParent(_rightSnapPoint.transform);
                _rightGun.transform.position = Vector3.zero;
                _rightGun.transform.localPosition = Vector3.zero;
                _rightGun.transform.localRotation = Quaternion.identity;
                _rightGun.transform.localScale = new Vector3(1, 1, 1);
            }

            _player.GetComponent<PlayerMovement>().UpdateValues();

            choiceManager.onLoad();

            HUDManager.Instance.ClearAllDisplays();

            m_canLeaveScene = true;
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
        GameObject[] _beacons = GameObject.FindGameObjectsWithTag("QuestBeacon");

        foreach(GameObject _enemy in _enemies)
        {
            Destroy(_enemy);
        }

        foreach (GameObject _target in _targets)
        {
            Destroy(_target);
        }

        foreach (GameObject _beacon in _beacons)
        {
            Destroy(_beacon);
        }

        int _numberOfEnemies = int.Parse(_loadLines[1]);

        for (int i = 0; i < _numberOfEnemies; i++)
        {
            EnemySavingObject _savedEnemy = JsonUtility.FromJson<EnemySavingObject>(_loadLines[i + 2]);
            Vector3 _enemyPos = new Vector3(float.Parse(_savedEnemy.positionX), float.Parse(_savedEnemy.positionY), float.Parse(_savedEnemy.positionZ));
            Quaternion _enemyRot = new Quaternion(float.Parse(_savedEnemy.rotationX), float.Parse(_savedEnemy.rotationY), float.Parse(_savedEnemy.rotationZ), float.Parse(_savedEnemy.rotationW));

            faction _newEnemyFaction = (faction)int.Parse(_savedEnemy.factionType);

            switch (_newEnemyFaction)
            {
                case faction.trader:
                    spawner.enemyPrefab = spawner.traderPrefab;
                    break;
                case faction.construction:
                    spawner.enemyPrefab = spawner.constructionPrefab;
                    break;
                case faction.explorer:
                    spawner.enemyPrefab = spawner.explorerPrefab;
                    break;
            }




            GameObject _newEnemy = Instantiate(spawner.enemyPrefab, _enemyPos, _enemyRot);

            _newEnemy.GetComponent<EnemyStats>().m_currentFaction = _newEnemyFaction;
            _newEnemy.GetComponent<EnemyManager>().enemySpawnPoint = spawner.spawnPoints[int.Parse(_savedEnemy.spawnIndex)];
            _newEnemy.GetComponent<EnemyManager>().enemySpawnPointIndex = int.Parse(_savedEnemy.spawnIndex);

            Transform _leftSnap = _newEnemy.transform.Find("Ship").Find("LeftSnap");
            Transform _rightSnap = _newEnemy.transform.Find("Ship").Find("RightSnap");
            Transform _engineSnap = _newEnemy.transform.Find("Ship").Find("EngineSnap");

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

            EngineData _tempEngine = ModuleManager.Instance.CreateEngineBlock(_savedEnemy.engineSeed);
            GameObject _tempEngineObject = ModuleManager.Instance.GenerateEngine(_tempEngine);

            ThrustEffectController thrust = _tempEngineObject.GetComponentInChildren<ThrustEffectController>();
            thrust.enabled = false;

            _tempEngineObject.GetComponent<EngineGenerator>().engineStatBlock = _tempEngine;
            _tempEngineObject.transform.SetParent(_engineSnap);

            _tempEngineObject.transform.localPosition = Vector3.zero;
            _tempEngineObject.transform.localRotation = Quaternion.identity;
            _tempEngineObject.transform.localEulerAngles = _tempEngineObject.transform.localEulerAngles + new Vector3(0, -90, 0);
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

        if (_savedPlayer.rightWeaponSeed != "" && _savedPlayer.rightWeaponSeed != "-1" && _savedPlayer.rightWeaponSeed != "1")
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
        if (_savedPlayer.leftWeaponSeed != "" && _savedPlayer.leftWeaponSeed != "-1" && _savedPlayer.leftWeaponSeed != "1")
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
            if (_weapon != "1")
            {
                if (_weapon.Length > 0)
                {
                    WeaponData _tempWeapon = ModuleManager.Instance.CreateStatBlock(_weapon);
                    PlayerInventoryManager.Instance.AvailableWeapons.Add(_tempWeapon);
                }
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

        if (_inventory.equippedLeftIndex != "-1")
        {
            PlayerInventoryManager.Instance.EquippedLeftWeapon = PlayerInventoryManager.Instance.AvailableWeapons[PlayerInventoryManager.Instance.EquippedLeftIndex];
        }
        else
        {
            PlayerInventoryManager.Instance.EquippedLeftWeapon = null;
        }
        if (_inventory.equippedRightIndex != "-1")
        {
            PlayerInventoryManager.Instance.EquippedRightWeapon = PlayerInventoryManager.Instance.AvailableWeapons[PlayerInventoryManager.Instance.EquippedRightIndex];
        }
        else
        {
            PlayerInventoryManager.Instance.EquippedRightWeapon = null;
        }
        if (_inventory.equippedEngineIndex != "-1")
        {
            PlayerInventoryManager.Instance.EquippedEngine = PlayerInventoryManager.Instance.AvailableEngines[PlayerInventoryManager.Instance.EquippedEngineIndex];
        }
        else
        {
            PlayerInventoryManager.Instance.EquippedEngine = null;
        }

        if (_savedPlayer.rightWeaponSeed == "1")
        {
            Transform _RightSnap = _player.transform.Find("WeaponRight");

            if(_RightSnap.childCount > 1)
            { 
                Destroy(_RightSnap.GetChild(0).gameObject);
            }

            GameObject _rightGun = ModuleManager.Instance.GenerateWeapon(PlayerInventoryManager.Instance.EquippedRightWeapon);
            _rightGun.transform.SetParent(_RightSnap.transform);
            _rightGun.transform.position = Vector3.zero;
            _rightGun.transform.localPosition = Vector3.zero;
            _rightGun.transform.localRotation = Quaternion.identity;
            _rightGun.transform.localScale = new Vector3(1, 1, 1);
        }
        else
        {
            if (_savedPlayer.rightWeaponSeed != "" && _savedPlayer.rightWeaponSeed != "-1" && _savedPlayer.rightWeaponSeed != "1")
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
            try
            {
                WeaponData _temp1 = ModuleManager.Instance.CreateStatBlock(_savedPlayer.rightWeaponSeed);

                PlayerInventoryManager.Instance.EquippedRightWeapon = _temp1;
            }
            catch { }
        }
        if (_savedPlayer.leftWeaponSeed == "1")
        {
            Transform _LeftSnap = _player.transform.Find("WeaponLeft");


            if (_LeftSnap.childCount > 1)
            {
                Destroy(_LeftSnap.GetChild(0).gameObject);
            }

            GameObject _leftGun = ModuleManager.Instance.GenerateWeapon(PlayerInventoryManager.Instance.EquippedLeftWeapon);
            _leftGun.transform.SetParent(_LeftSnap.transform);
            _leftGun.transform.localPosition = Vector3.zero;
            _leftGun.transform.localRotation = Quaternion.identity;
            _leftGun.transform.localScale = new Vector3(1, 1, 1);
        }
        else
        {

            if (_savedPlayer.leftWeaponSeed != "" && _savedPlayer.leftWeaponSeed != "-1" && _savedPlayer.leftWeaponSeed != "1")
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
            try
            {
                WeaponData _temp1 = ModuleManager.Instance.CreateStatBlock(_savedPlayer.leftWeaponSeed);

                PlayerInventoryManager.Instance.EquippedLeftWeapon = _temp1;
            }
            catch { }
        }

        try
        {
            PlayerInventoryManager.Instance.EquippedEngine = PlayerInventoryManager.Instance.AvailableEngines[int.Parse(_inventory.equippedEngineIndex)];
            PlayerInventoryManager.Instance.EquippedEngineIndex = int.Parse(_inventory.equippedEngineIndex);
        }
        catch { }

        //_loadLines[3 + _numberOfEnemies + _numberOfItems]
        int _numberOfQuests = int.Parse(_loadLines[4 + _numberOfEnemies + _numberOfItems]);
        int _questIndex = int.Parse(_loadLines[5 + _numberOfEnemies + _numberOfItems]);

        QuestManager.Instance.ClearQuests();

        QuestManager.Instance.TrackingQuestIndex = _questIndex;

        for (int i = 0; i < _numberOfQuests; i++)
        {
            QuestSavingObject _quest = JsonUtility.FromJson<QuestSavingObject>(_loadLines[i + 6 + _numberOfEnemies + _numberOfItems]);

            switch ((QuestType)int.Parse(_quest.questType))
            {
                case QuestType.kill:
                    QuestManager.Instance.CreateKillQuest(int.Parse(_quest.size), _quest.name, _quest.description);
                    QuestManager.Instance.CurrentQuests[QuestManager.Instance.CurrentQuests.Count - 1].QuestIncrement(int.Parse(_quest.currentAmountCompleted));
                    break;
                case QuestType.control:
                    QuestManager.Instance.CreateControlQuest(int.Parse(_quest.size), _quest.name, _quest.description);
                    QuestManager.Instance.CurrentQuests[QuestManager.Instance.CurrentQuests.Count - 1].QuestIncrement(int.Parse(_quest.currentAmountCompleted));
                    break;
                case QuestType.collect:
                    //QuestManager.Instance.CreateCollectQuest(int.Parse(_quest.m_size), _quest.m_name, _quest.m_description);
                    QuestManager.Instance.CurrentQuests[QuestManager.Instance.CurrentQuests.Count - 1].QuestIncrement(int.Parse(_quest.currentAmountCompleted));
                    break;
                case QuestType.recon:
                    QuestManager.Instance.CreateReconQuest(int.Parse(_quest.size), _quest.name, _quest.description);
                    QuestManager.Instance.CurrentQuests[QuestManager.Instance.CurrentQuests.Count - 1].QuestIncrement(int.Parse(_quest.currentAmountCompleted));
                    break;
                case QuestType.targets:
                    QuestManager.Instance.CreateTargetQuest(int.Parse(_quest.size), _quest.name, _quest.description);
                    QuestManager.Instance.CurrentQuests[QuestManager.Instance.CurrentQuests.Count - 1].QuestIncrement(int.Parse(_quest.currentAmountCompleted));
                    break;
            }
        }

        int _numberOfBeacons = int.Parse(_loadLines[6 + _numberOfEnemies + _numberOfItems + _numberOfQuests]);

        for(int i = 0; i < _numberOfBeacons; i++)
        {
            string str = _loadLines[i + 7 + _numberOfEnemies + _numberOfItems + _numberOfQuests];
            BeaconSavingObject _beacon = JsonUtility.FromJson<BeaconSavingObject>(_loadLines[i + 7 + _numberOfEnemies + _numberOfItems + _numberOfQuests]);

            Quest _quest = ScriptableObject.CreateInstance<Quest>();
            _quest.Name = _beacon.questName;
            _quest.Description = _beacon.description;
            _quest.Size = int.Parse(_beacon.size);
            _quest.RewardName = _beacon.rewardName;
            _quest.QuestType = (QuestType)int.Parse(_beacon.questType);

            GameObject _newBeacon = Instantiate(questBeacon, new Vector3(float.Parse(_beacon.positionX), float.Parse(_beacon.positionY), float.Parse(_beacon.positionZ)), new Quaternion(float.Parse(_beacon.rotationX), float.Parse(_beacon.rotationY), float.Parse(_beacon.rotationZ), float.Parse(_beacon.rotationW)));
            _newBeacon.GetComponent<QuestBeconDetection>().Quest = _quest;
            _newBeacon.GetComponent<QuestBeconDetection>().QuestType = _quest.QuestType;
            _newBeacon.GetComponent<QuestBeconDetection>().enabled = true;
        }

        _player.GetComponent<PlayerShooting>().buildWeapons();
        _player.GetComponent<PlayerMovement>().UpdateValues();

        choiceManager.onLoad();

        HUDManager.Instance.ClearAllDisplays();

        m_canLeaveScene = true;
    }

    public void LoadInventory()
    {
        string _fileName = PlayerPrefs.GetString("CurrentSave", "NoSave") + ".giant";
        GameObject _player = GameObject.FindGameObjectWithTag("Player");

        if (_fileName == "NoSave")
        {
            m_canLeaveScene = true;
            return;
        }

        string[] _loadLines = File.ReadAllLines(_fileName);

        int _numberOfEnemies = int.Parse(_loadLines[1]);
        int _numberOfItems = int.Parse(_loadLines[2 + _numberOfEnemies]);


        InventorySavingObject _inventory = JsonUtility.FromJson<InventorySavingObject>(_loadLines[3 + _numberOfEnemies + _numberOfItems]);

        PlayerInventoryManager.Instance.WipeInventory();

        foreach (string _weapon in _inventory.availableWeapons)
        {
            if (_weapon != "1")
            {
                if (_weapon.Length > 0)
                {
                    WeaponData _tempWeapon = ModuleManager.Instance.CreateStatBlock(_weapon);
                    PlayerInventoryManager.Instance.AvailableWeapons.Add(_tempWeapon);
                }
            }
        }
        foreach (string _engine in _inventory.availableEngines)
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

        if (_inventory.equippedLeftIndex != "-1")
        {
            PlayerInventoryManager.Instance.EquippedLeftWeapon = PlayerInventoryManager.Instance.AvailableWeapons[PlayerInventoryManager.Instance.EquippedLeftIndex];
        }
        else
        {
            PlayerInventoryManager.Instance.EquippedLeftWeapon = null;
        }
        if (_inventory.equippedRightIndex != "-1")
        {
            PlayerInventoryManager.Instance.EquippedRightWeapon = PlayerInventoryManager.Instance.AvailableWeapons[PlayerInventoryManager.Instance.EquippedRightIndex];
        }
        else
        {
            PlayerInventoryManager.Instance.EquippedRightWeapon = null;
        }
        if (_inventory.equippedEngineIndex != "-1")
        {
            PlayerInventoryManager.Instance.EquippedEngine = PlayerInventoryManager.Instance.AvailableEngines[PlayerInventoryManager.Instance.EquippedEngineIndex];
        }
        else
        {
            PlayerInventoryManager.Instance.EquippedEngine = null;
        }

        try
        {
            PlayerInventoryManager.Instance.EquippedEngine = PlayerInventoryManager.Instance.AvailableEngines[int.Parse(_inventory.equippedEngineIndex)];
            PlayerInventoryManager.Instance.EquippedEngineIndex = int.Parse(_inventory.equippedEngineIndex);
        }
        catch { }

        //_loadLines[3 + _numberOfEnemies + _numberOfItems]
        int _numberOfQuests = int.Parse(_loadLines[4 + _numberOfEnemies + _numberOfItems]);
        int _questIndex = int.Parse(_loadLines[5 + _numberOfEnemies + _numberOfItems]);

        QuestManager.Instance.ClearQuests();

        QuestManager.Instance.TrackingQuestIndex = _questIndex;

        for (int i = 0; i < _numberOfQuests; i++)
        {
            QuestSavingObject _quest = JsonUtility.FromJson<QuestSavingObject>(_loadLines[6 + _numberOfEnemies + _numberOfItems]);

            switch ((QuestType)int.Parse(_quest.questType))
            {
                case QuestType.kill:
                    QuestManager.Instance.CreateKillQuest(int.Parse(_quest.size), _quest.name, _quest.description);
                    QuestManager.Instance.CurrentQuests[QuestManager.Instance.CurrentQuests.Count - 1].QuestIncrement(int.Parse(_quest.currentAmountCompleted));
                    break;
                case QuestType.control:
                    QuestManager.Instance.CreateControlQuest(int.Parse(_quest.size), _quest.name, _quest.description);
                    QuestManager.Instance.CurrentQuests[QuestManager.Instance.CurrentQuests.Count - 1].QuestIncrement(int.Parse(_quest.currentAmountCompleted));
                    break;
                case QuestType.collect:
                    //QuestManager.Instance.CreateCollectQuest(int.Parse(_quest.m_size), _quest.m_name, _quest.m_description);
                    QuestManager.Instance.CurrentQuests[QuestManager.Instance.CurrentQuests.Count - 1].QuestIncrement(int.Parse(_quest.currentAmountCompleted));
                    break;
                case QuestType.recon:
                    QuestManager.Instance.CreateReconQuest(int.Parse(_quest.size), _quest.name, _quest.description);
                    QuestManager.Instance.CurrentQuests[QuestManager.Instance.CurrentQuests.Count - 1].QuestIncrement(int.Parse(_quest.currentAmountCompleted));
                    break;
                case QuestType.targets:
                    QuestManager.Instance.CreateTargetQuest(int.Parse(_quest.size), _quest.name, _quest.description);
                    QuestManager.Instance.CurrentQuests[QuestManager.Instance.CurrentQuests.Count - 1].QuestIncrement(int.Parse(_quest.currentAmountCompleted));
                    break;
            }
        }

        display.FillInventory();

        m_canLeaveScene = true;
    }

    public void ReturnToMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void ReturnToMenuDelayed(float _delay)
    {
        Invoke("ReturnToMenu", _delay);
    }
}
