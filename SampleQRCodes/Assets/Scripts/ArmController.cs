using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArmController : MonoBehaviour
{
    public GameObject leftArm, rightArm, leftHand, rightHand;
    private Transform leftForeArm, rightForeArm, head;
    private float[] baseLenght = new float[2] { 0.9f, 0.75f };
    public float[] lengths;
    public bool handsSet = false;
    // Start is called before the first frame update
    void Start()
    {
        lengths = new float[2] { 0.9f, 0.75f };
        leftForeArm = leftArm.transform.Find("LeftForeArm");
        rightForeArm = rightArm.transform.Find("RightForeArm");
        head = leftForeArm.root;
        //handsSet = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (handsSet)
        {
            //var diff = invTM(head,leftHand.transform) - invTM(head, leftArm.transform.parent);
            //var atan = Mathf.Atan2(diff.x, diff.z) * Mathf.Rad2Deg;
            //var lHandRot = Quaternion.Inverse(head.rotation) * leftHand.transform.rotation;
            //Quaternion _lookRotation = Quaternion.LookRotation((leftHand.transform.transform.position - leftArm.transform.parent.position).normalized);
            //leftArm.transform.parent.rotation = _lookRotation;
            //var tm = leftHand.transform.transform;
            ////-lHandRot.eulerAngles.y
            //var relArmPos = invTM(leftArm.transform.parent, leftArm.transform);
            //var relHandPos = invTM(leftArm.transform.parent, leftHand.transform);
            //var hadREL = relHandPos - relArmPos;
            //Debug.Log(hadREL);
            //var leftEndAngles = CalcAngles(relArmPos,relHandPos);
            //SetAngle(true, leftEndAngles);
            //leftArm.transform.parent.rotation = _lookRotation * Quaternion.Euler(0, 0, -lHandRot.eulerAngles.y);
            setJointAngles(leftHand.transform, leftArm.transform, leftForeArm);

            // var rot = leftArm.transform.rotation.eulerAngles;
            //leftArm.transform.rotation = Quaternion.Euler(rot.x, rot.y, );

            //var rightEndAngles = CalcAngles(rightArm.transform.position, rightHand.transform.position);
            //SetAngle(false, rightEndAngles);
            //diff = rightHand.transform.position - rightArm.transform.parent.position;
            //atan = Mathf.Atan2(diff.x, diff.z);
            //var rHandRot = Quaternion.Inverse(head.rotation) * rightHand.transform.rotation;
            //rightArm.transform.parent.rotation = Quaternion.Euler(0, atan * Mathf.Rad2Deg, -rHandRot.eulerAngles.y);
            setJointAngles(rightHand.transform, rightArm.transform, rightForeArm);
        }
    }

    void setJointAngles(Transform hand, Transform arm, Transform foreArm)
    {
        var diff = invTM(head, hand) - invTM(head, arm.parent);
        var atan = Mathf.Atan2(diff.x, diff.z) * Mathf.Rad2Deg;
        var lHandRot = (Quaternion.Inverse(head.rotation) * hand.rotation).eulerAngles;
        Quaternion _lookRotation = Quaternion.LookRotation((hand.position - arm.parent.position).normalized);
        arm.parent.rotation = _lookRotation;

        //-lHandRot.eulerAngles.y
        var relArmPos = invTM(arm.parent, arm);
        var relHandPos = invTM(arm.parent, hand);
        var hadREL = relHandPos - relArmPos;
        var angles = CalcAngles(relArmPos, relHandPos);
        arm.localRotation = Quaternion.Euler(angles[0] + 90f, 0, 0);
        foreArm.localRotation = Quaternion.Euler(angles[1], 0, 0);
        arm.parent.rotation = _lookRotation * Quaternion.Euler(0, 0, -lHandRot.y);
    }

    float[] CalcAngles(Vector3 armPos, Vector3 handPos, bool inverse = false)
    {
        float dist = Vector3.Distance(armPos, handPos);
        Vector3 diff = handPos-armPos;
        float atan = Mathf.Atan2(diff.z, diff.x);
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

        }
        var orig1 = joint0Angle;
        var orig2 = joint1Angle;

        joint0Angle = normAngle(-joint0Angle);
        joint1Angle = normAngle(-joint1Angle);
        if (joint0Angle < 0f || joint1Angle < 0f)
        {
            Debug.Log("WTF---------------------------------------------");
        }
        return new float[2] { joint0Angle, joint1Angle };
    }

    Vector3 invTM(Transform tm, Transform obj)
    {
        return tm.InverseTransformPoint(obj.position) * head.transform.localScale.y;
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
            //Vector3 Euler = leftArm.transform.localEulerAngles;
            //Euler.x = angle[0];
            //leftArm.transform.localRotation = Quaternion.Euler(Euler);

            //Euler = leftForeArm.localEulerAngles;
            //Euler.x = angle[1];
            //leftForeArm.transform.localRotation = Quaternion.Euler(Euler);
            var rot = leftArm.transform.rotation.eulerAngles;
            //leftArm.transform.rotation = Quaternion.Inverse(leftArm.transform.parent.rotation)*Quaternion.Euler(angle[0]-90f, 0,0);
            leftArm.transform.localRotation = Quaternion.Euler(angle[0]+90f, 0, 0);
            leftForeArm.transform.localRotation = Quaternion.Euler(angle[1], 0, 0);
        }
        else
        {
            rightArm.transform.localRotation = Quaternion.Euler(angle[0],0,0);
            rightForeArm.transform.localRotation = Quaternion.Euler(angle[1], 0, 0);
        }

    }

    public void setHands(GameObject rHand, GameObject lHand, float armLenght)
    {
        rightHand = rHand;
        leftHand = lHand;
        handsSet = true;
        if (armLenght < 0f)
            armLenght = 1.65f;
        var total = baseLenght[0] + baseLenght[1];
        var relLenght = baseLenght[0] / total * armLenght;
        var relLenghtForeArm = baseLenght[1] / total * armLenght;
        var leftArmMesh = leftArm.transform.GetChild(0);
        var rightArmMesh = rightArm.transform.GetChild(0);
        leftArmMesh.transform.localScale = new Vector3(0.02f, relLenght/20.0f, 0.02f);
        leftArmMesh.transform.localPosition = new Vector3(0, 0, relLenght / 20.0f);
        rightArmMesh.transform.localScale = new Vector3(0.02f, relLenght / 20.0f, 0.02f);
        rightArmMesh.transform.localPosition = new Vector3(0, 0, relLenght / 20.0f);

        var lefElbow = leftArm.transform.GetChild(1);
        var rightElbow = rightArm.transform.GetChild(1);
        leftForeArm.transform.localPosition = new Vector3(0, 0, relLenght / 10);
        rightForeArm.transform.localPosition = new Vector3(0, 0, relLenght / 10);
        lefElbow.transform.localPosition = new Vector3(0, 0, relLenght / 10);
        rightElbow.transform.localPosition = new Vector3(0, 0, relLenght / 10);
        var leftForeArmMesh = leftForeArm.transform.GetChild(0);
        var rightForeArmMesh = rightForeArm.transform.GetChild(0);
        leftForeArmMesh.transform.localScale = new Vector3(0.02f, relLenghtForeArm / 20.0f, 0.02f);
        rightForeArmMesh.transform.localScale = new Vector3(0.02f, relLenghtForeArm / 20.0f, 0.02f);
        leftForeArmMesh.transform.localPosition = new Vector3(0, 0,  relLenghtForeArm / 20.0f);
        rightForeArmMesh.transform.localPosition = new Vector3(0, 0, relLenghtForeArm / 20.0f);
        lengths[0] = relLenght;
        lengths[1] = relLenghtForeArm;
    }
}
