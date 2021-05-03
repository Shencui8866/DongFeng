using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace Josing.Net.UNetWork
{
    #if !UNITY_2019
    public class MessageToAllCmdString : UnityEngine.Networking.MessageBase 
    {
        short targetId;
        string cmd;

        public MessageToAllCmdString(short type, string cmd)
        {
            targetId = type;
            this.cmd = cmd;
        }

        public override void Deserialize(NetworkReader reader)
        {
            base.Deserialize(reader);
            targetId = reader.ReadInt16();
            cmd = reader.ReadString();
        }

        public override void Serialize(NetworkWriter writer)
        {
            base.Serialize(writer);
            writer.Write(targetId);
            writer.Write(cmd);
        }
    }

    public class MessageToAll : UnityEngine.Networking.MessageBase
    {
        short targetId;

        public MessageToAll(short type)
        {
            targetId = type;
        }

        public override void Deserialize(NetworkReader reader)
        {
            base.Deserialize(reader);
            targetId = reader.ReadInt16();
        }

        public override void Serialize(NetworkWriter writer)
        {
            base.Serialize(writer);
            writer.Write(targetId);
        }
    }

    public class Message : UnityEngine.Networking.MessageBase
    {

    }

    public class MessageTest : UnityEngine.Networking.MessageBase
    {
        public int testNum;
        public char testChar;

        public MessageTest()
        {
        }

        public MessageTest(char c, int i)
        {
            testNum = i;
            testChar = c;
        }

        public override void Deserialize(NetworkReader reader)
        {
            base.Deserialize(reader);
            Debug.Log(reader.Position);
            Debug.Log(reader.ReadInt16());
            Debug.Log(reader.ReadChar());
            reader.SeekZero();
            Debug.Log(reader.Position);
            Debug.Log(reader.ReadChar());
            Debug.Log(reader.ReadInt16());
            Debug.Log(reader.Position);
        }

        public override void Serialize(NetworkWriter writer)
        {
            base.Serialize(writer);
            writer.Write(testNum);
            writer.Write(testChar);
        }
    }
#endif
}

