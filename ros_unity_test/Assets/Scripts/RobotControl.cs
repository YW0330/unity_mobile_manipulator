using System;
using System.Collections;
using UnityEngine;
using Unity.Robotics.ROSTCPConnector;
using RosKinovaMsg = RosMessageTypes.KinovaTest.KinovaMsgMsg;

public class RobotControl : MonoBehaviour
{
    private ArticulationBody[] articulationChain;
    // Stores original colors of the part being highlighted
    public float stiffness;
    public float damping;
    [SerializeField] bool RosEnable = false;
    private float gripperCurrentPos;
    private float[] curr_pos = new float[7];
    void Start()
    {
        gripperCurrentPos = 0f;
        ROSConnection.GetOrCreateInstance().Subscribe<RosKinovaMsg>("kinovaInfo", kinovaInfoChange);
        articulationChain = this.GetComponentsInChildren<ArticulationBody>();
        float defDyanmicVal = 1.5f;
        foreach (ArticulationBody joint in articulationChain)
        {
            joint.gameObject.AddComponent<JointChange>();
            joint.jointFriction = defDyanmicVal;
            joint.angularDamping = defDyanmicVal;
            joint.useGravity = false; // 不考慮重力影響，即假設有做好重力補償
        }
    }

    void Update()
    {
        if (RosEnable)
            StartCoroutine(DelayFunc(curr_pos, gripperCurrentPos));
    }

    public void UpdatePosition(JointChange joint, float angle)
    {
        ArticulationDrive drive = joint.joint.xDrive;
        drive.stiffness = stiffness;
        drive.damping = damping;
        drive.target = angle;
        joint.joint.xDrive = drive;
    }
    IEnumerator DelayFunc(float[] jointAngle, float gripperAngle)
    {
        WaitForSeconds wait = new WaitForSeconds(0.01f);
        for (int i = 1; i < 8; i++)
        {
            JointChange joint = articulationChain[i].GetComponent<JointChange>();
            UpdatePosition(joint, jointAngle[i - 1]);
            yield return wait;
        }
        // 加入夾爪
        for (int k = 0; k < 2; k++)
        {
            JointChange joint = articulationChain[10 + k * 5].GetComponent<JointChange>();
            UpdatePosition(joint, gripperAngle); // 正數 left/right inner knuckle
            joint = articulationChain[11 + k * 5].GetComponent<JointChange>();
            UpdatePosition(joint, gripperAngle); // 正數 left/right outer knuckle
            joint = articulationChain[13 + k * 5].GetComponent<JointChange>();
            UpdatePosition(joint, -gripperAngle); // 負數 left/right inner finger
            yield return wait;
        }
    }

    private void kinovaInfoChange(RosKinovaMsg msg)
    {
        RosEnable = true;
        // joint
        for (int i = 0; i < 7; i++)
            curr_pos[i] = msg.jointPos[i];
        // gripper
        gripperCurrentPos = msg.gripperPos;
    }
}
