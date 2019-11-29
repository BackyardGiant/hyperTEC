using UnityEngine;

[System.Serializable]
public class asteroidClass
{
    public GameObject asteroidObject;
    [Range(0.001f, 1f)]
    public float rarity;
}