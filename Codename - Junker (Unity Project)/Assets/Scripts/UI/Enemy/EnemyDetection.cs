using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDetection : MonoBehaviour
{
    private Rect m_position;
    private Vector2 m_screenPos;

    private Camera m_camera;
    private Transform m_player;

    [SerializeField, Tooltip("Distance at which the crosshair should be visible. 350 seems appropriate")]
    private int m_viewDistance;

    private GameObject m_enemyTarget;

    public GameObject EnemyTarget { get => m_enemyTarget; set => m_enemyTarget = value; }

    private void Start()
    {
        m_camera = Camera.main;
        m_player = GameObject.FindGameObjectsWithTag("Player")[0].GetComponent<Transform>();
    }

    // Update is called once per frame
    private void Update()
    {
        //Set distance from enemy to player. Used to check if enemy is a target within valid distance
        float _playerDistance = Vector3.Distance(m_player.position, transform.position);

        m_screenPos = m_camera.WorldToScreenPoint(transform.position);

        if (_playerDistance < HUDManager.Instance.ViewDistance)
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
