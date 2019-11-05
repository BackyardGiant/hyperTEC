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
    [SerializeField, Tooltip("Contains the crosshair attached to the player camera")]
    private Crosshair m_aimingCrosshair;

    [SerializeField, Header("Target")]
    private Transform m_target;

    //[SerializeField, Header("Inventory")]
    //private Inventory playerInv;

    private Vector3 m_targetPosition;

    private bool m_playerCanShoot = true;

    #region Cooldowns
    [SerializeField, Header("Cooldown"), Tooltip("The cooldown speed of a fire rate of 0.")]
    private float m_quickerFireRate;
    [SerializeField, Tooltip("The cooldown speed of a fire rate of 100.")]
    private float m_slowerFireRate;
    private float m_rightWeaponCooldown;
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
    [SerializeField, Tooltip("The auto aim range, around 3")]
    private float m_autoAimDistance;
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

        m_autoAimDistance *= Mathf.Pow((Screen.height * Screen.width), 1f / 5f);
    }

    // Start is called before the first frame update
    void Start()
    {
        m_aimingCamera = Camera.main;

        float _range = -(m_slowerFireRate - m_quickerFireRate);
        float _rightFireRatePercentage = PlayerInventoryManager.Instance.EquippedRightWeapon.FireRate / 100;
        float _leftFireRatePercentage = PlayerInventoryManager.Instance.EquippedLeftWeapon.FireRate / 100;
        // Set the cooldowns of the equipped weapons. Should probably change to not be on Start() but it's okay here for now.
        m_rightWeaponCooldown = m_slowerFireRate + (_rightFireRatePercentage * _range);
        m_leftWeaponCooldown = m_slowerFireRate + (_leftFireRatePercentage * _range);
    }

    // Update is called once per frame
    void Update()
    {

        RaycastHit _hit;

        if (Physics.Raycast(m_aimingCamera.transform.position + (m_aimingCamera.transform.forward * 30), m_aimingCamera.transform.TransformDirection(Vector3.forward), out _hit, m_range))
        {
            Debug.DrawRay(m_aimingCamera.transform.position + (m_aimingCamera.transform.forward * 30), m_aimingCamera.transform.TransformDirection(Vector3.forward) * _hit.distance, Color.yellow);
            m_target.position = _hit.transform.position;
        }
        else
        {
            Debug.DrawRay(m_aimingCamera.transform.position + (m_aimingCamera.transform.forward * 30), m_aimingCamera.transform.TransformDirection(Vector3.forward) * m_range, Color.white);
            m_target.position = (m_aimingCamera.transform.position + (m_aimingCamera.transform.TransformDirection(Vector3.forward) * m_range));
        }

        if (HUDManager.Instance.ClosetEnemy != null)
        {
            if (Vector2.Distance(HUDManager.Instance.ClosetEnemyScreenPos, new Vector2(Screen.width / 2, Screen.height / 2)) < m_autoAimDistance)
            {
                m_aimingCrosshair.HasTarget = true;
                m_targetPosition = HUDManager.Instance.ClosetEnemy.transform.position;
                m_aimingCrosshair.TargetPosition = new Vector2(HUDManager.Instance.ClosetEnemyScreenPos.x, HUDManager.Instance.ClosetEnemyScreenPos.y);
                m_target.position = m_targetPosition;
            }
            else
            {
                m_aimingCrosshair.HasTarget = false;
            }
        }

        m_spawnLocations[0].transform.LookAt(m_target);
        m_spawnLocations[1].transform.LookAt(m_target);

        if (Input.GetAxis("RightTrigger") > 0.1f && m_rightWeaponActive && m_playerCanShoot && PlayerInventoryManager.Instance.EquippedRightWeapon != null)
        {
            SpawnBullet(0);
            m_rightWeaponActive = false;
            StartCoroutine(rightCooldown());
        }

        if (Input.GetAxis("LeftTrigger") > 0.1f && m_leftWeaponActive && m_playerCanShoot && PlayerInventoryManager.Instance.EquippedLeftWeapon != null)
        {
            SpawnBullet(1);
            m_leftWeaponActive = false;
            StartCoroutine(leftCooldown());
        }


    }

    // 0 means right hand side, 1 means left hand side
    void SpawnBullet(int _side)
    {
        //Temporary implementation of shooting sounds
        float _random = Random.Range(1f, 2f);
        AudioManager.Instance.Pitch("SimpleShoot", _random);
        AudioManager.Instance.Play("SimpleShoot");
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
