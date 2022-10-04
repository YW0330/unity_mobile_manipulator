using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Robotics.ROSTCPConnector;
using RosOdom = RosMessageTypes.Nav.OdometryMsg;

public class PlatformMoving : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject platform;

    private Vector3 position = Vector3.zero;
    private Quaternion rotation = Quaternion.identity;
    void Start()
    {
        ROSConnection.GetOrCreateInstance().Subscribe<RosOdom>("odom", odomChange);

    }

    // Update is called once per frame
    void Update()
    {
        platform.transform.position = position;
        platform.transform.rotation = rotation;
    }

    void odomChange(RosOdom odomMsg)
    {
        var pos = getPosition(odomMsg);
        position = new Vector3(-pos.y, 0, pos.x);
        var rot = getRotation(odomMsg);
        rotation = new Quaternion(0, -rot.z, 0, rot.w);
    }

    Vector3 getPosition(RosOdom odomMsg)
    {
        return new Vector3((float)odomMsg.pose.pose.position.x, (float)odomMsg.pose.pose.position.y, (float)odomMsg.pose.pose.position.z);
    }

    Quaternion getRotation(RosOdom odomMsg)
    {
        return new Quaternion(
            (float)odomMsg.pose.pose.orientation.x,
            (float)odomMsg.pose.pose.orientation.y,
            (float)odomMsg.pose.pose.orientation.z,
            (float)odomMsg.pose.pose.orientation.w
        );
    }


}
