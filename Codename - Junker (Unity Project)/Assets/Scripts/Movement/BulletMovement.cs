﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletMovement : MonoBehaviour
{
    private float m_lifeTime;
    private float m_speed;
    private float m_damage;
    private Rigidbody m_rbBullet;
    private bool m_hadPreSloMo;

    #region Slow Mo Manager
    private Vector3 m_velocityBeforeSlow;
    private Vector3 m_angleVelocityBeforeSlow;
    private Vector3 m_velocityAfterSlow;
    private Vector3 m_angleVelocityAfterSlow;
    private float m_velocityDifferenceBetweenSlow;
    private float m_angleVelocityDifferenceBetweenSlow;
    #endregion

    public float LifeTime { get => m_lifeTime; set => m_lifeTime = value; }
    public float Speed { get => m_speed; set => m_speed = value; }
    public float Damage { get => m_damage; set => m_damage = value; }

    // Start is called before the first frame update
    void Start()
    {
        m_rbBullet = gameObject.GetComponent<Rigidbody>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.tag == "Enemy")
        {
            collision.transform.GetComponent<EnemyStats>().TakeDamage(m_damage);
        }
        if (collision.transform.tag != "Bullet")
        {
            Destroy(gameObject);
        }
    }

    public void Instantiated()
    {
        Invoke("DestroySelf", m_lifeTime);
    }

    void DestroySelf()
    {
        Destroy(gameObject);
    }

    /// <summary>
    /// Saves forces before slow mo
    /// </summary>
    public void RespondToPreSlowMo()
    {
        m_velocityBeforeSlow = m_rbBullet.velocity;
        m_angleVelocityBeforeSlow = m_rbBullet.angularVelocity;
        m_hadPreSloMo = true;
    }

    /// <summary>
    /// Applies slow mo and saves forces after it has been applied
    /// </summary>
    public void RespondToPostSlowMo()
    {
        if (m_hadPreSloMo)
        {
            m_rbBullet.velocity *= GameManager.Instance.GameSpeed;
            m_rbBullet.angularVelocity *= GameManager.Instance.GameSpeed;

            m_velocityAfterSlow = m_rbBullet.velocity;
            m_angleVelocityAfterSlow = m_rbBullet.angularVelocity;

            m_velocityDifferenceBetweenSlow = m_velocityBeforeSlow.magnitude - m_velocityAfterSlow.magnitude;
            m_angleVelocityDifferenceBetweenSlow = m_angleVelocityBeforeSlow.magnitude - m_angleVelocityAfterSlow.magnitude;
        }
    }

    /// <summary>
    /// Returns lost force to object using the difference between before and after slow mo
    /// </summary>
    public void RespondToNormalSpeed()
    {
        if (m_hadPreSloMo)
        {
            m_rbBullet.velocity += m_rbBullet.velocity.normalized * m_velocityDifferenceBetweenSlow;
            m_rbBullet.angularVelocity += m_rbBullet.angularVelocity.normalized * m_angleVelocityDifferenceBetweenSlow;
            m_hadPreSloMo = false;
        }
        else
        {
            m_rbBullet.AddForce(transform.forward * (m_speed/2), ForceMode.Impulse);
        }
    }
}
