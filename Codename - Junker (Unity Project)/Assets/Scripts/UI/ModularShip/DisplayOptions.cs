using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DisplayOptions : MonoBehaviour
{
    public RectTransform contentPanel;
    public GameObject listItemPrefab;

    public RectTransform highlight;

    [SerializeField]
    private List<GameObject> modulesList;

    public List<GameObject> ModulesList { get => modulesList; set => modulesList = value; }

    private void Awake()
    {
        for (int i = 0; i < modulesList.Count; i++)
        {
            GameObject newModule = GameObject.Instantiate(modulesList[i]);
            newModule.transform.SetParent(contentPanel);
            modulesList[i] = newModule;
        }
    }

    public void UpdateHighlightPosition(int index)
    {
        highlight.position = modulesList[index].transform.position;
    }


}
