using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Robot_controll : MonoBehaviour
{
    // Start is called before the first frame update
    private static List<Transform> joints;
    private static float[] testangles;
    private static List<float[]> rotAxis;
    public bool defaultValues = true;
    public int numOfJoint = 7;
    public char[] axisList = new char[7] { '0', 'y', 'z', 'z', 'z', 'y', 'z', };

    void Start()
    {
        joints = new List<Transform>();
        joints.Add(this.transform.Find("Cube")); 
        var jointRenderer = joints[0].gameObject.GetComponent<Renderer>();
        var newColor = new Color(0, 1.0f, 0);
        jointRenderer.material.SetColor("_Color", newColor);
        rotAxis = new List<float[]>();
        for (int j = 0; j < numOfJoint; j++)
        {
            joints.Add(joints[j].GetChild(0));
            rotAxis.Add(new float[3]);
        }
        //initAxis();
        initAxis(defaultValues);
        testangles = new float[21];
        for (int j = 0; j < numOfJoint; j++)
        {
            for (int k = 0; k < 3; k++)
            {
                testangles[j * 3+k] = rotAxis[j][k]*0.01f;
            }
        }
    }

    private void initAxis(bool defValues = true)
    {
        if (defValues)
        {
            int i = 0;
            rotAxis[i++] = new float[3] { 0.0f, 0.0f, 0.0f };
            rotAxis[i++] = new float[3] { 0.0f, 1.0f, 0.0f };
            rotAxis[i++] = new float[3] { 0.0f, 0.0f, 1.0f };
            rotAxis[i++] = new float[3] { 0.0f, 0.0f, 1.0f };
            rotAxis[i++] = new float[3] { 0.0f, 0.0f, 1.0f };
            rotAxis[i++] = new float[3] { 0.0f, 1.0f, 0.0f };
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

    // Update is called once per frame
    void Update()
    {
        controllRobot(testangles);
    }

    private void controllRobot(float[] rotations)
    {
        for(int i = 0; i<7; i++)
        {
            joints[i].Rotate(new Vector3(rotations[i * 3], rotations[i * 3 + 1], rotations[i * 3 + 2]), Space.Self);
        }
    }
}
