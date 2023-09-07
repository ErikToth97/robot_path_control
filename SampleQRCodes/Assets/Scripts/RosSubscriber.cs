using UnityEngine;
using Unity.Robotics.ROSTCPConnector;
using RosMessageTypes.Geometry;
using RosMessageTypes.Ackermann;

public class RosSubscriber : MonoBehaviour
{
    // Start is called before the first frame update
    public string topic;
    private float lastTime;
    private Vector3 lastPos;
    public float offset;
    private Vector3[] prevPos, testPos;
    private float[] times, testTimes;
    private uint prevCnt, testCounter;
    private const uint pointNum = 4;
    public bool runPredictor = true;
    void Start()
    {
        Application.targetFrameRate = 60;
        this.topic = "";
        lastTime = Time.time;
        lastPos = this.transform.localPosition;
        prevPos = new Vector3[pointNum];
        times = new float[pointNum];
        testPos = new Vector3[75];
        testTimes = new float[75];
        float xPos = this.transform.localPosition.x;
        float piUnit = Mathf.PI / 50;
        for (int i = 0; i < 25; i++)
        {
            float y = 1.5f * Mathf.Cos(Mathf.PI/2-piUnit * i);
            float z = 1.5f * Mathf.Sin( piUnit * i);
            testPos[i] = new Vector3(xPos, -z, y);
        }
        for (int i = 0; i < 25; i++)
        {
            testPos[i+25] = new Vector3(xPos, -(1.5f+1.5f/24*i), 1.5f);
        }
        for (int i = 0; i < 25; i++)
        {
            float y = 1.5f * Mathf.Cos(Mathf.PI/2 + piUnit * i) + 1.5f;
            float z = 1.5f * Mathf.Sin(-Mathf.PI+piUnit * i)-3.0f;
            testPos[i+50] = new Vector3(xPos, z, y);
        }
        for (int i = 0; i < 75; i++)
        {
            testTimes[i] = 2.5f+0.2f * i;
        }
        //int i = 0;
        //testPos[i++] = new Vector3(0.0f, 1.5f, 0.0f);
        //testPos[i++] = new Vector3(0.0f, 1.5f, 1.42f);
        //testPos[i++] = new Vector3(0.0f, 1.5f, 1.5f);
        //testPos[i++] = new Vector3(0.0f, 1.5f, 1.5f);
        //testPos[i++] = new Vector3(0.0f, 1.42f, 1.5f);
        //testPos[i++] = new Vector3(0.0f, 0.71f, 1.5f);
        //testPos[i++] = new Vector3(0.0f, 0.0f, 1.5f);
        //testPos[i++] = new Vector3(0.0f, 0.0f, 1.5f);
        //testPos[i++] = new Vector3(0.0f, 0.0f, 1.46f);
        //testPos[i++] = new Vector3(0.0f, 0.0f, 1.00f);
        //testPos[i++] = new Vector3(0.0f, 0.0f, 0.56f);
        //testPos[i++] = new Vector3(0.0f, 0.0f, 0.05f);
        //testPos[i++] = new Vector3(0.0f, 0.0f, 0.0f);
        //i = 0;
        //testTimes[i++] = 0.0f;
        //testTimes[i++] = 3.55f;
        //testTimes[i++] = 3.95f;
        //testTimes[i++] = 4.95f;
        //testTimes[i++] = 5.344f;
        //testTimes[i++] = 7.144f;
        //testTimes[i++] = 8.95f;
        //testTimes[i++] = 9.95f;
        //testTimes[i++] = 10.25f;
        //testTimes[i++] = 11.95f;
        //testTimes[i++] = 12.95f;
        //testTimes[i++] = 14.95f;
        //testTimes[i++] = 15.25f;
        prevCnt = 0;
        testCounter = 0;
    }

    private void Update()
    {
        if(testCounter < testPos.Length && Time.time > testTimes[testCounter])
        {
            onReceiveTest(testCounter++);
        }
        if(runPredictor && prevCnt == pointNum)
            this.transform.localPosition = estPos(Time.time);
        else if (runPredictor && prevCnt>1)
        {
            var timediff = Time.time - times[prevCnt - 1];
            var der = estDerivative(prevPos[prevCnt - 1], prevPos[prevCnt - 2], times[prevCnt - 1] - times[prevCnt - 2]);
            Vector3 predictedPos = prevPos[prevCnt - 1] + der * timediff;
            this.transform.localPosition = predictedPos;
        }
    }

    public string getTopic()
    {
        return this.topic;
    }

    public void switchPrediction(bool predict)
    {
        this.runPredictor = predict;
    }

    private void OnDestroy()
    {
        ROSConnection.GetOrCreateInstance().Unsubscribe(topic);
    }

    public void subscribeToTopic(string topic)
    {
        if (this.name == "car1")
        {
            var occObj = this.transform.GetChild(0);
            occObj.localPosition = new Vector3(occObj.localPosition.x, occObj.localPosition.y, occObj.localPosition.z + 0.25f);
        }
        if (this.name == "car2")
        {
            var occObj = this.transform.GetChild(0);
            occObj.localPosition = new Vector3(occObj.localPosition.x, occObj.localPosition.y, occObj.localPosition.z + 0.26f);
        }
        this.topic = topic;
        //if (topic.Contains("vesc/low_level/ackermann_cmd_mux/output"))
        //    ROSConnection.GetOrCreateInstance().Subscribe<AckermannDriveStampedMsg>(topic, onReceiveWheelGas);
        //else
            ROSConnection.GetOrCreateInstance().Subscribe<PoseStampedMsg>(topic, this.onReceive);
    }

    void onReceiveTest(uint testCnt)
    {
        var posData = testPos[testCnt];
        var oriData = this.transform.localRotation;
        var oriQuat = new Quaternion((float)oriData.x, (float)oriData.y, (float)oriData.z, (float)oriData.w).eulerAngles;
        this.transform.localPosition = new Vector3((float)posData.x, (float)posData.z , (float)posData.y);
        this.transform.localRotation = Quaternion.Euler(oriQuat.x, oriQuat.z, -oriQuat.y);
        if (runPredictor)
        {
            if (prevCnt < pointNum)
            {
                prevPos[prevCnt] = this.transform.localPosition;
                times[prevCnt++] = Time.time;
            }
            else
            {
                for (int i = 0; i < pointNum - 1; i++)
                {
                    prevPos[i] = prevPos[i + 1];
                    times[i] = times[i + 1];
                }
                prevPos[pointNum - 1] = this.transform.localPosition;
                times[pointNum - 1] = Time.time;
            }
        }
    }

    void onReceive(PoseStampedMsg posMessage)
    {
        var posData = posMessage.pose.position;
        var oriData = posMessage.pose.orientation;
        var oriQuat = new Quaternion((float)oriData.x, (float)oriData.y, (float)oriData.z, (float)oriData.w).eulerAngles;
        this.transform.localPosition = new Vector3((float)posData.x, (float)posData.z - 0.18f, (float)posData.y);
        this.transform.localRotation = Quaternion.Euler(oriQuat.x, -oriQuat.z-90.0f, -oriQuat.y);
        if (runPredictor)
        {
            if (prevCnt < pointNum)
            {
                prevPos[prevCnt] = this.transform.localPosition;
                times[prevCnt++] = Time.time;
            }
            else
            {
                for (int i = 0; i < pointNum - 1; i++)
                {
                    prevPos[i] = prevPos[i + 1];
                    times[i] = times[i + 1];
                }
                prevPos[pointNum-1] = this.transform.localPosition;
            }
        }
    }
    void onReceiveWheelGas(AckermannDriveStampedMsg driveMsg)
    {
        float elapsedTime = Time.time - lastTime;
        lastTime = Time.time;
        float gas = driveMsg.drive.speed;
        float wheelRot = driveMsg.drive.steering_angle*Mathf.Rad2Deg* elapsedTime; 
        transform.Rotate(new Vector3(0, 1, 0) * wheelRot, Space.Self);
        Vector3 currentDir = -this.transform.forward;
        this.transform.position = this.transform.position + currentDir * gas * elapsedTime;
        lastPos = this.transform.position;
    }

    Vector3 estPos(float timeToEst)
    {
        if (runPredictor)
        {
            Vector3[] diffs = new Vector3[pointNum - 1];
            float[] diffTimes = new float[pointNum - 1];
            for (int i = 1; i < pointNum - 1; i++)
            {
                var est1 = estDerivative(prevPos[i], prevPos[i - 1], times[i] - times[i - 1]);
                var est2 = estDerivative(prevPos[i + 1], prevPos[i], times[i + 1] - times[i]);
                diffs[i - 1] = (est1 + est2) / 2;
                diffTimes[i - 1] = times[i];
            }
            diffs[pointNum - 2] = estDerivative(prevPos[pointNum - 1], prevPos[pointNum - 2], times[pointNum - 1] - times[pointNum - 2]);
            diffTimes[pointNum - 2] = times[pointNum - 1];
            var sec1 = estDerivative(diffs[1], diffs[0], diffTimes[1] - diffTimes[0]);
            var sec2 = estDerivative(diffs[2], diffs[1], diffTimes[2] - diffTimes[1]);
            var secondDiff = (sec1 + sec2) / 2;
            var timediff = timeToEst - times[pointNum - 1];
            Vector3 predictedPos = prevPos[pointNum - 1] + diffs[1] * timediff + secondDiff / 2 * timediff * timediff;
            if (timediff < 2.0f)
                predictedPos = predictedPos;
            else
                predictedPos = this.transform.localPosition;
            return predictedPos;
        }
        return this.transform.localPosition;
    }

    Vector3 estDerivative(Vector3 currPos, Vector3 prvPos, float timeDiff)
    {
        if (Mathf.Abs(timeDiff) < 0.00001f)
            return new Vector3(0, 0, 0);
        return (currPos- prvPos) /timeDiff;
    }

}
