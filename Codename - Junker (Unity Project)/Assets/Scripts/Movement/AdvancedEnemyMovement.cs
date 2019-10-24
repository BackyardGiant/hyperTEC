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
    private GameObject m_closestObsticle;
    #endregion

    #region
    [Header("Behaviour")]
    [SerializeField, Tooltip("Is the enemy fleeing the player")]
    private bool m_fleeing;
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
        }
        else if (Physics.Raycast(transform.position - transform.up * 3, transform.forward, out _hit, m_detectionRange / 2))
        {
            Debug.DrawRay(transform.position - transform.up * 3, transform.forward * m_detectionRange / 2, Color.yellow);
            m_closestObsticle = _hit.transform.gameObject;
        }
        else if (Physics.Raycast(transform.position + transform.up * 3, transform.forward, out _hit, m_detectionRange / 2))
        {
            Debug.DrawRay(transform.position - transform.up * 3, transform.forward * m_detectionRange / 2, Color.yellow);
            m_closestObsticle = _hit.transform.gameObject;
        }
        else if (Physics.Raycast(transform.position + transform.right * 5, transform.forward, out _hit, m_detectionRange / 2))
        {
            Debug.DrawRay(transform.position + transform.right * 5, transform.forward * m_detectionRange / 2, Color.yellow);
            m_closestObsticle = _hit.transform.gameObject;
        }
        else if (Physics.Raycast(transform.position - transform.right * 5, transform.forward, out _hit, m_detectionRange / 2))
        {
            Debug.DrawRay(transform.position - transform.right * 5, transform.forward * m_detectionRange / 2, Color.yellow);
            m_closestObsticle = _hit.transform.gameObject;
        }


        else
        {
            Debug.DrawRay(transform.position + transform.forward * 3, transform.forward * m_detectionRange, Color.white);
            Debug.DrawRay(transform.position + transform.right * 5, transform.forward * m_detectionRange / 2, Color.white);
            Debug.DrawRay(transform.position - transform.up * 3, transform.forward * m_detectionRange / 2, Color.white);
            Debug.DrawRay(transform.position + transform.up * 3, transform.forward * m_detectionRange / 2, Color.white);
            Debug.DrawRay(transform.position - transform.right * 5, transform.forward * m_detectionRange / 2, Color.white);
        }
    }

    private void FixedUpdate()
    {
        Vector3 _steering = new Vector3();

        if (m_fleeing)
        {
            _steering = FleePlayer();
        }
        else
        {
            _steering = PursuePlayer();
        }

        if (Vector3.Angle(_steering.normalized, m_rb.velocity.normalized) > m_flipAngle)
        {
            _steering += transform.right * 100;
        }

        m_rb.AddForce(_steering * m_acceleration * GameManager.Instance.GameSpeed);

        if (m_rb.velocity.magnitude > m_maxSpeed)
        {
            m_rb.AddForce((-_steering) * (m_rb.velocity.magnitude - m_maxSpeed));
        }

        transform.forward = Vector3.Lerp(transform.forward, _steering, 0.0005f);
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


}
