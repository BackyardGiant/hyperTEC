using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Quest
{
    private string m_name;
    private string m_description;
    private QuestType m_questType;
    private bool m_complete;
    private float m_percentageComplete;
    private int m_size;
    private int m_currentAmountCompleted;

    public QuestType QuestType { get => m_questType; }
    public bool Complete { get => m_complete; set => m_complete = value; }
    public float PercentageComplete { get => m_percentageComplete; set => m_percentageComplete = value; }
    public int Size { get => m_size; }
    public int CurrentAmountCompleted { get => m_currentAmountCompleted; }
    public string Name { get => m_name; }
    public string Description { get => m_description; }

    public Quest(QuestType _questType, int _size, string _name, string _description)
    {
        m_questType = _questType;
        m_size = _size;
        m_name = _name;
        m_description = _description;
    }

    public Quest(QuestType _questType, GameObject _objective)
    {
        m_questType = _questType;
    }

    public void QuestIncriment(int _incriment)
    {
        m_currentAmountCompleted += _incriment;

        m_percentageComplete = ((float)m_size / (float)m_currentAmountCompleted) * 100;

        CheckComplete();
    }

    public void QuestIncriment()
    {
        m_percentageComplete = 100;

        CheckComplete();
    }

    private void CheckComplete()
    {
        if(m_percentageComplete == 100)
        {
            m_complete = true;
            QuestManager.Instance.CompleteQuest(this);
        }
    }
}
