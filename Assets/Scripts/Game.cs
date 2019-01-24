using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class Game : PersistableObject
{
    public const int nowSaveVersion = 3;
    public const int version_1 = 1; //版本1储存的是shape的shapeId
    public const int version_2 = 2; //版本2储存的是shape的materialId
    public const int version_3 = 3; //版本3储存的是shape的颜色

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
            storage.Save(this, nowSaveVersion);
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
        instance.SetColor(Random.ColorHSV(
            hueMin: 0, hueMax: 1
            , saturationMin: 0.5f, saturationMax: 1
            , valueMin: 0.25f, valueMax: 1
            , alphaMin: 1, alphaMax: 1));
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
        writer.Write(shapes.Count);
        foreach (var item in shapes)
        {
            writer.Write(item.ShapeId);
            writer.Write(item.MaterialId);
            item.Save(writer);
        }
    }


    public override void Load(GameDataReader reader)
    {
        int version = reader.Version;
        if (version > nowSaveVersion)
        {
            //防止版本错误
            Debug.LogError("Unsupported future save version " + version);
            return;
        }

        //之前没有储存版本,现在加了,所以用负号,
        int count = version <= 0 ? -version : reader.ReadInt();
        for (int i = 0; i < count; i++)
        {
            int shapedId = version >= version_1 ? reader.ReadInt() : 0;
            int materialId = version >= version_2 ? reader.ReadInt() : 0;
            Shape instance = shapeFacotry.Get(shapedId, materialId);
            instance.Load(reader);
            shapes.Add(instance);
        }
    }
}