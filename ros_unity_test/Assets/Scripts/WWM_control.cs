using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Robotics.ROSTCPConnector;
using kinovaMsg = RosMessageTypes.KinovaTest.KinovaMsgMsg;

public class WWM_control : MonoBehaviour
{
    private ArticulationBody[] articulationChain;
    // Stores original colors of the part being highlighted
    public float stiffness;
    public float damping;
    [SerializeField] bool RosEnable = false;
    private float gripperCurrentPos;

    private float[] home = { 0f, 15f, 180f, -130f, 0f, 55f, 90f };
    private float[] prev_pos = { 0f, 15f, 180f, -130f, 0f, 55f, 90f };
    private float[] curr_pos = new float[7];

    private int[] countCircle = new int[7];

    void Start()
    {
        gripperCurrentPos = 0f;
        ROSConnection.GetOrCreateInstance().Subscribe<kinovaMsg>("kinovaInfo", kinovaInfoChange);
        articulationChain = this.GetComponentsInChildren<ArticulationBody>();
        float defDyanmicVal = 1.5f;
        for (int i = 0; i < articulationChain.Length; i++)
        {
            ArticulationBody joint = articulationChain[i].GetComponent<ArticulationBody>();
            joint.gameObject.AddComponent<JointChange>();
            joint.jointFriction = defDyanmicVal;
            joint.angularDamping = defDyanmicVal;
            if (i > 28 || i == 0)
                joint.useGravity = true;
            else
                joint.useGravity = false; // 不考慮重力影響，即假設有做好重力補償
        }
    }

    void Update()
    {
        if (!RosEnable)
            StartCoroutine(DelayFunc(home, gripperCurrentPos));
        else
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
        WaitForSeconds wait = new WaitForSeconds(0.0001f);
        for (int i = 2; i < 9; i++)
        {
            JointChange joint = articulationChain[i].GetComponent<JointChange>();
            UpdatePosition(joint, jointAngle[i - 2]);
            yield return wait;
        }
        // 加入夾爪
        for (int k = 0; k < 2; k++)
        {
            JointChange joint = articulationChain[11 + k * 5].GetComponent<JointChange>();
            UpdatePosition(joint, gripperAngle); // 正數 left/right inner knuckle
            joint = articulationChain[12 + k * 5].GetComponent<JointChange>();
            UpdatePosition(joint, gripperAngle); // 正數 left/right outer knuckle
            joint = articulationChain[14 + k * 5].GetComponent<JointChange>();
            UpdatePosition(joint, -gripperAngle); // 負數 left/right inner finger
            yield return wait;
        }
    }

    private void kinovaInfoChange(kinovaMsg msg)
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
    }
}
