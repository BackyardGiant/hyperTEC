using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShooting : MonoBehaviour
{
    [SerializeField, Header("Bullet Spawning"), Tooltip("Put objetcs here that the bullet will spawn from, right weapon is position 0 left weapon is position 1")]
    private GameObject[] m_spawnLocations;

    [SerializeField, Header("Bullets"), Tooltip("Bullet prefab")]
    private GameObject m_bulletPrefab;

    [SerializeField, Header("Camera"), Tooltip("This contains the main camera of the scene")]
    private Camera m_aimingCamera;

    [SerializeField, Header("Target")]
    private Transform m_target;

    private bool m_playerCanShoot = true;

    #region Cooldowns
    [SerializeField, Header("Cooldown"), Tooltip("The time taken for the right weapon to ready to fire")]
    private float m_rightWeaponCooldown;
    [SerializeField,Tooltip("The time taken for the right weapon to ready to fire")]
    private float m_leftWeaponCooldown;
    private bool m_rightWeaponActive = true;
    private bool m_leftWeaponActive = true;
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
    #endregion

    private static PlayerShooting s_instance;

    public static PlayerShooting Instance { get => s_instance; set => s_instance = value; }
    public GameObject[] SpawnLocations { get => m_spawnLocations; set => m_spawnLocations = value; }
    public bool PlayerCanShoot { get => m_playerCanShoot; set => m_playerCanShoot = value; }
    public float BulletSpeed { get => m_bulletSpeed; set => m_bulletSpeed = value; }

    private void Awake()
    {
        if (s_instance == null)
        {
            s_instance = this;
        }
        else
        {
            Destroy(s_instance.gameObject);
            s_instance = this;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        m_aimingCamera = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        RaycastHit hit;

        if (Physics.Raycast(m_aimingCamera.transform.position + (m_aimingCamera.transform.forward * 30), m_aimingCamera.transform.TransformDirection(Vector3.forward), out hit, m_range))
        {
            Debug.DrawRay(m_aimingCamera.transform.position + (m_aimingCamera.transform.forward * 30), m_aimingCamera.transform.TransformDirection(Vector3.forward) * hit.distance, Color.yellow);
            Debug.Log("Did Hit");
            m_target.position = hit.transform.position;
        }
        else
        {
            Debug.DrawRay(m_aimingCamera.transform.position + (m_aimingCamera.transform.forward * 30), m_aimingCamera.transform.TransformDirection(Vector3.forward) * m_range, Color.white);
            Debug.Log("Did not Hit");
            m_target.position = (m_aimingCamera.transform.position + (m_aimingCamera.transform.TransformDirection(Vector3.forward) * m_range));
        }

        m_spawnLocations[0].transform.LookAt(m_target);
        m_spawnLocations[1].transform.LookAt(m_target);

        if (Input.GetAxis("RightTrigger") > 0.1f && m_rightWeaponActive && m_playerCanShoot)
        {
            SpawnBullet(0);
            m_rightWeaponActive = false;
            StartCoroutine(rightCooldown());
        }

        if (Input.GetAxis("LeftTrigger") > 0.1f && m_leftWeaponActive && m_playerCanShoot)
        {
            SpawnBullet(1);
            m_leftWeaponActive = false;
            StartCoroutine(leftCooldown());
        }
    }

    void SpawnBullet(int _side)
    {
        GameObject newBullet = Instantiate(m_bulletPrefab, m_spawnLocations[_side].transform.position, m_spawnLocations[_side].transform.rotation);
        newBullet.GetComponent<BulletMovement>().LifeTime = m_bulletLifeTime;
        newBullet.GetComponent<BulletMovement>().Damage = m_bulletDamage;
        newBullet.GetComponent<Rigidbody>().AddForce(newBullet.transform.forward * m_bulletSpeed, ForceMode.Impulse);
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
