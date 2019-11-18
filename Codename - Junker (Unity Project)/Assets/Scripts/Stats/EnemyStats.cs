using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    public float CurrentHealth { get => m_currentHealth; }

    // Start is called before the first frame update
    void Start()
    {
        m_currentHealth = m_maxHealth;
    }

    public void TakeDamage(float _damageTaken)
    {
        m_currentHealth -= _damageTaken;
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

            int _value = PlayerPrefs.GetInt("EnemiesKilled");
            PlayerPrefs.SetInt("EnemiesKilled", _value + 1);
        }

        enemyCollider.enabled = false;
    }
}
