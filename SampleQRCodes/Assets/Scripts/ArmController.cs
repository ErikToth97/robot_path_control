using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArmController : MonoBehaviour
{
    public GameObject leftArm, rightArm, leftHand, rightHand;
    private Transform leftForeArm, rightForeArm;
    public float[] lengths;
    // Start is called before the first frame update
    void Start()
    {
        lengths = new float[2] { 0.9f, 0.75f };
        leftForeArm = leftArm.transform.Find("LeftForeArm");
        rightForeArm = rightArm.transform.Find("RightForeArm");
    }

    // Update is called once per frame
    void Update()
    {
        var leftEndAngles = CalcAngles(leftArm.transform, leftHand.transform.position);
        SetAngle(true, leftEndAngles);

        var rightEndAngles = CalcAngles(rightArm.transform, rightHand.transform.position);
        SetAngle(false, rightEndAngles);
        var diff = rightHand.transform.position - rightArm.transform.parent.position;
        var atan = Mathf.Atan2(diff.x, diff.z);
        rightArm.transform.parent.rotation = Quaternion.Euler(0, atan*Mathf.Rad2Deg, -rightHand.transform.rotation.eulerAngles.y); 
    }

    float[] CalcAngles(Transform joint1, Vector3 pos, bool inverse = true)
    {
        float dist = Vector3.Distance(joint1.position, pos);
        Vector3 diff = pos - joint1.position;
        float atan = Mathf.Atan2(-diff.y, diff.z);
        float joint0Angle = 0;
        float joint1Angle = 0;
        if (dist > lengths[0]+lengths[1])
        {
            joint0Angle = atan * Mathf.Rad2Deg;
        }
        else if (dist < lengths[0] + lengths[1])
        {

            float cosq2 = (dist * dist - lengths[0] * lengths[0] - lengths[1] * lengths[1]) / (2 * lengths[0] * lengths[1]);
            float q2 = Mathf.Acos(cosq2);
            float tanq1 = lengths[1] * Mathf.Sin(q2) / (lengths[0] + lengths[1] * Mathf.Cos(q2));
            float q1 = atan - Mathf.Atan(tanq1);
            if (inverse)
            {
                q1 = atan + Mathf.Atan(tanq1);
                q2 = -q2;
            }
            joint0Angle = q1 * Mathf.Rad2Deg;
            joint1Angle = q2 * Mathf.Rad2Deg;

            //float cosAlpha = (dist * dist + lengths[0] * lengths[0] - lengths[1] * lengths[1]) / (2 * dist * lengths[0]);
            //float alpha = Mathf.Acos(cosAlpha) * Mathf.Rad2Deg;

            //float cosBeta = (lengths[0] * lengths[0] + lengths[1] * lengths[1] - dist * dist) / (2 * lengths[1] * lengths[0]);
            //float beta = Mathf.Acos(cosBeta) * Mathf.Rad2Deg;
            //joint0Angle = (atan - alpha)-180f;
            //joint1Angle = 180f - beta;
        }
        var orig1 = joint0Angle;
        var orig2 = joint1Angle;

        joint0Angle = normAngle(joint0Angle);
        joint1Angle = normAngle(joint1Angle);
        if (joint0Angle < 0f || joint1Angle < 0f)
        {
            Debug.Log("WTF");
        }
        return new float[2] { joint0Angle, joint1Angle };
    }

    float normAngle(float angle)
    {
        if (angle < 0f)
            angle += 360f;
        if (angle > 360f)
            angle %= 360f;
        return angle;
    }

    void SetAngle(bool left, float[] angle)
    {
        if (left)
        {
            Vector3 Euler = leftArm.transform.localEulerAngles;
            Euler.x = angle[0];
            leftArm.transform.localRotation = Quaternion.Euler(Euler);

            Euler = leftForeArm.localEulerAngles;
            Euler.x = angle[1];
            leftForeArm.transform.localRotation = Quaternion.Euler(Euler);
        }
        else
        {
            rightArm.transform.localRotation = Quaternion.Euler(angle[0],0,0);
            rightForeArm.transform.localRotation = Quaternion.Euler(angle[1], 0, 0);
        }

    }
}
