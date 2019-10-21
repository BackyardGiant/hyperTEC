using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crosshair : MonoBehaviour
{
    private Rect m_position;
    private Rect m_playerPosition;
    private Vector3 m_screenPos;

    [SerializeField, Header("Neutral crosshair"), Tooltip("Changes the height of the crosshair on screen")]
    private float m_heightScale;
    [SerializeField, Tooltip("Opacity of the neutral crosshair")]
    private float m_nuetralOpacity;
    private float m_heightMod = 1; // Fixed pixel version of height scale
    public Texture2D crosshairDot;

    private Camera m_camera;
    [SerializeField, Header("Player crosshair")]
    private Transform m_playerLookTarget;
    [SerializeField, Tooltip("Sets the size of the crosshair")]
    private float m_crosshairScale;
    [SerializeField, Tooltip("Opacity of the player crosshair")]
    private float m_playerOpacity;
    public Texture2D crosshairCircle;

    private void Start()
    {
        //crosshairDot = Texture2D.whiteTexture;
        m_camera = GetComponent<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
        m_position = new Rect((Screen.width - crosshairDot.width * m_crosshairScale) / 2, (Screen.height - crosshairDot.height * m_crosshairScale) / 2, crosshairDot.width * m_crosshairScale, crosshairDot.height * m_crosshairScale);

        //m_screenPos = m_camera.WorldToScreenPoint(m_playerLookTarget.position); //returns vector for moving a 3d point into screen space 

        //m_playerPosition = new Rect(m_screenPos.x - (crosshairCircle.width * m_crosshairScale/ 2) , m_screenPos.y - (crosshairCircle.height * m_crosshairScale / 2), crosshairCircle.width * m_crosshairScale, crosshairCircle.height * m_crosshairScale); // Makes that vector a rect

    }

    void OnGUI()
    {
        GUI.color = new Color(1, 1, 1, m_nuetralOpacity);

        GUI.DrawTexture(m_position, crosshairDot);

        //GUI.color = new Color(1, 1, 1, m_playerOpacity);

        //GUI.DrawTexture(m_playerPosition, crosshairCircle);
    }
}
