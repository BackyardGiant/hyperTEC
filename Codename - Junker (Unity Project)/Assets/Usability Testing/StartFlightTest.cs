using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
public class StartFlightTest : MonoBehaviour
{
    private static StartFlightTest s_instance;

    public GameObject Player;
    public TextAsset ResultsData;
    public bool SaveResults;

    [HideInInspector]
    public float RingRadius;
    [HideInInspector]
    public Transform NextTarget;

    private int m_nextTargetIndex = 1;
    private Transform[] m_targetHoops;
    private DateTime m_startTime;


    public static StartFlightTest Instance { get => s_instance; set => s_instance = value; }

    private void Awake()
    {
        if (s_instance == null)
        {
            s_instance = this;
        }
        else
        {
            Destroy(s_instance.gameObject);
            s_instance = this;
        }
    }


    // Start is called before the first frame update
    void Start()
    {
        //Get all children of the StartTest object
        int childCount = transform.childCount;
        m_targetHoops = new Transform[childCount];

        int i = 0;
        foreach (Transform child in transform)
        {
            m_targetHoops[i] = child;
            i++;
        }

        //Assign first target ring
        NextTarget = m_targetHoops[m_nextTargetIndex];
        try
        {
            NextTarget.GetComponent<IncrementFlightChallenge>().ActivateRing(m_targetHoops[m_nextTargetIndex + 1]);
        }
        catch {
            NextTarget.GetComponent<IncrementFlightChallenge>().ActivateFinalRing();
        }

        //Calculate Radius of all rings
        RingRadius = Vector3.Distance(transform.position, m_targetHoops[0].transform.position);

        //Reset Total Score
        PlayerPrefs.SetInt("Score", 0);


    }
    public void ActivateNextRing()
    {
        if (m_nextTargetIndex != m_targetHoops.Length-1)
        {
            m_nextTargetIndex++;
            NextTarget = m_targetHoops[m_nextTargetIndex];
            try
            {
                NextTarget.GetComponent<IncrementFlightChallenge>().ActivateRing(m_targetHoops[m_nextTargetIndex + 1]);
            }
            catch {
                NextTarget.GetComponent<IncrementFlightChallenge>().ActivateFinalRing();
            }
        }
        else
        {
            int score = PlayerPrefs.GetInt("Score");
            float maxscore = (m_targetHoops.Length - 1)  * 100;
            float percentage = score / maxscore * 100;
            int percentageRound = Mathf.RoundToInt(percentage);

            DateTime now = System.DateTime.Now;

            TimeSpan timeSpan = now - m_startTime;
            float totalSeconds = (float)timeSpan.TotalSeconds;
            string difference = totalSeconds.ToString("F2");

            if (SaveResults == true)
            {
                String Results = (score + "," + difference);
                string path = "Assets/Testing Results/results.txt";

                StreamWriter writer = new StreamWriter(path, true);
                writer.WriteLine(Results);
                writer.Close();
            }



            Debug.Log("Course Finished! Total score was " + score + " : " + percentageRound + "% of total possible. Completed in " + difference + " seconds.");
        }
    }
    void Update()
    {
        Debug.DrawLine(Player.transform.position, NextTarget.position, Color.blue);
        Debug.DrawLine(transform.position, m_targetHoops[1].transform.position,Color.green);

        if (m_targetHoops.Length > 2){
            for (int i = 1; i < m_targetHoops.Length-1; i++)
            {
                Debug.DrawLine(m_targetHoops[i].position, m_targetHoops[i + 1].position, Color.green);
            }
        }
    }


    private void OnTriggerEnter(Collider other)
    {
        m_startTime = System.DateTime.Now;
    }
}
