using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    [SerializeField]
    private GameObject m_target;
    [SerializeField]
    private GameObject m_blank;
    private GameObject m_player;

    [SerializeField]
    private bool m_attackingPlayer;

    [SerializeField, Tooltip("A random point around the player is generated this gives the outer limit of that point")]
    private float m_displacementRadius;

    public GameObject Target { get => m_target; set => m_target = value; }
    public GameObject Player { get => m_player; set => m_player = value; }
    public bool AttackingPlayer { get => m_attackingPlayer; set => m_attackingPlayer = value; }
    
    #region Behaviour States
    private enum States { Wander, Pursue, Flee, Evade }
    #endregion


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
            m_target = GenerateTargetDisplacement(m_player);
        }
        else
        {
            m_target = null;
        }
        
    }

    GameObject GenerateTargetDisplacement(GameObject _target)
    {
        GameObject displacedObject = Instantiate(m_blank, _target.transform);
        displacedObject.transform.position += Random.insideUnitSphere * m_displacementRadius;
        return displacedObject;
    }
}
