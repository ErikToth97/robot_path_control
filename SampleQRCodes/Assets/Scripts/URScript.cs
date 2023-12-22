using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;

public class URScript : MonoBehaviour
{
    // Start is called before the first frame update	
    private string HOST="10.1.2.80";
    private int port = 30002;
    private Socket socketConnection;
    private bool started = false, sent = false;
    void Start()
    {
        socketConnection = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
      
    }

    // Update is called once per frame
    void Update()
    {
        if (!started)
        {
            System.Net.IPAddress ipAdd = System.Net.IPAddress.Parse(HOST);
            System.Net.IPEndPoint remoteEP = new IPEndPoint(ipAdd, port);
            socketConnection.Connect(remoteEP);
            started = true;
        }  
        else if (!sent && socketConnection.Connected)
        {
            //sendData();
            sent = true;
        }
        else if (!socketConnection.Connected)
        {
            Debug.Log("FAILED TO CONNECT");
        }
    }

    public void sendData(double[] angles = null)
    {
        if (!started || !socketConnection.Connected)
            return;
        string strQhome = "movej([-2.456,-1.57, -1.7,-1.32, 1.57,-0.707], a=0.4, v=0.1, t=0, r=0)" + "\n";
        float[] anglesf = new float[angles.Length];
        if (angles != null)
        {
            anglesf[0] = (float)-angles[0];
            anglesf[1] = (float)-angles[1]-Mathf.PI / 2;
            anglesf[2] = (float)-angles[2];
            anglesf[3] = (float)-angles[3] - Mathf.PI / 2;
            anglesf[4] = (float)angles[4];
            anglesf[5] = (float)-angles[5];
            strQhome = "movej([";
            for (int i = 0; i < anglesf.Length - 1; i++)
            {
                if (anglesf[i] > Mathf.PI)
                    anglesf[i] = anglesf[i] - 2 * Mathf.PI;
                if (anglesf[i] < -Mathf.PI)
                    anglesf[i] = anglesf[i] + 2 * Mathf.PI;
                strQhome += anglesf[i].ToString("0.00", System.Globalization.CultureInfo.InvariantCulture) + ",";
            }
            strQhome += anglesf[5].ToString("0.00", System.Globalization.CultureInfo.InvariantCulture) + "], a=1.4, v=0.1, t=0, r=0)" + "\n";
        }

        byte[] sendURbuffer = Encoding.ASCII.GetBytes(strQhome);
        socketConnection.Send(sendURbuffer);

        //socketConnection.Send(Encoding.ASCII.GetBytes("get_actual_TCP_pose()\n"));
        //byte[] answ= new byte[1024];
        //socketConnection.Receive(answ);
        //string decoded = Encoding.ASCII.GetString(answ);
    }
}
