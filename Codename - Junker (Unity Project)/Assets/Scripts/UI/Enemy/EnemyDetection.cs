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

    public Camera Camera;
    public Transform Player;
    public Texture2D enemyCrosshairSquare;
    public Texture2D enemyPointerArrow;

    [SerializeField, Header("Enemy Detection Square Crosshair"), Tooltip("Color of the Enemy Detection Crosshair")]
    private Color m_crosshairColour;
    [SerializeField,Tooltip("Size of the Enemy Detection Crosshair")]
    private float m_enemyCrosshairSize;
    [SerializeField, Tooltip("Distance at which the crosshair should be visible. 200 seems appropriate")]
    private int m_viewDistance;


    [SerializeField, Header("Enemy Pointer Arrow Crosshair"), Tooltip("Color of the Enemy Detection Arrow")]
    private Color m_arrowColour;
    [SerializeField, Tooltip("Size of the Enemy Detection Arrow")]
    private float m_arrowSize;
    [SerializeField, Tooltip("Gap between arrows and edge of screen")]
    private float m_edgeBuffer;



    // Update is called once per frame
    void Update()
    {
        //Set distance from enemy to player. Used to check if enemy is a target within valid distance
        m_playerDistance = Vector3.Distance(Player.position, transform.position);

        m_screenPos = Camera.WorldToScreenPoint(transform.position);

        if (m_playerDistance < m_viewDistance)
        {
            m_screenPos.y = Screen.height - m_screenPos.y - m_enemyCrosshairSize / 2;
            m_screenPos.x = m_screenPos.x - m_enemyCrosshairSize / 2;
            if (IsVisibleFrom(GetComponent<Renderer>(), Camera.main))
            {
                //Draw Rect for on screen enemy square
                m_position = new Rect(m_screenPos, new Vector2(m_enemyCrosshairSize, m_enemyCrosshairSize));
            }
            else
            {
                m_screenPos.y = Mathf.Clamp(m_screenPos.y, m_edgeBuffer, Screen.height - m_enemyCrosshairSize - m_edgeBuffer);
                m_screenPos.x = Mathf.Clamp(m_screenPos.x, m_edgeBuffer, Screen.width - m_enemyCrosshairSize - m_edgeBuffer);
                m_position = new Rect(m_screenPos, new Vector2(m_enemyCrosshairSize, m_enemyCrosshairSize));

            }
        }
    }

    void OnGUI()
    {
        //Draws regular square crosshair on enemy if it's within distance and view of player.
        if (IsVisibleFrom(GetComponent<Renderer>(),Camera.main) && m_playerDistance < m_viewDistance)
        {
            GUI.color = m_crosshairColour;
            GUI.DrawTexture(m_position, enemyCrosshairSquare);
        }
        else if(m_playerDistance < m_viewDistance)
        {

            GUI.color = m_arrowColour;
            float _angle = Vector2.Angle(new Vector2(Screen.width/2,Screen.height/2),m_screenPos);
            

            Matrix4x4 matrixBackup = GUI.matrix;
            {
                GUIUtility.RotateAroundPivot(_angle,m_screenPos);
                GUI.DrawTexture(m_position, enemyPointerArrow);
            }
            GUI.matrix = matrixBackup;
    
        }
    }

    private bool IsVisibleFrom(Renderer renderer, Camera camera)
    {
        //Creates planes emitting from selected camera to detect if object is visible. Returns true if it is.
        Plane[] planes = GeometryUtility.CalculateFrustumPlanes(camera);
        return GeometryUtility.TestPlanesAABB(planes, renderer.bounds);
    }
}
