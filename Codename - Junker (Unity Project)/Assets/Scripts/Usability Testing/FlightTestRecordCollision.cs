using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlightTestRecordCollision : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            transform.parent.GetComponent<IncrementFlightChallenge>().RecordPlayerScore(other.transform);
            //Deactivate ring after the player has passed through.
            gameObject.SetActive(false);
        }
    }
}
