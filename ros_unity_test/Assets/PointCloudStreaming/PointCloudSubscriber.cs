using System;
using RosPointCloud2 = RosMessageTypes.Sensor.PointCloud2Msg;
using Unity.Robotics.ROSTCPConnector;
using UnityEngine;


namespace CustomerVision
{
    public class PointCloudSubscriber : MonoBehaviour
    {
        private byte[] byteArray;
        private bool isMessageReceived = false;
        bool readyToProcessMessage = true;
        private int size;
        private ROSConnection ros;
        private Vector3[] pcl;
        private Color[] pcl_color;

        int width;
        int height;
        int row_step;
        int point_step;

        void Start()
        {
            ros = ROSConnection.GetOrCreateInstance();
            ros.Subscribe<RosPointCloud2>("/camera/depth_registered/points", ReceiveMessage);
        }

        public void Update()
        {

            if (isMessageReceived)
            {
                PointCloudRendering();
                isMessageReceived = false;
            }


        }

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

        //点群の座標を変換
        void PointCloudRendering()
        {
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

                pcl[n] = new Vector3(x, z, y);
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
