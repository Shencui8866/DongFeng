using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class FLMessageBase : MessageBase
{
    public string data;
    public FLMessageBase(string data)
    {
        this.data = data;
    }
    public override void Serialize(NetworkWriter writer)
    {
        base.Serialize(writer);
        writer.Write(data);
    }
    public override void Deserialize(NetworkReader reader)
    {
        base.Deserialize(reader);
        data = reader.ReadString();
    }
}
public class FLMessageSendToAllBase : MessageBase
{
    public short msgType;
    public string data;
    public FLMessageSendToAllBase(short msgType,  string data)
    {
        this.msgType = msgType;
        this.data = data;
    }
    public override void Serialize(NetworkWriter writer)
    {
        base.Serialize(writer);
        writer.Write(msgType);
        writer.Write(data);
    }
    public override void Deserialize(NetworkReader reader)
    {
        base.Deserialize(reader);
        msgType = reader.ReadInt16();
        data = reader.ReadString();
    }
}
