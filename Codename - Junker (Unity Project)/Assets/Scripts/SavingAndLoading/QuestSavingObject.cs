using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class QuestSavingObject
{
    public string m_name;
    public string m_description;
    public string m_questType;
    public string m_percentageComplete;
    public string m_size;
    public string m_currentAmountCompleted;

    public QuestSavingObject(string _name, string _description, string _type, string _percentageComplete, string _size, string _currentAmountComplete)
    {
        m_name = _name;
        m_description = _description;
        m_questType = _type;
        m_percentageComplete = _percentageComplete;
        m_size = _size;
        m_currentAmountCompleted = _currentAmountComplete;
    }
}
