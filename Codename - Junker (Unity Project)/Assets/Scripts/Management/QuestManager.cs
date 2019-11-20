using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum QuestType
{
    kill, collect, control, recon, targets 
}

public class QuestManager : MonoBehaviour
{
    private static QuestManager s_instance;
    private List<Quest> m_currentQuests = new List<Quest>();
    private int m_trackingQuestIndex;

    public List<Quest> CurrentQuests { get => m_currentQuests; set => m_currentQuests = value; }
    public static QuestManager Instance { get => s_instance; set => s_instance = value; }
    public int TrackingQuestIndex { get => m_trackingQuestIndex; set => m_trackingQuestIndex = value; }

    private void Awake()
    {
        //Singleton Implementation
        if (s_instance == null)
        {
            s_instance = this;
        }
        else
        {
            Destroy(s_instance.gameObject);
            s_instance = this;
        }
    }

    #region Quest Creation
    public void CreateKillQuest(int _numberOfKills, string _name, string _description)
    {
        Quest newKillQuest = new Quest(QuestType.kill, _numberOfKills, _name, _description);
        m_currentQuests.Add(newKillQuest);
    }

    public void CreateCollectQuest(GameObject _objectToCollect, string _name, string _description)
    {
        Quest newCollectQuest = new Quest(QuestType.collect, _objectToCollect, _name, _description);
        m_currentQuests.Add(newCollectQuest);
    }

    public void CreateControlQuest(int _numberOfSectors, string _name, string _description)
    {
        Quest newControlQuest = new Quest(QuestType.control, _numberOfSectors, _name, _description);
        m_currentQuests.Add(newControlQuest);
    }

    public void CreateReconQuest(int _numberOfDiscoveries, string _name, string _description)
    {
        Quest newReconQuest = new Quest(QuestType.recon, _numberOfDiscoveries, _name, _description);
        m_currentQuests.Add(newReconQuest);
    }

    public void CreateTargetQuest(int _numberOfTargets, string _name, string _description)
    {
        Quest newTargetQuest = new Quest(QuestType.collect, _numberOfTargets, _name, _description);
        m_currentQuests.Add(newTargetQuest);
    }
    #endregion

    #region Quest Progression
    public void IncrementKillQuests()
    {
        foreach (Quest _quest in m_currentQuests)
        {
            if (_quest.QuestType == QuestType.kill)
            {
                _quest.QuestIncrement(1);
            }
        }
    }
    public void IncrimentCollectQuests()
    {
        foreach (Quest _quest in m_currentQuests)
        {
            if (_quest.QuestType == QuestType.collect)
            {
                _quest.QuestIncriment();
            }
        }
    }
    public void IncrimentControlQuests()
    {
        foreach (Quest _quest in m_currentQuests)
        {
            if (_quest.QuestType == QuestType.control)
            {
                _quest.QuestIncrement(1);
            }
        }
    }
    public void IncrimentReconQuests()
    {
        foreach (Quest _quest in m_currentQuests)
        {
            if (_quest.QuestType == QuestType.recon)
            {
                _quest.QuestIncrement(1);
            }
        }
    }
    public void IncrimentTargetQuests()
    {
        foreach (Quest _quest in m_currentQuests)
        {
            if (_quest.QuestType == QuestType.targets)
            {
                _quest.QuestIncrement(1);
            }
        }
    }
    #endregion

    public void ClearQuests()
    {
        m_currentQuests = new List<Quest>();
        m_trackingQuestIndex = 0;
    }

    public void CompleteQuest(Quest _completedQuest)
    {

    }
}
