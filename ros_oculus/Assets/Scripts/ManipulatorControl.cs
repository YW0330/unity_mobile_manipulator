using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Robotics.ROSTCPConnector;
using RosKinovaMsg = RosMessageTypes.KinovaTest.KinovaMsgMsg;

public class ManipulatorControl : MonoBehaviour
{
    private ArticulationBody[] articulationChain;
    // Stores original colors of the part being highlighted
    public float stiffness;
    public float damping;
    bool isMessageReceived = false;
    private float gripperCurrentPos;
    private float[] home = { 0f, 15f, 180f, -130f, 0f, 55f, 90f };
    private float[] prev_pos = { 0f, 15f, 180f, -130f, 0f, 55f, 90f };
    private float[] curr_pos = new float[7];
    private int[] countCircle = new int[7];
    void Start()
    {
        gripperCurrentPos = 0f;
        ROSConnection.GetOrCreateInstance().Subscribe<RosKinovaMsg>("kinovaInfo", kinovaInfoChange);
        articulationChain = this.GetComponentsInChildren<ArticulationBody>();
        float defDyanmicVal = 2f;
        foreach (ArticulationBody joint in articulationChain)
        {
            joint.jointFriction = defDyanmicVal;
            joint.angularDamping = defDyanmicVal;
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

    public void UpdatePosition(ArticulationBody joint, float angle)
    {
        ArticulationDrive drive = joint.xDrive;
        drive.stiffness = stiffness;
        drive.damping = damping;
        drive.target = angle;
        joint.xDrive = drive;
    }
    IEnumerator DelayFunc(float[] jointAngle, float gripperAngle)
    {
        WaitForSeconds wait = new WaitForSeconds(0.0001f);
        for (int i = 1; i < 8; i++)
        {
            ArticulationBody joint = articulationChain[i].GetComponent<ArticulationBody>();
            UpdatePosition(joint, jointAngle[i - 1]);
            yield return wait;
        }
        // 加入夾爪
        for (int k = 0; k < 2; k++)
        {
            ArticulationBody joint = articulationChain[10 + k * 5].GetComponent<ArticulationBody>();
            UpdatePosition(joint, gripperAngle); // 正數 left/right inner knuckle
            joint = articulationChain[11 + k * 5].GetComponent<ArticulationBody>();
            UpdatePosition(joint, gripperAngle); // 正數 left/right outer knuckle
            joint = articulationChain[13 + k * 5].GetComponent<ArticulationBody>();
            UpdatePosition(joint, -gripperAngle); // 負數 left/right inner finger
            yield return wait;
        }
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
