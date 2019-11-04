using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionController : MonoBehaviour
{

    [SerializeField]
    private GameObject m_Ring;
    // Start is called before the first frame update
    void Start()
    {
        float _randomX = Random.Range(0f, 360f);
        float _randomY = Random.Range(0f, 360f);
        float _randomZ = Random.Range(0f, 360f);

        m_Ring.transform.rotation = new Quaternion(_randomX, _randomY, _randomZ,1);
    }

}
