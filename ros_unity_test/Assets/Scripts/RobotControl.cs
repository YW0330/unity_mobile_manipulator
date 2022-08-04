using System;
using System.Collections;
using UnityEngine;
using Unity.Robotics.ROSTCPConnector;
using kinovaMsg = RosMessageTypes.KinovaTest.KinovaMsgMsg;

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
        ROSConnection.GetOrCreateInstance().Subscribe<kinovaMsg>("kinovaInfo", kinovaInfoChange);
        articulationChain = this.GetComponentsInChildren<ArticulationBody>();
        int defDyanmicVal = 10;
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
        if (!RosEnable)
            StartCoroutine(DelayFunc(home));
    }

    public void UpdatePosition(JointChange joint, float angle)
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
            JointChange joint = articulationChain[i].GetComponent<JointChange>();
            UpdatePosition(joint, angle[i - 1]);
            yield return wait;
        }
        // 加入夾爪
        for (int k = 0; k < 2; k++)
        {
            JointChange joint = articulationChain[10 + k * 5].GetComponent<JointChange>();
            UpdatePosition(joint, 0); // 正數 left/right inner knuckle
            joint = articulationChain[11 + k * 5].GetComponent<JointChange>();
            UpdatePosition(joint, 0); // 正數 left/right outer knuckle
            joint = articulationChain[13 + k * 5].GetComponent<JointChange>();
            UpdatePosition(joint, 0); // 負數 left/right inner finger
            yield return wait;
        }
    }

    private void kinovaInfoChange(kinovaMsg msg)
    {
        // joint
        for (int i = 0; i < 7; i++)
        {
            JointChange joint = articulationChain[i + 1].GetComponent<JointChange>();
            joint.setSpeed(msg.jointVel[i]);
        }
        // gripper
        float speed = msg.gripperVel;
        if (gripperCurrentPos > msg.gripperPos)
            speed *= -1;
        for (int k = 0; k < 2; k += 1)
        {
            JointChange joint = articulationChain[10 + k * 5].GetComponent<JointChange>();
            joint.setSpeed(speed);
            joint = articulationChain[11 + k * 5].GetComponent<JointChange>();
            joint.setSpeed(speed);
            joint = articulationChain[13 + k * 5].GetComponent<JointChange>();
            joint.setSpeed(-speed);
        }
        gripperCurrentPos = msg.gripperPos;
    }
}
