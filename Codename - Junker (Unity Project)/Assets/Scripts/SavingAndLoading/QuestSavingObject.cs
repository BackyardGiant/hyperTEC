using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class QuestSavingObject
{
    public string name;
    public string description;
    public string questType;
    public string percentageComplete;
    public string size;
    public string currentAmountCompleted;
    public string[] factions;

    public QuestSavingObject(string _name, string _description, string _type, string _percentageComplete, string _size, string _currentAmountComplete, string[] _factions)
    {
       name = _name;
       description = _description;
       questType = _type;
       percentageComplete = _percentageComplete;
       size = _size;
       currentAmountCompleted = _currentAmountComplete;
       factions = _factions;
    }
}
