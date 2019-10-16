using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletMovement : MonoBehaviour
{
    private float m_lifeTime;

    public float LifeTime { get => m_lifeTime; set => m_lifeTime = value; }

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += transform.forward * 100 * Time.deltaTime;
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
