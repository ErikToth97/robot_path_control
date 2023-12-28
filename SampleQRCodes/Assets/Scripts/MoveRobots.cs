using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveRobots : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject futureArm, UR5;
    private Vector3 lastPos;
    private Quaternion lastOri;
    void Start()
    {
        lastPos = this.transform.position;
        lastOri = this.transform.rotation;
    }

    // Update is called once per frame
    void Update()
    {
        if(lastPos != this.transform.position || lastOri != this.transform.rotation)
        {
            futureArm.transform.position += this.transform.position - lastPos;
            UR5.transform.position += this.transform.position - lastPos;
            lastPos = this.transform.position;
            
            lastOri = this.transform.rotation;
            futureArm.transform.rotation = lastOri;
            UR5.transform.rotation = lastOri;
        }
    }
}
