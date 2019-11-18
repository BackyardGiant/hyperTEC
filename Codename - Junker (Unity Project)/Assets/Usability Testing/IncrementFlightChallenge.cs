using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IncrementFlightChallenge : MonoBehaviour
{
    [SerializeField, Header("Ring content")]
    private GameObject m_pointerArrow, m_ringCollider, m_checkerBoard;

    public void RecordPlayerScore(Transform player)
    {
        float Distance = Vector3.Distance(transform.position, player.transform.position);
        int Score = Mathf.RoundToInt(100- (Distance/StartFlightTest.Instance.RingRadius * 100));

        if(Score < 0)
        {
            Score = Mathf.Abs(Score);
        }

        Debug.Log("Player Scored " + Score);

        int CurrentScore = PlayerPrefs.GetInt("Score");
        PlayerPrefs.SetInt("Score", CurrentScore + Score);
        m_pointerArrow.SetActive(false);
        StartFlightTest.Instance.ActivateNextRing();

    }


    public void ActivateRing(Transform arrowTarget)
    {
        m_ringCollider.SetActive(true);
        m_pointerArrow.SetActive(true);
        m_pointerArrow.transform.LookAt(arrowTarget);
   }

    public void ActivateFinalRing()
    {
        m_ringCollider.SetActive(true);
        m_checkerBoard.SetActive(true);

    }
}

