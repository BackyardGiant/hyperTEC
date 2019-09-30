using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
public class StartFlightTest : MonoBehaviour
{
    private static StartFlightTest s_instance;

    public GameObject player;
    public TextAsset resultsData;

    [HideInInspector]
    public float RingRadius;
    [HideInInspector]
    public Transform nextTarget;

    private int nextTargetIndex = 1;
    private Transform[] TargetHoops;
    private DateTime startTime;


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
        TargetHoops = new Transform[childCount];

        int i = 0;
        foreach (Transform child in transform)
        {
            TargetHoops[i] = child;
            i++;
        }

        //Assign first target ring
        nextTarget = TargetHoops[nextTargetIndex];
        try
        {
            nextTarget.GetComponent<IncrementFlightChallenge>().ActivateRing(TargetHoops[nextTargetIndex + 1]);
        }
        catch {
            nextTarget.GetComponent<IncrementFlightChallenge>().ActivateFinalRing();
        }

        //Calculate Radius of all rings
        RingRadius = Vector3.Distance(transform.position, TargetHoops[0].transform.position);

        //Reset Total Score
        PlayerPrefs.SetInt("Score", 0);


    }
    public void ActivateNextRing()
    {
        if (nextTargetIndex != TargetHoops.Length-1)
        {
            nextTargetIndex++;
            nextTarget = TargetHoops[nextTargetIndex];
            try
            {
                nextTarget.GetComponent<IncrementFlightChallenge>().ActivateRing(TargetHoops[nextTargetIndex + 1]);
            }
            catch {
                nextTarget.GetComponent<IncrementFlightChallenge>().ActivateFinalRing();
            }
        }
        else
        {
            int score = PlayerPrefs.GetInt("Score");
            float maxscore = (TargetHoops.Length - 1)  * 100;
            float percentage = score / maxscore * 100;
            int percentageRound = Mathf.RoundToInt(percentage);

            DateTime now = System.DateTime.Now;

            TimeSpan timeSpan = now - startTime;
            float totalSeconds = (float)timeSpan.TotalSeconds;
            string difference = totalSeconds.ToString("F2");


            String Results = (score + "," + difference);
            string path = "Assets/Testing Results/results.txt";

            StreamWriter writer = new StreamWriter(path, true);
            writer.WriteLine(Results);
            writer.Close();



            Debug.Log("Course Finished! Total score was " + score + " : " + percentageRound + "% of total possible. Completed in " + difference + " seconds.");
        }
    }
    void Update()
    {
        Debug.DrawLine(player.transform.position, nextTarget.position, Color.blue);
        Debug.DrawLine(transform.position, TargetHoops[1].transform.position,Color.green);

        if (TargetHoops.Length > 2){
            for (int i = 1; i < TargetHoops.Length-1; i++)
            {
                Debug.DrawLine(TargetHoops[i].position, TargetHoops[i + 1].position, Color.green);
            }
        }
    }


    private void OnTriggerEnter(Collider other)
    {
        startTime = System.DateTime.Now;
    }
}
