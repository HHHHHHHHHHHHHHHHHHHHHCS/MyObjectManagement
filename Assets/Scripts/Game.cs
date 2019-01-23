using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class Game : PersistableObject
{
    private const int saveVersion = 1;
    private const int version_1 = 1; //版本1储存的是shapeId


    public ShapeFactory shapeFacotry;
    public PersistenStorage storage;

    public KeyCode createKey = KeyCode.C;
    public KeyCode newGameKey = KeyCode.N;
    public KeyCode saveKey = KeyCode.S;
    public KeyCode loadKey = KeyCode.L;

    private List<Shape> shapes;

    private void Awake()
    {
        shapes = new List<Shape>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(createKey))
        {
            CreateShape();
        }
        else if (Input.GetKeyDown(newGameKey))
        {
            BeginNewGame();
        }
        else if (Input.GetKeyDown(saveKey))
        {
            storage.Save(this);
        }
        else if (Input.GetKeyDown(loadKey))
        {
            BeginNewGame();
            storage.Load(this);
        }
    }


    private void CreateShape()
    {
        Shape instance = shapeFacotry.GetRandom();
        Transform t = instance.transform;
        t.localPosition = Random.insideUnitSphere * 5f;
        t.localRotation = Random.rotation;
        t.localScale = Vector3.one * Random.Range(0.1f, 1f);
        shapes.Add(instance);
    }

    private void BeginNewGame()
    {
        foreach (var obj in shapes)
        {
            Destroy(obj.gameObject);
        }

        shapes.Clear();
    }


    public override void Save(GameDataWriter writer)
    {
        writer.Write(-saveVersion); //之前没有储存版本,新加储存版本用符号防止意外
        writer.Write(shapes.Count);
        foreach (var item in shapes)
        {
            writer.Write(item.ShapeId);
            item.Save(writer);
        }
    }


    public override void Load(GameDataReader reader)
    {
        int version = -reader.ReadInt();
        if (version > saveVersion)
        {
            //防止版本错误
            Debug.LogError("Unsupported future save version " + version);
            return;
        }

        //之前没有储存版本,现在加了,所以用负号,
        int count = version <= 0 ? -version : reader.ReadInt();
        for (int i = 0; i < count; i++)
        {
            int shapedId = 0;
            if (version >= version_1)
            {
                shapedId = reader.ReadInt();
            }

            Shape instance = shapeFacotry.Get(shapedId);
            instance.Load(reader);
            shapes.Add(instance);
        }
    }
}