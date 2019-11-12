using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WeaponStatManager : MonoBehaviour
{
    public Text name;
    public Image thumbnail;

    [SerializeField]
    private WeaponData data;
    public WeaponData Data { get => data; set => data = value; }

    public void PopulateData()
    {
        name.text = data.Name;
        thumbnail.sprite = data.Thumbnail;

        if(thumbnail.sprite == null)
        {
            Color _tempColour = thumbnail.color;
            _tempColour.a = 0f;
            thumbnail.color = _tempColour;
        }
    }
}
