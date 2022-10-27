using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using RosMessageTypes.Nav;
using Unity.Robotics.ROSTCPConnector;
using Unity.Robotics.ROSTCPConnector.ROSGeometry;

public class BuildWallFromMap : MonoBehaviour
{
    int width;
    int height;
    List<sbyte> data;
    void Start()
    {
        ROSConnection ros = ROSConnection.GetOrCreateInstance();
        ros.Subscribe<OccupancyGridMsg>("map", mapChange);
    }

    void Update()
    {
    }
    void mapChange(OccupancyGridMsg msg)
    {
        width = (int)msg.info.width;
        height = (int)msg.info.height;
        data = msg.data.ToList();
        var origin = msg.info.origin.position.From<FLU>();
        var rotation = msg.info.origin.orientation.From<FLU>();
        rotation.eulerAngles += new Vector3(0, -90, 0); // TODO: Account for differing texture origin
        var scale = msg.info.resolution;

        Vector3 drawOrigin = origin - rotation * new Vector3(scale * 0.5f, 0, scale * 0.5f);
        // Vector3 drawOrigin = origin - rotation * new Vector3(scale, 0, scale);
        drawMap(origin, rotation, scale);
    }

    void drawMap(Vector3 pose, Quaternion rotation, float scale)
    {
        GameObject parents = new GameObject("Wall");
        for (int i = 0; i < height; i++)
        {
            for (int j = 0; j < width; j++)
            {
                if (data[i * width + j] == 100)
                {
                    Vector3 point = new Vector3(j, 0, i);
                    GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    cube.transform.position = point * scale;
                    cube.transform.localScale = new Vector3(0.05f, 0.05f, 0.05f);
                    cube.transform.parent = parents.transform;
                }
            }
        }
        parents.transform.position = pose;
        parents.transform.rotation = rotation;
    }
}