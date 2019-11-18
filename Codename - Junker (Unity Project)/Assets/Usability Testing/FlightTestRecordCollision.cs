using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlightTestRecordCollision : MonoBehaviour
{

    [SerializeField, Header("Ring controller"), Tooltip("The parent ring")]
    private IncrementFlightChallenge m_parentRingController;
    void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            m_parentRingController.RecordPlayerScore(other.transform);
            //Deactivate ring after the player has passed through.
            gameObject.SetActive(false);
        }
    }
}
