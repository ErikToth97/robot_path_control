using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class robotPathSimulation : MonoBehaviour
{
    // Start is called before the first frame update
    private List<Vector3> syncPositions;
    private float stepTime = 3.0f, startTime;
    private int ind = 0;
    private bool simuStarted = false;
    public GameObject robotArm;
    void Start()
    {
        syncPositions = new List<Vector3>();
        syncPositions.Add(new Vector3(0.173f, 0f, -0.707f));
        syncPositions.Add(new Vector3(0.555f, 0.329f, -0.425f));
        syncPositions.Add(new Vector3(0.425f, 0.329f, -0.189f));
        syncPositions.Add(new Vector3(0.85f, 0.15f, -0.189f));
        syncPositions.Add(new Vector3(0.425f, 0.329f, -0.189f));
        syncPositions.Add(new Vector3(0.425f, 0.329f, 0.236f));
        syncPositions.Add(new Vector3(0.776f, 0f, 0.246f));
        this.gameObject.transform.position = syncPositions[0];
        startTime = Time.time;
        ind = 1;
    }

    // Update is called once per frame
    void Update()
    {
        if (simuStarted)
        {
            if (ind < syncPositions.Count)
            {
                var timeDiff = Time.time - startTime;
                if (timeDiff >= stepTime)
                {
                    startTime = Time.time;
                    this.gameObject.transform.position = syncPositions[ind];
                    ind++;
                    var angles = robotArm.GetComponent<InverseMapControll>().getAngles();
                    string debLog = "";
                    for (int i = 0; i < 6; i++)
                    {
                        debLog += angles[i] + " ";
                    }
                    Debug.Log(debLog);
                }
                else
                {
                    var t = timeDiff / stepTime;
                    var newPos = (1 - t) * syncPositions[ind - 1] + t * syncPositions[ind];
                    this.gameObject.transform.position = newPos;
                }
            }
            else
                simuStarted = false;
        }
      
    }

    public void startSimulation()
    {
        if (!simuStarted)
        {
            ind = 1;
            startTime = Time.time;
            simuStarted = true;
        }
        else
            simuStarted = false;
    }
}
