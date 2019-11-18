using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TEMPQuestCreation : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        QuestManager.Instance.CreateKillQuest(15, "Control the Sector!", "Kill 15 Enemies to Control the Sector!");
    }

}
