﻿using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class GameDataReader 
{
    private BinaryReader reader;

    public GameDataReader(BinaryReader br)
    {
        reader = br;
    }

    public float ReadFloat()
    {
        return reader.ReadSingle();
    }

    public float ReadInt()
    {
        return reader.ReadInt32();
    }

    public Quaternion ReadQuaternion()
    {
        Quaternion value;
        value.x = reader.ReadSingle();
        value.y = reader.ReadSingle();
        value.z = reader.ReadSingle();
        value.w = reader.ReadSingle();
        return value;
    }

    public Vector3 ReadVector3()
    {
        Vector3 value;
        value.x = reader.ReadSingle();
        value.y = reader.ReadSingle();
        value.z = reader.ReadSingle();
        return value;
    }
}
