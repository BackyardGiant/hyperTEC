using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TempEnemyInstantiate : MonoBehaviour
{
    public GameObject[] weaponBodies;

    public Transform spawnPoint;

    public GameObject enemyPrefab;

    public int enemyLimit;

    private Vector3 m_scale = new Vector3(1, 1, 1);

    private void Start()
    {
        for (int i = 0; i < enemyLimit; i++)
        {
            SpawnEnemy();
        }
    }

    private void Update()
    {
        if(GameObject.FindGameObjectsWithTag("Enemy").Length < enemyLimit)
        {
            SpawnEnemy();
        }
    }

    private void SpawnEnemy()
    {
        GameObject _tempEnemy = Instantiate(enemyPrefab, Random.insideUnitSphere * 300, spawnPoint.rotation);

        Transform _leftSnap = _tempEnemy.transform.Find("ConstructionShip#1").Find("LeftSnap");
        Transform _rightSnap = _tempEnemy.transform.Find("ConstructionShip#1").Find("RightSnap");

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
