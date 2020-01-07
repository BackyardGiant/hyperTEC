using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class Quest : ScriptableObject
{
    [SerializeField]
    private string m_name;
    [SerializeField]
    private string m_description;
    [SerializeField]
    private GameObject m_reward;
    [SerializeField]
    private string m_rewardName;
    [SerializeField]
    private QuestType m_questType;
    [SerializeField]
    private QuestFactions[] m_factions;
    private bool m_complete;
    private float m_percentageComplete;
    [SerializeField]
    private int m_size;
    private int m_currentAmountCompleted;

    public QuestType QuestType { get => m_questType; set => m_questType = value; }
    public bool Complete { get => m_complete; set => m_complete = value; }
    public float PercentageComplete { get => m_percentageComplete; set => m_percentageComplete = value; }
    public int Size { get => m_size; set => m_size = value; }
    public int CurrentAmountCompleted { get => m_currentAmountCompleted; }
    public string Name { get => m_name; set => m_name = value; }
    public string Description { get => m_description; set => m_description = value; }
    public string RewardName { get => m_rewardName; set => m_rewardName = value; }
    public GameObject Reward { get => m_reward; set => m_reward = value; }
    public QuestFactions[] Factions { get => m_factions; set => m_factions = value; }

    public Quest(QuestType _questType, int _size, string _name, string _description)
    {
        m_questType = _questType;
        m_size = _size;
        m_name = _name;
        m_description = _description;
    }

    public Quest(QuestType _questType, int _size, string _name, string _description, QuestFactions[] factions)
    {
        m_questType = _questType;
        m_size = _size;
        m_name = _name;
        m_description = _description;
        m_factions = factions;
    }

    public Quest(QuestType _questType, GameObject _objective, string _name, string _description)
    {
        m_questType = _questType;
        m_name = _name;
        m_description = _description;
    }

    public void QuestIncrement(int _incriment)
    {
        m_currentAmountCompleted += _incriment;

        m_percentageComplete = ((float)m_currentAmountCompleted / (float)m_size) * 100;

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




//Property of BackyardGiant
