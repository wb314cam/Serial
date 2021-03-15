using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using System.IO;
using UnityEngine.Experimental.VFX;
using UnityScript.Steps;

public class SerialReader : MonoBehaviour
{
	public string address = "172.17.70.24";
	public int port = 9999;
	public string fileName;
	public bool recording = false;
	public bool connected;
	private TcpClient socketConnection;
	private Thread clientReceiveThread;
	public int fileNo = 0;
	public int windowSize = 100;
	public int windowSample = 0;
	public float signaltime;
	public Vector3 acceleration;
	public Vector3 angularVelocity;
	public bool recordForPlayback;
	private void WriteToDisk(string s, string filepath)
	{
		File.AppendAllText(filepath, s);
	}
	// Use this for initialization 	
	void Start()
	{
		ConnectToTcpServer();
		
	}
	// Update is called once per frame
	void Update()
	{
		if(!connected)
			SendMessage();
		if (Input.GetKeyDown(KeyCode.Space))
		{
			windowSample = 0;
			recording ^= true;
			if (recording) fileNo++;
		}

	}
	private void FixedUpdate()
	{
		//	SendMessage();
		
		if (recording) windowSample++;
		if (windowSample > windowSize)
		{
			recording = false;
			windowSample = 0;
		}
		//transform.position += acceleration;
		//Vector3 r = transform.rotation.eulerAngles;
		//r += angularVelocity;
		//Quaternion q = Quaternion.Euler(r);
		//transform.rotation = q;
	}
	/// <summary> 	
	/// Setup socket connection. 	
	/// </summary> 	
	private void ConnectToTcpServer()
	{
		try
		{
			clientReceiveThread = new Thread(new ThreadStart(ListenForData));
			clientReceiveThread.IsBackground = true;
			clientReceiveThread.Start();
			
		}
		catch (Exception e)
		{
			Debug.Log("On client connect exception " + e);
		}
	}

	private float Signif(float x, float f)
	{
		return Mathf.Round(x * Mathf.Pow(10f, f)) / Mathf.Pow(10f, f);
	}
	/// <summary> 	
	/// Runs in background clientReceiveThread; Listens for incomming data. 	
	/// </summary>    
	
	private void ListenForData()
	{
		try
		{
			socketConnection = new TcpClient(address, port);
			
			Byte[] bytes = new Byte[1024];
			while (true)
			{
				//string outString = "";
				// Get a stream object for reading 				
				using (NetworkStream stream = socketConnection.GetStream())
				{
					int length;
					// Read incomming stream into byte arrary. 					
					while ((length = stream.Read(bytes, 0, bytes.Length)) != 0)
					{
						connected = true;
						var incommingData = new byte[length];
						Array.Copy(bytes, 0, incommingData, 0, length);
						// Convert byte array to string message. 						
						string serverMessage = Encoding.ASCII.GetString(incommingData);
						//outString += serverMessage;
						char[] delims = { ',', '\n' };
						string[] s = serverMessage.Split(delims);
						//Debug.Log(outString);
						Vector3 v = Vector3.zero;
						
							try 
							{
								 acceleration = new Vector3(Signif(float.Parse(s[3]), 6f), 
								    Signif(float.Parse(s[1]), 6f),
									Signif(float.Parse(s[2]), 6f));
							
							angularVelocity = new Vector3(Signif(float.Parse(s[6]), 6f),
									Signif(float.Parse(s[4]), 6f),
									Signif(float.Parse(s[5]), 6f));// 
							if (recordForPlayback)
							{
								acceleration -= acceleration.normalized;
								angularVelocity *= Mathf.Deg2Rad;
							}

						} 
							catch(Exception)
							{

							}
						
						
						

						if (recording)
						{
							
							WriteToDisk(serverMessage, fileName + fileNo.ToString() + ".csv");
						}
					}
				}
			
			}
		}
		catch (SocketException socketException)
		{
			Debug.Log("Socket exception: " + socketException);
		}
	}
	/// <summary> 	
	/// Send message to server using socket connection. 	
	/// </summary> 	
	private void SendMessage()
	{
		if (socketConnection == null)
		{
			Debug.Log("No Connection");
			return;
		}
		try
		{
			
			// Get a stream object for writing. 			
			NetworkStream stream = socketConnection.GetStream();
			if (stream.CanWrite)
			{
				string clientMessage = "Requesting Data";
				// Convert string message to byte array.                 
				byte[] clientMessageAsByteArray = Encoding.ASCII.GetBytes(clientMessage);
				// Write byte array to socketConnection stream.                 
				stream.Write(clientMessageAsByteArray, 0, clientMessageAsByteArray.Length);
				//Debug.Log("Client sent his message - should be received by server");
				
			}
		}
		catch (SocketException socketException)
		{
			Debug.Log("Socket exception: " + socketException);
		}
	}
}
