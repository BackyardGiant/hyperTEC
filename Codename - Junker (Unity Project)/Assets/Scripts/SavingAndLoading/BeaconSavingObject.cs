using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeaconSavingObject
{
    public string positionX;
    public string positionY;
    public string positionZ;
    public string rotationX;
    public string rotationY;
    public string rotationZ;
    public string rotationW;
    public string questName;
    public string description;
    public string questType;
    public string size;
    public string rewardName;
    public string[] factions;

    public BeaconSavingObject(Vector3 _pos, Quaternion _rot, string _name, string _description, string _type, string _size, string _rewardName, string[] _factions)
    {
        positionX = _pos.x.ToString();
        positionY = _pos.y.ToString();
        positionZ = _pos.z.ToString();
        rotationX = _rot.x.ToString();
        rotationY = _rot.y.ToString();
        rotationZ = _rot.z.ToString();
        rotationW = _rot.w.ToString();
        questName = _name;
        description = _description;
        questType = _type;
        size = _size;
        rewardName = _rewardName;
        factions = _factions;
    }
}
