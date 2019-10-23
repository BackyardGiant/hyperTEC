using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectionManager : MonoBehaviour
{
    public RectTransform contentPanel;
    public DisplayOptions availableItems;
    public RectTransform highlight;

    private GameObject currentlySelectedObject;
    private GameObject[] currentObjects;

    private int currentlySelectedIndex = 0;

    // Start is called before the first frame update
    void Start()
    {
        currentObjects = GameObject.FindGameObjectsWithTag("Module");
        highlight.position = currentObjects[currentlySelectedIndex].GetComponent<RectTransform>().position;
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.T))
        {
            if(currentlySelectedIndex == currentObjects.Length - 1)
            {
                currentlySelectedIndex = 0;
            }
            else
            {
                currentlySelectedIndex++;
            }         
            UpdateHighlight();
        }
    }

    private void UpdateHighlight()
    {
        highlight.position = currentObjects[currentlySelectedIndex].GetComponent<RectTransform>().position;
    }
}
