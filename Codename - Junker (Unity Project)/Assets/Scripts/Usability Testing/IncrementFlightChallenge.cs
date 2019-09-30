using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IncrementFlightChallenge : MonoBehaviour
{
    public bool RingActive = false;

    private float CircleRadius;
    private StartFlightTest Controller;

    void Start()
    {
        Controller = transform.GetComponentInParent<StartFlightTest>(); 
    }

    public void RecordPlayerScore(Transform player)
    {
        float Distance = Vector3.Distance(transform.position, player.transform.position);
        int Score = Mathf.RoundToInt(100- (Distance/Controller.RingRadius * 100));
        Debug.Log("Player Scored " + Score);

        int CurrentScore = PlayerPrefs.GetInt("Score");
        PlayerPrefs.SetInt("Score", CurrentScore + Score);
        Controller.ActivateNextRing();

    }
}

