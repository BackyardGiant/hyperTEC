using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;

public class GameManager : MonoBehaviour
{
    private static GameManager s_instance;

    public GameEvent preSlowDown;
    public GameEvent postSlowDown;
    public GameEvent normalSpeed;
    public TempEnemyInstantiate spawner;
    public GameObject lootTemplate;

    private Vector3 m_defaultScale = new Vector3(1, 1, 1);
    private Vector3 m_defaultOffset = new Vector3(0.278f, -0.226f, -0.802f); // taken from eyeballing the offset in the inspector

    private Vector3 m_scale = new Vector3(1, 1, 1);

    [SerializeField, Range(0,1), Tooltip("The speed of objects in the game on a scale of 0-1")]
    private float m_gameSpeed;

    private float m_passedSpeed;

    public static GameManager Instance { get => s_instance; set => s_instance = value; }
    public float GameSpeed { get => m_gameSpeed; private set => m_gameSpeed = value; }

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
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F1))
        {
            PlayerPrefs.DeleteAll();
        }

        Scene currentScene = SceneManager.GetActiveScene();
        string sceneName = currentScene.name;

        if (Input.GetButtonDown("Inventory") && sceneName == "MainScene")
        {
            SceneManager.LoadScene("ModularShip");
        }
        else if(Input.GetButtonDown("Inventory") && sceneName == "ModularShip")
        { 
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

    public void SaveGame()
    {
        string _saveLine = "";
        string _enemySaveLine = "";

        GameObject _player = GameObject.FindGameObjectWithTag("Player");

        PlayerSavingObject playerSave = new PlayerSavingObject(_player.transform.position, _player.transform.rotation, PlayerInventoryManager.Instance.EquippedRightWeapon.Seed, PlayerInventoryManager.Instance.EquippedLeftWeapon.Seed, PlayerInventoryManager.Instance.EquippedEngine.Seed);

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
            else
            {
                string _seed = _target.transform.GetChild(0).GetComponent<WeaponGenerator>().statBlock.Seed;
                _lootSaves.Add(new LootSavingObject(_target.transform.position, _target.transform.rotation, _seed, _type.ToString()));
            }
        }

        string amountOfLoot = _lootSaves.Count.ToString();

        _saveLine = JsonUtility.ToJson(playerSave);

        //byte[] _saveLineBytes = System.Text.Encoding.UTF8.GetBytes(_saveLine);

        string _fileName = "SaveFile" + System.DateTime.UtcNow.ToString() + ".giant";

        _fileName = _fileName.Replace("/", "_");
        _fileName = _fileName.Replace(" ", "_");
        _fileName = _fileName.Replace(":", "_");

        File.Open(_fileName, FileMode.OpenOrCreate, FileAccess.Write).Dispose();

        File.AppendAllText(_fileName, _saveLine + System.Environment.NewLine);

        File.AppendAllText(_fileName, amountOfEnemies + System.Environment.NewLine);

        foreach(EnemySavingObject _enemySave in _enemySaves)
        {
            _enemySaveLine = JsonUtility.ToJson(_enemySave);
            File.AppendAllText(_fileName, _enemySaveLine + System.Environment.NewLine);
        }

        File.AppendAllText(_fileName, amountOfLoot + System.Environment.NewLine);

        foreach (LootSavingObject _lootSave in _lootSaves)
        {
            _enemySaveLine = JsonUtility.ToJson(_lootSave);
            File.AppendAllText(_fileName, _enemySaveLine + System.Environment.NewLine);
        }

        PlayerPrefs.SetString("LatestSave", _fileName);
    }

    public void SaveGame(string _fileName)
    {
        string _saveLine = "";
        string _enemySaveLine = "";

        GameObject _player = GameObject.FindGameObjectWithTag("Player");

        PlayerSavingObject playerSave = new PlayerSavingObject(_player.transform.position, _player.transform.rotation, PlayerInventoryManager.Instance.EquippedRightWeapon.Seed, PlayerInventoryManager.Instance.EquippedLeftWeapon.Seed, PlayerInventoryManager.Instance.EquippedEngine.Seed);

        GameObject[] _enemies = GameObject.FindGameObjectsWithTag("Enemy");
        List<EnemySavingObject> enemySaves = new List<EnemySavingObject>();

        foreach (GameObject _enemy in _enemies)
        {
            WeaponData _enemyWeaponRight = _enemy.transform.GetChild(0).Find("RightSnap").GetChild(0).GetComponent<WeaponGenerator>().statBlock;
            WeaponData _enemyWeaponLeft = _enemy.transform.GetChild(0).Find("LeftSnap").GetChild(0).GetComponent<WeaponGenerator>().statBlock;
            enemySaves.Add(new EnemySavingObject(_enemy.transform.position, _enemy.transform.rotation, _enemyWeaponRight.Seed, _enemyWeaponLeft.Seed));
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
            else
            {
                string _seed = _target.transform.GetChild(0).GetComponent<WeaponGenerator>().statBlock.Seed;
                _lootSaves.Add(new LootSavingObject(_target.transform.position, _target.transform.rotation, _seed, _type.ToString()));
            }
        }

        string amountOfLoot = _lootSaves.Count.ToString();

        _saveLine = JsonUtility.ToJson(playerSave);

        //byte[] _saveLineBytes = System.Text.Encoding.UTF8.GetBytes(_saveLine);

        File.Open(_fileName, FileMode.OpenOrCreate, FileAccess.Write).Dispose();

        File.WriteAllText(_fileName, System.String.Empty);

        File.AppendAllText(_fileName, _saveLine + System.Environment.NewLine);

        File.AppendAllText(_fileName, amountOfEnemies + System.Environment.NewLine);

        foreach (EnemySavingObject _enemySave in enemySaves)
        {
            _enemySaveLine = JsonUtility.ToJson(_enemySave);
            File.AppendAllText(_fileName, _enemySaveLine + System.Environment.NewLine);
        }

        File.AppendAllText(_fileName, amountOfLoot + System.Environment.NewLine);

        foreach (LootSavingObject _lootSave in _lootSaves)
        {
            _enemySaveLine = JsonUtility.ToJson(_lootSave);
            File.AppendAllText(_fileName, _enemySaveLine + System.Environment.NewLine);
        }

        PlayerPrefs.SetString("LatestSave", _fileName);
    }

    public void LoadGame()
    {
        string _fileName = PlayerPrefs.GetString("LatestSave");

        File.Open(_fileName, FileMode.OpenOrCreate, FileAccess.Read).Dispose();

        string[] _loadLines = File.ReadAllLines(_fileName);

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

            WeaponData _temp1 = ModuleManager.Instance.CreateStatBlock(_savedLoot.seed);
            GameObject _temp = ModuleManager.Instance.GenerateWeapon(_temp1);
            _temp.GetComponent<WeaponGenerator>().statBlock = _temp1;

            _temp.transform.SetParent(_newLoot.transform);

            _temp.transform.localPosition = m_defaultOffset;
            _temp.transform.localRotation = Quaternion.identity;

            _temp.transform.localScale = m_defaultScale;
        }

        GameObject _player = GameObject.FindGameObjectWithTag("Player");

        _player.transform.position = new Vector3(float.Parse(_savedPlayer.positionX), float.Parse(_savedPlayer.positionY), float.Parse(_savedPlayer.positionZ));
        _player.transform.rotation = new Quaternion(float.Parse(_savedPlayer.rotationX), float.Parse(_savedPlayer.rotationY), float.Parse(_savedPlayer.rotationZ), float.Parse(_savedPlayer.rotationW));

        if (_savedPlayer.rightWeaponSeed != "")
        {
            Transform _rightSnapPoint = _player.transform.Find("WeaponRight");

            Destroy(_rightSnapPoint.GetChild(0).gameObject);

            WeaponData _temp1 = ModuleManager.Instance.CreateStatBlock(_savedPlayer.rightWeaponSeed);
            GameObject _tempRightGun = ModuleManager.Instance.GenerateWeapon(_temp1);
            _tempRightGun.GetComponent<WeaponGenerator>().statBlock = _temp1;
            _tempRightGun.transform.SetParent(_rightSnapPoint);
            _tempRightGun.transform.localPosition = Vector3.zero;
            _tempRightGun.transform.localRotation = Quaternion.identity;
            _tempRightGun.transform.localScale = m_scale;
        }
        if (_savedPlayer.leftWeaponSeed != "")
        {
            Transform _leftSnapPoint = _player.transform.Find("WeaponLeft");

            Destroy(_leftSnapPoint.GetChild(0).gameObject);

            WeaponData _temp1 = ModuleManager.Instance.CreateStatBlock(_savedPlayer.leftWeaponSeed);
            GameObject _tempLeftGun = ModuleManager.Instance.GenerateWeapon(_temp1);
            _tempLeftGun.GetComponent<WeaponGenerator>().statBlock = _temp1;
            _tempLeftGun.transform.SetParent(_leftSnapPoint);
            _tempLeftGun.transform.localPosition = Vector3.zero;
            _tempLeftGun.transform.localRotation = Quaternion.identity;
            _tempLeftGun.transform.localScale = m_scale;
        }
    }

    public void LoadGame(string _fileName)
    {

    }
}
