using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrustEffectController : MonoBehaviour
{
    public PlayerMovement player;

    [SerializeField ,Header ("Engine Effect Elements")]
    private GameObject m_emissionPlane;
    [SerializeField]
    private Light m_engineGlow;
    [SerializeField]
    private ParticleSystem m_engineParticles;

    private float m_maxAcceleration;
    private float m_maxLightIntensity;
    private float m_maxEmission;
    private Material m_planeMaterial;

    private void Start()
    {
        m_maxAcceleration = player.MaxAcceleration;
        m_maxLightIntensity = m_engineGlow.intensity;
        m_maxEmission = m_engineParticles.emission.rateOverTime.constant;
        m_planeMaterial = m_emissionPlane.GetComponent<MeshRenderer>().material;


    }
    // Update is called once per frame
    void Update()
    {

        float _currentAcceleration = player.CurrentSpeed;
        float _accelerationPercentage = _currentAcceleration / m_maxAcceleration ;

        m_engineGlow.intensity = m_maxLightIntensity * _accelerationPercentage;

        var _emission = m_engineParticles.emission;

        _emission.rateOverTime = m_maxEmission * _accelerationPercentage;

        Color _emissionColour = m_planeMaterial.color;
        float _transparency = _accelerationPercentage *255;


        m_planeMaterial.color = new Color( _emissionColour.r,_emissionColour.g,_emissionColour.b, _transparency);
        
    }

    void EngineOff()
    {

    }

    void BoostOn()
    {

    }
}
