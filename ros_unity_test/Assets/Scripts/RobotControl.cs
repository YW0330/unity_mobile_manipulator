using System;
using System.Collections;
using UnityEngine;
using Unity.Robotics.ROSTCPConnector;
using RosJointInfo = RosMessageTypes.KinovaTest.KinovaMsgMsg;
public class RobotControl : MonoBehaviour
{
    private ArticulationBody[] articulationChain;
    // Stores original colors of the part being highlighted
    public float stiffness;
    public float damping;
    [SerializeField] bool RosEnable = false;

    private int[] home = { 0, 15, 180, -130, 0, 55, 90 };
    void Start()
    {
        ROSConnection.GetOrCreateInstance().Subscribe<RosJointInfo>("jointInfo", PosChange);
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
    IEnumerator DelayFunc(int[] angle)
    {
        WaitForSeconds wait = new WaitForSeconds(0.001f);
        for (int i = 1; i < 8; i++)
        {
            JointPosChange joint = articulationChain[i].GetComponent<JointPosChange>();
            UpdatePosition(joint, angle[i - 1]);
            yield return wait;
        }
    }

    private void PosChange(RosJointInfo msg)
    {
        for (int i = 0; i < 7; i++)
        {
            JointPosChange joint = articulationChain[i + 1].GetComponent<JointPosChange>();
            joint.setSpeed(msg.jointVel[i]);
        }
    }
}
