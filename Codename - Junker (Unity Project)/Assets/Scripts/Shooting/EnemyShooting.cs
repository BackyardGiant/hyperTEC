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
    [SerializeField, Tooltip("The attck angle that the enemy will fire within")]
    private float m_attackAngle;
    [SerializeField, Tooltip("The spread around the target")]
    private float m_universalBulletSpread;

    private float m_rightBulletDamage;
    private float m_leftBulletDamage;

    private float m_rightBulletAccuracy;
    private float m_leftBulletAccuracy;
    #endregion

    #region Cooldowns
    private float m_rightWeaponCooldown;
    private float m_leftWeaponCooldown;

    [SerializeField, Header("Cooldown"), Tooltip("The cooldown speed of a fire rate of 0.")]
    private float m_quickerFireRate;
    [SerializeField, Tooltip("The cooldown speed of a fire rate of 100.")]
    private float m_slowerFireRate;
    private bool m_rightWeaponActive = true;
    private bool m_leftWeaponActive = true;
    #endregion

    private GameObject m_leftBulletType;
    private GameObject m_rightBulletType;

    [SerializeField, Header("Bullets"), Tooltip("Bullet prefab")]
    private GameObject m_defaultBulletPrefab;
    [SerializeField]
    private GameObject m_traderBulletPrefab, m_exploratoryBulletPrefab, m_constructionBulletPrefab;

    [SerializeField, Header("Damage Ranges"), Tooltip("lowest and highest range of damage.")]
    private float m_lowDamage;
    [SerializeField]
    private float m_highDamage;

    [SerializeField, Header("Accuracy Ranges"), Tooltip("lowest and highest range of accuracy.")]
    private float m_lowAccuracy;
    [SerializeField]
    private float m_highAccuracy;


    private AudioSource m_rightWeaponSound, m_leftWeaponSound;
    private float m_rightWeaponSoundDelay, m_leftWeaponSoundDelay;

    [SerializeField, Header("Bullet Spawning"), Tooltip("Put objetcs here that the bullet will spawn from, right weapon is position 0 left weapon is position 1")]
    private GameObject[] m_spawnLocations;

    [SerializeField, Header("Bullets"), Tooltip("Bullet prefab")]
    private GameObject m_bulletPrefab;

    private EnemyManager m_manager;

    // Start is called before the first frame update
    void Start()
    {
        GameObject[] previousAudio = GameObject.FindGameObjectsWithTag("WeaponAudio");

        foreach (GameObject _source in previousAudio)
        {
            if (_source.transform.parent == gameObject.transform)
            {
                Destroy(_source);
            }
        }

        WeaponData _rightWeapon = null;
        WeaponData _leftWeapon = null;

        Transform _leftSnap = transform.Find("Ship").Find("LeftSnap");
        Transform _rightSnap = transform.Find("Ship").Find("RightSnap");

        _rightWeapon = _rightSnap.GetChild(0).GetComponent<WeaponGenerator>().statBlock;
        _leftWeapon = _leftSnap.GetChild(0).GetComponent<WeaponGenerator>().statBlock;

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
            m_rightWeaponSound = AudioManager.Instance.PlayEnemyGun((AudioManager.WeaponSounds)(int)_rightWeapon.CurrentFireRateType, _rightWeapon.FireRateIndex, this.gameObject, true);

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
            m_leftWeaponSound = AudioManager.Instance.PlayEnemyGun((AudioManager.WeaponSounds)(int)_leftWeapon.CurrentFireRateType, _leftWeapon.FireRateIndex, this.gameObject, true);

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
            if (GameManager.Instance.GameSpeed != 0)
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
                    Vector3 offset1 = Random.insideUnitSphere * (m_universalBulletSpread * (Vector3.Distance(m_player.transform.position, transform.position) / m_range));
                    Vector3 offset2 = Random.insideUnitSphere * (m_universalBulletSpread * (Vector3.Distance(m_player.transform.position, transform.position) / m_range));

                    m_spawnLocations[0].transform.LookAt(m_player.transform.position + (offset1 * (1 - m_rightBulletAccuracy)));
                    m_spawnLocations[1].transform.LookAt(m_player.transform.position + (offset2 * (1 - m_leftBulletAccuracy)));

                    float _random1 = Random.Range(1f, 2f);
                    float _random2 = Random.Range(1f, 2f);

                    if (m_rightWeaponActive)
                    {
                        m_rightWeaponSound.pitch = _random1;
                        m_rightWeaponSound.Play();
                        m_rightWeaponActive = false;
                        StartCoroutine(rightCooldown());
                    }

                    if (m_leftWeaponActive)
                    {
                        m_leftWeaponSound.pitch = _random2;
                        m_leftWeaponSound.Play();
                        m_leftWeaponActive = false;
                        StartCoroutine(leftCooldown());
                    }
                }
            }
        }
    }

    void SpawnBullet(int _side)
    {
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
}
