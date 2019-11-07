using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerSavingObject
{
    Vector3 m_position;
    Quaternion m_rotation;
    string m_rightWeaponSeed;
    string m_leftWeaponSeed;
    string m_engineSeed;


    public PlayerSavingObject(Vector3 _pos, Quaternion _rot, string _rightSeed, string _leftSeed, string _engineSeed)
    {
        m_position = _pos;
        m_rotation = _rot;
        m_rightWeaponSeed = _rightSeed;
        m_leftWeaponSeed = _leftSeed;
        m_engineSeed = _engineSeed;
    }

    public Vector3 Position { get => m_position; set => m_position = value; }
    public Quaternion Rotation { get => m_rotation; set => m_rotation = value; }
    public string RightWeaponSeed { get => m_rightWeaponSeed; set => m_rightWeaponSeed = value; }
    public string LeftWeaponSeed { get => m_leftWeaponSeed; set => m_leftWeaponSeed = value; }
    public string EngineSeed { get => m_engineSeed; set => m_engineSeed = value; }
}
