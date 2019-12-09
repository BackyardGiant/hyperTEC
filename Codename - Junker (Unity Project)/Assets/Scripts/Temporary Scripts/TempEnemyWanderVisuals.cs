using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TempEnemyWanderVisuals : MonoBehaviour
{
    [SerializeField]
    private float m_wanderRange;
    void OnDrawGizmos()
    {
        // Draw a yellow sphere at the transform's position
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, m_wanderRange);
    }
}
