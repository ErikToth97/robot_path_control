using Microsoft.MixedReality.Toolkit.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using TMPro;
using UnityEngine;

public class CarHandlerTCP : MonoBehaviour
{
   // private TcpClient socketConnection;
    //private Thread clientReceiveThread;

	private bool disconnected;
	private bool stopThread;
	private string IpAddr;
	private string currentVehicle;
	private TMP_InputField tmInput;

	private static Dictionary<string, float[]> transformBuffer;
	private static float[] pathTimes;
	private static float[][] pathPos;
	public static Dictionary<string, GameObject> vehicles;
	public GameObject basicDummy;
	private static Dictionary<string, float> prevSpeed;
	public GameObject referencePoint, ipAddressInput;
	private float timer, pathLengthInTime;
	private int pathInd;


	private Boolean switched, switchSUV, locked, ipChanged, testStarted, setTimer;
	// Start is called before the first frame update
	void Start()
    {
		transformBuffer = new Dictionary<string, float[]>();
		prevSpeed = new Dictionary<string, float>();
		IpAddr = "192.168.1.31";
		float[] newFloat = new float[6];
		transformBuffer.Add("0", newFloat);
		//for(int i = 0; i < 18; i++)
  //      {
		//	float[] newFloat = new float[6];
		//	newFloat[0] = 0.25f * (float)i;
		//	transformBuffer.Add(i + "", newFloat);
		//}
		vehicles = new Dictionary<string, GameObject>();
		disconnected = true;
		stopThread = false;
		pathPos = new float[1000][];
		for (int i = 0; i < 1000; i++)
			pathPos[i] = new float[3];
		pathTimes = new float[1000];
		Application.targetFrameRate = 60;
		switched = false;
		switchSUV = false;
		locked = false;
		ipChanged = false;
		currentVehicle = "redF1";
		timer = 0.0f;
		pathLengthInTime = 0.0f;
		testStarted = false;
		setTimer = false;
		tmInput = ipAddressInput.GetComponent<TMP_InputField>();
		pathInd = 0;
		//ConnectToTcpServer();
	}

	void OnDestroy()
    {
		stopThread = true;
		//clientReceiveThread.Join();
		//clientReceiveThread.Abort();
    }

	private void setVehicleColor(GameObject instance)
    {
		Transform body = instance.transform;
		Renderer bodyRenderer = null;
		Color newColor = getVehicleColor();

		if (currentVehicle == "redF1")
        {
			body = instance.transform.Find("Sketchfab_model/root/GLTF_SceneRootNode/1988 mclaren_2/color");
			bodyRenderer = body.gameObject.GetComponent<Renderer>();
			bodyRenderer.material.SetColor("_Color", newColor);
		}
		else if(currentVehicle == "suvCar")
        {
			body = instance.transform.Find("root/body/mainBody");
			bodyRenderer = body.gameObject.GetComponent<Renderer>();
			bodyRenderer.material.SetColor("_Color", newColor);
		}
		else if(currentVehicle == "droneProp")
		{
			body = instance.transform.Find("default");
			bodyRenderer = body.GetComponent<Renderer>();
			bodyRenderer.material.SetColor("_Color", newColor);

			for (int k = 0; k < bodyRenderer.materials.Length; k++)
			{
				bodyRenderer.materials[k].color = newColor;
			}

		}
	}
	
	private Color getVehicleColor()
    {
		var colorOffset = 3 - vehicles.Count % 3;
		Color newColor = new Color();
		if (vehicles.Count < 3)
			newColor = new Color(0.333f * colorOffset, 1.0f - 0.333f * colorOffset, 0);
		else if (vehicles.Count < 6)
			newColor = new Color(1.0f - 0.333f * colorOffset, 0, 0.333f * colorOffset);
		else if (vehicles.Count < 9)
			newColor = new Color(0, 0.333f * colorOffset, 1.0f - 0.333f * colorOffset);
		else if (vehicles.Count < 12)
			newColor = new Color(1.0f - 0.333f * colorOffset, 1.0f, 0.333f * colorOffset);
		else if (vehicles.Count < 15)
			newColor = new Color(1.0f, 1.0f - 0.333f * colorOffset, 0.333f * colorOffset);
		else if (vehicles.Count < 18)
			newColor = new Color(1.0f - 0.333f * colorOffset, 0.333f * colorOffset, 1.0f);
		return newColor;
	}

	void Update()
	{
		//if (clientReceiveThread != null && !clientReceiveThread.IsAlive)
		//{
		//	clientReceiveThread.Abort();
		//	disconnected = true;
		//}
		//if (ipChanged && disconnected)
  //      {
  //          stopThread = false;
  //          ipChanged = false;
  //      }
  //      if (disconnected && !stopThread)
  //      {
		//	disconnected = false;
		//	ConnectToTcpServer();
		//}
  //      if (setTimer)
  //      {
		//	timer += Time.deltaTime;
		//	if(timer >= pathLengthInTime+2.5)
  //          {
		//		setTimer = false;
		//		testStarted = false;
  //          }
  //      }
  //      if (transformBuffer.Count == 0)
  //      {
  //          if (testStarted)
  //          {
  //              var futureTime = timer + 2.0f;
  //              float[] times = new float[pathInd];
  //              Array.Copy(pathTimes, 0, times, 0, pathInd);
  //              int currInd = Array.BinarySearch(times, futureTime);
  //              if (currInd < 0)
  //                  currInd = ~currInd;
  //              if (currInd == 0)
  //                  currInd = 1;
  //              var prevStamp = pathPos[currInd - 1];
  //              var nextStamp = prevStamp;
  //              float[] newPos = new float[3];
  //              if (currInd < pathInd)
  //              {
  //                  nextStamp = pathPos[currInd];

  //                  var deltaTime = futureTime - pathTimes[currInd - 1];
  //                  var relativeTime = deltaTime / (pathTimes[currInd] - pathTimes[currInd - 1]);
  //                  for (int i = 0; i < 3; i++)
  //                  {
  //                      newPos[i] = prevStamp[i] + (nextStamp[i] - prevStamp[i]) * relativeTime;
  //                  }
  //              }
  //              else
  //              {
  //                  newPos = prevStamp;
  //              }
  //              vehicles["0"].transform.localPosition = new Vector3(newPos[0], newPos[2], newPos[1]);
  //          }

  //      }
  //      if (transformBuffer.Count > 0 && !locked)
		//{
		//	locked = true;
		//	Dictionary<String, float[]> transforms = new Dictionary<string, float[]>(transformBuffer);
		//	transformBuffer.Clear();
		//	locked = false;
		//	foreach (var tm in transforms)
		//	{
		//		if (!testStarted)
  //              {
  //                  if (tm.Value[2] > 0.011)
  //                  {
  //                      testStarted = true;
  //                      timer = 0.0f;
  //                      setTimer = true;
  //                      pathLengthInTime = pathTimes[pathInd - 1];
  //                  }


  //              }
		//		try
  //              {
		//			var carKey = int.Parse(tm.Key);
		//			if (!vehicles.ContainsKey(tm.Key))
		//			{
		//				//prevSpeed[tm.Key] = 0.0f;
		//				GameObject instance = Instantiate(Resources.Load(currentVehicle, typeof(GameObject))) as GameObject;
		//				instance.name = "vehicle" + tm.Key;
		//				instance.transform.SetParent(referencePoint.transform, false);
		//				instance.transform.localPosition = new Vector3(0.0f, 0.0f, 0.0f);
		//				setVehicleColor(instance);
		//				vehicles.Add(tm.Key, instance);
		//			}
		//			if(switchSUV)
  //                  {
		//				if(tm.Value[0] < -29.0f && tm.Value[0] > -31.0f)
  //                      {
		//					basicDummy.SetActive(true);
		//					basicDummy.transform.localPosition = new Vector3(6.16f, -0.5f, -2.04f);
		//				}
  //                  }

		//			//if (testStarted)
		//			//{
		//			//	var futureTime = timer + 2.0f;
		//			//	float[] times = new float[pathInd];
		//			//	Array.Copy(pathTimes, 0, times, 0, pathInd);
		//			//	int currInd = Array.BinarySearch(times, futureTime);
		//			//	if (currInd < 0)
		//			//		currInd = ~currInd;
		//			//	if (currInd == 0)
		//			//		currInd = 1;
		//			//	var prevStamp = pathPos[currInd - 1];
		//			//	var nextStamp = prevStamp;
		//			//	float[] newPos = new float[3];
		//			//	if (currInd < pathInd)
		//			//	{
		//			//		nextStamp = pathPos[currInd];

		//			//		var deltaTime = futureTime - pathTimes[currInd - 1];
		//			//		var relativeTime = deltaTime / (pathTimes[currInd] - pathTimes[currInd - 1]);
		//			//		for (int i = 0; i < 3; i++)
		//			//		{
		//			//			newPos[i] = prevStamp[i] + (nextStamp[i] - prevStamp[i]) * relativeTime;
		//			//		}
		//			//	}
		//			//	else
		//			//	{
		//			//		newPos = prevStamp;
		//			//	}
		//			//	vehicles["0"].transform.localPosition = new Vector3(newPos[0], newPos[2], newPos[1]);
		//			//}

		//			//vehicles[tm.Key].transform.localPosition = new Vector3(tm.Value[0], tm.Value[2]-0.459f, tm.Value[1]+0.85f);
		//			vehicles[tm.Key].transform.localRotation = Quaternion.Euler(tm.Value[4], tm.Value[3]+180.0f, tm.Value[5]);
					
		//		}
  //              catch
  //              {
		//			Debug.Log("Wrong index format");
  //              }
				
		//	}
		//}
	}

	public void resetScene()
    {
		foreach(var vehicle in vehicles)
        {
			Destroy(vehicle.Value);
        }
		vehicles.Clear();
		transformBuffer.Clear();
		stopThread = false;
		switched = false;
		switchSUV = false;
		locked = false;
	}

	public void SwitchToDrones()
	{
		if (switched)
			return;
		switched = true;
		currentVehicle = "droneProp";
		int i = 0;
		Dictionary<string, GameObject> drones = new Dictionary<string, GameObject>();
		foreach (var vehicle in vehicles)
		{
			GameObject instance = Instantiate(Resources.Load(currentVehicle, typeof(GameObject))) as GameObject;
			instance.name = "vehicle" + i;
			i++;
			instance.transform.SetParent(referencePoint.transform);
			var droneBody = instance.transform.Find("default");
			var droneRenderer = droneBody.GetComponent<Renderer>();
			var body = vehicle.Value.transform.Find("Sketchfab_model/root/GLTF_SceneRootNode/1988 mclaren_2/color");
			var vehicleRenderer = body.gameObject.GetComponent<Renderer>();
			for(int k = 0; k < droneRenderer.materials.Length; k++)
            {
				droneRenderer.materials[k].color = vehicleRenderer.material.GetColor("_Color");
			}
			instance.transform.localPosition = vehicle.Value.transform.localPosition;
			instance.transform.localRotation = vehicle.Value.transform.localRotation;

			setTooltip(instance.transform, vehicle.Value.transform);

			drones.Add(vehicle.Key, instance);
		}
		foreach (var drone in drones)
		{
			var car = vehicles[drone.Key];
			vehicles[drone.Key] = drone.Value;
			Destroy(car);
		}
		
	}

	private void setTooltip(Transform t1, Transform t2)
    {
		var T1TeamTxt = t1.Find("TeamName");
		var T1txtMesh = T1TeamTxt.GetComponent<ToolTip>();
		var T2teamTxt = t2.Find("TeamName");
		var T2txtMesh = T2teamTxt.GetComponent<ToolTip>();
		T1txtMesh.ToolTipText = T2txtMesh.ToolTipText;

		var T1DescTxt = t1.Find("Description");
		var T1DescMesh = T1DescTxt.GetComponent<ToolTip>();
		var T2DescTxt = t2.Find("Description");
		var T2DescMesh = T2DescTxt.GetComponent<ToolTip>();
		T1DescMesh.ToolTipText = T2DescMesh.ToolTipText;

	}

	public void SwitchToCars()
	{
		if (!switched && !switchSUV)
			return;
		int i = 0;
		currentVehicle = "redF1";
		Dictionary<string, GameObject> cars = new Dictionary<string, GameObject>();
		foreach (var vehicle in vehicles)
		{
			GameObject instance = Instantiate(Resources.Load(currentVehicle, typeof(GameObject))) as GameObject;
			instance.name = "vehicle" + i;
			i++;
			instance.transform.SetParent(referencePoint.transform);
			var body = instance.transform.Find("Sketchfab_model/root/GLTF_SceneRootNode/1988 mclaren_2/color");
			var carRenderer = body.GetComponent<Renderer>();
			Renderer vehicleRenderer;
            if (switched)
            {
				var droneBody = vehicle.Value.transform.Find("default");
				vehicleRenderer = droneBody.GetComponent<Renderer>();
			}
            else
            {
				var suvBody = vehicle.Value.transform.Find("root/body/mainBody");
				vehicleRenderer = suvBody.gameObject.GetComponent<Renderer>();
			}
			carRenderer.material.SetColor("_Color", vehicleRenderer.material.GetColor("_Color"));
			instance.transform.localPosition = vehicle.Value.transform.localPosition;
			instance.transform.localRotation = vehicle.Value.transform.localRotation;
			setTooltip(instance.transform, vehicle.Value.transform);
			cars.Add(vehicle.Key, instance);
		}
		foreach (var car in cars)
		{
			var drone = vehicles[car.Key];
			vehicles[car.Key] = car.Value;
			Destroy(drone);
		}
		switched = false;
		switchSUV = false;
	}

	public void SwitchToSUV()
	{
		SwitchToCars();
		if (switchSUV)
			return;
		switchSUV = true;
		currentVehicle = "suvCar";
		int i = 0;
		Dictionary<string, GameObject> suvs = new Dictionary<string, GameObject>();
		foreach (var vehicle in vehicles)
		{
			GameObject instance = Instantiate(Resources.Load(currentVehicle, typeof(GameObject))) as GameObject;
			instance.name = "vehicle" + i;
			i++;
			instance.transform.SetParent(referencePoint.transform);
			var suvBody = instance.transform.Find("root/body/mainBody");
			var suvRenderer = suvBody.gameObject.GetComponent<Renderer>();
			var body = vehicle.Value.transform.Find("Sketchfab_model/root/GLTF_SceneRootNode/1988 mclaren_2/color");
			var vehicleRenderer = body.gameObject.GetComponent<Renderer>();
			suvRenderer.material.SetColor("_Color", vehicleRenderer.material.GetColor("_Color"));
			
			instance.transform.localPosition = vehicle.Value.transform.localPosition;
			instance.transform.localRotation = vehicle.Value.transform.localRotation;

			setTooltip(instance.transform, vehicle.Value.transform);
			suvs.Add(vehicle.Key, instance);
		}
		foreach (var suv in suvs)
		{
			var car = vehicles[suv.Key];
			vehicles[suv.Key] = suv.Value;
			Destroy(car);
		}
	}
	private void ConnectToTcpServer()
	{

		//try
		//{
		//	Debug.Log("Try connecting");
		//	if (clientReceiveThread != null)
		//		clientReceiveThread.Abort();
		//	clientReceiveThread = new Thread(new ThreadStart(ListenForData));
		//	clientReceiveThread.IsBackground = true;
		//	clientReceiveThread.Start();
		//	disconnected = false;
		//}
		//catch (Exception e)
		//{
		//	disconnected = true;
		//	if (clientReceiveThread != null)
		//		clientReceiveThread.Abort();
		//	Debug.Log("On client connect exception " + e);
		//}
	}
	/// <summary> 	
	/// Runs in background clientReceiveThread; Listens for incomming data. 	
	/// </summary>     
	private void ListenForData()
	{
		//try
		//{
		//	Debug.Log("Connecting to " + IpAddr);
		//	//socketConnection = new TcpClient("192.168.8.154", 30000);
		//	socketConnection = new TcpClient(IpAddr, 30000);

		//	//socketConnection.ReceiveTimeout = 30000;
		//	Byte[] bytes = new Byte[1024];
		//	while (!stopThread)
		//	{
		//		// Get a stream object for reading 	
		//		using (NetworkStream stream = socketConnection.GetStream())
		//		{
		//			int length;
		//		// Read incomming stream into byte arrary. 	
		//			try
		//			{
		//				while (!stopThread && (length = stream.Read(bytes, 0, bytes.Length)) != 0)
		//				{
		//					var incommingData = new byte[length];
		//					Array.Copy(bytes, 0, incommingData, 0, length);
		//					// Convert byte array to string message. 						
		//					string serverMessage = Encoding.ASCII.GetString(incommingData);
		//					var lines = serverMessage.Split('\n');
		//					for (int i = 0; i < lines.Length; i++)
		//					{
		//						var words = lines[i].Split(' ');
		//						if (words.Length == 7)
		//						{
		//							float[] transformMatrix = new float[6];
		//							for (int k = 1; k < words.Length; k++)
		//							{
		//								bool success = false;
		//								if (words[k] != "")
		//									success = float.TryParse(words[k], NumberStyles.Float, CultureInfo.InvariantCulture, out transformMatrix[k - 1]);
		//								if (!success)
		//									transformMatrix[k - 1] = 0.0f;
		//							}
		//							while (locked)
		//							{
		//								Thread.Sleep(1);
		//							}
		//							locked = true;
		//							if (!transformBuffer.ContainsKey(words[0]))
		//								transformBuffer.Add(words[0], transformMatrix);
		//							else
		//								transformBuffer[words[0]] = transformMatrix;
		//							locked = false;
									
		//						}
  //                              else
  //                              {
		//							if (words[0] == "-1")
		//							{
		//								if (i == 0)
		//								{
		//									pathInd = 0;
		//									timer = 0.0f;
		//								}
		//								float[] pathData = new float[4];
		//								for (int k = 1; k < 5; k++)
		//								{
		//									bool success = false;
		//									if (words[k] != "")
		//									{
		//										if (k == 1)
		//											success = float.TryParse(words[k], NumberStyles.Float, CultureInfo.InvariantCulture, out pathTimes[pathInd]);
		//										else
		//											success = float.TryParse(words[k], NumberStyles.Float, CultureInfo.InvariantCulture, out pathPos[pathInd][k - 2]);
		//									}
		//									if (!success)
		//									{
		//										if (k == 1)
		//											pathTimes[pathInd] = 0.0f;
		//										else
		//											pathPos[pathInd][k - 1] = 0.0f;
		//									}


		//								}
		//								pathInd += 1;
		//							}
		//						}
		//					}
		//				}
		//			}
		//			catch (SocketException socketException)
		//			{
		//				disconnected = true;
		//				Debug.Log("Socket exception: " + socketException);
		//			}
		//		}				
		//	}
		//}
		//catch (SocketException socketException)
		//{
		//	disconnected = true;
		//	Debug.Log("Socket exception: " + socketException);
		//}
		//disconnected = true;
	}

	public void ipAddrChange()
    {
		stopThread = true;
		IpAddr = tmInput.text;
		ipChanged = true;
    }
	/// <summary> 	
	/// Send message to server using socket connection. 	
	/// </summary> 	
	private void SendMessage()
	{
		//if (socketConnection == null)
		//{
		//	return;
		//}
		//try
		//{
		//	// Get a stream object for writing. 			
		//	NetworkStream stream = socketConnection.GetStream();
		//	if (stream.CanWrite)
		//	{
		//		string clientMessage = "This is a message from one of your clients.";
		//		// Convert string message to byte array.                 
		//		byte[] clientMessageAsByteArray = Encoding.ASCII.GetBytes(clientMessage);
		//		// Write byte array to socketConnection stream.                 
		//		stream.Write(clientMessageAsByteArray, 0, clientMessageAsByteArray.Length);
		//		Debug.Log("Client sent his message - should be received by server");
		//	}
		//}
		//catch (SocketException socketException)
		//{
		//	Debug.Log("Socket exception: " + socketException);
		//}
	}
}
