using System;
using System.Collections;
using UnityEngine;
using Unity.Robotics.ROSTCPConnector;
using RosJointInfo = RosMessageTypes.KinovaTest.KinovaMsgMsg;
namespace MyController
{
    public class RobotControl : MonoBehaviour
    {
        private ArticulationBody[] articulationChain;
        // Stores original colors of the part being highlighted
        public float stiffness;
        public float damping;
        [SerializeField] bool RosEnable = false;

        private int[] home = { 0, 15, 180, -130, 0, 55, 90 };
        private int[] currentPos = new int[7];
        void Start()
        {
            ROSConnection.GetOrCreateInstance().Subscribe<RosJointInfo>("jointInfo", PosChange);
            articulationChain = this.GetComponentsInChildren<ArticulationBody>();
            int defDyanmicVal = 10;
            foreach (ArticulationBody joint in articulationChain)
            {
                joint.gameObject.AddComponent<JointPosition>();
                joint.jointFriction = defDyanmicVal;
                joint.angularDamping = defDyanmicVal;
            }
        }

        void Update()
        {
            if (!RosEnable)
                StartCoroutine(DelayFunc(home));
        }

        public void UpdatePosition(JointPosition joint, float angle)
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
                JointPosition joint = articulationChain[i].GetComponent<JointPosition>();
                UpdatePosition(joint, angle[i - 1]);
                yield return wait;
            }
        }

        private void PosChange(RosJointInfo msg)
        {

            for (int i = 0; i < 7; i++)
            {
                JointPosition joint = articulationChain[i + 1].GetComponent<JointPosition>();
                joint.setSpeed(msg.jointVel[i]);
            }

        }
    }
}
