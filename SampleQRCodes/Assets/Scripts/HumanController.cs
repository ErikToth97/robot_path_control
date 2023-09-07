using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;

public class HumanController : MonoBehaviour
{
    private TcpClient socketConnection;
    private Thread clientReceiveThread;

    private bool disconnected;
    private bool stopThread;

    private static Dictionary<string, float[]> transformBuffer;

	Animator m_Animator;
    // Start is called before the first frame update
    void Start()
    {
		m_Animator = gameObject.GetComponent<Animator>();
		transformBuffer = new Dictionary<string, float[]>();
        disconnected = true;
        stopThread = false;
        Application.targetFrameRate = 60;
        ConnectToTcpServer();
    }

    private void ConnectToTcpServer()
    {
        try
        {
            Debug.Log("Try connecting");
            clientReceiveThread = new Thread(new ThreadStart(ListenForData));
            clientReceiveThread.IsBackground = true;
            clientReceiveThread.Start();
            disconnected = false;
        }
        catch (Exception e)
        {
            disconnected = true;
            clientReceiveThread.Abort();
            Debug.Log("On client connect exception " + e);
        }
    }

	private void ListenForData()
	{
		try
		{
			//socketConnection = new TcpClient("192.168.8.154", 30000);
			socketConnection = new TcpClient("10.0.4.126", 30002);
		   Byte[] bytes = new Byte[1024];
			while (!stopThread)
			{
				// Get a stream object for reading 	
				using (NetworkStream stream = socketConnection.GetStream())
				{
					int length;
					// Read incomming stream into byte arrary. 					
					while ((length = stream.Read(bytes, 0, bytes.Length)) != 0)
					{
						var incommingData = new byte[length];
						Array.Copy(bytes, 0, incommingData, 0, length);
						// Convert byte array to string message. 						
						string serverMessage = Encoding.ASCII.GetString(incommingData);
						var lines = serverMessage.Split('\n');
						for (int i = 0; i < lines.Length; i++)
						{
							var words = lines[i].Split(' ');
							if (words.Length == 7)
							{
								float[] transformMatrix = new float[6];
								for (int k = 1; k < words.Length; k++)
								{
									if (words[k] != "")
										transformMatrix[k - 1] = float.Parse(words[k], CultureInfo.InvariantCulture);
								}
								if (!transformBuffer.ContainsKey(words[0]))
									transformBuffer.Add(words[0], transformMatrix);
								else
									transformBuffer[words[0]] = transformMatrix;
							}
						}
					}
				}
			}
		}
		catch (SocketException socketException)
		{
			disconnected = true;
		}
	}

	void OnDestroy()
    {
        stopThread = true;
        clientReceiveThread.Abort();
    }

    // Update is called once per frame
    void Update()
    {
			if (disconnected && !stopThread)
				ConnectToTcpServer();
			if (transformBuffer.Count > 0)
			{
				Dictionary<String, float[]> transforms = new Dictionary<string, float[]>(transformBuffer);
				transformBuffer.Clear();
				foreach (var tm in transforms)
				{
					try
					{
						gameObject.transform.localPosition = new Vector3(tm.Value[0], tm.Value[2] - 0.459f, tm.Value[1] + 0.85f);
						gameObject.transform.localRotation = Quaternion.Euler(tm.Value[4], tm.Value[3] + 180.0f, tm.Value[5]);
						m_Animator.speed = tm.Value[7];
					}
					catch
					{
						Debug.Log("Wrong index format");
					}

				}
			}
	}

	public void changeSpeed()
    {
		m_Animator.speed = 0.5f;

	}

	public void resetTM()
    {
		gameObject.transform.localPosition = new Vector3(0, -0.5f, 0);

		gameObject.transform.localRotation = Quaternion.Euler(0, 0, 0);
	}
}
