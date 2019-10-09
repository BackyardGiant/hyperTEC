using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDetection : MonoBehaviour
{
    private Rect m_position;
    private Vector3 m_screenPos;
    private Vector3 m_viewportPos;
    private Vector2 m_arrowDrawPos;
    private float m_playerDistance;//Distance from enemy to player

    [Header ("")]
    public Camera Camera;
    public Transform Player;

    [SerializeField, Tooltip("Distance at which the crosshair should be visible. 350 seems appropriate")]
    private int m_viewDistance;

    private GameObject m_target;

    public GameObject Target { get => m_target; set => m_target = value; }

    // Update is called once per frame
    void Update()
    {
        //Set distance from enemy to player. Used to check if enemy is a target within valid distance
        m_playerDistance = Vector3.Distance(Player.position, transform.position);

        m_screenPos = Camera.WorldToScreenPoint(transform.position);

        if (m_playerDistance < m_viewDistance)
        {
            if (IsVisibleFrom(GetComponent<Renderer>(), Camera.main))
            {
                //If player is visible and within range, draw the target. 
                HUDManager.Instance.DrawEnemyTarget(m_screenPos, this);
            }
            else
            {
                //If it isn't visible, draw the arrow.
                HUDManager.Instance.DrawEnemyArrow(m_screenPos, this);


            }
        }
        else
        {
            HUDManager.Instance.ClearEnemyDetection(this);
        }
    }
    private bool IsVisibleFrom(Renderer renderer, Camera camera)
    {
        //Creates planes emitting from selected camera to detect if object is visible. Returns true if it is.
        Plane[] planes = GeometryUtility.CalculateFrustumPlanes(camera);
        return GeometryUtility.TestPlanesAABB(planes, renderer.bounds);
    }
}
