using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestBeconDetection : MonoBehaviour
{
    private Transform m_player;
    private Camera m_camera;
    private Vector2 m_screenPos;


    [SerializeField, Tooltip("Select/Set Loot Type")]
    private QuestType m_questType;

    [SerializeField, Tooltip("Put quest here")]
    private Quest m_quest;

    private GameObject m_questTarget;

    public GameObject QuestTarget { get => m_questTarget; set => m_questTarget = value; }
    public QuestType QuestType { get => m_questType; set => m_questType = value; }
    public Quest Quest { get => m_quest; set => m_quest = value; }

    private void Start()
    {
        try
        {
            m_camera = Camera.main;
            m_player = GameObject.FindGameObjectsWithTag("Player")[0].GetComponent<Transform>();
        }
        catch { }
    }
    // Update is called once per frame
    void Update()
    {
        float _playerDistance = Vector3.Distance(m_player.position, transform.position);
        m_screenPos = m_camera.WorldToScreenPoint(this.transform.position);

        if (_playerDistance < HUDManager.Instance.LootViewDistance && IsVisibleFrom(GetComponent<Renderer>(), Camera.main))
        {
            HUDManager.Instance.DrawBeaconTarget(m_screenPos, this);
        }
        else
        {
            HUDManager.Instance.ClearBeaconTarget(this);
        }

    }

    private bool IsVisibleFrom(Renderer renderer, Camera camera)
    {
        //Creates planes emitting from selected camera to detect if object is visible. Returns true if it is.
        Plane[] planes = GeometryUtility.CalculateFrustumPlanes(camera);
        return GeometryUtility.TestPlanesAABB(planes, renderer.bounds);
    }
}
