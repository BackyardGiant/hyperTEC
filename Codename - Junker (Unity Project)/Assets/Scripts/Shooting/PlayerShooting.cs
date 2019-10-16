using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShooting : MonoBehaviour
{
    [SerializeField, Header("Bullet Spawning"), Tooltip("Put objetcs here that the bullet will spawn from, right weapon is position 0 left weapon is position 1")]
    private GameObject[] m_spawnLocations;

    [SerializeField, Header("Bullets"), Tooltip("Bullet prefab")]
    private GameObject m_bulletPrefab;

    #region Cooldowns
    [SerializeField, Header("Cooldown"), Tooltip("The time taken for the right weapon to ready to fire")]
    private float m_rightWeaponCooldown;
    [SerializeField,Tooltip("The time taken for the right weapon to ready to fire")]
    private float m_leftWeaponCooldown;
    private bool m_rightWeaponActive = true;
    private bool m_leftWeaponActive = true;
    #endregion
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetAxis("RightTrigger") > 0.1f && m_rightWeaponActive)
        {
            SpawnBullet(0);
            m_rightWeaponActive = false;
            StartCoroutine(rightCooldown());
        }

        if (Input.GetAxis("LeftTrigger") > 0.1f && m_leftWeaponActive)
        {
            SpawnBullet(1);
            m_leftWeaponActive = false;
            StartCoroutine(leftCooldown());
        }
    }

    void SpawnBullet(int _side)
    {
        GameObject newBullet = Instantiate(m_bulletPrefab, m_spawnLocations[_side].transform.position, m_spawnLocations[0].transform.rotation);
        newBullet.GetComponent<BulletMovement>().LifeTime = 2f;
        newBullet.GetComponent<BulletMovement>().Instantiated();
    }

    IEnumerator rightCooldown()
    {
        yield return new WaitForSeconds(m_rightWeaponCooldown);
        m_rightWeaponActive = true;
    }

    IEnumerator leftCooldown()
    {
        yield return new WaitForSeconds(m_leftWeaponCooldown);
        m_leftWeaponActive = true;
    }
}
