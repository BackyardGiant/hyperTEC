using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LootDetection : MonoBehaviour
{
    public enum m_lootTypes {Weapon,Engine,Shield};
    private Transform m_player;
    private Camera m_camera;
    private Vector2 m_screenPos;


    [SerializeField, Tooltip("Select/Set Loot Type")]
    private m_lootTypes m_lootType;

    private GameObject m_lootTarget;

    public GameObject LootTarget { get => m_lootTarget; set => m_lootTarget = value; }
    public m_lootTypes LootType { get => m_lootType; set => m_lootType = value; }

    private void Start()
    {
        try {
            m_camera = Camera.main;
            m_player = GameObject.FindGameObjectsWithTag("Player")[0].GetComponent<Transform>();
        }catch { }
    }
    // Update is called once per frame
    void Update()
    {
        float _playerDistance = Vector3.Distance(m_player.position, transform.position);
        m_screenPos = m_camera.WorldToScreenPoint(this.transform.position);

        if (_playerDistance < HUDManager.Instance.LootViewDistance && IsVisibleFrom(GetComponent<Renderer>(), Camera.main))
        {
            HUDManager.Instance.DrawLootTarget(m_screenPos,this);
        }
        else
        {
            HUDManager.Instance.ClearLootTarget(this);
        }

    }

    private bool IsVisibleFrom(Renderer renderer, Camera camera)
    {
        //Creates planes emitting from selected camera to detect if object is visible. Returns true if it is.
        Plane[] planes = GeometryUtility.CalculateFrustumPlanes(camera);
        return GeometryUtility.TestPlanesAABB(planes, renderer.bounds);
    }
}
