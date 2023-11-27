using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RayCastSpline : MonoBehaviour
{
    public GameObject hitObj;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        RaycastHit hit;
        int layerMask = 1 << 2;
        layerMask = ~layerMask;
        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.down), out hit,5, layerMask))
        {
            hitObj.transform.position = hit.point;
            Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.down) * hit.distance, Color.yellow);
            Debug.Log("Did Hit");
        }
        else
        {
            Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.down) * 5, Color.white);
            Debug.Log("Did not Hit");
        }
    }
}
