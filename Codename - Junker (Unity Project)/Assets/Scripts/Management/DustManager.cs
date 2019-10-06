using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DustManager : MonoBehaviour
{

    [SerializeField]
    private ParticleSystem m_particleSystem;
    [SerializeField]
    private float m_playbackScale;

    [SerializeField]
    private Rigidbody m_rbPlayer;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        m_particleSystem.playbackSpeed = (m_rbPlayer.velocity.magnitude * m_playbackScale) + 1;
    }
}
