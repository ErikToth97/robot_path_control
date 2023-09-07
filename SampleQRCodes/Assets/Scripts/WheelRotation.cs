using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WheelRotation : MonoBehaviour
{
    public Transform frontLeftWheel;
    public Transform frontRightWheel;
    public Transform rearLeftWheel;
    public Transform rearRightWheel;
    private Vector3 previousPosition;

    public float maxSteerAngle = 45f;
    public float turnSpeed = 5f;

    private float steerAngle = 0f;
    private float currentVelocity;
    private float currentSpeed;
    void Start()
    {
        // Initialize the previous position to the object's starting position
        previousPosition = this.transform.position;
    }

    void rotateWheel(Transform wheel)
    {
        //wheel.localRotation = wheel.localRotation*Quaternion.AngleAxis(currentSpeed, Vector3.forward);
        wheel.Rotate(new Vector3(0, 0, -1.0f), currentSpeed);
    }
    void Update()
    {
        Vector3 currentVelocity = (this.transform.position - previousPosition) / Time.deltaTime;
        currentSpeed = currentVelocity.magnitude*1.5f;

        if (currentSpeed > 5)
            currentSpeed = 5;
        float steeringAngle = -Vector3.Angle(currentVelocity, this.transform.forward);

        rotateWheel(frontLeftWheel);
        rotateWheel(frontRightWheel);
        rotateWheel(rearRightWheel);
        rotateWheel(rearLeftWheel);
        frontLeftWheel.localEulerAngles = new Vector3(frontLeftWheel.localEulerAngles.x, Mathf.Clamp(steeringAngle, -maxSteerAngle, maxSteerAngle), frontLeftWheel.localEulerAngles.z);
        frontRightWheel.localEulerAngles = new Vector3(frontRightWheel.localEulerAngles.x, Mathf.Clamp(steeringAngle, -maxSteerAngle, maxSteerAngle), frontRightWheel.localEulerAngles.z);

       // rearLeftWheel.localEulerAngles = new Vector3(rearLeftWheel.localEulerAngles.x, rearLeftWheel.localEulerAngles.y, currentVelocity * turnSpeed);
        //rearRightWheel.localEulerAngles = new Vector3(rearRightWheel.localEulerAngles.x, rearRightWheel.localEulerAngles.y, currentVelocity * turnSpeed);
        previousPosition = transform.position;
    }
}