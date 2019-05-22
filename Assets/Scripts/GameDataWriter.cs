using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class GameDataWriter
{
    private BinaryWriter writer;

    public GameDataWriter(BinaryWriter bw)
    {
        writer = bw;
    }

    public void Write(ShapeInstance value)
    {
        writer.Write(value.IsValid ? value.Shape.SaveIndex : -1);
    }

    public void Write(float value)
    {
        writer.Write(value);
    }

    public void Write(int value)
    {
        writer.Write(value);
    }

    public void Write(Vector3 value)
    {
        writer.Write(value.x);
        writer.Write(value.y);
        writer.Write(value.z);
    }

    public void Write(Quaternion value)
    {
        writer.Write(value.x);
        writer.Write(value.y);
        writer.Write(value.z);
        writer.Write(value.w);
    }

    public void Write(Color color)
    {
        writer.Write(color.r);
        writer.Write(color.g);
        writer.Write(color.b);
        writer.Write(color.a);
    }

    public void Write(Random.State value)
    {
        //因为Random.state 是四位浮点数,并且我们无法公共的访问
        //但是可以通过json 来实现
        //Debug.Log(JsonUtility.ToJson(value));
        writer.Write(JsonUtility.ToJson(value));
    }
}