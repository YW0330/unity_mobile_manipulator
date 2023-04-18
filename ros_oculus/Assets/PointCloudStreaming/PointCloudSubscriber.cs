﻿using System;
using UnityEngine;
#if UDP
using System.Collections.Generic;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Linq;
#else
using Unity.Robotics.ROSTCPConnector;
using RosPointCloud2 = RosMessageTypes.Sensor.PointCloud2Msg;
#endif

namespace CustomerVision
{
    public class PointCloudSubscriber : MonoBehaviour
    {
        private byte[] byteArray;
        private int size;
        private Vector3[] pcl;
        private Color[] pcl_color;
        int width;
        int height;
        int row_step;
        int point_step;
#if UDP
        [SerializeField] int port = 9085;
        Thread receiveThread;
        UdpClient client;
        int data_step;
        IEnumerable<byte> pointData = Enumerable.Empty<byte>();
#else
        private ROSConnection ros;
#endif
        private bool isMessageReceived = false;

        void Start()
        {
#if UDP
            receiveThread = new Thread(new ThreadStart(ReceiveMessage));
            receiveThread.IsBackground = true;
            receiveThread.Start();
#else
            ros = ROSConnection.GetOrCreateInstance();
            ros.Subscribe<RosPointCloud2>("/camera/depth_registered/points", ReceiveMessage);
#endif
        }

        public void Update()
        {
            if (isMessageReceived)
            {
                PointCloudRendering();
                isMessageReceived = false;
            }
        }


#if UDP
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
                    row_step = arr[2];
                    point_step = arr[3];
                    data_step = arr[4];
                    pointData = Enumerable.Empty<byte>();
                    for (int i = 0; i < height; i += data_step)
                        pointData = pointData.Concat(client.Receive(ref anyIP));
                    isMessageReceived = true;
                }
                catch
                {
                    continue;
                }
            }
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
#else
        void ReceiveMessage(RosPointCloud2 message)
        {
            size = message.data.GetLength(0);

            byteArray = new byte[size];
            byteArray = message.data;


            width = (int)message.width;
            height = (int)message.height;
            row_step = (int)message.row_step;
            point_step = (int)message.point_step;

            size = size / point_step;
            isMessageReceived = true;
        }
#endif

        //点群の座標を変換
        void PointCloudRendering()
        {
#if UDP
            byteArray = pointData.ToArray();
            if (byteArray.Length != row_step * height)
                return;
            size = byteArray.Length / point_step;
#endif
            pcl = new Vector3[size];
            pcl_color = new Color[size];

            int x_posi;
            int y_posi;
            int z_posi;

            float x;
            float y;
            float z;

            int rgb_posi;
            int rgb_max = 255;

            float r;
            float g;
            float b;

            //この部分でbyte型をfloatに変換         
            for (int n = 0; n < size; n++)
            {
                x_posi = n * point_step + 0;
                y_posi = n * point_step + 4;
                z_posi = n * point_step + 8;

                x = BitConverter.ToSingle(byteArray, x_posi);
                y = BitConverter.ToSingle(byteArray, y_posi);
                z = BitConverter.ToSingle(byteArray, z_posi);


                rgb_posi = n * point_step + 16;

                b = byteArray[rgb_posi + 0];
                g = byteArray[rgb_posi + 1];
                r = byteArray[rgb_posi + 2];

                r = r / rgb_max;
                g = g / rgb_max;
                b = b / rgb_max;

                pcl[n] = new Vector3(-y, z, x);
                pcl_color[n] = new Color(r, g, b);
            }
        }

        public Vector3[] GetPCL()
        {
            return pcl;
        }

        public Color[] GetPCLColor()
        {
            return pcl_color;
        }
    }
}
