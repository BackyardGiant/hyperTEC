using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicFreeviewCamera : MonoBehaviour
{
     private Camera m_camera;
    [SerializeField] private float m_moveSpeed;
    private float m_finalSpeed;
    void Start()
    {
        m_camera = this.GetComponent<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.LeftShift))
        {
            m_finalSpeed = m_moveSpeed * 2;
        }
        else
        {
            m_finalSpeed = m_moveSpeed;
        }
        if (Input.GetKey(KeyCode.W))
        {
            transform.Translate(transform.forward * m_finalSpeed, Space.World);
        }
        if (Input.GetKey(KeyCode.S))
        {
            transform.Translate(-transform.forward * m_finalSpeed * 0.8f, Space.World);
        }
        if (Input.GetKey(KeyCode.A))
        {
            transform.Translate(-transform.right * m_finalSpeed, Space.World);
        }
        if (Input.GetKey(KeyCode.D))
        {
            transform.Translate(transform.right * m_finalSpeed, Space.World);
        }

        float mouseX = (Input.mousePosition.x / Screen.width) - 0.5f;
        float mouseY = (Input.mousePosition.y / Screen.height) - 0.5f;
        transform.localRotation = Quaternion.Euler(new Vector4(-1f * (mouseY * 180f), mouseX * 360f, transform.localRotation.z));

    }
}
