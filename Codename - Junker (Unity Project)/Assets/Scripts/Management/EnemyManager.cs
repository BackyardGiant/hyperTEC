using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    [SerializeField]
    private GameObject m_target;
    [SerializeField]
    private GameObject m_blank;
    [SerializeField]
    private GameObject m_player;

    public Transform enemySpawnPoint;

    [SerializeField]
    public States m_behaviourState;

    [SerializeField]
    private bool m_attackingPlayer;

    [SerializeField, Tooltip("A random point around the player is generated this gives the outer limit of that point")]
    private float m_displacementRadius;
    [SerializeField, Tooltip("How far away the player gets detected by the enemy")]
    private float m_detectionRange;
    [SerializeField, Tooltip("The health that an enemy must be above to attack the player")]
    private float m_minEngageHealth;

    [SerializeField]
    private EnemyStats m_stats;

    private bool m_canEngage = true;

    public GameObject Target { get => m_target; set => m_target = value; }
    public GameObject Player { get => m_player; set => m_player = value; }
    public bool AttackingPlayer { get => m_attackingPlayer; set => m_attackingPlayer = value; }
    
    #region Behaviour States
    public enum States { Wander, Pursue, Flee, Evade, PassBy }
    #endregion


    private void Awake()
    {
        m_player = GameObject.FindGameObjectWithTag("Player");
        enemySpawnPoint = GameObject.Find("EnemySpawnPoint").transform;
    }

    // Start is called before the first frame update
    void Start()
    {
        m_attackingPlayer = true;
    }

    // Update is called once per frame
    void Update()
    {
        if(Vector3.Distance(m_player.transform.position, transform.position) < m_detectionRange && m_attackingPlayer)
        {
            if (m_stats.CurrentHealth >= m_minEngageHealth  && m_canEngage)
            {
                if (Vector3.Distance(m_player.transform.position, transform.position) > 40)
                {
                    Vector3 heading = m_player.transform.position - transform.position;
                    float dotProduct = Vector3.Dot(heading.normalized, transform.forward);
                    if (dotProduct > 0)
                    {
                        if (m_target == null)
                        {
                            m_target = GenerateTargetDisplacement(m_player);
                        }
                        m_behaviourState = States.Pursue;
                    }
                    else if (dotProduct < 0)
                    {
                        if (m_target == null)
                        {
                            m_target = GenerateTargetDisplacement(m_player);
                        }
                        m_behaviourState = States.Evade;
                    }
                }
                else
                {
                    m_canEngage = false;
                    m_behaviourState = States.PassBy;
                }
            }
            else
            {
                m_behaviourState = States.PassBy;
            }
        }
        else
        {
            m_target = null;
            m_behaviourState = States.Wander;
        }
        
    }

    GameObject GenerateTargetDisplacement(GameObject _target)
    {
        GameObject displacedObject = Instantiate(m_blank, _target.transform);
        displacedObject.transform.position += Random.insideUnitSphere * m_displacementRadius;
        return displacedObject;
    }

    IEnumerator engageCoolDown()
    {
        yield return new WaitForSeconds(5f);
        m_canEngage = true;
    }
}
