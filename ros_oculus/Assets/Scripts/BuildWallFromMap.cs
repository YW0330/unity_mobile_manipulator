using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using RosMessageTypes.Nav;
using Unity.Robotics.ROSTCPConnector;
using Unity.Robotics.ROSTCPConnector.ROSGeometry;

public class Map
{
    public int width;
    public int height;
    public List<sbyte> data;
    public Vector3 origin;
    public Quaternion rotation;
    public float scale;
}

public class BuildWallFromMap : MonoBehaviour
{
    Map map;
    GameObject parents;
    ROSConnection ros;
    bool isMessageReceived = false;
    void Start()
    {
        ros = ROSConnection.GetOrCreateInstance();
        ros.Subscribe<OccupancyGridMsg>("map", mapChange);
        map = new Map();
    }

    void Update()
    {
        if (isMessageReceived)
        {
            drawMap();
            isMessageReceived = false;
        }
    }
    void mapChange(OccupancyGridMsg msg)
    {
        map.width = (int)msg.info.width;
        map.height = (int)msg.info.height;
        map.data = msg.data.ToList();
        map.origin = msg.info.origin.position.From<FLU>();
        map.rotation = msg.info.origin.orientation.From<FLU>();
        map.rotation.eulerAngles += new Vector3(0, -90, 0); // TODO: Account for differing texture origin
        map.scale = msg.info.resolution;
        isMessageReceived = true;
    }

    void drawMap()
    {
        Destroy(parents);
        parents = new GameObject("Wall");
        for (int i = 0; i < map.height; i++)
        {
            for (int j = 0; j < map.width; j++)
            {
                if (map.data[i * map.width + j] == 100)
                {
                    Vector3 point = new Vector3(j, 0, i);
                    GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    cube.GetComponent<MeshRenderer>().material = Resources.Load("Materials/Wall", typeof(Material)) as Material;
                    cube.transform.position = point * map.scale;
                    cube.transform.localScale = new Vector3(0.05f, 0.6f, 0.05f);
                    cube.transform.parent = parents.transform;
                }
            }
        }
        parents.transform.position = map.origin;
        parents.transform.rotation = map.rotation;
    }
}