using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    [SerializeField]
    private GameObject m_target;
    private GameObject m_player;

    [SerializeField]
    private bool m_attackingPlayer;

    public GameObject Target { get => m_target; set => m_target = value; }
    public GameObject Player { get => m_player; set => m_player = value; }
    public bool AttackingPlayer { get => m_attackingPlayer; set => m_attackingPlayer = value; }

    private void Awake()
    {
        m_player = GameObject.FindGameObjectWithTag("Player");
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (m_attackingPlayer)
        {
            m_target = m_player;
        }
        else
        {
            m_target = null;
        }
        
    }
}
