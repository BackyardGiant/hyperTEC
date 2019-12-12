using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private UsabilityTestingController m_testing;
    [SerializeField]
    private GameEvent m_playerDeath;

    [SerializeField]
    private FloatVariable[] m_playerHealth = new FloatVariable[4];

    [SerializeField]
    private int m_healthMax;

    [SerializeField]
    private float m_rechargeRate, m_rechargePeriod;

    private float m_timeSinceDamage;

    private int _currentSave;

    private bool _isRecharging = false;
    private bool _isDead = false;

    [SerializeField]
    private GameObject m_explosion;
    [SerializeField]
    private DropWeapons m_dropWeaponsScript;

    [SerializeField]
    [Range(0f, 1f)]
    private float m_imageFill;

    [SerializeField]
    GameObject[] m_playerToBeDestroyed;

    private void Start()
    {
        switch(PlayerPrefs.GetString("CurrentSave"))
        {
            case "Save1":
                _currentSave = 0;
                break;
            case "Save2":
                _currentSave = 1;
                break;
            case "Save3":
                _currentSave = 2;
                break;
            case "Save4":
                _currentSave = 3;
                break;
        }
        ResetHealth();
        _isDead = false;
        HUDManager.Instance.Healthbar.fillAmount = m_playerHealth[_currentSave].Value;
    }

    private void Update()
    {
        if (m_timeSinceDamage > m_rechargePeriod && m_playerHealth[_currentSave].Value != m_healthMax && !_isDead)
        {
            m_playerHealth[_currentSave].Value += (m_rechargeRate * 50 * Time.deltaTime);
            HUDManager.Instance.Healthbar.fillAmount = (float)m_playerHealth[_currentSave].Value / m_healthMax;
        }

        m_timeSinceDamage += Time.deltaTime;
    }

    public void TakeDamage(float _dmg)
    {
        m_testing.sendCombatPos();
        if (m_playerHealth[_currentSave].Value - _dmg > 0)
        {
            m_playerHealth[_currentSave].Value -= _dmg;
            AudioManager.Instance.PlayWorld("PlayerTakeDamage", this.gameObject, false, true);
            HUDManager.Instance.Healthbar.fillAmount = m_playerHealth[_currentSave].Value / m_healthMax;
            m_timeSinceDamage = 0;
            //Debug.Log("<color=green>CURRENT HEALTH : </color>" + m_playerHealth[_currentSave].Value);
            CameraShake.Instance.Shake(0.15f, 0.3f);
        }
        else
        {

            _isDead = true;
            GameManager.Instance.SaveGame(_isDead);
            m_playerDeath.Raise();

            HUDManager.Instance.Healthbar.fillAmount = 0;

            GetComponent<Rigidbody>().velocity = Vector3.zero;

            GetComponent<Collider>().enabled = false;
            GetComponent<PlayerMovement>().enabled = false;

            m_dropWeaponsScript.DropWithoutChance();
            Instantiate(m_explosion, transform.position, transform.rotation);
            int _random = Random.Range(1, 4);
            AudioManager.Instance.PlayWorld("ExplosionLong" + _random, this.gameObject, true, true);

            foreach(GameObject _objectToDestroy in m_playerToBeDestroyed)
            {
                Destroy(_objectToDestroy);
            }

            GameManager.Instance.ReturnToMenuDelayed(2f);

            Debug.Log("<color=green>DEAD</color>");
        }      
    }

    public void ResetHealth(int _save)
    {
        m_playerHealth[_save].Value = m_healthMax;
    }

    private void ResetHealth()
    {
        foreach(FloatVariable healthBar in m_playerHealth)
        {
            healthBar.Value = m_healthMax;
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.name.Contains("asteroid") || collision.gameObject.name.Contains("Asteroid"))
        {
            //Explode when you hit an asteroid
            Vector3 _impactVelocity = transform.GetComponent<Rigidbody>().GetPointVelocity(collision.transform.position);
            Vector3 _impactVelAbs = new Vector3(Mathf.Abs(_impactVelocity.x), Mathf.Abs(_impactVelocity.y), Mathf.Abs(_impactVelocity.z));

            //If the impact velocity is above a certain threshold, die on impact. I have tested this to be suitable at 40.
            if (_impactVelAbs.x > 90.0f || _impactVelAbs.y > 90.0f || _impactVelAbs.z > 90.0f)
            {
                m_testing.sendCombatPos();
                _isDead = true;
                GameManager.Instance.SaveGame(_isDead);
                m_playerDeath.Raise();

                HUDManager.Instance.Healthbar.fillAmount = 0;

                GetComponent<Rigidbody>().velocity = Vector3.zero;

                GetComponent<Collider>().enabled = false;
                GetComponent<PlayerMovement>().enabled = false;

                m_dropWeaponsScript.DropWithoutChance();
                Instantiate(m_explosion, transform.position, transform.rotation);
                int _random = Random.Range(1, 4);
                AudioManager.Instance.PlayWorld("ExplosionLong" + _random, this.gameObject, true, true);

                foreach (GameObject _objectToDestroy in m_playerToBeDestroyed)
                {
                    Destroy(_objectToDestroy);
                }

                GameManager.Instance.ReturnToMenuDelayed(2f);
            }
            else if (_impactVelAbs.x > 20.0f || _impactVelAbs.y > 20.0f || _impactVelAbs.z > 20.0f)
            {
                //Take a third of your health if you hit an asteroid at a decent speed
                TakeDamage(m_healthMax / 2);
            }
        }

        if (collision.gameObject.tag == "Enemy")
        {
            //Explode when you hit an asteroid
            Vector3 _impactVelocity = transform.GetComponent<Rigidbody>().GetPointVelocity(collision.transform.position);
            Vector3 _impactVelAbs = new Vector3(Mathf.Abs(_impactVelocity.x), Mathf.Abs(_impactVelocity.y), Mathf.Abs(_impactVelocity.z));
            if (_impactVelAbs.x > 60.0f || _impactVelAbs.y > 60.0f || _impactVelAbs.z > 60.0f)
            {
                //Take a third of your health if you hit an asteroid at a decent speed
                TakeDamage(m_healthMax / 3);
            }
        }
    }

}
