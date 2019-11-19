using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(EnemyManager))]
public class EnemyShooting : MonoBehaviour
{
    #region Target
    private GameObject m_player;
    private GameObject m_target;
    [SerializeField, Tooltip("This object will be used to raycast towards the target to see if it is visable")]
    private GameObject m_lookAtPlayer;
    private bool m_targetInSight;
    #endregion

    #region Stats
    [SerializeField, Header("Stats"), Tooltip("The range that the bullets aim towards using the camera, 1000 is default")]
    private uint m_range = 1000;
    [SerializeField, Tooltip("The range that the bullets stay active for (Range of bullet)")]
    private float m_bulletLifeTime;
    [SerializeField, Tooltip("The speed that the projectile moves at")]
    private float m_bulletSpeed;
    [SerializeField, Tooltip("The damage that the projectile deals")]
    private float m_bulletDamage;
    [SerializeField, Tooltip("The attck angle that the enemy will fire within")]
    private float m_attackAngle;
    #endregion

    #region Cooldowns
    [SerializeField, Header("Cooldown"), Tooltip("The time taken for the right weapon to ready to fire")]
    private float m_rightWeaponCooldown;
    [SerializeField, Tooltip("The time taken for the right weapon to ready to fire")]
    private float m_leftWeaponCooldown;
    private bool m_rightWeaponActive = true;
    private bool m_leftWeaponActive = true;
    #endregion

    [SerializeField, Header("Bullet Spawning"), Tooltip("Put objetcs here that the bullet will spawn from, right weapon is position 0 left weapon is position 1")]
    private GameObject[] m_spawnLocations;

    [SerializeField, Header("Bullets"), Tooltip("Bullet prefab")]
    private GameObject m_bulletPrefab;

    private EnemyManager m_manager;

    // Start is called before the first frame update
    void Start()
    {
        m_manager = GetComponent<EnemyManager>();
        m_player = m_manager.Player;
        m_target = m_manager.Target;
    }

    // Update is called once per frame
    void Update()
    {
        m_target = m_manager.Target;

        if (m_target != null)
        {

            RaycastHit _hit;

            m_lookAtPlayer.transform.LookAt(m_player.transform); // Game object that always looks towards the target

            if (Vector3.Angle(m_player.transform.position - transform.position, transform.forward) < m_attackAngle && Physics.Raycast(m_lookAtPlayer.transform.position + (m_lookAtPlayer.transform.forward * 30), m_lookAtPlayer.transform.forward, out _hit, Mathf.Infinity))
            {
                if (_hit.transform.gameObject.tag == "Player")
                {
                    m_targetInSight = true;
                }
                else
                {
                    m_targetInSight = false;
                }
            }
            else
            {
                m_targetInSight = false;
            }

            if (m_targetInSight)
            {
                m_spawnLocations[0].transform.LookAt(m_player.transform);
                m_spawnLocations[1].transform.LookAt(m_player.transform);


                if (m_rightWeaponActive)
                {
                    SpawnBullet(0);
                    m_rightWeaponActive = false;
                    StartCoroutine(rightCooldown());
                }

                if (m_leftWeaponActive)
                {
                    SpawnBullet(1);
                    m_leftWeaponActive = false;
                    StartCoroutine(leftCooldown());
                }
            }
        }
    }

    void SpawnBullet(int _side)
    {
        GameObject newBullet = Instantiate(m_bulletPrefab, m_spawnLocations[_side].transform.position, m_spawnLocations[_side].transform.rotation);
        newBullet.GetComponent<BulletBehaviour>().SpawnedBy = gameObject;
        newBullet.GetComponent<BulletBehaviour>().LifeTime = m_bulletLifeTime;
        newBullet.GetComponent<BulletBehaviour>().Damage = m_bulletDamage;
        newBullet.GetComponent<BulletBehaviour>().Speed = m_bulletSpeed;
        newBullet.GetComponent<Rigidbody>().AddForce(newBullet.transform.forward * m_bulletSpeed * GameManager.Instance.GameSpeed, ForceMode.Impulse);
        newBullet.GetComponent<BulletBehaviour>().Instantiated();
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
