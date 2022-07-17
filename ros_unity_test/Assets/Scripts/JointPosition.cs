using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Robotics.UrdfImporter;

public class JointPosition : MonoBehaviour
{
    MyController.RobotControl controller;
    public float speed;
    public float torque;
    public float acceleration;
    public ArticulationBody joint;

    private float newTargetDelta;
    void Start()
    {
        newTargetDelta = 0f;
        controller = (MyController.RobotControl)this.GetComponentInParent(typeof(MyController.RobotControl));
        joint = this.GetComponent<ArticulationBody>();
        controller.UpdatePosition(this, newTargetDelta);
        speed = controller.speed;
        torque = controller.torque;
        acceleration = controller.acceleration;
    }

    void FixedUpdate()
    {

        speed = controller.speed;
        torque = controller.torque;
        acceleration = controller.acceleration;


        if (joint.jointType != ArticulationJointType.FixedJoint)
        {

            ArticulationDrive currentDrive = joint.xDrive;

            if (joint.jointType == ArticulationJointType.RevoluteJoint)
            {
                if (joint.twistLock == ArticulationDofLock.LimitedMotion)
                {
                    if (newTargetDelta + currentDrive.target > currentDrive.upperLimit)
                    {
                        currentDrive.target = currentDrive.upperLimit;
                    }
                    else if (newTargetDelta + currentDrive.target < currentDrive.lowerLimit)
                    {
                        currentDrive.target = currentDrive.lowerLimit;
                    }
                    else
                    {
                        currentDrive.target += newTargetDelta;
                    }
                }
                else
                {
                    currentDrive.target += newTargetDelta;
                }
            }
            joint.xDrive = currentDrive;
        }
    }
}
