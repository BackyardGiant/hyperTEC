using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum faction { explorer, initial, trader, construction}

public class EnemyStats : MonoBehaviour
{
    [SerializeField, Header("Basic"), Tooltip("The maximum HP for an enemy")]
    private float m_maxHealth;
    [SerializeField, Tooltip("Current enemy HP")]
    private float m_currentHealth;
    [SerializeField]
    private GameEvent m_death;
    [SerializeField]
    private GameObject m_explosion;

    [SerializeField]
    private DropWeapons m_dropWeaponsScript;


    [SerializeField]
    private ParticleSystem m_smokeEffect;

    private EnemyManager m_manager; // For temp stuff but don't remove 03/12/2019
    private float m_smokeEmissionCount;
    

    public faction m_currentFaction;

    public float CurrentHealth { get => m_currentHealth; }

    // Start is called before the first frame update
    void Start()
    {
        m_manager = GetComponent<EnemyManager>();
        m_currentHealth = m_maxHealth;

        var emission = m_smokeEffect.emission;
        m_smokeEmissionCount = emission.rateOverTime.constant;
        emission.rateOverTime = 0;
    }

    public void TakeDamage(float _damageTaken)
    {
        compareFaction();
        m_currentHealth -= _damageTaken;

        /////////////////////////////////////////////
        // For temp stuff 03/12/2019

        m_manager.AttackingPlayer = true;

        /////////////////////////////////////////////
       
        ReactToHealthChange();
    }

    public void Heal(float _healthToGain)
    {
        m_currentHealth += _healthToGain;
        m_currentHealth = Mathf.Clamp(m_currentHealth, 0, m_maxHealth);
        ReactToHealthChange();
    }

    private void ReactToHealthChange()
    {
        float _healthPercentage = 1 - m_currentHealth / m_maxHealth; //Inverted to control emission of smoke
        var emission = m_smokeEffect.emission;
        emission.rateOverTime = m_smokeEmissionCount * _healthPercentage;
        if (m_currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Collider enemyCollider = GetComponent<Collider>();
        try
        {
            Destroy(GetComponent<EnemyDetection>().EnemyTarget.gameObject);
        }
        catch { Debug.Log("No target assigned to destroyed object"); }
        m_death.Raise();



        if (enemyCollider.enabled == true)
        {
            m_dropWeaponsScript.Drop();
            Instantiate(m_explosion, transform.position, transform.rotation);
            int _random = Random.Range(1, 4);
            AudioManager.Instance.PlayWorld("ExplosionLong" + _random, this.gameObject, true, true);

            Destroy(gameObject);

            QuestManager.Instance.IncrementKillQuests();

            string _save = PlayerPrefs.GetString("CurrentSave");
            int _value;
            if (_save == "Save1") {
                _value = PlayerPrefs.GetInt("EnemiesKilled1");
                PlayerPrefs.SetInt("EnemiesKilled1", _value + 1);
            }
            else if (_save == "Save2") {
                _value = PlayerPrefs.GetInt("EnemiesKilled2");
                PlayerPrefs.SetInt("EnemiesKilled2", _value + 1);
            }
            else if (_save == "Save3")
            {
                _value = PlayerPrefs.GetInt("EnemiesKilled3");
                PlayerPrefs.SetInt("EnemiesKilled3", _value + 1);
            }
            else if (_save == "Save4")
            {
                _value = PlayerPrefs.GetInt("EnemiesKilled4");
                PlayerPrefs.SetInt("EnemiesKilled4", _value + 1);
            }


            _value = PlayerPrefs.GetInt("EnemiesKilled");
            PlayerPrefs.SetInt("EnemiesKilled", _value + 1);
        }

        enemyCollider.enabled = false;
    }

    private bool compareFaction()
    {
        bool _value;
        if (returnPlayerFaction() == this.m_currentFaction.ToString())
        {
            _value = true;
            try { HUDManager.Instance.DisplayWarningSetAmount("Friendly Fire", 1); } catch { }
        }
        else
        {
            _value = false;
        }


        return _value;
    }

    private string returnPlayerFaction()
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
