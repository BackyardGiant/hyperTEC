using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EngineStatManager : MonoBehaviour
{
    public Text name;
    public Image thumbnail;

    [SerializeField]
    private EngineData data;
    [SerializeField]
    private Sprite m_constructionEngine, m_defaultEngine, m_traderEngine, m_explorerEngine;
    public EngineData Data { get => data; set => data = value; }

    public void PopulateData()
    {
        name.text = data.name;

        switch(data.CurrentFaction)
        {
            case EngineData.faction.construction:
                thumbnail.sprite = m_constructionEngine;
                break;
            case EngineData.faction.explorer:
                thumbnail.sprite = m_explorerEngine;
                break;
            case EngineData.faction.initial:
                thumbnail.sprite = m_defaultEngine;
                break;
            case EngineData.faction.trader:
                thumbnail.sprite = m_traderEngine;
                break;
        }
        

        if (thumbnail.sprite == null)
        {
            Color _tempColour = thumbnail.color;
            _tempColour.a = 0f;
            thumbnail.color = _tempColour;
        }
    }
}
