using Microsoft.MixedReality.Toolkit.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using TMPro;
using UnityEngine;

public class DesctiptionHanderlTCP : MonoBehaviour
{
    //private TcpClient socketConnection;
    //private Thread clientReceiveThread;

    private bool disconnected, ipChanged;
    private bool stopThread;
    private string IpAddr;
    private TMP_InputField tmInput;

    public GameObject referencePoint, ipAddressInput;

    private static Dictionary<string,string[]> msgBuffer;
    // Start is called before the first frame update
    void Start()
    {

        string[] msg = new string[2];
        msg[0] = "TeamName";
        msg[1] = "100";
        stopThread = false;
        msgBuffer = new Dictionary<string, string[]>();
        disconnected = true;
        ipChanged = false;
        msgBuffer.Add("0", msg);
        IpAddr = "10.0.4.126";
        tmInput = ipAddressInput.GetComponent<TMP_InputField>();
       // ConnectToTcpServer();
    }


    void OnDestroy()
    {
        stopThread = true;
        //clientReceiveThread.Abort();
    }

    // Update is called once per frame
    void Update()
    {
        //if (clientReceiveThread != null && !clientReceiveThread.IsAlive)
        //{
        //    clientReceiveThread.Abort();
        //    disconnected = true;
        //}
           
        //if (ipChanged && disconnected)
        //{
        //    stopThread = false;
        //    ipChanged = false;
        //}
        //if (disconnected && !stopThread)
        //{
        //    disconnected = false;
        //    ConnectToTcpServer();
        //}
        //if (msgBuffer.Count > 0)
        //{
        //    Dictionary<string, string[]> msgs = new Dictionary<string, string[]>(msgBuffer);
        //    msgBuffer.Clear();
        //    foreach(var msg in msgs)
        //    {
        //        var vehicle = referencePoint.transform.Find("vehicle" + msg.Key);
        //        if(vehicle == null)
        //        {
        //            msgBuffer.Add(msg.Key,msg.Value);
        //            return;
        //        }
        //        var teamTxt = vehicle.Find("TeamName");
        //        var txtMesh = teamTxt.GetComponent<ToolTip>();
        //        txtMesh.ToolTipText = msg.Value[0];

        //        var descTxt = vehicle.Find("Description");
        //        var descMesh = descTxt.GetComponent<ToolTip>();
        //        descMesh.ToolTipText = msg.Value[1];
        //    }
        //}
    }

    private void ConnectToTcpServer()
    {
        //try
        //{
        //    Debug.Log("Try connecting");
        //    if (clientReceiveThread != null)
        //        clientReceiveThread.Abort();
        //    clientReceiveThread = new Thread(new ThreadStart(ListenForData));
        //    clientReceiveThread.IsBackground = true;
        //    clientReceiveThread.Start();
        //    disconnected = false;
        //}
        //catch (Exception e)
        //{
        //    disconnected = true;
        //    if(clientReceiveThread != null)
        //        clientReceiveThread.Abort();
        //    Debug.Log("On client connect exception " + e);
        //}
    }

	private void ListenForData()
	{
		//try
		//{
  //          //socketConnection = new TcpClient("192.168.8.154", 30001);
  //          socketConnection = new TcpClient(IpAddr, 30001);
  //          //socketConnection.ReceiveTimeout = 30000;
  //          byte[] bytes = new byte[2048];
		//	while (!stopThread)
		//	{
		//		// Get a stream object for reading 				
		//		using (NetworkStream stream = socketConnection.GetStream())
		//		{
		//			int length;
  //                  // Read incomming stream into byte arrary. 	
  //                  try
  //                  {
  //                      while (!stopThread && (length = stream.Read(bytes, 0, bytes.Length)) != 0)
  //                      {
  //                          var incommingData = new byte[length];
  //                          Array.Copy(bytes, 0, incommingData, 0, length);
  //                          //Debug.Log(incommingData);
  //                          // Convert byte array to string message.	
  //                          string serverMessage = Encoding.UTF7.GetString(incommingData);
  //                          //serverMessage = Encoding.UTF32.GetString(bytes);

  //                          var lines = serverMessage.Split('\n');
  //                          for (int i = 0; i < lines.Length; i++)
  //                          {
  //                              var words = lines[i].Split('#');
  //                              if (words.Length == 4)
  //                              {
  //                                  msgBuffer[words[0]] = new string[2];
  //                                  msgBuffer[words[0]][0] = words[1];
  //                                  msgBuffer[words[0]][1] = words[2] + "\n" + words[3];
  //                              }

  //                          }
  //                      }
  //                  }
  //                  catch (SocketException socketException)
  //                  {
  //                      disconnected = true;
  //                      Debug.Log("Socket exception: " + socketException);
  //                  }
  //              }
		//	}
		//}
		//catch (SocketException socketException)
		//{
		//	disconnected = true;
		//	Debug.Log("Socket exception: " + socketException);
		//}
	}

    public void ipAddrChange()
    {
        stopThread = true;
        IpAddr = tmInput.text;
        ipChanged = true;
    }
}
