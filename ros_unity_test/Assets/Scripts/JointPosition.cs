using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Robotics.UrdfImporter;

public class JointPosition : MonoBehaviour
{
    MyController.RobotControl controller;
    public float speed;
    public ArticulationBody joint;

    void Start()
    {
        speed = 0f;
        controller = (MyController.RobotControl)this.GetComponentInParent(typeof(MyController.RobotControl));
        joint = this.GetComponent<ArticulationBody>();
    }

    void FixedUpdate()
    {
        if (joint.jointType != ArticulationJointType.FixedJoint)
        {

            ArticulationDrive currentDrive = joint.xDrive;
            float newTargetDelta = Time.fixedDeltaTime * speed;

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
    public void setSpeed(float jointSpeed)
    {
        speed = jointSpeed;
    }
}
