using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EnemySavingObject
{
    public string positionX;
    public string positionY;
    public string positionZ;
    public string rotationX;
    public string rotationY;
    public string rotationZ;
    public string rotationW;
    public string rightWeaponSeed;
    public string leftWeaponSeed;
    public string engineSeed;


    public EnemySavingObject(Vector3 _pos, Quaternion _rot, string _rightSeed, string _leftSeed, string _engineSeed)
    {
        positionX = _pos.x.ToString();
        positionY = _pos.y.ToString();
        positionZ = _pos.z.ToString();
        rotationX = _rot.x.ToString();
        rotationY = _rot.y.ToString();
        rotationZ = _rot.z.ToString();
        rotationW = _rot.w.ToString();
        rightWeaponSeed = _rightSeed;
        leftWeaponSeed = _leftSeed;
        engineSeed = _engineSeed;
    }

}
