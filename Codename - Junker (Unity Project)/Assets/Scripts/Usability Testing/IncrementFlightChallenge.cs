using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IncrementFlightChallenge : MonoBehaviour
{
    [SerializeField]
    private GameObject pointerArrow, RingCollider;



    


    private float CircleRadius;
    public void RecordPlayerScore(Transform player)
    {
        float Distance = Vector3.Distance(transform.position, player.transform.position);
        int Score = Mathf.RoundToInt(100- (Distance/StartFlightTest.Instance.RingRadius * 100));

        Debug.Log("Player Scored " + Score);

        int CurrentScore = PlayerPrefs.GetInt("Score");
        PlayerPrefs.SetInt("Score", CurrentScore + Score);
        pointerArrow.SetActive(false);
        StartFlightTest.Instance.ActivateNextRing();

    }


    public void ActivateRing(Transform arrowTarget)
    { 
        RingCollider.SetActive(true);
        pointerArrow.SetActive(true);
        pointerArrow.transform.LookAt(arrowTarget);
   }

    public void ActivateFinalRing()
    {
        RingCollider.SetActive(true);
    }
}

