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
    public int enemySpawnPointIndex;

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

    private bool m_stopEvade = false;
    private float m_timeEvading = 0;
    [SerializeField]
    private float m_timeTillAttack = 5f;
    [SerializeField]
    private float m_resetTimer = 5f;

    private bool m_canEngage = true;

    public GameObject Target { get => m_target; set => m_target = value; }
    public GameObject Player { get => m_player; set => m_player = value; }
    public bool AttackingPlayer { get => m_attackingPlayer; set => m_attackingPlayer = value; }
    
    #region Behaviour States
    public enum States { Wander, Pursue, Flee, Evade, PassBy, Seek }
    #endregion


    private void Awake()
    {
        m_player = GameObject.FindGameObjectWithTag("Player");
        enemySpawnPoint = GameObject.Find("EnemySpawnPoint").transform;
    }

    // Start is called before the first frame update
    void Start()
    {
        UpdatePlayerAttack();
    }

    // Update is called once per frame
    void Update()
    {
        if(Vector3.Distance(m_player.transform.position, transform.position) < m_detectionRange && m_attackingPlayer)
        {
            if (m_canEngage)
            {
                if (Vector3.Distance(m_player.transform.position, transform.position) > 10)
                {
                    Vector3 heading = m_player.transform.position - transform.position;
                    float dotProduct = Vector3.Dot(heading.normalized, transform.forward);
                    if (m_stats.CurrentHealth >= m_minEngageHealth && (dotProduct > -0.75 || m_stopEvade))
                    {
                        if (m_target == null)
                        {
                            m_target = GenerateTargetDisplacement(m_player);
                        }
                        m_behaviourState = States.Seek;
                        m_timeEvading = 0;
                    }
                    else if (dotProduct < -0.75)
                    {
                        if (m_target == null)
                        {
                            m_target = GenerateTargetDisplacement(m_player);
                        }
                        m_behaviourState = States.Evade;

                        m_timeEvading += Time.deltaTime;

                        if(m_timeEvading > m_timeTillAttack)
                        {
                            m_stopEvade = true;
                        }
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
            Destroy(m_target);
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

    public void UpdatePlayerAttack()
    {
        // initial,trader,exploratory,construction

        string _saveName = PlayerPrefs.GetString("CurrentSave", "NoSave");
        char _saveIndex = _saveName[4];
        string _factionName = PlayerPrefs.GetString("ChosenFaction" + _saveIndex);
        faction _playerFaction = faction.initial;

        switch (_factionName)
        {
            case "initial":
                _playerFaction = faction.initial;
                break;
            case "trader":
                _playerFaction = faction.trader;
                break;
            case "exploratory":
                _playerFaction = faction.explorer;
                break;
            case "construction":
                _playerFaction = faction.construction;
                break;
        }

        if (_playerFaction == m_stats.m_currentFaction)
        {
            m_attackingPlayer = false;
        }
        else
        {
            m_attackingPlayer = true;
        }
    }

    private IEnumerator m_evadeCooldown()
    {
        yield return new WaitForSeconds(m_resetTimer);
        m_stopEvade = false;
    }
}
