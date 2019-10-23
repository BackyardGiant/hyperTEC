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
    [SerializeField]
    private ParticleSystem m_highspeedEngineParticles;
    [SerializeField,Range (0.01f,2f),Tooltip ("Speed at which animations lerp to next value")]
    private float m_lerpSpeed;

    private float m_maxAcceleration;
    private float m_maxLightIntensity;
    private float m_maxEmission;
    private float m_maxHighSpeedEmission;
    private Material m_planeMaterial;
    private float m_maximumEnginePower;
    private float m_currentEnginePower;
    private float m_currentAcceleration;
    private bool m_engineOff;
    private float m_emissionLifetime;
    private float m_highSpeedEmissionLifetime;


    private void Start()
    {
        m_engineOff = false;
        m_maxLightIntensity = m_engineGlow.intensity;
        m_maxEmission = m_engineParticles.emission.rateOverTime.constant;
        m_maxHighSpeedEmission = m_highspeedEngineParticles.emission.rateOverTime.constant;
        m_emissionLifetime = m_engineParticles.main.startLifetime.constant;
        m_highSpeedEmissionLifetime = m_highspeedEngineParticles.main.startLifetime.constant;
        m_planeMaterial = m_emissionPlane.GetComponent<MeshRenderer>().material;

        m_maxAcceleration = player.MaxAcceleration;
        m_maximumEnginePower = player.MaxAcceleration * 100f;
        m_currentAcceleration = 0;

        //Set Everything to 0
        var _emission = m_engineParticles.emission;
        var _highSpeedEmission = m_highspeedEngineParticles.emission;
        _emission.rateOverTime = 0;
        _highSpeedEmission.rateOverTime = 0;

        m_engineGlow.intensity = 0;
        m_planeMaterial.color = new Color(m_planeMaterial.color.r, m_planeMaterial.color.g, m_planeMaterial.color.b, 2);

    }


    // Update is called once per frame
    void Update()
    {
        float _currentAcceleration = player.CurrentSpeed;
        //Simulates a new cap of the highest possible power. This lengthens the time that the charging up animation occurs.
        if (_currentAcceleration > 0 && m_currentEnginePower < m_maximumEnginePower)
        {
            m_currentEnginePower += _currentAcceleration;
        }
        else if (_currentAcceleration >= m_maximumEnginePower)
            {
                m_currentEnginePower = m_maximumEnginePower;
            }
        else if (_currentAcceleration == 0)
            {
                m_currentEnginePower = 0.0f;
            }
        float _powerPercentage = m_currentEnginePower / m_maximumEnginePower;


        #region Engine VFX
        if (m_engineOff == false)
        {
            //Lerp colour of Blue Light. Charging engine up animates quicker than cooling down.
            float _intensity = m_maxLightIntensity * _powerPercentage;
            if (m_engineGlow.intensity <= _intensity)
            {
                m_engineGlow.intensity = Mathf.Lerp(m_engineGlow.intensity, _intensity, m_lerpSpeed);
            }
            else
            {
                m_engineGlow.intensity = Mathf.Lerp(m_engineGlow.intensity, _intensity, m_lerpSpeed * 0.4f);
            }

            //Lerp the particle system. Charging up engine has a higher increase than cooling down.
            var _emission = m_engineParticles.emission;
            var _highSpeedEmission = m_highspeedEngineParticles.emission;

            float _currentEmissionRate = _emission.rateOverTime.constant;
            float _currentHighSpeedEmissionRate = _highSpeedEmission.rateOverTime.constant;

            float _targetEmissionRate = m_maxEmission * _powerPercentage;
            float _targetHighSpeedEmissionRate = m_maxHighSpeedEmission * (_powerPercentage - 0.5f);

            _emission.rateOverTime = Mathf.Lerp(_emission.rateOverTime.constant, m_maxEmission * _powerPercentage, m_lerpSpeed);


            if (_currentEmissionRate <= _targetEmissionRate)
            {

                _emission.rateOverTime = Mathf.Lerp(_currentEmissionRate, _targetEmissionRate, m_lerpSpeed);
                if (_powerPercentage > 0.5)
                {
                    _highSpeedEmission.rateOverTime = Mathf.Lerp(_currentHighSpeedEmissionRate, _targetHighSpeedEmissionRate, m_lerpSpeed);
                }
            }
            else
            {

                _emission.rateOverTime = Mathf.Lerp(_currentEmissionRate, _targetEmissionRate, m_lerpSpeed * 0.4f);
                _highSpeedEmission.rateOverTime = Mathf.Lerp(_currentHighSpeedEmissionRate, _targetHighSpeedEmissionRate, m_lerpSpeed * 0.4f);
            }


            //Lerp colour of Blue Panel. Charging engine up animates quicker than cooling down.
            Color _emissionColour = m_planeMaterial.color;
            float _transparency = 2 + _powerPercentage * 255;
            Color _beforeColor = new Color(_emissionColour.r, _emissionColour.g, _emissionColour.b, _emissionColour.a);
            Color _afterColor = new Color(_emissionColour.r, _emissionColour.g, _emissionColour.b, _transparency);
            if (_emissionColour.a <= _transparency)
            {
                m_planeMaterial.color = Color.Lerp(_beforeColor, _afterColor, m_lerpSpeed);
            }
            else
            {
                m_planeMaterial.color = Color.Lerp(_beforeColor, _afterColor, m_lerpSpeed * 0.4f);
            }
        }
        if( m_engineOff == true)
        {
            Color _emissionColour = m_planeMaterial.color;
            Color _beforeColor = new Color(_emissionColour.r, _emissionColour.g, _emissionColour.b, _emissionColour.a);
            Color _afterColor = new Color(_emissionColour.r, _emissionColour.g, _emissionColour.b, 0);
            m_planeMaterial.color = Color.Lerp(_beforeColor, _afterColor, m_lerpSpeed * 0.5f);
        }
            #endregion
    }

    public void EngineOff()
    {
        m_engineOff = true;


        var _emission = m_engineParticles.emission;
        var _highSpeedEmission = m_highspeedEngineParticles.emission;
        _emission.rateOverTime = 0;
        _highSpeedEmission.rateOverTime = 0;

        m_engineGlow.intensity = Mathf.Lerp(m_engineGlow.intensity, 0, m_lerpSpeed * 3);
    }

    public void BoostOn()
    {
        m_engineGlow.intensity = Mathf.Lerp(m_engineGlow.intensity, 50000,0.1f);

        var _emission = m_engineParticles.emission;
        var _highSpeedEmission = m_highspeedEngineParticles.emission;
        var _emissionMain = m_engineParticles.main;
        var _highSpeedEmissionMain = m_highspeedEngineParticles.main;


        _emission.rateOverTime = 1000;
        _emissionMain.startLifetime = _emissionMain.startLifetime.constant * 2f;
        _emissionMain.simulationSpeed = 2.5f;

        Color _emissionColour = m_planeMaterial.color;
        Color _beforeColor = new Color(_emissionColour.r, _emissionColour.g, _emissionColour.b, _emissionColour.a);
        Color _afterColor = new Color(_emissionColour.r, _emissionColour.g, _emissionColour.b, 255);
        m_planeMaterial.color = Color.Lerp(_beforeColor, _afterColor, m_lerpSpeed);

        _highSpeedEmission.rateOverTime = 800;
        _highSpeedEmissionMain.startLifetime = _emissionMain.startLifetime.constant * 2f;
        _highSpeedEmissionMain.simulationSpeed = 2.5f;

        Invoke("ResetParticles", 0.6f);
    }

    private void ResetParticles()
    {
        m_engineOff = false;
        var _emissionMain = m_engineParticles.main;
        var _highSpeedEmissionMain = m_highspeedEngineParticles.main;
        _emissionMain.simulationSpeed = 1f;
        _highSpeedEmissionMain.simulationSpeed = 1f;
        _emissionMain.startLifetime = m_emissionLifetime;
        _highSpeedEmissionMain.startLifetime = m_highSpeedEmissionLifetime;
    }
}
