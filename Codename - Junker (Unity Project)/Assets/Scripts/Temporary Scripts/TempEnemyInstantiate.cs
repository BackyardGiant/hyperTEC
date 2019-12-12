using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TempEnemyInstantiate : MonoBehaviour
{
    public GameObject[] weaponBodies;

    public Transform[] spawnPoints;

    public GameObject traderPrefab;
    public GameObject explorerPrefab;
    public GameObject constructionPrefab;

    public GameObject enemyPrefab;

    public int enemyLimit;

    private Vector3 m_scale = new Vector3(1, 1, 1);

    private void Start()
    {
        enemyPrefab = constructionPrefab;
        for (int i = 0; i < enemyLimit; i++)
        {
            if (i < 5)
            {
                enemyPrefab = constructionPrefab;
                SpawnEnemy(spawnPoints[0], 3, 0);
            }
            else if (i < 10)
            {
                enemyPrefab = traderPrefab;
                SpawnEnemy(spawnPoints[1], 2, 1);
            }
            else if (i < 15)
            {
                enemyPrefab = explorerPrefab;
                SpawnEnemy(spawnPoints[2], 0, 2);
            }
            else if (i < 20)
            {
                enemyPrefab = traderPrefab;
                SpawnEnemy(spawnPoints[3], 2, 3);
            }
            else if (i < 25)
            {
                enemyPrefab = explorerPrefab;
                SpawnEnemy(spawnPoints[4], 0, 4);
            }
            else if (i < 30)
            {
                enemyPrefab = constructionPrefab;
                SpawnEnemy(spawnPoints[5], 3, 5);
            }
            else if (i < 35)
            {
                enemyPrefab = explorerPrefab;
                SpawnEnemy(spawnPoints[6], 0, 6);
            }
            else if (i < 40)
            {
                enemyPrefab = explorerPrefab;
                SpawnEnemy(spawnPoints[7], 0, 7);
            }
            else if (i < 45)
            {
                enemyPrefab = constructionPrefab;
                SpawnEnemy(spawnPoints[8], 3, 8);
            }
            else if (i < 50)
            {
                enemyPrefab = traderPrefab;
                SpawnEnemy(spawnPoints[9], 2, 9);
            }
            else if (i < 55)
            {
                enemyPrefab = traderPrefab;
                SpawnEnemy(spawnPoints[10], 2, 10);
            }
            else if (i < 60)
            {
                enemyPrefab = constructionPrefab;
                SpawnEnemy(spawnPoints[11], 3, 11);
            }
        }
    }

    private void Update()
    {
        // Continously spawn enemies, ensuring that enemyLimit is met.
        //if(GameObject.FindGameObjectsWithTag("Enemy").Length < enemyLimit)
        //{
        //    SpawnEnemy();
        //}
    }

    private void SpawnEnemy(Transform _spawnPoint, int factionIndex, int index)
    {
        GameObject _tempEnemy = Instantiate(enemyPrefab, _spawnPoint.position + (Random.insideUnitSphere * 400), _spawnPoint.rotation);

        _tempEnemy.GetComponent<EnemyStats>().m_currentFaction = (faction)factionIndex;

        _tempEnemy.GetComponent<AdvancedEnemyMovement>().StartPosition = _spawnPoint.position;
        _tempEnemy.GetComponent<EnemyManager>().enemySpawnPoint = _spawnPoint;
        _tempEnemy.GetComponent<EnemyManager>().enemySpawnPointIndex = index;

        Transform _leftSnap = _tempEnemy.transform.Find("Ship").Find("LeftSnap");
        Transform _rightSnap = _tempEnemy.transform.Find("Ship").Find("RightSnap");
        Transform _engineSnap = _tempEnemy.transform.Find("Ship").Find("EngineSnap");

        EngineData _tempEngineBlock = ModuleManager.Instance.CreateEngineBlock((int)_tempEnemy.GetComponent<EnemyStats>().m_currentFaction);
        GameObject _tempEngine = ModuleManager.Instance.GenerateEngine(_tempEngineBlock);
        _tempEngine.GetComponent<EngineGenerator>().engineStatBlock = _tempEngineBlock;

        ThrustEffectController thrust = _tempEngine.GetComponentInChildren<ThrustEffectController>();
        thrust.enabled = false;

        _tempEngine.transform.SetParent(_engineSnap);

        _tempEngine.transform.localPosition = Vector3.zero;

        WeaponData _temp1 = ModuleManager.Instance.CreateStatBlock(factionIndex);
        GameObject _tempLeftGun = ModuleManager.Instance.GenerateWeapon(_temp1); //Instantiate(weaponBodies[Random.Range(0, weaponBodies.Length - 1)], _leftSnap);
        _tempLeftGun.GetComponent<WeaponGenerator>().statBlock = _temp1;
        _tempLeftGun.transform.SetParent(_leftSnap);
        _tempLeftGun.transform.localPosition = Vector3.zero;
        _tempLeftGun.transform.localRotation = Quaternion.identity;
        _tempLeftGun.transform.localScale = m_scale;
        //_tempLeftGun.GetComponent<WeaponGenerator>().GenerateGun();

        WeaponData _temp2 = ModuleManager.Instance.CreateStatBlock(factionIndex);
        GameObject _tempRightGun = ModuleManager.Instance.GenerateWeapon(_temp2); //Instantiate(weaponBodies[Random.Range(0, weaponBodies.Length - 1)], _rightSnap);
        _tempRightGun.GetComponent<WeaponGenerator>().statBlock = _temp2;
        _tempRightGun.transform.SetParent(_rightSnap);
        _tempRightGun.transform.localPosition = Vector3.zero;
        _tempRightGun.transform.localRotation = Quaternion.identity;
        _tempRightGun.transform.localScale = m_scale;
        //_tempRightGun.GetComponent<WeaponGenerator>().GenerateGun();
    }
}
