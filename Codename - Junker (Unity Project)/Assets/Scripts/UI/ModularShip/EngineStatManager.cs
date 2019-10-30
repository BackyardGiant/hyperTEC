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
        data.Name = name.text;
        thumbnail.sprite = data.Thumbnail;
    }
}
