using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Robotics.ROSTCPConnector;
using RosFloat32Msg = RosMessageTypes.Std.Float32Msg;

public class GripperPositionPublisher : MonoBehaviour
{
    HandController handController;
    [SerializeField] float publishMsgFreq = 0.5f;
    private ROSConnection ros;
    private string topicname = "triggerVal";
    private float timeElapsed;

    void Start()
    {
        handController = GetComponent<HandController>();
        ros = ROSConnection.GetOrCreateInstance();
        ros.RegisterPublisher<RosFloat32Msg>(topicname);
    }

    void FixedUpdate()
    {
        timeElapsed += Time.deltaTime;
        if (timeElapsed > publishMsgFreq)
        {
            RosFloat32Msg msg = new RosFloat32Msg(handController.GetTriggerValue());
            ros.Publish(topicname, msg);
            timeElapsed = 0;
        }
    }
}
