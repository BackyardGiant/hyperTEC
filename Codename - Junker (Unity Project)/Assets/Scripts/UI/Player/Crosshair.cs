using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Crosshair : MonoBehaviour
{
    private Vector2 m_position;

    [SerializeField, Header("Neutral crosshair"), Tooltip("Changes the height of the crosshair on screen")]
    private float m_heightScale;
    [SerializeField, Tooltip("Opacity of the neutral crosshair")]
    private float m_nuetralOpacity;
    private float m_heightMod = 1; // Fixed pixel version of height scale
    private Vector2 m_defaultPosition;
    private Vector2 m_targetPosition;
    private bool m_hasTarget = false;
    public Texture2D crosshairDot;
    public GameObject hudImage;
    private Camera m_camera;

    //[SerializeField, Header("Player crosshair")]
    //private Transform m_playerLookTarget;
    //[SerializeField, Tooltip("Sets the size of the crosshair")]
    //private float m_crosshairScale;
    //[SerializeField, Tooltip("Opacity of the player crosshair")]
    //private float m_playerOpacity;
    //public Texture2D crosshairCircle;

    public Vector2 TargetPosition { get => m_targetPosition; set => m_targetPosition = value; }
    public bool HasTarget { get => m_hasTarget; set => m_hasTarget = value; }

    private void Start()
    {
        //crosshairDot = Texture2D.whiteTexture;
        m_camera = GetComponent<Camera>();

        m_defaultPosition = new Vector2(Screen.width / 2, Screen.height / 2);
    }

    // Update is called once per frame
    void Update()
    {
        if (m_hasTarget)
        {
            m_position = m_targetPosition;
        }
        else
        {
            m_position = m_defaultPosition;
        }

        hudImage.transform.position = m_position;

        hudImage.GetComponent<Image>().color = new Color(hudImage.GetComponent<Image>().color.r, hudImage.GetComponent<Image>().color.g, hudImage.GetComponent<Image>().color.b, m_nuetralOpacity);
    }
}
