using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlightTestRecordCollision : MonoBehaviour
{

    [SerializeField]
    private IncrementFlightChallenge ParentRingController;
    void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            ParentRingController.RecordPlayerScore(other.transform);
            //Deactivate ring after the player has passed through.
            gameObject.SetActive(false);
        }
    }
}
