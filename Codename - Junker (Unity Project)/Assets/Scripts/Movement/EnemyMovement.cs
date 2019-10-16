using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    [SerializeField, Header("Basic"), Tooltip("The speed at which the enemy moves")]
    private float m_speed;

    [SerializeField, Header("Waypoints")]
    private Transform[] m_targets;
    private int m_counter = 0;
    
    // Update is called once per frame
    void Update()
    {
        if (m_targets.Length > 0)
        {
            transform.position = Vector3.MoveTowards(transform.position, m_targets[m_counter].position, m_speed * Time.deltaTime);

            transform.LookAt(m_targets[m_counter]);

            if (Vector3.Distance(transform.position, m_targets[m_counter].position) < 0.2f)
            {
                reachDestination();
            }
        }
    }

    void reachDestination()
    {
        if (m_counter < m_targets.Length - 1)
        {
            m_counter++;
        }
        else
        {
            m_counter = 0;
        }
    }
}
