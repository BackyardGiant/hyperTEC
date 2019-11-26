using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TempEnemyInstantiate : MonoBehaviour
{
    public GameObject[] weaponBodies;

    public Transform[] spawnPoints;

    public GameObject enemyPrefab;

    public int enemyLimit;

    private Vector3 m_scale = new Vector3(1, 1, 1);

    private void Start()
    {
        for (int i = 0; i < enemyLimit; i++)
        {
            if (i < 5)
            {
                SpawnEnemy(spawnPoints[0]);
            }
            else if (i < 10)
            {
                SpawnEnemy(spawnPoints[1]);
            }
            else if (i < 15)
            {
                SpawnEnemy(spawnPoints[2]);
            }
            else if (i < 20)
            {
                SpawnEnemy(spawnPoints[3]);
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

    private void SpawnEnemy(Transform _spawnPoint)
    {
        GameObject _tempEnemy = Instantiate(enemyPrefab, _spawnPoint.position + (Random.insideUnitSphere * 300), _spawnPoint.rotation);

        // Currently all are construction
        _tempEnemy.GetComponent<EnemyStats>().m_currentFaction = faction.construction;
        //

        _tempEnemy.GetComponent<AdvancedEnemyMovement>().StartPosition = _spawnPoint.position;

        Transform _leftSnap = _tempEnemy.transform.Find("ConstructionShip#1").Find("LeftSnap");
        Transform _rightSnap = _tempEnemy.transform.Find("ConstructionShip#1").Find("RightSnap");
        Transform _engineSnap = _tempEnemy.transform.Find("ConstructionShip#1").Find("EngineSnap");

        EngineData _tempEngineBlock = ModuleManager.Instance.CreateEngineBlock((int)_tempEnemy.GetComponent<EnemyStats>().m_currentFaction);
        GameObject _tempEngine = ModuleManager.Instance.GenerateEngine(_tempEngineBlock);
        _tempEngine.GetComponent<EngineGenerator>().engineStatBlock = _tempEngineBlock;
        _tempEngine.transform.SetParent(_engineSnap);

        _tempEngine.transform.localPosition = Vector3.zero;

        WeaponData _temp1 = ModuleManager.Instance.CreateStatBlock();
        GameObject _tempLeftGun = ModuleManager.Instance.GenerateWeapon(_temp1); //Instantiate(weaponBodies[Random.Range(0, weaponBodies.Length - 1)], _leftSnap);
        _tempLeftGun.GetComponent<WeaponGenerator>().statBlock = _temp1;
        _tempLeftGun.transform.SetParent(_leftSnap);
        _tempLeftGun.transform.localPosition = Vector3.zero;
        _tempLeftGun.transform.localRotation = Quaternion.identity;
        _tempLeftGun.transform.localScale = m_scale;
        //_tempLeftGun.GetComponent<WeaponGenerator>().GenerateGun();

        WeaponData _temp2 = ModuleManager.Instance.CreateStatBlock();
        GameObject _tempRightGun = ModuleManager.Instance.GenerateWeapon(_temp2); //Instantiate(weaponBodies[Random.Range(0, weaponBodies.Length - 1)], _rightSnap);
        _tempRightGun.GetComponent<WeaponGenerator>().statBlock = _temp2;
        _tempRightGun.transform.SetParent(_rightSnap);
        _tempRightGun.transform.localPosition = Vector3.zero;
        _tempRightGun.transform.localRotation = Quaternion.identity;
        _tempRightGun.transform.localScale = m_scale;
        //_tempRightGun.GetComponent<WeaponGenerator>().GenerateGun();
    }
}
