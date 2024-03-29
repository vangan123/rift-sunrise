﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

using FrameWork;

namespace Common
{
    public enum EPacketFieldType
    {
        False = 0,
        True = 1,
        Unsigned7BitEncoded = 2,
        Signed7BitEncoded = 3,
        Raw4Bytes = 4,
        Raw8Bytes = 5,
        ByteArray = 6,
        Invalid = 7,
        Terminator = 8,
        Entry = 9,
        Packet = 10,
        List = 11,
        Dictionary = 12
    }

    public abstract class ISerializableFieldAttribute : Attribute
    {
        public int Index = 0;

        public ISerializableFieldAttribute(int Index)
        {
            this.Index = Index;
        }

        public abstract Type GetSerializableType();
    }

    [Serializable]
    public abstract class ISerializableField
    {
        public EPacketFieldType PacketType;

        public int Index;
        public object val;

        public abstract void Deserialize(ref PacketInStream Data);
        public abstract bool Serialize(ref PacketOutStream Data,bool Force);

        public abstract void ApplyToFieldInfo(FieldInfo Info,ISerializablePacket Packet, Type Field);

        public uint GetUint()
        {
            if (val is byte[])
            {
                byte[] Data = val as byte[];
                return Marshal.ConvertToUInt32(Data[3], Data[2], Data[1], Data[0]);
            }
            else if (val is uint)
                return (uint)val;

            return (uint)val;
        }

        public float GetFloat()
        {
            if (val is byte[])
            {
                byte[] Data = val as byte[];
                return Marshal.ConvertToFloat(Data[3], Data[2], Data[1], Data[0]);
            }
            else if (val is float)
                return (float)val;

            return 0;
        }

        public byte[] GetBytes()
        {
            if (val is byte[])
                return val as byte[];

            return new byte[0];
        }

        public ISerializablePacket GetPacket()
        {
            return val as ISerializablePacket;
        }

        public string GetString()
        {
            if (val is byte[])
                return Marshal.ConvertToString(val as byte[]);
            else if (val is string)
                return (string)val;

            return "";
        }

        public long GetLong()
        {
            if (val is long)
                return (long)val;
            else if (val is byte[])
                return BitConverter.ToInt64((byte[])val, 0);
            else 
                return 0;
        }
    }
}
