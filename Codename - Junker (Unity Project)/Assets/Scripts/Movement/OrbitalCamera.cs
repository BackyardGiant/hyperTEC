using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrbitalCamera : MonoBehaviour
{
    private const float Y_ANGLE_MIN = -50.0f;
    private const float Y_ANGLE_MAX = 50.0f;
    private const float X_ANGLE_MIN = -155.0f;
    private const float X_ANGLE_MAX = 155.0f;
    public Transform lookAt;
    public GameObject target;

    private float distance = 1.0f;
    private float currentX = 0.0f;
    private float currentY = 0.0f;
    public float turningSpeed = 60;

    public float rotateSpeed = 5;
    Vector3 offset;

    public float snapSpeed;

    public float CameraDistance;
    public float CameraAngle;

    public float CurrentX { get => currentX; set => currentX = value; }
    public float CurrentY { get => currentY; set => currentY = value; }

    private void Start()
    {
        offset = target.transform.localPosition - transform.localPosition;
    }

    private void Update()
    {
        if (Mathf.Abs(Input.GetAxis("LookX")) > 0 || Mathf.Abs(Input.GetAxis("LookY")) > 0)
        {
            CurrentY += Input.GetAxis("LookX") * turningSpeed;
            CurrentX += Input.GetAxis("LookY") * turningSpeed;
        }
        else
        {
            CurrentY = Mathf.Lerp(CurrentY, 0, snapSpeed / 100);
            CurrentX = Mathf.Lerp(CurrentX, 0, snapSpeed / 100);
        }

        CurrentY = Mathf.Clamp(currentY, Y_ANGLE_MIN, Y_ANGLE_MAX);
        CurrentX = Mathf.Clamp(currentX, X_ANGLE_MIN, X_ANGLE_MAX);
    }

    private void LateUpdate()
    {

        //if (Input.GetAxis("Vertical") == 0)
        //{
        //    Vector3 dir = new Vector3(0, 0, distance);
        //    Quaternion rotation = Quaternion.Euler(-1 * currentY, currentX, 0);
        //    camTransform.position = Vector3.Lerp(transform.position, lookAt.position + rotation * dir * 1.6f, Time.deltaTime * 3);
        //    camTransform.LookAt(lookAt.position);
        //}

        //else
        //{
        Quaternion rotation = Quaternion.Euler(-1 * currentY, currentX, 0);

        transform.localPosition = Vector3.Lerp(transform.localPosition, target.transform.localPosition - (rotation * offset * CameraDistance), Time.deltaTime * 4);

        transform.LookAt(target.transform.position, target.transform.up);
        //}
    }
}
