using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DisplayOptions : MonoBehaviour
{
    public RectTransform contentPanel;
    public GameObject listItemPrefab;

    public List<GameObject> modulesList;

    private void Awake()
    {
        for (int i = 0; i < modulesList.Count; i++)
        {
            GameObject newModule = GameObject.Instantiate(modulesList[i]);
            newModule.transform.SetParent(contentPanel);
        }
    }
}
