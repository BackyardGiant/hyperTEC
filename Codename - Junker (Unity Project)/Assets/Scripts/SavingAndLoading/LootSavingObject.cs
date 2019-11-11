using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class LootSavingObject
{
    public string positionX;
    public string positionY;
    public string positionZ;
    public string rotationX;
    public string rotationY;
    public string rotationZ;
    public string rotationW;
    public string type;
    public string seed;


    public LootSavingObject(Vector3 _pos, Quaternion _rot, string _seed, string _type)
    {
        positionX = _pos.x.ToString();
        positionY = _pos.y.ToString();
        positionZ = _pos.z.ToString();
        rotationX = _rot.x.ToString();
        rotationY = _rot.y.ToString();
        rotationZ = _rot.z.ToString();
        rotationW = _rot.w.ToString();
        type = _type;
        seed = _seed;
    }
}
