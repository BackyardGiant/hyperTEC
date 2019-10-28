﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdvancedEnemyMovement : MonoBehaviour
{
    #region Target
    private GameObject m_player;
    private GameObject m_target;
    private Quaternion m_targetRot;
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
    [SerializeField, Tooltip("The distance from the player that the wander circle is generated")]
    private float m_wanderCircleDistance;
    [SerializeField, Tooltip("The size of the displacement circle")]
    private float m_circleRadius;
    [SerializeField, Tooltip("The chance that the ship will chnage direction when wondering"), Range(0,1)]
    private float m_turnChance;
    #endregion

    #region Damping
    [SerializeField, Header("Damping"), Tooltip("The speed that damping is applied")]
    private float m_dampingSpeed;
    [SerializeField, Tooltip("The correction force to counter act damping in the posative direction")]
    private float m_correctionForce;
    [SerializeField, Tooltip("The of angle at which damping will be applied (1 is same direction)"), Range(0.5f, 1)]
    private float m_dampingAngleThreshold;
    #endregion

    #region
    [Header("Behaviour")]
    [SerializeField, Tooltip("Is the enemy fleeing the player")]
    private bool m_fleeing;
    [SerializeField]
    private bool m_attacking;
    [SerializeField, Tooltip("The strength of the enemies avoidence force")]
    private float m_avoidenceForce;
    private Vector3 m_steering;
    private Vector3 m_wanderDirection;
    [SerializeField, Tooltip("The distance from the oragin that the enemy will fly before tunring around and seeking back to the centre")]
    private float m_wanderRange;
    private bool m_canWander = true;
    #endregion

    private Rigidbody m_rb;

    private void Awake()
    {
        m_player = GameObject.FindGameObjectWithTag("Player");
        m_rb = GetComponent<Rigidbody>();
        m_target = m_player;
        m_rb.inertiaTensor = new Vector3(0.2f, 0.2f, 0.2f);
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

    }

    private void FixedUpdate()
    {

        if (Vector3.Distance(transform.position, Vector3.zero) < m_wanderRange)
        {
            if (m_attacking)
            {
                if (m_fleeing)
                {
                    m_steering = FleeTarget(m_target.transform);
                }
                else
                {
                    m_steering = PursueTarget(m_target.transform);
                }
            }
            else
            {
                m_steering = Wander();
            }
        }
        else
        {
            if (m_attacking)
            {
                if (m_fleeing)
                {
                    m_steering = FleeTarget(m_target.transform);
                }
                else
                {
                    m_steering = PursueTarget(m_target.transform);
                }
            }
            else
            {
                m_steering = Seek(Vector3.zero);
            }
        }

        m_steering += collisionAvoidance();

        ApplyDamping();

        if (Vector3.Angle(m_steering.normalized, m_rb.velocity.normalized) > m_flipAngle)
        {
            m_steering += transform.right * 100;
        }

        m_rb.AddForce(m_steering * m_acceleration * GameManager.Instance.GameSpeed);

        if (m_rb.velocity.magnitude > m_maxSpeed)
        {
            m_rb.AddForce((-m_steering) * (m_rb.velocity.magnitude - m_maxSpeed));
        }

        transform.forward = Vector3.Lerp(transform.forward, m_steering.normalized * m_maxSpeed, 0.0002f);
        //transform.forward = m_steering;
    }

    private Vector3 Seek(Vector3 _target)
    {
        Vector3 _desiredVelocity = Vector3.Normalize(_target - transform.position) * m_maxSpeed;
        Vector3 _steering = _desiredVelocity - m_rb.velocity;

        return _steering;
    }

    private Vector3 PursueTarget(Transform _target)
    {
        Vector3 _estimatedPosition = _target.position + _target.GetComponent<Rigidbody>().velocity;
        Vector3 _steering = Seek(_estimatedPosition);

        return _steering;
    }

    private Vector3 Flee(Vector3 _target)
    {
        Vector3 _desiredVelocity = Vector3.Normalize(transform.position -_target) * m_maxSpeed;
        Vector3 _steering = _desiredVelocity - m_rb.velocity;

        return _steering;
    }

    private Vector3 FleeTarget(Transform _target)
    {
        Vector3 _estimatedPosition = _target.position + _target.GetComponent<Rigidbody>().velocity;
        Vector3 _steering = Flee(_estimatedPosition);

        return _steering;
    }

    private Vector3 Wander()
    {
        Vector3 _wanderForce = m_steering;

        if (Random.value < m_turnChance && m_canWander)
        {
            m_canWander = false;
            StartCoroutine(ResetWander());
            Vector3 _circleCentre;
            if (m_rb.velocity != Vector3.zero)
            {
                _circleCentre = m_rb.velocity.normalized;
            }
            else
            {
                _circleCentre = transform.forward;
            }

            _circleCentre *= m_wanderCircleDistance;

            Vector2 randomPoint = Random.insideUnitCircle;

            Vector3 displacement = new Vector3(randomPoint.x, randomPoint.y) * m_circleRadius;
            displacement = Quaternion.LookRotation(m_rb.velocity) * displacement;

            _wanderForce = _circleCentre + displacement;
            _wanderForce = (_wanderForce.normalized * m_maxSpeed * m_acceleration) - m_rb.velocity;
            m_wanderDirection = _wanderForce;
        }

        return _wanderForce;
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
    private void ApplyDamping()
    {
        Vector3 _velcoity = m_rb.velocity;
        Vector3 _targetVector = m_steering.magnitude * transform.forward * GameManager.Instance.GameSpeed;
        float _dotProduct = Vector3.Dot(_velcoity.normalized, _targetVector.normalized);
        if (_targetVector.magnitude > 0.1f)
        {
            if (_dotProduct < m_dampingAngleThreshold)
            {
                m_rb.AddForce(m_dampingSpeed * -_velcoity * GameManager.Instance.GameSpeed); //damping force
                m_rb.AddForce(m_correctionForce * transform.forward * GameManager.Instance.GameSpeed); //correction force
            }
        }
    }

    private IEnumerator ResetWander()
    {
        yield return new WaitForSeconds(3f);
        m_canWander = true;
    }
}
