using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveAround : MonoBehaviour
{
    public GameObject jeep;
    public GameObject SUV;
    public GameObject sportCar;
    public GameObject referencePoint;

    private float RotationSpeed = 1;

    private float CircleRadius = 2;

    private float ElevationOffset = 0;

    private Vector3 positionOffset;
    private float angle, angle2, angle3;
    // Start is called before the first frame update
    void Start()
    {
        angle2 = 3.1415f;
        angle3 = 3.1415f / 2;
    }

    void moveObejct(GameObject ob, float offAngle)
    {
        Vector3 posOffset = new Vector3();
        posOffset.Set(Mathf.Cos(offAngle) * CircleRadius, ElevationOffset, Mathf.Sin(offAngle) * CircleRadius);
        var moveDir = ob.transform.localPosition;
        ob.transform.localPosition = posOffset;
        moveDir = posOffset - moveDir;
        Quaternion rotation = Quaternion.LookRotation(moveDir, Vector3.up).normalized;
        Quaternion ninety = Quaternion.Euler(0, -90, 0);
        ob.transform.localRotation = rotation * ninety;
    }

    // Update is called once per frame
    void Update()
    {
        //ElevationOffset = referencePoint.transform.position.y;
        moveObejct(SUV, angle);
        moveObejct(jeep, angle2);
        moveObejct(sportCar, angle3);
        angle += Time.deltaTime * RotationSpeed;
        angle2 += Time.deltaTime * RotationSpeed;
        angle3 += Time.deltaTime * RotationSpeed;

    }
}
