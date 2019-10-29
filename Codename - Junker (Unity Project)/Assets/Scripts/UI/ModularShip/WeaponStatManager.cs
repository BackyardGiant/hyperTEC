﻿using System.Collections;
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
        data.Name = name.text;
        thumbnail.sprite = data.Thumbnail;
    }
}
