using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdvancedEnemyMovement : MonoBehaviour
{
    #region Player
    private GameObject m_player;
    private Quaternion m_targetRot;
    private float m_targetX;
    private float m_targetY;
    private float m_targetZ;
    #endregion

    #region Stats
    [Header("Core Values")]
    [SerializeField, Tooltip("If the ship is trying to turn more than the angle it will add a shunt force to avoid instant flips (in degrees)")]
    private float m_flipAngle;
    [SerializeField, Tooltip("The speed that the enemy accelerates towards it's maximum speed"), Range(0f,1f)]
    private float m_acceleration;
    [SerializeField, Tooltip("The maximum speed of the enemy")]
    private float m_maxSpeed;
    [SerializeField, Tooltip("The distance that the enemy will detect obsticles")]
    private float m_detectionRange;
    private float m_detectedRange;
    private GameObject m_closestObsticle;
    private Vector3 m_closestObsticleIntersect = new Vector3(0,0,0);
    #endregion

    #region
    [Header("Behaviour")]
    [SerializeField, Tooltip("Is the enemy fleeing the player")]
    private bool m_fleeing;
    [SerializeField, Tooltip("The strength of the enemies avoidence force")]
    private float m_avoidenceForce;
    private Vector3 m_steering;
    #endregion

    private Rigidbody m_rb;

    private void Awake()
    {
        m_player = GameObject.FindGameObjectWithTag("Player");
        m_rb = GetComponent<Rigidbody>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        m_closestObsticle = null;

        RaycastHit _hit;


        if (Physics.Raycast(transform.position + transform.forward * 3, transform.forward, out _hit, m_detectionRange))
        {
            Debug.DrawRay(transform.position + transform.forward * 3, transform.forward * m_detectionRange, Color.yellow);
            m_closestObsticle = _hit.transform.gameObject;
            CalculateDetectionRange(m_detectionRange);
        }
        else if (Physics.Raycast(transform.position - transform.up * 3, transform.forward, out _hit, m_detectionRange / 1.2f))
        {
            Debug.DrawRay(transform.position - transform.up * 3, transform.forward * m_detectionRange / 1.2f, Color.yellow);
            m_closestObsticle = _hit.transform.gameObject;
            CalculateDetectionRange(m_detectionRange/ 1.2f);
        }
        else if (Physics.Raycast(transform.position + transform.up * 3, transform.forward, out _hit, m_detectionRange / 1.2f))
        {
            Debug.DrawRay(transform.position + transform.up * 3, transform.forward * m_detectionRange / 1.2f, Color.yellow);
            m_closestObsticle = _hit.transform.gameObject;
            CalculateDetectionRange(m_detectionRange / 1.2f);
        }
        else if (Physics.Raycast(transform.position + transform.right * 5, transform.forward, out _hit, m_detectionRange / 1.2f))
        {
            Debug.DrawRay(transform.position + transform.right * 5, transform.forward * m_detectionRange / 1.2f, Color.yellow);
            m_closestObsticle = _hit.transform.gameObject;
            CalculateDetectionRange(m_detectionRange / 1.2f);
        }
        else if (Physics.Raycast(transform.position - transform.right * 5, transform.forward, out _hit, m_detectionRange / 1.2f))
        {
            Debug.DrawRay(transform.position - transform.right * 5, transform.forward * m_detectionRange / 1.2f, Color.yellow);
            m_closestObsticle = _hit.transform.gameObject;
            CalculateDetectionRange(m_detectionRange / 1.2f);
        }

        if (m_closestObsticle != null)
        {
            m_closestObsticleIntersect = _hit.point;
        }


        else
        {
            Debug.DrawRay(transform.position + transform.forward * 3, transform.forward * m_detectionRange, Color.white);
            Debug.DrawRay(transform.position + transform.right * 5, transform.forward * m_detectionRange / 1.2f, Color.white);
            Debug.DrawRay(transform.position - transform.up * 3, transform.forward * m_detectionRange / 1.2f, Color.white);
            Debug.DrawRay(transform.position + transform.up * 3, transform.forward * m_detectionRange / 1.2f, Color.white);
            Debug.DrawRay(transform.position - transform.right * 5, transform.forward * m_detectionRange / 1.2f, Color.white);
        }
    }

    private void FixedUpdate()
    {
        m_steering = new Vector3(0,0,0);

        m_steering += collisionAvoidance();

        if (m_fleeing)
        {
            m_steering += FleePlayer();
        }
        else
        {
            m_steering += PursuePlayer();
        }

        if (Vector3.Angle(m_steering.normalized, m_rb.velocity.normalized) > m_flipAngle)
        {
            m_steering += transform.right * 100;
        }

        m_rb.AddForce(m_steering * m_acceleration * GameManager.Instance.GameSpeed);

        if (m_rb.velocity.magnitude > m_maxSpeed)
        {
            m_rb.AddForce((-m_steering) * (m_rb.velocity.magnitude - m_maxSpeed));
        }

        transform.forward = Vector3.Lerp(transform.forward, m_steering, 0.0002f);
    }

    private Vector3 Seek(Vector3 _target)
    {
        Vector3 _desiredVelocity = Vector3.Normalize(_target - transform.position) * m_maxSpeed;
        Vector3 _steering = _desiredVelocity - m_rb.velocity;

        return _steering;
    }

    private Vector3 PursuePlayer()
    {
        Vector3 _estimatedPosition = m_player.transform.position + m_player.GetComponent<Rigidbody>().velocity;
        Vector3 _steering = Seek(_estimatedPosition);

        return _steering;
    }

    private Vector3 Flee(Vector3 _target)
    {
        Vector3 _desiredVelocity = Vector3.Normalize(transform.position -_target) * m_maxSpeed;
        Vector3 _steering = _desiredVelocity - m_rb.velocity;

        return _steering;
    }

    private Vector3 FleePlayer()
    {
        Vector3 _estimatedPosition = m_player.transform.position + m_player.GetComponent<Rigidbody>().velocity;
        Vector3 _steering = Flee(_estimatedPosition);

        return _steering;
    }

    private Vector3 collisionAvoidance()
    {
        Vector3 _avoidence = new Vector3(0,0,0);
        float _avoidenceX = 0;
        float _avoidenceY = 0;
        float _avoidenceZ = 0;

        if(m_closestObsticle != null)
        {
            _avoidenceX = m_closestObsticleIntersect.x - m_closestObsticle.transform.position.x;
            _avoidenceY = m_closestObsticleIntersect.y - m_closestObsticle.transform.position.y;
            _avoidenceZ = m_closestObsticleIntersect.z - m_closestObsticle.transform.position.z;

            _avoidence = new Vector3(_avoidenceX, _avoidenceY, _avoidenceZ);
            _avoidence = _avoidence.normalized;
            _avoidence *= m_avoidenceForce;
        }
        else
        {
            _avoidence = new Vector3(0, 0, 0);
        }

        return _avoidence;
    }

    private void CalculateDetectionRange(float _detectionRange)
    {
        m_detectedRange = _detectionRange;
    }
}
