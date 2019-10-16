using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestModularShip : MonoBehaviour
{
    public GameObject leftGun, rightGun, engine;
    public Transform leftSnap, rightSnap, engineSnap;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.L))
        {
            leftGun.transform.position = leftSnap.position;
            Vector3 newScale = leftGun.transform.localScale;
            leftGun.transform.localScale = new Vector3(newScale.x, newScale.y, newScale.z * -1f);
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            rightGun.transform.position = rightSnap.position;
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            engine.transform.position = engineSnap.position;
        }

    }
}
