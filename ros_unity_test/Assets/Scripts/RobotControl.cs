using System;
using System.Collections;
using Unity.Robotics;
using UnityEngine;

namespace MyController
{
    public class RobotControl : MonoBehaviour
    {
        private ArticulationBody[] articulationChain;
        // Stores original colors of the part being highlighted
        public float stiffness;
        public float damping;
        public float speed = 5f; // Units: degree/s
        public float torque = 100f; // Units: Nm or N
        public float acceleration = 5f;// Units: m/s^2 / degree/s^2

        private float[] home = { 0f, 15f, 180f, -130f, 0f, 55f, 90f };
        void Start()
        {
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
            StartCoroutine(DelayFunc());
        }

        public void UpdatePosition(JointPosition joint, float angle)
        {
            ArticulationDrive drive = joint.joint.xDrive;
            drive.stiffness = stiffness;
            drive.damping = damping;
            drive.target = angle;
            joint.joint.xDrive = drive;
        }
        IEnumerator DelayFunc()
        {
            WaitForSeconds wait = new WaitForSeconds(1);
            for (int i = 1; i < 8; i++)
            {
                JointPosition joint = articulationChain[i].GetComponent<JointPosition>();
                UpdatePosition(joint, home[i - 1]);
                yield return wait;
            }
        }
    }
}
