using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStats : MonoBehaviour
{
    [SerializeField, Header("Basic"), Tooltip("The maximum HP for an enemy")]
    private float m_maxHealth;
    [SerializeField, Tooltip("Current enemy HP")]
    private float m_currentHealth;

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
        try
        {
            Destroy(GetComponent<EnemyDetection>().Target.gameObject);
        }
        catch { Debug.Log("No target assigned to destroyed object"); }
        Destroy(gameObject);
    }
}
