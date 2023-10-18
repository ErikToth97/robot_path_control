using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using InverseMap;
using System;

public class InverseMapControll : MonoBehaviour
{
    // Start is called before the first frame update
    UR5 _inverse;
    private List<Transform> joints;
    public int branchInd = 0;
    public double tcpdist = 100;
    public GameObject endEffector, robotArm;
    private Vector3 lastPos;
    private Quaternion lastOri;

    private static float[] testangles;
    private static List<float[]> rotAxis;
    public bool defaultValues = true;
    public int numOfJoint = 6;
    public char[] axisList = new char[6] {'y', 'z', 'z', 'z', 'y', 'z', };
    void Start()
    {
        joints = new List<Transform>();
        joints.Add(this.transform.Find("Cube/Cube.001"));
        rotAxis = new List<float[]>();
        rotAxis.Add(new float[3]);
        for (int j = 0; j < numOfJoint-1; j++)
        {
            joints.Add(joints[j].GetChild(0));
            rotAxis.Add(new float[3]);
        }
        //initAxis();
        initAxis(defaultValues);

        _inverse = new UR5();
        lastPos = new Vector3();
        lastOri = new Quaternion();
    }

    public double[] GetInverse(double[] pose = null, double[] jointOffsets = null, int gripperId = -1)
    {
        var pos = endEffector.transform.position * 1000f;
        lastPos = pos;
        double[] full = new double[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
        var ori = endEffector.transform.rotation.eulerAngles;
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
            if (inverseBranches[b][1] > -100*Mathf.Deg2Rad && inverseBranches[b][1] < 100* Mathf.Deg2Rad)
                validBranchInd.Add(b);
        }
        double[][] validBranches = new double[validBranchInd.Count][];
        for (int i = 0; i < validBranchInd.Count; i++)
        {
            validBranches[i] = inverseBranches[validBranchInd[i]];
        }
        if (validBranches.Length == 0)
            return new double[] { 0 };
        bestJoints = validBranches[0];
        if (validBranchInd.Count > branchInd)
            bestJoints = validBranches[branchInd];
        //for (int b = 0; b < inverseBranches.Length; ++b)
        //{
        //    double jointRotSum = 0, calculated, alternative;
        //    double[] branchBest = new double[6];
        //    for (int j = 0; j < inverseBranches[b].Length; ++j)
        //    {
        //        // Check +-360 rotations, in valid direction
        //        calculated = inverseBranches[b][j];
        //        branchBest[j] = calculated;
        //        alternative = calculated < 0 ? calculated + (2 * Math.PI) : calculated - (2 * Math.PI);
        //        if (Math.Abs(actualJoints[j] - calculated) > Math.Abs(actualJoints[j] - alternative))
        //            branchBest[j] = alternative;
        //        // Smallest distance to actual joints
        //        jointRotSum += Math.Abs(actualJoints[j] - branchBest[j]);
        //    }
        //    if (jointRotSum < bestJointRotSum)
        //    {
        //        bestJointRotSum = jointRotSum;
        //        branchBest.CopyTo(bestJoints, 0);
        //    }

        //}

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
        var pos = endEffector.transform.position * 1000f;
        var ori = endEffector.transform.rotation;
        if (lastPos != pos || lastOri != ori)
        {   // 0 +20 -45
            lastOri = ori;
            double[] angles = GetInverse(null, new double[] { -Math.PI/2, Math.PI / 2, 0, Math.PI/2, 0, 0 }, -1);
            if(angles.Length == 6)
            {
                float[] anglesf = new float[6];
                for (int i = 0; i < 6; i++)
                {
                    anglesf[i] = (float)angles[i]*Mathf.Rad2Deg;
                }
                controllRobot(anglesf);
            }
        }
    }

    private void controllRobot(float[] rotations)
    {
        for (int i = 0; i < numOfJoint; i++)
        {
            //joints[i].Rotate(new Vector3(rotations[i * 3], rotations[i * 3 + 1], rotations[i * 3 + 2]), Space.Self);
            joints[i].localRotation = Quaternion.Euler(rotations[i] * rotAxis[i][0], rotations[i] * rotAxis[i][1], rotations[i] * rotAxis[i][2]);
        }
        if((joints[5].position - endEffector.transform.position).magnitude > 0.1+ tcpdist/1000)
        {
            rotations[3] += 180;
            joints[5].localRotation = Quaternion.Euler(rotations[5] * rotAxis[5][0], rotations[5] * rotAxis[5][1], rotations[5] * rotAxis[5][2]);
        }
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
}
