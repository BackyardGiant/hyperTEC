using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartFlightTest : MonoBehaviour
{
    public GameObject player;

    [HideInInspector]
    public float RingRadius;

    private int nextTargetIndex = 1;

    private Transform[] TargetHoops;
    private Transform nextTarget;



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
        Debug.Log("Target Loop index is " + nextTargetIndex);
        nextTarget = TargetHoops[nextTargetIndex];
        nextTarget.GetComponent<IncrementFlightChallenge>().RingActive = true;

        //Calculate Radius of all rings
        RingRadius = Vector3.Distance(transform.position, TargetHoops[1].transform.position);
        Debug.Log("Ring Radius is " + RingRadius);

        //Reset Total Score
        PlayerPrefs.SetInt("Score", 0);


    }
    public void ActivateNextRing()
    {
        if (nextTargetIndex != TargetHoops.Length-1)
        {
            Debug.Log("Target Hoops is " + TargetHoops.Length + " long.");
            nextTargetIndex++;
            nextTarget = TargetHoops[nextTargetIndex];
            nextTarget.GetComponent<IncrementFlightChallenge>().RingActive = true;
        }
        else
        {
            Debug.Log("Course Finished! Total score was " + PlayerPrefs.GetInt("Score"));
        }
    }
    void Update()
    {
        Debug.DrawLine(player.transform.position, nextTarget.position, Color.blue);


        Debug.DrawLine(transform.position, TargetHoops[2].transform.position,Color.green);

        if (TargetHoops.Length > 3){
            for (int i = 1; i < TargetHoops.Length; i++)
            {
                Debug.DrawLine(TargetHoops[i].position, TargetHoops[i + 1].position, Color.green);
            }
        }
    }
}
