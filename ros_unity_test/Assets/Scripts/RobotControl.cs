using System;
using System.Collections;
using UnityEngine;
using Unity.Robotics.ROSTCPConnector;
using JointInfo = RosMessageTypes.KinovaTest.KinovaMsgMsg;
using GripperInfo = RosMessageTypes.KinovaTest.GripperMsgMsg;

public class RobotControl : MonoBehaviour
{
    private ArticulationBody[] articulationChain;
    // Stores original colors of the part being highlighted
    public float stiffness;
    public float damping;
    [SerializeField] bool RosEnable = false;
    private float gripperCurrentPos;

    private float[] home = { 0, 15, 180, -130, 0, 55, 90 };
    void Start()
    {
        gripperCurrentPos = 0f;
        ROSConnection.GetOrCreateInstance().Subscribe<JointInfo>("jointInfo", PosChange);
        ROSConnection.GetOrCreateInstance().Subscribe<GripperInfo>("gripperInfo", GripperChange);
        articulationChain = this.GetComponentsInChildren<ArticulationBody>();
        int defDyanmicVal = 10;
        foreach (ArticulationBody joint in articulationChain)
        {
            joint.gameObject.AddComponent<JointPosChange>();
            joint.jointFriction = defDyanmicVal;
            joint.angularDamping = defDyanmicVal;
            joint.useGravity = false; // 不考慮重力影響，即假設有做好重力補償
        }
    }

    void Update()
    {
        if (!RosEnable)
            StartCoroutine(DelayFunc(home));
    }

    public void UpdatePosition(JointPosChange joint, float angle)
    {
        ArticulationDrive drive = joint.joint.xDrive;
        drive.stiffness = stiffness;
        drive.damping = damping;
        drive.target = angle;
        joint.joint.xDrive = drive;
    }
    IEnumerator DelayFunc(float[] angle)
    {
        WaitForSeconds wait = new WaitForSeconds(0.001f);
        for (int i = 1; i < 8; i++)
        {
            JointPosChange joint = articulationChain[i].GetComponent<JointPosChange>();
            UpdatePosition(joint, angle[i - 1]);
            yield return wait;
        }
        // 加入夾爪
        for (int k = 0; k < 2; k++)
        {
            JointPosChange joint = articulationChain[10 + k * 5].GetComponent<JointPosChange>();
            UpdatePosition(joint, 0); // 正數 left/right inner knuckle
            joint = articulationChain[11 + k * 5].GetComponent<JointPosChange>();
            UpdatePosition(joint, 0); // 正數 left/right outer knuckle
            joint = articulationChain[13 + k * 5].GetComponent<JointPosChange>();
            UpdatePosition(joint, 0); // 負數 left/right inner finger
            yield return wait;
        }
    }

    private void PosChange(JointInfo msg)
    {
        for (int i = 0; i < 7; i++)
        {
            JointPosChange joint = articulationChain[i + 1].GetComponent<JointPosChange>();
            joint.setSpeed(msg.jointVel[i]);
        }
    }

    private void GripperChange(GripperInfo msg)
    {
        float speed = msg.gripperVel;
        if (gripperCurrentPos > msg.gripperPos)
            speed *= -1;
        for (int k = 0; k < 2; k += 1)
        {
            JointPosChange joint = articulationChain[10 + k * 5].GetComponent<JointPosChange>();
            joint.setSpeed(speed);
            joint = articulationChain[11 + k * 5].GetComponent<JointPosChange>();
            joint.setSpeed(speed);
            joint = articulationChain[13 + k * 5].GetComponent<JointPosChange>();
            joint.setSpeed(-speed);
        }
        gripperCurrentPos = msg.gripperPos;
    }
}
