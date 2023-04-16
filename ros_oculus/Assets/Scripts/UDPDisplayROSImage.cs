using UnityEngine;
using System.Collections.Generic;
using System;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Linq;

public class UDPDisplayROSImage : MonoBehaviour
{
    [SerializeField] int port = 9090;
    Thread receiveThread;
    UdpClient client;
    Texture2D texture2D;
    bool isMessageReceived = false;
    int width;
    int height;
    int step;
    int data_step;

    IEnumerable<byte> imgData = Enumerable.Empty<byte>();
    void Start()
    {
        InitUDP();
    }
    void Update()
    {
        if (isMessageReceived)
        {
            ImageChange();
            isMessageReceived = false;
        }
    }

    private void InitUDP()
    {
        receiveThread = new Thread(new ThreadStart(ReceiveMessage));
        receiveThread.IsBackground = true;
        receiveThread.Start();
    }

    private void ReceiveMessage()
    {
        client = new UdpClient(port);
        while (true)
        {
            try
            {
                IPEndPoint anyIP = new IPEndPoint(IPAddress.Any, port);
                int[] arr = Array.ConvertAll(Encoding.UTF8.GetString(client.Receive(ref anyIP)).Split(' '), int.Parse);
                width = arr[0];
                height = arr[1];
                step = arr[2];
                data_step = arr[3];
                imgData = Enumerable.Empty<byte>();
                for (int i = 0; i < height; i += data_step)
                    imgData = imgData.Concat(client.Receive(ref anyIP));
                isMessageReceived = true;
            }
            catch
            {
                continue;
            }
        }
    }

    private void ImageChange()
    {
        byte[] data = imgData.ToArray();
        if (data.Length != step * height)
            return;
        Texture2D.Destroy(texture2D);
        texture2D = new Texture2D(width, height, TextureFormat.RGB24, false);
        texture2D.LoadRawTextureData(data);
        texture2D.Apply();
        GetComponent<Renderer>().material.mainTexture = texture2D;
    }

    private void OnDisable()
    {
        // Unity 在離開當前場景後會自動呼叫這個函數
        receiveThread.Join();
        receiveThread.Abort();// 強制中斷當前執行緒
        client.Close();
    }

    private void OnApplicationQuit()
    {
        // 當應用程式結束時會自動呼叫這個函數
        if (receiveThread.IsAlive)
            receiveThread.Abort();
    }
}
