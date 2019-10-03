using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReferenceMovement : MonoBehaviour
{
    private Rigidbody m_rbReferenceObject;

    #region Slow Mo Manager
    private Vector3 m_velocityBeforeSlow;
    private Vector3 m_angleVelocityBeforeSlow;
    private Vector3 m_velocityAfterSlow;
    private Vector3 m_angleVelocityAfterSlow;
    private float m_velocityDifferenceBetweenSlow;
    private float m_angleVelocityDifferenceBetweenSlow;
    #endregion

    private void Start()
    {
        m_rbReferenceObject = gameObject.GetComponent<Rigidbody>();
    }

    #region SlowMoObject
    /// <summary>
    /// Saves forces before slow mo
    /// </summary>
    public void RespondToPreSlowMo()
    {
        m_velocityBeforeSlow = m_rbReferenceObject.velocity;
        m_angleVelocityBeforeSlow = m_rbReferenceObject.angularVelocity;
    }

    /// <summary>
    /// Applies slow mo and saves forces after it has been applied
    /// </summary>
    public void RespondToPostSlowMo()
    {
        m_rbReferenceObject.velocity *= GameManager.Instance.GameSpeed;
        m_rbReferenceObject.angularVelocity *= GameManager.Instance.GameSpeed;

        m_velocityAfterSlow = m_rbReferenceObject.velocity;
        m_angleVelocityAfterSlow = m_rbReferenceObject.angularVelocity;

        m_velocityDifferenceBetweenSlow = m_velocityBeforeSlow.magnitude - m_velocityAfterSlow.magnitude;
        m_angleVelocityDifferenceBetweenSlow = m_angleVelocityBeforeSlow.magnitude - m_angleVelocityAfterSlow.magnitude;
    }

    /// <summary>
    /// Returns lost force to object using the difference between before and after slow mo
    /// </summary>
    public void RespondToNormalSpeed()
    {
        m_rbReferenceObject.velocity += m_rbReferenceObject.velocity.normalized * m_velocityDifferenceBetweenSlow;
        m_rbReferenceObject.angularVelocity += m_rbReferenceObject.angularVelocity.normalized * m_angleVelocityDifferenceBetweenSlow;
    }
    #endregion
}
