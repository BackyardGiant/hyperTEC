using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField]
    private GameEvent m_playerDeath;

    [SerializeField]
    private IntVariable[] m_playerHealth = new IntVariable[4];

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
            m_playerHealth[_currentSave].Value += (int)m_rechargeRate;
            HUDManager.Instance.Healthbar.fillAmount = (float)m_playerHealth[_currentSave].Value / m_healthMax;
        }

        m_timeSinceDamage += Time.deltaTime;
    }

    public void TakeDamage(float _dmg)
    {
        if (m_playerHealth[_currentSave].Value - _dmg > 0)
        {
            m_playerHealth[_currentSave].Value -= (int)_dmg;
            AudioManager.Instance.PlayWorld("ExplosionShort4", this.gameObject, false, true);
            HUDManager.Instance.Healthbar.fillAmount = (float)m_playerHealth[_currentSave].Value / m_healthMax;
            m_timeSinceDamage = 0;
            Debug.Log("<color=green>CURRENT HEALTH : </color>" + m_playerHealth[_currentSave].Value);
        }
        else
        {
            _isDead = true;
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
        foreach(IntVariable healthBar in m_playerHealth)
        {
            healthBar.Value = m_healthMax;
        }
    }
}
