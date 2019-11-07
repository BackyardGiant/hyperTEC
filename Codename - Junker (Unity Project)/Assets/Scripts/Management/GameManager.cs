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

        GameObject _player = GameObject.FindGameObjectWithTag("Player");

        PlayerSavingObject playerSave = new PlayerSavingObject(_player.transform.position, _player.transform.rotation, PlayerInventoryManager.Instance.EquippedRightWeapon.Seed, PlayerInventoryManager.Instance.EquippedLeftWeapon.Seed, PlayerInventoryManager.Instance.EquippedEngine.Seed);

        _saveLine = JsonUtility.ToJson(playerSave);

        byte[] _saveLineBytes = System.Text.Encoding.UTF8.GetBytes(_saveLine);

        string _fileName = "SaveFile" + System.DateTime.UtcNow.ToString() + ".giant";

        _fileName = _fileName.Replace("/", "_");
        _fileName = _fileName.Replace(" ", "_");
        _fileName = _fileName.Replace(":", "_");

        File.Open(_fileName, FileMode.OpenOrCreate, FileAccess.Write).Dispose();

        File.WriteAllBytes(_fileName, _saveLineBytes);

        PlayerPrefs.SetString("LatestSave", _fileName);
    }

    public void SaveGame(string _fileName)
    {
        string _saveLine = "";

        GameObject _player = GameObject.FindGameObjectWithTag("Player");

        PlayerSavingObject playerSave = new PlayerSavingObject(_player.transform.position, _player.transform.rotation, PlayerInventoryManager.Instance.EquippedRightWeapon.Seed, PlayerInventoryManager.Instance.EquippedLeftWeapon.Seed, PlayerInventoryManager.Instance.EquippedEngine.Seed);

        _saveLine = JsonUtility.ToJson(playerSave);

        byte[] _saveLineBytes = System.Text.Encoding.UTF8.GetBytes(_saveLine);

        File.Open(_fileName, FileMode.OpenOrCreate, FileAccess.Write).Dispose();

        File.WriteAllBytes(_fileName, _saveLineBytes);
    }

    public void LoadGame()
    {
        string _fileName = PlayerPrefs.GetString("LatestSave");

        File.Open(_fileName, FileMode.OpenOrCreate, FileAccess.Read).Dispose();

        byte[] _loadLineBytes = File.ReadAllBytes(_fileName);

        PlayerSavingObject _savedPlayer = JsonUtility.FromJson<PlayerSavingObject>(System.Text.Encoding.UTF8.GetString(_loadLineBytes, 0, _loadLineBytes.Length));

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
