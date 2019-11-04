using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TempEnemyInstantiate : MonoBehaviour
{
    public GameObject[] weaponBodies;

    public Transform spawnPoint;

    public GameObject enemyPrefab;

    public int enemyLimit;

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
        GameObject _tempEnemy = Instantiate(enemyPrefab, Random.insideUnitSphere * 100, spawnPoint.rotation);

        Transform _leftSnap = _tempEnemy.transform.Find("ConstructionShip#1").Find("LeftSnap");
        Transform _rightSnap = _tempEnemy.transform.Find("ConstructionShip#1").Find("RightSnap");

        GameObject _tempLeftGun = Instantiate(weaponBodies[Random.Range(0, weaponBodies.Length - 1)], _leftSnap);
        _tempLeftGun.transform.localPosition = Vector3.zero;
        _tempLeftGun.GetComponent<WeaponGenerator>().GenerateGun();

        GameObject _tempRightGun = Instantiate(weaponBodies[Random.Range(0, weaponBodies.Length - 1)], _rightSnap);
        _tempRightGun.transform.localPosition = Vector3.zero;
        _tempRightGun.GetComponent<WeaponGenerator>().GenerateGun();
    }
}
