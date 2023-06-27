using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Robotics.ROSTCPConnector;
using RosTwistMsg = RosMessageTypes.Geometry.TwistMsg;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;

public class PlatformVelPublisher : MonoBehaviour
{
    HandController handController;
    [SerializeField] float publishMsgFreq = 0.5f;
    private ROSConnection ros;

    private string topicname = "cmd_vel";

    private float timeElapsed;

    private RosTwistMsg msg;

    void Start()
    {
        handController = GetComponent<HandController>();
        ros = ROSConnection.GetOrCreateInstance();
        ros.RegisterPublisher<RosTwistMsg>(topicname);
        msg = new RosTwistMsg();
    }
    void Update()
    {
        timeElapsed += Time.deltaTime;
        if ((handController.GetPrimaryBtnCount() % 2) != 0 && (timeElapsed > publishMsgFreq))
        {
            if (handController.GetGripBtnPressed())
            {
                Vector2 tmp = handController.GetPrimary2DAxisValue();
                SetTwist(tmp[1], tmp[0]);
            }
            else
                SetTwist(0, 0);
            ros.Publish(topicname, msg);
            timeElapsed = 0;
        }
    }

    private void SetTwist(float linear_x, float angular_z)
    {
        msg.linear.x = Mathf.Abs(linear_x) > 0.5 ? linear_x * 0.08f : 0;
        msg.linear.y = 0;
        msg.linear.z = 0;
        msg.angular.x = 0;
        msg.angular.y = 0;
        msg.angular.z = Mathf.Abs(angular_z) > 0.5 ? -(angular_z * 0.2f) : 0;
    }
}
