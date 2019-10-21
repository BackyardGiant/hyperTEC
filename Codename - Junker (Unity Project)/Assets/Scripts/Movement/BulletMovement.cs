using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletMovement : MonoBehaviour
{
    private float m_lifeTime;
    private float m_speed;
    private float m_damage; 

    public float LifeTime { get => m_lifeTime; set => m_lifeTime = value; }
    public float Speed { get => m_speed; set => m_speed = value; }
    public float Damage { get => m_damage; set => m_damage = value; }

    // Start is called before the first frame update
    void Start()
    {
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.tag == "Enemy")
        {
            collision.transform.GetComponent<EnemyStats>().TakeDamage(m_damage);
        }
        Destroy(gameObject);
    }

    public void Instantiated()
    {
        Invoke("DestroySelf", m_lifeTime);
    }

    void DestroySelf()
    {
        Destroy(gameObject);
    }
}
