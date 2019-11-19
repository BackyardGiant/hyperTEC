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
    public EngineData Data { get => data; set => data = value; }

    public void PopulateData()
    {
        name.text = data.name;
        thumbnail.sprite = data.Thumbnail;

        if (thumbnail.sprite == null)
        {
            Color _tempColour = thumbnail.color;
            _tempColour.a = 0f;
            thumbnail.color = _tempColour;
        }
    }
}
