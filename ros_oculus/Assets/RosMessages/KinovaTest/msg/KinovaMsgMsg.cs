//Do not edit! This file was generated by Unity-ROS MessageGeneration.
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using Unity.Robotics.ROSTCPConnector.MessageGeneration;

namespace RosMessageTypes.KinovaTest
{
    [Serializable]
    public class KinovaMsgMsg : Message
    {
        public const string k_RosMessageName = "kinova_test/kinovaMsg";
        public override string RosMessageName => k_RosMessageName;

        public double time;
        public float[] jointPos;
        public double[] kinova_X;
        public double[] kinova_Xd;
        public double[] kinova_axis;
        public double[] kinova_axisd;
        public float gripperPos;

        public KinovaMsgMsg()
        {
            this.time = 0.0;
            this.jointPos = new float[7];
            this.kinova_X = new double[3];
            this.kinova_Xd = new double[3];
            this.kinova_axis = new double[3];
            this.kinova_axisd = new double[3];
            this.gripperPos = 0.0f;
        }

        public KinovaMsgMsg(double time, float[] jointPos, double[] kinova_X, double[] kinova_Xd, double[] kinova_axis, double[] kinova_axisd, float gripperPos)
        {
            this.time = time;
            this.jointPos = jointPos;
            this.kinova_X = kinova_X;
            this.kinova_Xd = kinova_Xd;
            this.kinova_axis = kinova_axis;
            this.kinova_axisd = kinova_axisd;
            this.gripperPos = gripperPos;
        }

        public static KinovaMsgMsg Deserialize(MessageDeserializer deserializer) => new KinovaMsgMsg(deserializer);

        private KinovaMsgMsg(MessageDeserializer deserializer)
        {
            deserializer.Read(out this.time);
            deserializer.Read(out this.jointPos, sizeof(float), 7);
            deserializer.Read(out this.kinova_X, sizeof(double), 3);
            deserializer.Read(out this.kinova_Xd, sizeof(double), 3);
            deserializer.Read(out this.kinova_axis, sizeof(double), 3);
            deserializer.Read(out this.kinova_axisd, sizeof(double), 3);
            deserializer.Read(out this.gripperPos);
        }

        public override void SerializeTo(MessageSerializer serializer)
        {
            serializer.Write(this.time);
            serializer.Write(this.jointPos);
            serializer.Write(this.kinova_X);
            serializer.Write(this.kinova_Xd);
            serializer.Write(this.kinova_axis);
            serializer.Write(this.kinova_axisd);
            serializer.Write(this.gripperPos);
        }

        public override string ToString()
        {
            return "KinovaMsgMsg: " +
            "\ntime: " + time.ToString() +
            "\njointPos: " + System.String.Join(", ", jointPos.ToList()) +
            "\nkinova_X: " + System.String.Join(", ", kinova_X.ToList()) +
            "\nkinova_Xd: " + System.String.Join(", ", kinova_Xd.ToList()) +
            "\nkinova_axis: " + System.String.Join(", ", kinova_axis.ToList()) +
            "\nkinova_axisd: " + System.String.Join(", ", kinova_axisd.ToList()) +
            "\ngripperPos: " + gripperPos.ToString();
        }

#if UNITY_EDITOR
        [UnityEditor.InitializeOnLoadMethod]
#else
        [UnityEngine.RuntimeInitializeOnLoadMethod]
#endif
        public static void Register()
        {
            MessageRegistry.Register(k_RosMessageName, Deserialize);
        }
    }
}
