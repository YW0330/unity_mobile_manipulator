using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Robotics.ROSTCPConnector;
using RosKinovaMsg = RosMessageTypes.KinovaTest.KinovaMsgMsg;

public class WWM_control : MonoBehaviour
{
    private ArticulationBody[] articulationChain;
    public float stiffness;
    public float damping;
    bool isMessageReceived = false;
    private float gripperCurrentPos = 0f;
    private float[] home = { 0f, 15f, 180f, -130f, 0f, 55f, 90f };
    private float[] prev_pos = { 0f, 15f, 180f, -130f, 0f, 55f, 90f };
    private float[] curr_pos = new float[7];
    private int[] countCircle = new int[7];

    ROSConnection ros;
    void Start()
    {
        ros = ROSConnection.GetOrCreateInstance();
        ros.Subscribe<RosKinovaMsg>("kinovaInfo", kinovaInfoChange);
        articulationChain = this.GetComponentsInChildren<ArticulationBody>();
        float defDyanmicVal = 2f;
        for (int i = 0; i < articulationChain.Length; i++)
        {
            ArticulationBody joint = articulationChain[i].GetComponent<ArticulationBody>();
            joint.jointFriction = defDyanmicVal;
            joint.angularDamping = defDyanmicVal;
            if (i > 27)
                joint.useGravity = true;
            else
                joint.useGravity = false; // 不考慮重力影響，即假設有做好重力補償
        }
    }

    void Update()
    {
        if (!isMessageReceived)
            StartCoroutine(DelayFunc(home, gripperCurrentPos));
        else
            StartCoroutine(DelayFunc(curr_pos, gripperCurrentPos));

    }

    IEnumerator DelayFunc(float[] jointAngle, float gripperAngle)
    {
        WaitForSeconds wait = new WaitForSeconds(0.0001f);
        for (int i = 2; i < 9; i++)
        {
            ArticulationBody joint = articulationChain[i].GetComponent<ArticulationBody>();
            UpdateJointPosition(joint, jointAngle[i - 2]);
            yield return wait;
        }
        // 加入夾爪
        for (int k = 0; k < 2; k++)
        {
            ArticulationBody joint = articulationChain[11 + k * 5].GetComponent<ArticulationBody>();
            UpdateJointPosition(joint, gripperAngle); // 正數 left/right inner knuckle
            joint = articulationChain[12 + k * 5].GetComponent<ArticulationBody>();
            UpdateJointPosition(joint, gripperAngle); // 正數 left/right outer knuckle
            joint = articulationChain[14 + k * 5].GetComponent<ArticulationBody>();
            UpdateJointPosition(joint, -gripperAngle); // 負數 left/right inner finger
            yield return wait;
        }
    }
    private void UpdateJointPosition(ArticulationBody joint, float angle)
    {
        ArticulationDrive drive = joint.xDrive;
        drive.stiffness = stiffness;
        drive.damping = damping;
        drive.target = angle;
        joint.xDrive = drive;
    }

    private void kinovaInfoChange(RosKinovaMsg msg)
    {
        // joint
        for (int i = 0; i < 7; i++)
        {
            curr_pos[i] = msg.jointPos[i] + 360 * countCircle[i];
            if (curr_pos[i] - prev_pos[i] > 270)
            {
                curr_pos[i] -= 360;
                countCircle[i]--;
            }
            else if (curr_pos[i] - prev_pos[i] < -270)
            {
                curr_pos[i] += 360;
                countCircle[i]++;
            }
            prev_pos[i] = curr_pos[i];
        }
        // gripper
        gripperCurrentPos = msg.gripperPos;
        isMessageReceived = true;
    }
}
