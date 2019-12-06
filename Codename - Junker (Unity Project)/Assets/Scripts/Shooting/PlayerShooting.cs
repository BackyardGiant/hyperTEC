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
    private float m_rightWeaponSoundDelay, m_leftWeaponSoundDelay;

    private bool m_playerCanShoot = true;

    #region Cooldowns
    private float m_rightWeaponCooldown;
    private float m_leftWeaponCooldown;
    private bool m_rightWeaponActive = true;
    private bool m_leftWeaponActive = true;
    #endregion

    [SerializeField, Header("Damage Ranges"), Tooltip("lowest and highest range of damage.")]
    private float m_lowDamage;
    [SerializeField]
    private float m_highDamage;

    [SerializeField, Header("Accuracy Ranges"), Tooltip("lowest and highest range of accuracy.")]
    private float m_lowAccuracy;
    [SerializeField]
    private float m_highAccuracy;

    [SerializeField, Header("Cooldown"), Tooltip("The cooldown speed of a fire rate of 0.")]
    private float m_quickerFireRate;
    [SerializeField, Tooltip("The cooldown speed of a fire rate of 100.")]
    private float m_slowerFireRate;

    #region Stats
    [SerializeField, Header("Stats"), Tooltip("The range that the bullets aim towards using the camera, 1000 is default")]
    private uint m_range = 1000;
    [SerializeField, Tooltip("The range that the bullets stay active for (Range of bullet)")]
    private float m_bulletLifeTime;
    [SerializeField, Tooltip("The speed that the projectile moves at")]
    private float m_bulletSpeed;
    [SerializeField, Tooltip("The auto aim range, around 3")]
    private float m_autoAimDistance;
    [SerializeField, Tooltip("The spread around the target")]
    private float m_universalBulletSpread;

    private float m_rightBulletDamage;
    private float m_leftBulletDamage;

    private float m_rightBulletAccuracy;
    private float m_leftBulletAccuracy;
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
        buildWeapons();        
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.Instance.GameSpeed != 0)
        {
            if (m_spawnLocations[0] != null && m_spawnLocations[1] != null)
            {
                RaycastHit _hit;

                if (Physics.Raycast(m_aimingCamera.transform.position + (m_aimingCamera.transform.forward * 100), m_aimingCamera.transform.TransformDirection(Vector3.forward), out _hit, m_range))
                {
                    Debug.DrawRay(m_aimingCamera.transform.position + (m_aimingCamera.transform.forward * 100), m_aimingCamera.transform.TransformDirection(Vector3.forward) * _hit.distance, Color.yellow);
                    m_target.position = _hit.transform.position;
                }
                else
                {
                    Debug.DrawRay(m_aimingCamera.transform.position + (m_aimingCamera.transform.forward * 30), m_aimingCamera.transform.TransformDirection(Vector3.forward) * m_range, Color.white);
                    m_target.position = (m_aimingCamera.transform.position + (m_aimingCamera.transform.TransformDirection(Vector3.forward) * m_range));
                }

                if (HUDManager.Instance.ClosetEnemy != null && HUDManager.Instance.ClosetEnemy.GetComponent<EnemyStats>().m_currentFaction.ToString() != returnFaction())
                {
                    if (Vector2.Distance(HUDManager.Instance.ClosestEnemyScreenPos, new Vector2(Screen.width / 2, Screen.height / 2)) < m_autoAimDistance)
                    {
                        m_aimingCrosshair.HasTarget = true;
                        m_targetPosition = HUDManager.Instance.ClosetEnemy.transform.position + (HUDManager.Instance.ClosetEnemy.GetComponent<Rigidbody>().velocity * ((HUDManager.Instance.ClosetEnemy.GetComponent<Rigidbody>().velocity.magnitude / m_bulletSpeed) + Vector3.Distance(HUDManager.Instance.ClosetEnemy.transform.position, transform.position) * 0.0008f));
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

                Vector3 offset1 = Random.insideUnitSphere * (m_universalBulletSpread * (Vector3.Distance(m_target.position, transform.position) / m_range));
                Vector3 offset2 = Random.insideUnitSphere * (m_universalBulletSpread * (Vector3.Distance(m_target.position, transform.position) / m_range));

                m_spawnLocations[0].transform.LookAt(m_target.position + (offset1 * (1 - m_rightBulletAccuracy)));
                m_spawnLocations[1].transform.LookAt(m_target.position + (offset2 * (1 - m_leftBulletAccuracy)));

                float _random1 = Random.Range(1f, 2f);
                float _random2 = Random.Range(1f, 2f);

                if (Input.GetAxis("RightTrigger") > 0.1f && m_rightWeaponActive && m_playerCanShoot && PlayerInventoryManager.Instance.EquippedRightWeapon != null)
                {
                    m_rightWeaponSound.pitch = _random1;
                    m_rightWeaponSound.Play();
                    m_rightWeaponActive = false;
                    StartCoroutine(rightCooldown());
                }

                if (Input.GetAxis("LeftTrigger") > 0.1f && m_leftWeaponActive && m_playerCanShoot && PlayerInventoryManager.Instance.EquippedLeftWeapon != null)
                {
                    m_leftWeaponSound.pitch = _random2;
                    m_leftWeaponSound.Play();
                    m_leftWeaponActive = false;
                    StartCoroutine(leftCooldown());
                }
            }
        }
    }

    // 0 means right hand side, 1 means left hand side
    void SpawnBullet(int _side)
    {
        //Temporary implementation of shooting sounds
        GameObject newBullet;
        if (_side == 0)
        {
            newBullet = Instantiate(m_rightBulletType, m_spawnLocations[_side].transform.position, m_spawnLocations[_side].transform.rotation);
            newBullet.GetComponent<BulletBehaviour>().Damage = m_rightBulletDamage;
        }
        else
        {
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
        yield return new WaitForSeconds(m_rightWeaponSoundDelay);
        SpawnBullet(0);
        yield return new WaitForSeconds(m_rightWeaponCooldown);
        m_rightWeaponActive = true;
    }
    IEnumerator leftCooldown()
    {
        yield return new WaitForSeconds(m_leftWeaponSoundDelay);
        SpawnBullet(1);
        yield return new WaitForSeconds(m_leftWeaponCooldown);
        m_leftWeaponActive = true;
    }

    public void buildWeapons()
    {
        GameObject[] previousAudio = GameObject.FindGameObjectsWithTag("WeaponAudio");

        foreach(GameObject _source in previousAudio)
        {
            Destroy(_source);
        }

        WeaponData _rightWeapon = null;
        WeaponData _leftWeapon = null;

        try
        {
            _rightWeapon = PlayerInventoryManager.Instance.EquippedRightWeapon;
        }
        catch { }

        try
        {
            _leftWeapon = PlayerInventoryManager.Instance.EquippedLeftWeapon;
        }
        catch { }

        m_aimingCamera = Camera.main;

        float _rangeFireRate = -(m_slowerFireRate - m_quickerFireRate);
        float _rangeDamage = (m_highDamage - m_lowDamage);
        float _rangeAccuracy = (m_highAccuracy - m_lowAccuracy);

        float _leftFireRatePercentage;
        float _rightFireRatePercentage;
        float _leftDamagePercentage;
        float _rightDamagePercentage;
        float _leftAccuracyPercentage;
        float _rightAccuracyPercentage;

        if (_rightWeapon != null)
        {
            //SOUND
            m_rightWeaponSound = AudioManager.Instance.PlayWeapon((AudioManager.WeaponSounds)(int)_rightWeapon.CurrentFireRateType, _rightWeapon.FireRateIndex, this.gameObject, true);

            if (_rightWeapon.CurrentFireRateType == WeaponData.fireRateType.slow)
            {
                m_rightWeaponSoundDelay = AudioManager.Instance.longWeaponDelays[_rightWeapon.FireRateIndex];
            }
            else
            {
                m_rightWeaponSoundDelay = 0;
            }

            //FIRE RATE
            _rightFireRatePercentage = _rightWeapon.FireRate / 100f;
            m_rightWeaponCooldown = m_slowerFireRate + (_rightFireRatePercentage * _rangeFireRate);

            //DAMAGE
            _rightDamagePercentage = _rightWeapon.Damage / 100f;
            m_rightBulletDamage = m_lowDamage + (_rangeDamage * _rightDamagePercentage);

            //Accuracy
            _rightAccuracyPercentage = _rightWeapon.Accuracy / 100f;
            m_rightBulletAccuracy = m_lowAccuracy + (_rangeAccuracy * _rightAccuracyPercentage);

            //ACCURACY

            //RELOAD TIME

            //BULLET MATERIAL
            if (_rightWeapon.CurrentFaction == WeaponData.faction.initial)
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
            m_leftWeaponSound = AudioManager.Instance.PlayWeapon((AudioManager.WeaponSounds)(int)_leftWeapon.CurrentFireRateType, _leftWeapon.FireRateIndex, this.gameObject, true);

            if (_leftWeapon.CurrentFireRateType == WeaponData.fireRateType.slow)
            {
                m_leftWeaponSoundDelay = AudioManager.Instance.longWeaponDelays[_leftWeapon.FireRateIndex];
            }
            else
            {
                m_leftWeaponSoundDelay = 0;
            }

            //FIRE RATE
            _leftFireRatePercentage = _leftWeapon.FireRate / 100f;
            m_leftWeaponCooldown = m_slowerFireRate + (_leftFireRatePercentage * _rangeFireRate);

            //DAMAGE
            _leftDamagePercentage = _leftWeapon.Damage / 100f;
            m_leftBulletDamage = m_lowDamage + (_rangeDamage * _leftDamagePercentage);

            //ACCURACY
            _leftAccuracyPercentage = _leftWeapon.Accuracy / 100f;
            m_leftBulletAccuracy = m_lowAccuracy + (_rangeAccuracy * _leftAccuracyPercentage);

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

    private string returnFaction()
    {
        string _result = "NotSet";

        string _saveName = PlayerPrefs.GetString("CurrentSave", "NoSave");
        char _saveIndex = _saveName[4];
        string _factionName = PlayerPrefs.GetString("ChosenFaction" + _saveIndex);

        switch (_factionName)
        {
            case "initial":
                _result = "initial";
                break;
            case "trader":
                _result = "trader";
                break;
            case "exploratory":
                _result = "explorer";
                break;
            case "construction":
                _result = "construction";
                break;
        }
        return _result;
    }
}
