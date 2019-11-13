using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShooting : MonoBehaviour
{
    [SerializeField, Header("Bullet Spawning"), Tooltip("Put objetcs here that the bullet will spawn from, right weapon is position 0 left weapon is position 1")]
    private GameObject[] m_spawnLocations;

    [SerializeField, Header("Bullets"), Tooltip("Bullet prefab")]
    private GameObject m_defaultBulletPrefab;
    [SerializeField]
    private GameObject m_traderBulletPrefab, m_exploratoryBulletPrefab, m_constructionBulletPrefab;

    [SerializeField, Header("Camera"), Tooltip("This contains the main camera of the scene")]
    private Camera m_aimingCamera;
    [SerializeField, Tooltip("Contains the crosshair attached to the player camera")]
    private Crosshair m_aimingCrosshair;

    [SerializeField, Header("Target")]
    private Transform m_target;

    //[SerializeField, Header("Inventory")]
    //private Inventory playerInv;

    private Vector3 m_targetPosition;
    private AudioSource m_rightWeaponSound, m_leftWeaponSound;

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

    [SerializeField, Header("Damage Ranges"), Tooltip("lowest and highest range of damage.")]
    private float m_lowDamage;
    [SerializeField]
    private float m_highDamage;

    #region Stats
    [SerializeField, Header("Stats"), Tooltip("The range that the bullets aim towards using the camera, 1000 is default")]
    private uint m_range = 1000;
    [SerializeField, Tooltip("The range that the bullets stay active for (Range of bullet)")]
    private float m_bulletLifeTime;
    [SerializeField, Tooltip("The speed that the projectile moves at")]
    private float m_bulletSpeed;
    [SerializeField, Tooltip("The auto aim range, around 3")]
    private float m_autoAimDistance;



    private float m_rightBulletDamage;
    private float m_leftBulletDamage;
    #endregion


    private GameObject m_leftBulletType;
    private GameObject m_rightBulletType;

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

        WeaponData _rightWeapon = null;
        WeaponData _leftWeapon = null;

        try
        {
             _rightWeapon = PlayerInventoryManager.Instance.EquippedRightWeapon;
        }
        catch{ }

        try
        {
            _leftWeapon = PlayerInventoryManager.Instance.EquippedLeftWeapon;
        }
        catch { }

        m_aimingCamera = Camera.main;

        float _rangeFireRate = -(m_slowerFireRate - m_quickerFireRate);
        float _rangeDamage = (m_highDamage - m_lowDamage);

        float _leftFireRatePercentage;
        float _rightFireRatePercentage;
        float _leftDamagePercentage;
        float _rightDamagePercentage;


        if(_rightWeapon != null)
        {
            //SOUND
            m_rightWeaponSound = AudioManager.Instance.PlayWeapon((AudioManager.WeaponSounds)(int)_rightWeapon.CurrentFireRateType, _rightWeapon.FireRateIndex, this.gameObject, true);
            
            //FIRE RATE
            _rightFireRatePercentage = _rightWeapon.FireRate / 100f;
            m_rightWeaponCooldown = m_slowerFireRate + (_rightFireRatePercentage * _rangeFireRate);

            //DAMAGE
            _rightDamagePercentage = _rightWeapon.Damage / 100f;
            m_rightBulletDamage = m_lowDamage + (_rangeDamage * _rightDamagePercentage);

            //ACCURACY

            //RELOAD TIME

            //BULLET MATERIAL
            if(_rightWeapon.CurrentFaction == WeaponData.faction.initial)
            {
                m_rightBulletType = m_defaultBulletPrefab;
            }
            else if (_rightWeapon.CurrentFaction == WeaponData.faction.trader)
            {
                m_rightBulletType = m_traderBulletPrefab;
            }
            else if (_rightWeapon.CurrentFaction == WeaponData.faction.explorer)
            {
                m_rightBulletType = m_exploratoryBulletPrefab;
            }
            else if (_rightWeapon.CurrentFaction == WeaponData.faction.construction)
            {
                m_rightBulletType = m_constructionBulletPrefab;
            }

        }

        if (_leftWeapon != null)
        {
            //SOUND
            m_leftWeaponSound = AudioManager.Instance.PlayWeapon((AudioManager.WeaponSounds)(WeaponData.fireRateType)_leftWeapon.CurrentFireRateType, _leftWeapon.FireRateIndex, this.gameObject, true);

            //FIRE RATE
            _leftFireRatePercentage = _leftWeapon.FireRate / 100f;
            m_leftWeaponCooldown = m_slowerFireRate + (_leftFireRatePercentage * _rangeFireRate);

            //DAMAGE
            _leftDamagePercentage = _leftWeapon.Damage / 100f;
            m_leftBulletDamage = m_lowDamage + (_rangeDamage * _leftDamagePercentage);

            //ACCURACY

            //RELOAD TIME

            //BULLET MATERIAL
            if (_leftWeapon.CurrentFaction == WeaponData.faction.initial)
            {
                m_leftBulletType = m_defaultBulletPrefab;
            }
            else if (_leftWeapon.CurrentFaction == WeaponData.faction.trader)
            {
                m_leftBulletType = m_traderBulletPrefab;
            }
            else if (_leftWeapon.CurrentFaction == WeaponData.faction.explorer)
            {
                m_leftBulletType = m_exploratoryBulletPrefab;
            }
            else if (_leftWeapon.CurrentFaction == WeaponData.faction.construction)
            {
                m_leftBulletType = m_constructionBulletPrefab;
            }


        }
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.Instance.GameSpeed != 0)
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
                if (Vector2.Distance(HUDManager.Instance.ClosestEnemyScreenPos, new Vector2(Screen.width / 2, Screen.height / 2)) < m_autoAimDistance)
                {
                    m_aimingCrosshair.HasTarget = true;
                    m_targetPosition = HUDManager.Instance.ClosetEnemy.transform.position;
                    m_aimingCrosshair.TargetPosition = new Vector2(HUDManager.Instance.ClosestEnemyScreenPos.x, HUDManager.Instance.ClosestEnemyScreenPos.y);
                    m_target.position = m_targetPosition;
                }
                else
                {
                    m_aimingCrosshair.HasTarget = false;
                }
            }
            else
            {
                m_aimingCrosshair.HasTarget = false;
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
    }

    // 0 means right hand side, 1 means left hand side
    void SpawnBullet(int _side)
    {
        //Temporary implementation of shooting sounds
        float _random = Random.Range(1f, 2f);
        GameObject newBullet;
        if (_side == 0)
        {
            m_rightWeaponSound.pitch = _random;
            m_rightWeaponSound.Play();
            newBullet = Instantiate(m_rightBulletType, m_spawnLocations[_side].transform.position, m_spawnLocations[_side].transform.rotation);
            newBullet.GetComponent<BulletBehaviour>().Damage = m_rightBulletDamage;
        }
        else
        {
            m_leftWeaponSound.pitch = _random;
            m_leftWeaponSound.Play();
            newBullet = Instantiate(m_leftBulletType, m_spawnLocations[_side].transform.position, m_spawnLocations[_side].transform.rotation);
            newBullet.GetComponent<BulletBehaviour>().Damage = m_leftBulletDamage;


        }

        newBullet.GetComponent<BulletBehaviour>().SpawnedBy = gameObject;
        newBullet.GetComponent<BulletBehaviour>().LifeTime = m_bulletLifeTime;

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
