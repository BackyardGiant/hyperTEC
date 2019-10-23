using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DisplayOptions : MonoBehaviour
{
    public RectTransform contentPanel;
    public RectTransform highlight;

    [SerializeField]
    private List<GameObject> m_modulesList;

    public List<GameObject> ModulesList { get => m_modulesList; set => m_modulesList = value; }

    private void Awake()
    {
        for (int i = 0; i < m_modulesList.Count; i++)
        {
            GameObject _newModule = GameObject.Instantiate(m_modulesList[i]);
            _newModule.transform.SetParent(contentPanel);
            m_modulesList[i] = _newModule;
        }
    }

    public void UpdateHighlightPosition(int index)
    {
        highlight.position = m_modulesList[index].transform.position;
    }


}
