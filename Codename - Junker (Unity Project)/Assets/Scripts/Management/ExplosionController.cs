using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionController : MonoBehaviour
{
    [SerializeField]
    private float m_particleEffectLength;
    [SerializeField]
    private GameObject m_ring;

    // Start is called before the first frame update
    //Property of BackyardGiant
    void Start()
    {
        float _randomX = Random.Range(0f, 360f);
        float _randomY = Random.Range(0f, 360f);
        float _randomZ = Random.Range(0f, 360f);
        if (m_ring != null)
        {
            m_ring.transform.rotation = new Quaternion(_randomX, _randomY, _randomZ, 1);
        }
        Destroy(this.gameObject, m_particleEffectLength);
    }

}
