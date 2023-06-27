using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Robotics.ROSTCPConnector;
using RosBoolMsg = RosMessageTypes.Std.BoolMsg;
public class EmergencyStopPublisher : MonoBehaviour
{
    HandController handController;
    [SerializeField] float publishMsgFreq = 0.5f;
    private ROSConnection ros;
    private string topicname = "stop";
    private float timeElapsed;
    void Start()
    {
        handController = GetComponent<HandController>();
        ros = ROSConnection.GetOrCreateInstance();
        ros.RegisterPublisher<RosBoolMsg>(topicname);
    }

    void Update()
    {
        timeElapsed += Time.deltaTime;
        if (timeElapsed > publishMsgFreq)
        {
            RosBoolMsg msg = new RosBoolMsg(handController.GetSecondaryBtnIsPressed());
            ros.Publish(topicname, msg);
            timeElapsed = 0;
        }
    }
}
