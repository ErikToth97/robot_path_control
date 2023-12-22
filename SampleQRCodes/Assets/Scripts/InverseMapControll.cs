using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using InverseMap;
using System;

public class InverseMapControll : MonoBehaviour
{
    // Start is called before the first frame update
    UR5 _inverse;
    public GameObject futureArm;
    public float futureTime = 1.0f;
    private List<Transform> joints, futureJoints;
    private Transform rayStartPos, futureRay;
    private List<float[]> savedAngles;
    private float stepTime = 6.0f, startTime, lastTime, realStepTime, realLastTime, realStartTime;
    private int ind = 0, futureInd = 1, realInd, splineInd = 0;
    public int branchInd = 0;
    public double tcpdist = 100;
    public GameObject endEffector, futureEffector;
    private Vector3 lastPos, outMoveDir, outRayDir, outRayStart;
    private Quaternion lastOri;
    private List<Vector3> movingPoints, syncPositions, avodaincePath;
    private float[] syncTimes;
    private List<float[]> syncPoints;
    private bool[] crucialPoints;
    private GameObject syncPosObj;

    private bool lastAnglesSet = false, obstacleAvoidance = false, splineReady = false;
    private double[] lastAngles;
    private static float[] testangles;
    private static List<float[]> rotAxis;
    private List<GameObject> hitPlaces;
    private float[] splinU = new float[] { 1f, 4f, 3.75f, 3.73333333333333f, 3.73214285714286f, 
                                        3.73205741626794f, 3.73205128205128f, 3.73205084163518f, 3.73205081001473f, 3.73205080774448f, 
                                        3.73205080758149f, 3.73205080756978f, 3.73205080756894f, 3.73205080756888f, 3.73205080756888f };
    private float[] spineL = new float[] { 1f, 0.25f, 0.266666666666667f, 0.267857142857143f, 0.267942583732057f,
                                          0.267948717948718f, 0.267949158364823f, 0.267949189985272f, 0.267949192255519f, 0.267949192418515f,
                                          0.267949192430218f, 0.267949192431058f, 0.267949192431118f, 0.267949192431122f, 0.267949192431123f };

    public bool defaultValues = true, savedPath = true, pause = false, startSavedPath = false;
    public int numOfJoint = 6;
    public char[] axisList = new char[6] {'y', 'z', 'z', 'z', 'y', 'z', };
    void Start()
    {
        futureJoints = new List<Transform>();
        futureJoints.Add(futureArm.transform.Find("Cube/Cube.001"));
        joints = new List<Transform>();
        joints.Add(this.transform.Find("Cube/Cube.001"));
        rotAxis = new List<float[]>();
        rotAxis.Add(new float[3]);
        for (int j = 0; j < numOfJoint-1; j++)
        {
            joints.Add(joints[j].GetChild(0));
            futureJoints.Add(futureJoints[j].GetChild(0));
            rotAxis.Add(new float[3]);
        }
        rayStartPos = joints[5].GetChild(0).GetChild(0);
        futureRay = futureJoints[5].GetChild(0).GetChild(0);
        //initAxis();
        initAxis(defaultValues);

        _inverse = new UR5();
        lastPos = new Vector3();
        lastOri = new Quaternion();
        lastAngles = new double[6];
        savedAngles = new List<float[]>();
        savedAngles.Add(new float[6] { -22.35f, 41.41f, 76.87f, -28.28f, 90f, 157.64f });
        savedAngles.Add(new float[6] { -61.48f, 29.47f, 46.83f, 13.683f, 90f, 118.51f });
        savedAngles.Add(new float[6] { -90.92f, 8.83f, 74.9f, 6.28f, 90f, 89.075f });
        savedAngles.Add(new float[6] { -90.63f, 47.524f, 43.08f, -0.612f, 90f, 89.364f });
        savedAngles.Add(new float[6] { -90.92f, 8.83f, 74.9f, 6.28f, 90f, 89.075f });
        savedAngles.Add(new float[6] { -132.0f, 22.665f,56.966f, 10.36f, 90f, 47.915f });
        savedAngles.Add(new float[6] { -126.32f,43.824f, 72.47f, -26.29f, 90f, 53.68f });
        savedAngles.Add(new float[6] { -90.63f, 47.524f, 43.08f, -0.612f, 90f, 89.364f });
        savedAngles.Add(new float[6] { -22.35f, 41.41f, 76.87f, -28.28f, 90f, 157.64f });
        savedAngles.Add(new float[6] { -22.35f, 41.41f, 76.87f, -28.28f, 90f, 157.64f });
        syncTimes = new float[savedAngles.Count];
        for (int i = 0; i < savedAngles.Count; i++)
        {
            syncTimes[i] = i * 3.0f;
        }
        startTime = Time.time;
        lastTime = 0f;
        ind = 1;
        hitPlaces = new List<GameObject>();
        hitPlaces.Add(new GameObject("Down"));
        hitPlaces.Add(new GameObject("Up"));
        hitPlaces.Add(new GameObject("FromBase"));
        hitPlaces.Add(new GameObject("FutureX"));
        hitPlaces.Add(new GameObject("FutureY"));
        movingPoints = new List<Vector3>();
        syncPoints = savedAngles;

        syncPosObj = new GameObject("syncPosObj");
        syncPositions = new List<Vector3>();
        syncPositions.Add(new Vector3(0.173f, 0.0f, -0.707f));
        syncPositions.Add(new Vector3(0.55f, 0.33f, -0.42f));
        syncPositions.Add(new Vector3(0.55f, 0.33f, -0.1f));
        syncPositions.Add(new Vector3(0.80f, 0.15f, -0.1f));
        syncPositions.Add(new Vector3(0.55f, 0.33f, -0.1f));
        syncPositions.Add(new Vector3(0.55f, 0.33f, 0.35f));
        syncPositions.Add(new Vector3(0.66f, 0.0f, 0.35f));
        syncPositions.Add(new Vector3(0.80f, 0.15f, -0.1f));
        syncPositions.Add(new Vector3(0.173f, 0.0f, -0.707f));
        syncPositions.Add(new Vector3(0.173f, 0.0f, -0.707f));

        crucialPoints = new bool[syncPoints.Count];
        for (int i = 0; i < syncPoints.Count; i++)
        {
            crucialPoints[i] = false;
        }
        crucialPoints[3] = true;
        crucialPoints[6] = true;


        for (int i = 0; i < syncPositions.Count; i++)
        {
            var hitPlace = Instantiate(Resources.Load("HitPlace", typeof(GameObject))) as GameObject;
            hitPlace.name = "syncPosObj" + i;
            hitPlace.transform.SetParent(syncPosObj.transform);
            hitPlace.transform.position = syncPositions[i];
            hitPlace.transform.rotation = Quaternion.Euler(0, -180f, 180f);

            var bodyRenderer = hitPlace.GetComponent<Renderer>();
            bodyRenderer.material.SetColor("_Color", new Color(1f, 1.0f, 1));
            if(crucialPoints[i])
                bodyRenderer.material.SetColor("_Color", new Color(0f, 0f, 1));
        }

        avodaincePath = new List<Vector3>();

        realStepTime = 6.0f;
        realLastTime = 0.0f;
        realInd = 1;
        realStartTime = Time.time;
    }

    public double[] GetInverse(GameObject effector, double[] pose = null, double[] jointOffsets = null, int gripperId = -1)
    {
        var pos = effector.transform.position * 1000f;
        lastPos = pos;
        double[] full = new double[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
        var ori = effector.transform.rotation.eulerAngles;
        var switchedAxis = new Vector3(ori.x, -ori.z, -ori.y);
        Matrix4x4 rot = Matrix4x4.Rotate(Quaternion.Euler(switchedAxis));
        _inverse.SetInputVariable(new string[] { "Ox", "Oy", "Oz", "ax", "ay", "az", "bx", "by", "bz", "cx", "cy", "cz","tcpdist" }, 
            new double[] { -pos.x, pos.z, pos.y,
                            rot[0, 0], rot[0, 1], rot[0, 2], 
                            rot[1, 0], rot[1, 1], rot[1, 2],
                            rot[2, 0], rot[2, 1], rot[2, 2], 
                            tcpdist});
        _inverse.Calculate();
        var inverseBranches = _inverse.GetOutputVariables(new string[] { "q1", "q2", "q3", "q4", "q5", "q6" }, _inverse.BranchValid);

        if (inverseBranches.Length == 0)
            return new double[] { 0 };

        //return inverseBranches[0];
        //double[] actualJoints = joints;
        double[] bestJoints = new double[6];

        double bestJointRotSum = double.MaxValue;
        List<int> validBranchInd = new List<int>();
        for (int b = 0; b < inverseBranches.Length; ++b)
        {
            var degInvB = moveAngleIntoRange((inverseBranches[b][1]+ jointOffsets[1]) * Mathf.Rad2Deg);
            if (degInvB > -100.0 && degInvB < 100.0)
                validBranchInd.Add(b);
        }
        double[][] validBranches = new double[validBranchInd.Count][];
        for (int i = 0; i < validBranchInd.Count; i++)
        {
            validBranches[i] = inverseBranches[validBranchInd[i]];
        }
        if (validBranches.Length == 0)
            return new double[] { 0 };
        double[] diffs = new double[validBranches.Length];
        if (lastAnglesSet)
        {
            double minDiff = 100000.0;
            int minInd = -1;
            for (int j = 0; j < validBranches.Length; j++)
            {
                double totalAngleDiff = 0;
                var movedBranchValues = moveAnglesIntoRange(validBranches[j], true);
                for (int i = 0; i < 6; i++)
                {
                    var diff = Math.Abs(movedBranchValues[i] - lastAngles[i]);
                    while (diff > Math.PI)
                        diff -= Math.PI;
                    diff = moveAngleIntoRange(diff, true);
                    totalAngleDiff += diff;
                }
                if (totalAngleDiff < minDiff)
                {
                    minDiff = totalAngleDiff;
                    minInd = j;
                }
                diffs[j] = totalAngleDiff;
            }
            if (minInd > -1)
            {
                //if (minDiff > 2.0)
                //{
                //    Debug.Log("FLIPPED");
                //    return new double[] { 0 };
                //}
                bestJoints = validBranches[minInd];
                lastAngles = moveAnglesIntoRange(bestJoints, true);
            }
        }
        else
        {
            bestJoints = validBranches[0];
            if (validBranchInd.Count > branchInd)
                bestJoints = validBranches[branchInd];
            for (int i = 0; i < validBranchInd.Count; i++)
            {
                var joint1Value = moveAngleIntoRange(validBranches[i][1], true);
                if (joint1Value > -Math.PI / 2 && joint1Value < bestJoints[1])
                {
                    bestJoints = validBranches[i];
                    bestJoints[1] = joint1Value;
                }
                else if (joint1Value < -Math.PI / 2 && joint1Value > bestJoints[1])
                {
                    bestJoints = validBranches[i];
                    bestJoints[1] = joint1Value;
                }
            }
            lastAngles = moveAnglesIntoRange(bestJoints, true);
            lastAnglesSet = true;
        }
        double[] branchMultiply = new double[6] { 1, 1, 1, 1, -1, 1 };
        for (int i = 0; i < 6; i++)
        {
            bestJoints[i] *= branchMultiply[i];
            bestJoints[i] += jointOffsets[i];
        }
        return bestJoints;
    }

    // Update is called once per frame
    void Update()
    {

       if(splineReady && Time.time - startTime >= 0.15f)
       {
            if (splineInd < hitPlaces[0].transform.childCount)
            {
                var endEff = hitPlaces[0].transform.GetChild(splineInd++);
                var pos = endEff.transform.position * 1000f;
                var ori = endEff.transform.rotation;
                double[] angles = GetInverse(endEff.gameObject, null, new double[] { -Math.PI / 2, Math.PI / 2, 0, Math.PI / 2, 0, 0 }, -1);
                if (angles.Length == 6)
                {
                    float[] anglesf = new float[6];
                    for (int i = 0; i < 6; i++)
                    {
                        anglesf[i] = (float)angles[i] * Mathf.Rad2Deg;
                    }
                    controllRobot(anglesf);
                }
            }
            startTime = Time.time;
            return;
        }
       else if (splineReady)
        {
            return;
        }
        if (pause)
        {

            startTime = Time.time - realLastTime;
            realStartTime = Time.time - realLastTime;
            return;
        }

        realLastTime = Time.time - realStartTime;
        if (realLastTime >= realStepTime)
        {
            var syncInd = getSyncPoint(Time.time - startTime);
            ind = syncInd;
            startTime = Time.time;
            realStartTime = Time.time;
        }

        if (savedPath && startSavedPath)
        {
            if (obstacleAvoidance)
            {
                if (Time.time - startTime >= 0.05f)
                {
                    var futureInd = (int)(futureTime / 0.05f)+1;
                    endEffector.transform.position = avodaincePath[0];
                    moveToEffector(endEffector);
                    if (avodaincePath.Count > futureInd)
                    {
                        futureEffector.transform.position = avodaincePath[futureInd];
                        moveToEffector(futureEffector, true);
                    }
                    else
                    {
                        var timeDiff = (futureInd - avodaincePath.Count) * 0.05f-1.5f;
                        stepRobot(timeDiff, ind, true);
                    }

                    avodaincePath.RemoveAt(0);
                    if (avodaincePath.Count == 0)
                        obstacleAvoidance = false;
                    var remaining = (Time.time - startTime) - 0.05f;
                    startTime = Time.time-remaining;
                    realStartTime = startTime;
                }
                return;
            }

            lastTime = Time.time - startTime;
            if (ind < savedAngles.Count)
            {
                stepRobot(lastTime, ind, true);

                if (lastTime >= stepTime)
                {
                    startTime = Time.time;
                    controllRobot(savedAngles[ind]);
                    ind++;
                }
                else
                    stepRobot(lastTime, ind);
            }
            else
            {
                //startSavedPath = false;
                ind = 1;
                realInd = 1;
                startTime = Time.time;
                realStartTime = Time.time;
            }

        }
        else
        {
            startTime = Time.time;
            realStartTime = Time.time;
            ind = 1;
            realInd = 1;
            var pos = endEffector.transform.position * 1000f;
            var ori = endEffector.transform.rotation;
            if (lastPos != pos || lastOri != ori)
            {   // 0 +20 -45
                lastOri = ori;
                double[] angles = GetInverse(endEffector,null, new double[] { -Math.PI / 2, Math.PI / 2, 0, Math.PI / 2, 0, 0 }, -1);
                if (angles.Length == 6)
                {
                    float[] anglesf = new float[6];
                    for (int i = 0; i < 6; i++)
                    {
                        anglesf[i] = (float)angles[i] * Mathf.Rad2Deg;
                    }
                    controllRobot(anglesf);
                }
            }
        }
        //-22.35 41.41 76.87 -28.28 90 157.64
        //
        //-55.436 14.865 67.663 7.47 90 124.564
    }

    void moveToEffector(GameObject effector, bool future = false)
    {
        double[] angles = GetInverse(effector, null, new double[] { -Math.PI / 2, Math.PI / 2, 0, Math.PI / 2, 0, 0 }, -1);
        if (angles.Length == 6)
        {
            float[] anglesf = new float[6];
            for (int i = 0; i < 6; i++)
            {
                anglesf[i] = (float)angles[i] * Mathf.Rad2Deg;
            }
            if (future)
                controllFutureRobot(anglesf);
            else
                controllRobot(anglesf);
        }
    
    }

    void stepRobot(float timeDiff, int angleInd,bool future = false)
    {
        if (future)
        {
            timeDiff += futureTime;
            if (timeDiff >= stepTime)
            {
                if (angleInd + 1 < savedAngles.Count)
                {
                    timeDiff -= stepTime;
                    var t = timeDiff / stepTime;
                    float[] newAngles = new float[6];
                    for (int i = 0; i < 6; i++)
                    {
                        newAngles[i] = (1 - t) * savedAngles[angleInd][i] + t * savedAngles[angleInd + 1][i];
                    }
                    controllFutureRobot(newAngles);
                }
                else
                    controllFutureRobot(savedAngles[angleInd]);
            }
            else
            {
                var t = timeDiff / stepTime;
                float[] newAngles = new float[6];
                for (int i = 0; i < 6; i++)
                {
                    newAngles[i] = (1 - t) * savedAngles[angleInd - 1][i] + t * savedAngles[angleInd][i];
                }
                controllFutureRobot(newAngles);
            }
        }
        else
        {
            var t = timeDiff / stepTime;
            float[] newAngles = new float[6];
            for (int i = 0; i < 6; i++)
            {
                newAngles[i] = (1 - t) * savedAngles[ind - 1][i] + t * savedAngles[ind][i];
            }
            controllRobot(newAngles);
            movingPoints.Add(futureRay.position);
            if (movingPoints.Count > 5)
            {
                movingPoints.RemoveAt(0);
            }
        }
    }

    private int getSyncPoint(float timeDiff)
    {
        if(realInd < savedAngles.Count)
        {
            realInd++;
            return realInd;
        }
        return -1;
    }

    private void controllRobot(float[] rotations)
    {
        for (int i = 0; i < numOfJoint; i++)
        {
            //joints[i].Rotate(new Vector3(rotations[i * 3], rotations[i * 3 + 1], rotations[i * 3 + 2]), Space.Self);
            joints[i].localRotation = Quaternion.Euler(rotations[i] * rotAxis[i][0], rotations[i] * rotAxis[i][1], rotations[i] * rotAxis[i][2]);
        }
        if(!savedPath && (joints[5].position - endEffector.transform.position).magnitude > 0.1+ tcpdist/1000)
        {
            rotations[3] += 180;
            joints[5].localRotation = Quaternion.Euler(rotations[5] * rotAxis[5][0], rotations[5] * rotAxis[5][1], rotations[5] * rotAxis[5][2]);
        }
    }

    public void startSaved()
    {
        if (startSavedPath)
        {
            startSavedPath = false;
        }
        else
        {
            ind = 1;
            startTime = Time.time;
            pause = false;
            startSavedPath = true;
        }
    }
    public void pauseSimu()
    {
        pause = !pause;
    }

    private void controllFutureRobot(float[] rotations)
    {
        for (int i = 0; i < numOfJoint; i++)
        {
            //joints[i].Rotate(new Vector3(rotations[i * 3], rotations[i * 3 + 1], rotations[i * 3 + 2]), Space.Self);
            futureJoints[i].localRotation = Quaternion.Euler(rotations[i] * rotAxis[i][0], rotations[i] * rotAxis[i][1], rotations[i] * rotAxis[i][2]);
        }
    }


    private double moveAngleIntoRange(double angle, bool rad = false)
    {
        if (rad)
        {
            if (angle > Math.PI)
                angle -= 2 * Math.PI;
            if (angle < -Math.PI)
                angle += 2 * Math.PI;
        }
        else
        {
            if (angle > 180)
                angle -= 360;
            if (angle < -180)
                angle += 360;
        }
        return angle;
    }

    private double[] moveAnglesIntoRange(double[] angles, bool rad = false)
    {
        double[] ret = new double[6];
        for (int i = 0; i < 6; i++)
        {
            ret[i] = angles[i];
            if (rad)
            {
                if (angles[i] > Math.PI)
                    ret[i] = angles[i]- 2 * Math.PI;
                if (angles[i] < -Math.PI)
                    ret[i] = angles[i] + 2 * Math.PI;
            }
            else
            {
                if (angles[i] > 180)
                    ret[i] = angles[i]-360;
                if (angles[i] < -180)
                    ret[i] = angles[i] + 360;
            }
        }
        return ret;
    }

    private void initAxis(bool defValues = true)
    {
        if (defValues)
        {
            int i = 0;
            rotAxis[i++] = new float[3] { 0.0f, 1.0f, 0.0f };
            rotAxis[i++] = new float[3] { 0.0f, 0.0f, 1.0f };
            rotAxis[i++] = new float[3] { 0.0f, 0.0f, 1.0f };
            rotAxis[i++] = new float[3] { 0.0f, 0.0f, 1.0f };
            rotAxis[i++] = new float[3] { 0.0f, -1.0f, 0.0f };
            rotAxis[i++] = new float[3] { 0.0f, 0.0f, 1.0f };
        }
        else
        {
            for (int i = 0; i < numOfJoint; i++)
            {
                switch (axisList[i])
                {
                    case 'x':
                        rotAxis[i] = new float[3] { 1.0f, 0.0f, 0.0f };
                        break;
                    case 'y':
                        rotAxis[i] = new float[3] { 0.0f, 1.0f, 0.0f };
                        break;
                    case 'z':
                        rotAxis[i] = new float[3] { 0.0f, 0.0f, 1.0f };
                        break;
                    default:
                        rotAxis[i] = new float[3] { 0.0f, 0.0f, 0.0f };
                        break;
                }
            }
        }
    }
    public double[] getAngles()
    {
        double[] ret = new double[6];
        double[] jointOffsets = new double[6] { -Math.PI / 2, Math.PI / 2, 0, Math.PI / 2, 0, 0 };
        double[] branchMultiply = new double[6] { 1, 1, 1, 1, -1, 1 };
        for (int i = 0; i < 6; i++)
        {
            ret[i] = lastAngles[i];
            ret[i] *= branchMultiply[i];
            ret[i] += jointOffsets[i];
            ret[i] *= Mathf.Rad2Deg;
        }
        return ret;
    }

    public void CollisionDetected(GameObject obj, GameObject collider, bool future)
    {
        if (pause)
            return;
        if (future)
        {
            pause = true;
            Debug.Log(collider.name + " collided with futureArm/"+obj.name);
            var syncInd = getNextAvaiableSyncPoint();
            var collisionData = getCollisionData(collider.transform.position, syncInd);
            if (collisionData == null)
                return;
            var hitAbove = collisionData[0][0] + collisionData[1][0] + collisionData[2][0];
            var hitBelow = collisionData[3][0] + collisionData[4][0] + collisionData[5][0];
            if(Mathf.Abs(hitAbove)+ Mathf.Abs(hitBelow) == 0)
            {
                obstacleAvoidance = true;
                var dir = (syncPositions[syncInd] - rayStartPos.position).normalized;
                var timeToComplete = stepTime - lastTime + stepTime;
                var distance = (syncPositions[syncInd] - rayStartPos.position).magnitude;
                var steps = (int)(timeToComplete*20);
                var stepLenght = distance / steps;
                for (int i = 1; i < steps; i++)
                {
                    avodaincePath.Add(rayStartPos.position + i*stepLenght * dir);
                }
                pause = false;
                ind = syncInd+1;
                startTime = Time.time;
                realInd = syncInd+1;
                return;
            }
            else
            {
                for (int i = 0; i < 3; i++)
                {
                    int j = 0;
                    while (collisionData[i * 3+j][0] == 0)
                    {
                        j++;
                    }
                    for (int k = 0; k < 3; k++)
                    {
                        collisionData[i * 3 + k][0]= collisionData[i * 3 + j][0];
                    }
                }
              
                var spData = createSplineData(collisionData);
                float[] x = new float[spData[0].Length];
                for (int i = 0; i < x.Length; i++)
                {
                    x[i] = i * 0.015f;
                }
                var sp = calcSpline(0, 0.015f * spData[0].Length, x, spData[0], 0.015f, spData[0].Length - 1);
                Vector3 moveDir, rayDir, offset, startPos;
                float stepSize = x[x.Length - 1] / 50;
                moveDir = syncPositions[syncInd] - rayStartPos.position;
                moveDir.y = 0;
                rayDir = Vector3.down;
                offset = new Vector3(0, 0.5f, 0);
                startPos = rayStartPos.position + offset;
                moveDir = moveDir.normalized;
                for (int i = 0; i < 50; i++)
                {
                    var hitPlace = Instantiate(Resources.Load("HitPlace", typeof(GameObject))) as GameObject;
                    hitPlace.name = "Spline_" + i + "_" + hitPlaces[0].transform.childCount;
                    hitPlace.transform.SetParent(hitPlaces[0].transform);
                    var move = (i + 1) * moveDir * stepSize;
                    int ind = (int)((i * stepSize+0.001) / 0.015f);
                    float xi = i * stepSize;
                    float value = 0;
                    for (int j = 0; j < 4; j++)
                    {
                        var currX = Mathf.Pow(xi-sp[4][ind], j);
                        value += sp[j][ind] * currX;
                    }
                    hitPlace.transform.position = startPos + move + rayDir * value;
                    hitPlace.transform.rotation = Quaternion.Euler(0, -180f, 180f);
                }
                splineReady = true;
                //for (int i = 0; i < 1; i++)
                //{

                //    switch (i)
                //    {
                //        case 0:
                //            {
                //                moveDir = syncPositions[syncInd] - rayStartPos.position;
                //                moveDir.y = 0;
                //                rayDir = Vector3.down;
                //                offset = new Vector3(0, 0.5f, 0);
                //                startPos = rayStartPos.position + offset;

                //                var hitPlace = Instantiate(Resources.Load("HitPlace", typeof(GameObject))) as GameObject;
                //                hitPlace.name = "Endplace_" + i + "_" + hitPlaces[i].transform.childCount;
                //                hitPlace.transform.SetParent(hitPlaces[i].transform);
                //                hitPlace.transform.position = startPos + moveDir + rayDir * collisionData[3 * i + 1][collisionData[3 * i + 1].Length - 1];
                //                break;
                //            }
                //        case 1:
                //            {
                //                moveDir = syncPositions[syncInd] - rayStartPos.position;
                //                moveDir.y = 0;
                //                rayDir = Vector3.up;
                //                offset = new Vector3(0, -0.5f, 0);
                //                startPos = rayStartPos.position + offset;

                //                var hitPlace = Instantiate(Resources.Load("HitPlace", typeof(GameObject))) as GameObject;
                //                hitPlace.name = "EndPlace_" + i + "_" + hitPlaces[i].transform.childCount;
                //                hitPlace.transform.SetParent(hitPlaces[i].transform);
                //                hitPlace.transform.position = startPos + moveDir + rayDir * collisionData[3 * i + 1][collisionData[3 * i + 1].Length - 1];
                //                break;
                //            }
                //        default:
                //            {
                //                moveDir = outMoveDir;
                //                rayDir = outRayDir;
                //                startPos = outRayStart;
                //                var hitPlace = Instantiate(Resources.Load("HitPlace", typeof(GameObject))) as GameObject;
                //                hitPlace.name = "EndPlace_" + i + "_" + hitPlaces[i].transform.childCount;
                //                hitPlace.transform.SetParent(hitPlaces[i].transform);
                //                hitPlace.transform.position = startPos + moveDir + rayDir * collisionData[3 * i + 1][collisionData[3 * i + 2].Length - 1];
                //                break;
                //            }

                //    }


                //    moveDir = moveDir.normalized;
                //    for (int j = 0; j < spData[i].Length - 1; j++)
                //    {
                //        var hitPlace = Instantiate(Resources.Load("HitPlace", typeof(GameObject))) as GameObject;
                //        hitPlace.name = "MissPlace_" + i + "_" + hitPlaces[i].transform.childCount;
                //        hitPlace.transform.SetParent(hitPlaces[i].transform);
                //        hitPlace.transform.position = startPos + (j + 1) * moveDir * 0.015f + rayDir * spData[i][j];
                //    }
                //}
            }

        }
        else
        {

            Debug.Log(collider.name + " collided with eSeries_UR3e/"+obj.name);
            //FORCE STOP
        }
    }

    private float[][] getCollisionData(Vector3 collisionPos, int syncInd)
    {
        if(syncInd == -1)
        {
            return null;
        }
        float[][] collisionData = new float[9][];
        for (int i = 0; i < 2; i++)
        {
            collisionData[i*3] = castRays(i, collisionPos, new Vector3(0,0,0), syncInd);
            collisionData[i*3+1] = castRays(i, collisionPos, new Vector3(0, 0, -0.037f), syncInd);
            collisionData[i*3+2] = castRays(i, collisionPos, new Vector3(0, 0, 0.037f), syncInd);
        }
        collisionData[6] = castRays(2, collisionPos, new Vector3(0, 0, 0), syncInd, true);
        collisionData[7] = castRays(2, collisionPos, new Vector3(0, 0.04f, -0.037f), syncInd);
        collisionData[8] = castRays(2, collisionPos, new Vector3(0, -0.04f, 0.037f), syncInd);

        return collisionData;
    }
    
    private float[] castRays(int dir, Vector3 collisionPos, Vector3 inputOffset, int syncInd, bool setOutput=false)
    {
        Vector3 origRayPos = rayStartPos.position;
        var endPos = syncPositions[syncInd];
        var endOffset = rayStartPos.TransformPoint(inputOffset + rayStartPos.localPosition)- rayStartPos.position;
        endOffset.y = 0;
        endPos += endOffset;
        rayStartPos.localPosition += inputOffset;
        float[] rayData = new float[32];
        RaycastHit hit;
        int layerMask = 1 << 2;
        layerMask = ~layerMask;
        Vector3 moveDir, rayDir, offset;
        switch (dir)
        {
            case 0:
                moveDir = endPos - rayStartPos.position;
                rayDir = Vector3.down;// rayStartPos.TransformDirection(Vector3.right);
                offset = new Vector3(0, 0.5f, 0);
                rayStartPos.position += offset;
                rayData[rayData.Length - 1] = 0.5f - moveDir.y;
                break;
            case 1:
                moveDir = endPos - rayStartPos.position;
                rayDir = Vector3.up;// rayStartPos.TransformDirection(Vector3.left);
                offset = new Vector3(0, -0.5f, 0);
                rayStartPos.position += offset;
                rayData[rayData.Length - 1] = 0.5f + moveDir.y;
                break;
            case 2:
                
                var newPos = joints[0].position;
                newPos.y = collisionPos.y+inputOffset.y;
                var oldPos = rayStartPos.position;
                rayStartPos.position = newPos;
                var collisionOffset = collisionPos;
                collisionOffset.y += inputOffset.y;
                rayDir = (collisionOffset - newPos).normalized;
                if ((origRayPos.x >= 0 && (futureRay.position.z - origRayPos.z) > 0) || (origRayPos.x < 0 && (futureRay.position.z - origRayPos.z) <= 0))
                {
                    moveDir = Quaternion.Euler(0, -90f, 0) * rayDir;
                }
                else
                {
                    moveDir = Quaternion.Euler(0, 90f, 0) * rayDir;
                }

                var distance = Mathf.Abs(-moveDir.z * oldPos.x + moveDir.x * oldPos.z + (newPos.z - moveDir.z * (newPos.x / moveDir.x))) / moveDir.magnitude;
                rayData[rayData.Length - 1] = Mathf.Abs(-moveDir.z * endPos.x + 
                                                        moveDir.x * endPos.z + 
                                                        (newPos.z - moveDir.z * (newPos.x / moveDir.x))) 
                                                        / moveDir.magnitude;
                offset = new Vector3(1, 0, 0).normalized;
                offset = offset * distance;
                rayStartPos.position -= moveDir * 0.225f;
                break;
            default:
                moveDir = new Vector3(1, 0, 0);
                rayDir = Vector3.forward;
                offset = new Vector3();
                break;
        }
        moveDir.y = 0;
        moveDir = moveDir.normalized;

        var baseRayPos = rayStartPos.position;// + moveDir * 0.05f;
        if (setOutput)
        {
            outMoveDir = moveDir;
            outRayDir = rayDir;
            outRayStart = baseRayPos;
            outRayStart.y = endPos.y;
        }
        for (int i = 1; i < rayData.Length-1; i++)
        {
            var rayPos = baseRayPos + i * moveDir * 0.015f;
            if (Physics.Raycast(rayPos, rayDir, out hit, 1.5f, layerMask))
            {
                Debug.DrawRay(rayPos, rayDir * hit.distance, Color.yellow);
                rayData[0] = offset.magnitude;
                if (hit.distance < offset.magnitude)
                    rayData[0] = -rayData[0];
                rayData[i] = hit.distance - 0.05f;
                if (dir == 1)
                    rayData[i] -= 0.28f;
                //var hitPlace = Instantiate(Resources.Load("HitPlace", typeof(GameObject))) as GameObject;
                //hitPlace.name = "HitPlace_" + i + "_" + hitPlaces[dir].transform.childCount;
                //hitPlace.transform.SetParent(hitPlaces[dir].transform);
                //hitPlace.transform.position = hit.point - rayDir * 0.33f;
                //Debug.Log("Did Hit");
            }
            else
            {
                var distToEndEffector = (offset).magnitude;
                rayData[i] = distToEndEffector;
            }
        }
        rayStartPos.position = origRayPos;
        return rayData;
    }

    private int getNextAvaiableSyncPoint()
    {
        var currInd = ind;
        var timeDiff = Time.time - startTime;
        var futureDiff = timeDiff + futureTime;
        if (futureDiff >= stepTime && currInd + 1 < savedAngles.Count && !crucialPoints[currInd])
        {
            currInd = currInd + 1;
        }
        if (!Physics.CheckSphere(syncPositions[currInd], 0.1f))
        {
            return currInd;
        }
        else if (!crucialPoints[currInd])
        {
            return currInd + 1;
        }
        return -1;
    }

    private float[][] createSplineData(float[][] collisionData)
    {

        float[][] splineData = new float[3][];
        int[] firstNonZero = new int[] { -1, -1, -1 };
        int[] lastNonZero = new int[] { -1, -1, -1 };
       
        for (int i = 0; i < 3; i++)
        {
            splineData[i] = new float[collisionData[i * 3].Length-1];
            for (int j = 1; j < collisionData[i * 3].Length-1; j++)
            {
                splineData[i][j - 1] = Mathf.Min(new float[] { collisionData[i * 3][j], collisionData[i * 3 + 1][j], collisionData[i * 3 + 2][j] });
                if (firstNonZero[i] == -1 && splineData[i][j-1] != Mathf.Abs(collisionData[i*3][0]))
                {
                    firstNonZero[i] = j-1;
                }
            }
        }
        for (int i = 0; i < 3; i++)
        {
            for (int j = splineData[i].Length-2; j>0; j--)
            {
                if (lastNonZero[i] == -1 && splineData[i][j] != Mathf.Abs(collisionData[i*3][0]))
                {
                    lastNonZero[i] = j;
                    break;
                }

            }
        }

        float[] endDists = new float[3];
        endDists[0] = collisionData[0][collisionData[1].Length - 1];
        endDists[1] = collisionData[3][collisionData[4].Length - 1];
        endDists[2] = collisionData[6][collisionData[7].Length - 1];

        for (int i = 0; i < 3; i++)
        {
            var diff = (splineData[i][firstNonZero[i]] - splineData[i][0])/ (firstNonZero[i]);
            for (int j = 0; j < firstNonZero[i]; j++)
            {
                splineData[i][j] += diff * j;
            }

            var diff2 = (endDists[i] - splineData[i][lastNonZero[i]]) / (splineData[i].Length-lastNonZero[i]-2);
            for (int j = lastNonZero[i]+1; j < splineData[i].Length-1; j++)
            {
                splineData[i][j] = splineData[i][j-1] + diff2;
            }
            splineData[i][splineData[i].Length - 1] = splineData[i][splineData[i].Length - 2];
        }

        return splineData;
    }

    private float[][] calcSpline(float a, float b,float[] x, float[] y, float h, int pointNum)
    {

        int[] bi = new int[pointNum];
        bi[0] = 0; bi[pointNum - 1] = 0;
        for (int i = 1; i < pointNum - 2; i++)
        {
            bi[i] = 1;
        }
        double[] Ux = new double[pointNum];
        Ux[0] = 0;
        for (int i = 1; i < 15 && i < pointNum; i++)
        {
            var Ai = 3 * (y[i - 1] - 2 * y[i] + y[i + 1])/(h*h);
            Ux[i] = Ai - spineL[i - 1] * Ux[i - 1];
        }
        for (int i = 15; i < pointNum; i++)
        {
            var Ai = 3 * (y[i - 1] - 2 * y[i] + y[i + 1]) / (h * h);
            Ux[i] = Ai - spineL[14] * Ux[i - 1];
        }
        Ux[pointNum-1] = -spineL[14] * Ux[pointNum - 2];

        double[][] Sp = new double[5][];
        for (int i = 0; i < 5; i++)
        {
            Sp[i] = new double[pointNum];
        }
        double[] C = new double[pointNum + 1];
        C[pointNum] = 0;
        for (int i = pointNum-1; i >=15 ; i--)
        {
            C[i] = (Ux[i] - C[i + 1] * bi[i]) / splinU[14];
            Sp[3][i] = (C[i + 1] - C[i]) / (3 * h);
            Sp[1][i] = (y[i+1]-y[i])/h-h*(C[i+1]+2*C[i])/ 3;
        }
        int ii = 14;
        if (pointNum - 1 < 14)
            ii = pointNum - 1;
        for (int i =ii; i >= 0; i--)
        {
            C[i] = (Ux[i] - C[i + 1] * bi[i]) / splinU[i];
            Sp[3][i] = (C[i + 1] - C[i]) / (3 * h);
            Sp[1][i] = (y[i + 1] - y[i]) / h - h * (C[i + 1] + 2 * C[i]) / 3;
        }
        for (int i = 0; i < pointNum; i++)
        {
            Sp[0][i] = y[i];
            Sp[2][i] = C[i];
            Sp[4][i] = x[i];

        }
        float[][] ret = new float[5][];
        for (int i = 0; i < 5; i++)
        {
            ret[i] = Array.ConvertAll(Sp[i], Convert.ToSingle);
        }
        return ret;
    }

}
