using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading;
using UnityEngine;

public class CarHandler : MonoBehaviour
{
    public static HttpListener listener;
    private static Dictionary<string, float[]> transformBuffer;
    public static Dictionary<string, GameObject> vehicles;
    public GameObject referencePoint;

    // Start is called before the first frame update
    void Start()
    {
        transformBuffer = new Dictionary<string, float[]>();
        vehicles = new Dictionary<string, GameObject>();
        try
        {
            listener = new HttpListener();
            listener.Prefixes.Add("http://192.168.0.107:4444/");
            listener.Prefixes.Add("http://192.168.0.107:4444/json/");
            listener.AuthenticationSchemes = AuthenticationSchemes.Anonymous;
            listener.Start();
            var listenerThread = new Thread(startListener);
            listenerThread.Start();
        }
        catch
        {
            Debug.Log("Could not start http listener");
        }
      
    }

    private void startListener()
    {
        while (true)
        {
            var result = listener.BeginGetContext(ListenerCallback, listener);
            result.AsyncWaitHandle.WaitOne();
        }
    }

    private void ListenerCallback(IAsyncResult result)
    {
        var context = listener.EndGetContext(result);

        Debug.Log("Method: " + context.Request.HttpMethod);
        Debug.Log("LocalUrl: " + context.Request.Url.LocalPath);

        if (context.Request.QueryString.AllKeys.Length > 0)
            foreach (var key in context.Request.QueryString.AllKeys)
            {

                Debug.Log("Key: " + key + ", Value: " + context.Request.QueryString.GetValues(key)[0]);
            }

        if (context.Request.HttpMethod == "POST")
        {
            var data_text = new StreamReader(context.Request.InputStream,
                                context.Request.ContentEncoding).ReadToEnd();
            var words = data_text.Split(' ');
            float[] transformMatrix = new float[9];
            for (int i = 1; i < words.Length; i++)
            {
                transformMatrix[i] = float.Parse(words[i]);
            }
            transformBuffer.Add(words[0], transformMatrix);
        }

    }



    // Update is called once per frame
    void Update()
    {
        if(transformBuffer.Count > 0)
        {
            Dictionary<String, float[]> transforms = new Dictionary<string, float[]>(transformBuffer);
            transformBuffer.Clear();
            foreach(var tm in transforms)
            {
                if (!vehicles.ContainsKey(tm.Key))
                {
                    if (referencePoint.transform.Find(tm.Key))
                        vehicles.Add(tm.Key, referencePoint.transform.Find(tm.Key).gameObject);
                    else
                        continue;
                }
                vehicles[tm.Key].transform.position = new Vector3(tm.Value[0], tm.Value[1], tm.Value[2]);
                vehicles[tm.Key].transform.rotation = new Quaternion(tm.Value[3], tm.Value[4], tm.Value[5], tm.Value[6]);
            }
        }
    }
}
