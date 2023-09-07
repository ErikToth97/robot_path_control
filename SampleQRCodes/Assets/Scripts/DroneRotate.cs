using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroneRotate : MonoBehaviour
{
    public GameObject[] objectsCW;
    public GameObject[] objectsCCW;
    public float  yAngle;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        foreach(GameObject g in objectsCW)
        {
            //transform.RotateAround(g.transform.position, Vector3.up, 20 * Time.deltaTime);
            g.transform.Rotate(0, yAngle, 0, Space.Self);
        }
        foreach (GameObject g in objectsCCW)
        {
            //transform.RotateAround(g.transform.position, Vector3.up, 20 * Time.deltaTime);
            g.transform.Rotate(0, -yAngle, 0, Space.Self);
        }
    }
}
