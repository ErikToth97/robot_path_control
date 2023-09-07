using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using Unity.Robotics.ROSTCPConnector;
using TMPro;

#if WINDOWS_UWP
using Windows.Storage;
#endif


public class RosConnectIP : MonoBehaviour
{
    ROSConnection m_Ros;
    public GameObject ipAddressInput, referencePoint;
    private TMP_InputField tmInput;
    // ROS Connector
    private string rosIP;
    private bool switched,resetting;
    public static Dictionary<string, GameObject> occluders, vehicles;
    private List<string> subscribedTopics;
    float lastTime;


    // Start is called before the first frame update
    void Start()
    {
        Application.targetFrameRate = 60;
#if WINDOWS_UWP
        // Get IP address from localSettings
        var localSettings = ApplicationData.Current.LocalSettings;
        if(localSettings.Values["IP"] != null){
             tmInput.text = localSettings.Values["IP"].ToString();
        }
#endif
        tmInput = ipAddressInput.GetComponent<TMP_InputField>();
        m_Ros = ROSConnection.GetOrCreateInstance();
        m_Ros.TFTopics = new string[0];
        vehicles = new Dictionary<string, GameObject>();
        occluders = new Dictionary<string, GameObject>();
        switched = false;
        resetting = false;
        rosIP = tmInput.text;
        lastTime = Time.time;
        subscribedTopics = new List<string>();
        if (m_Ros.ConnectOnStart)
        {
            m_Ros.Connect(rosIP, m_Ros.RosPort);
            m_Ros.GetTopicList(topicList);
        }
    }

    private void OnApplicationQuit()
    {
        m_Ros.Disconnect();
    }
    private void OnDestroy()
    {
        m_Ros.Disconnect();
        Destroy(m_Ros);
    }
    private void Update()
    {
        if (!resetting && !m_Ros.HasConnectionError && Time.time - lastTime > 1)
        {
            m_Ros.GetTopicList(topicList);
            lastTime = Time.time;
        }
     
    }

    public void switchPrediction(bool runPrediction)
    {
        foreach(var occluder in occluders)
        {
            occluder.Value.GetComponent<RosSubscriber>().switchPrediction(runPrediction);
        }
        foreach(var vehicle in vehicles)
        {
            vehicle.Value.GetComponent<RosSubscriber>().switchPrediction(runPrediction);
        }
    }

    public void resetScene()
    {
        resetting = true;
        occluders.Clear();
        vehicles.Clear();
        foreach(var topic in subscribedTopics)
        {
            m_Ros.Unsubscribe(topic);
        }
        resetting = false;
    }

    public void topicList(string[] topics)
    {
        foreach(var topic in topics)
        {
           
            string firstWord = topic.Substring(topic.IndexOf('/') + 1);
            if (firstWord != "" && firstWord.Contains('/'))
                firstWord = firstWord.Substring(0, firstWord.IndexOf('/'));

            //if (topic.Contains("vesc/low_level/ackermann_cmd_mux/output") && !occluders.ContainsKey(firstWord))
            //{
            //    GameObject instance = Instantiate(Resources.Load("OccluderCar", typeof(GameObject))) as GameObject;
            //    instance.name = firstWord;
            //    instance.transform.SetParent(referencePoint.transform, false);
            //    instance.transform.localPosition = new Vector3(0.0f, 0.0f, 0.0f);
            //    instance.GetComponent<RosSubscriber>().subscribeToTopic(topic);
            //    occluders.Add(firstWord, instance);
            //}

            if (firstWord.Contains("car") && topic.Contains("pose") )
            {

                if ((firstWord == "car1" || firstWord == "car2" || firstWord == "car3") && !occluders.ContainsKey(firstWord) && !vehicles.ContainsKey(firstWord))
                {
                    //GameObject instance = Instantiate(Resources.Load("OccluderCube", typeof(GameObject))) as GameObject;
                    GameObject instance = Instantiate(Resources.Load("OccluderCube", typeof(GameObject))) as GameObject;
                    instance.name = firstWord;
                    instance.transform.SetParent(referencePoint.transform, false);
                    instance.transform.localPosition = new Vector3(0.0f, 0.0f, 0.0f);
                    instance.GetComponent<RosSubscriber>().subscribeToTopic(topic);
                    subscribedTopics.Add(topic);
                    occluders.Add(firstWord, instance);

                }
                else if (!occluders.ContainsKey(firstWord) && !vehicles.ContainsKey(firstWord))
                {
                    GameObject instance = Instantiate(Resources.Load("suvCarSmall", typeof(GameObject))) as GameObject;
                    instance.name = firstWord;
                    instance.transform.SetParent(referencePoint.transform, false);
                    instance.transform.localPosition = new Vector3(0.0f, 0.0f, 0.0f);
                    instance.GetComponent<RosSubscriber>().subscribeToTopic(topic);
                    subscribedTopics.Add(topic);
                    vehicles.Add(firstWord, instance);
                }

            }
          
        }
    }

    public void switchOccluder()
    {
        foreach (var vehicle in occluders)
        {
            var occluderSpehere = vehicle.Value.transform.Find("OccluderObj/Box");
            occluderSpehere.gameObject.SetActive(switched);
        }
        switched = !switched;
    }

    public void Connect(string inputIP="")
    {
        rosIP = inputIP;
        if (inputIP =="")
            rosIP = tmInput.text;
        m_Ros.Disconnect();
        m_Ros.Connect(rosIP, m_Ros.RosPort);

#if WINDOWS_UWP
        // Save IP address to localSettings
        var localSettings = ApplicationData.Current.LocalSettings;
        localSettings.Values["IP"] = rosIP;   
#endif
    }

    public void ipAddrChange()
    {
        rosIP = tmInput.text;
        m_Ros.Disconnect();
        m_Ros.Connect(rosIP, m_Ros.RosPort);
#if WINDOWS_UWP
        // Save IP address to localSettings
        var localSettings = ApplicationData.Current.LocalSettings;
        localSettings.Values["IP"] = rosIP;   
#endif
    }
}
