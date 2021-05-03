using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace Josing.Net
{
#if !UNITY_2019
    public class MessageToAll : MessageBase
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

    public class Message : MessageBase
    {

    }
#endif
}

