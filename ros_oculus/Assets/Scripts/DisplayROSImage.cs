using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Robotics.ROSTCPConnector;
using RosImage = RosMessageTypes.Sensor.ImageMsg;

public class DisplayROSImage : MonoBehaviour
{
    ROSConnection ros;
    Texture2D textRos;
    private bool isMessageReceived = false;

    void Start()
    {
        ros = ROSConnection.GetOrCreateInstance();
        ros.Subscribe<RosImage>("/camera/color/image_raw", imageChange);
    }
    void Update()
    {
        if (isMessageReceived)
        {
            GetComponent<Renderer>().material.mainTexture = textRos;
            isMessageReceived = false;
        }
    }
    void imageChange(RosImage img)
    {
        Texture2D.Destroy(textRos);
        textRos = new Texture2D((int)img.width, (int)img.height, TextureFormat.RGB24, false);
        textRos.LoadRawTextureData(img.data);
        textRos.Apply();
        isMessageReceived = true;
    }
}
