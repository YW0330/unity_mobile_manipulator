using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Robotics.ROSTCPConnector;
using RosBoolMsg = RosMessageTypes.Std.BoolMsg;
public class ControlModePublisher : MonoBehaviour
{
    [SerializeField] HandController handController;

    [SerializeField] float publishMsgFreq = 0.5f;
    private ROSConnection ros;

    private string topicname = "controlMode";

    private float timeElapsed;

    void Start()
    {
        ros = ROSConnection.GetOrCreateInstance();
        ros.RegisterPublisher<RosBoolMsg>(topicname);
    }

    void FixedUpdate()
    {
        timeElapsed += Time.deltaTime;
        if (timeElapsed > publishMsgFreq)
        {
            RosBoolMsg msg = new RosBoolMsg((handController.GetPrimaryBtnCount() % 2) != 0);
            ros.Publish(topicname, msg);
            timeElapsed = 0;
        }
    }
}
