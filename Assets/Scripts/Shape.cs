using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shape : PersistableObject
{
    private static readonly int colorPropertyId = Shader.PropertyToID("_Color");
    private static MaterialPropertyBlock sharedPropertyBlock;

    [SerializeField] private MeshRenderer[] meshRenderers;

    private int shapeId = int.MinValue;

    public int MaterialId { get; private set; }
    //public Vector3 AngularVelocity { get; set; }
    //public Vector3 Velocity { get; set; }


    private Color[] colors;
    private ShapeFactory originFactory;
    private List<ShapeBehavior> behaviorList = new List<ShapeBehavior>();

    public ShapeFactory OriginFactory
    {
        get { return originFactory; }
        set
        {
            if (originFactory == null)
            {
                originFactory = value;
            }
            else
            {
                Debug.LogError("Not allowed to change origin factory.");
            }
        }
    }

    public int ShapeId
    {
        get => shapeId;
        set
        {
            if (shapeId == int.MinValue && value != int.MinValue)
            {
                shapeId = value;
            }
            else
            {
                Debug.LogError("Not allowed to change shapeId.");
            }
        }
    }

    public int ColorCount
    {
        get { return colors.Length; }
    }

    private void Awake()
    {
        colors = new Color[meshRenderers.Length];
    }

    public void GameUpdate()
    {
        foreach (var item in behaviorList)
        {
            item.GameUpdate(this);
        }
    }

    public void SetMaterial(Material material, int materialId)
    {
        foreach (var mr in meshRenderers)
        {
            mr.material = material;
        }

        MaterialId = materialId;
    }

    public void SetColor(Color color)
    {
        if (sharedPropertyBlock == null)
        {
            sharedPropertyBlock = new MaterialPropertyBlock();
        }

        sharedPropertyBlock.SetColor(colorPropertyId, color);
        for (int i = 0; i < meshRenderers.Length; i++)
        {
            colors[i] = color;
            meshRenderers[i].SetPropertyBlock(sharedPropertyBlock);
        }
    }

    public void SetColor(int index, Color color)
    {
        if (sharedPropertyBlock == null)
        {
            sharedPropertyBlock = new MaterialPropertyBlock();
        }

        sharedPropertyBlock.SetColor(colorPropertyId, color);
        colors[index] = color;
        meshRenderers[index].SetPropertyBlock(sharedPropertyBlock);
    }

    private void LoadColors(GameDataReader reader)
    {
        int count = reader.ReadInt();
        int min = Mathf.Min(count, colors.Length);
        ;
        int i = 0;
        for (; i < min; i++)
        {
            SetColor(i, reader.ReadColor());
        }

        if (count > min)
        {
            for (; i < count; i++)
            {
                reader.ReadColor();
            }
        }
        else if (count < min)
        {
            for (; i < min; i++)
            {
                SetColor(i, Color.white);
            }
        }
    }

    public void Recycle()
    {
        foreach (var item in behaviorList)
        {
            Destroy(item);
        }

        behaviorList.Clear();
        OriginFactory.Reclaim(this);
    }

    public override void Save(GameDataWriter writer)
    {
        base.Save(writer);
        foreach (var color in colors)
        {
            writer.Write(color);
        }

        writer.Write(behaviorList.Count);

        foreach (var item in behaviorList)
        {
            writer.Write((int) item.BehaviorType);
            item.Save(writer);
        }
    }

    public override void Load(GameDataReader reader)
    {
        base.Load(reader);
        if (reader.Version >= Game.version_8)
        {
            LoadColors(reader);
        }
        else
        {
            SetColor(reader.Version >= Game.version_3 ? reader.ReadColor() : Color.white);
        }

        //AngularVelocity = reader.Version >= Game.version_7 ? reader.ReadVector3() : Vector3.zero;
        //Velocity = reader.Version >= 7 ? reader.ReadVector3() : Vector3.zero;
        if (reader.Version >= Game.version_10)
        {
            int behaviorCount = reader.ReadInt();
            for (int i = 0; i < behaviorCount; i++)
            {
                AddBehavior((ShapeBehavior.ShapeBehaviorType) reader.ReadInt()).Load(reader);
            }
        }
        else if (reader.Version >= Game.version_7)
        {
            AddBehavior<RotationShapeBehavior>().AngularVelocity = reader.ReadVector3();
            AddBehavior<MovementShapeBehavior>().Velocity = reader.ReadVector3();
        }
    }

    public void AddBehavior(ShapeBehavior behavior)
    {
        behaviorList.Add(behavior);
    }

    public T AddBehavior<T>() where T : ShapeBehavior
    {
        T behavior = gameObject.AddComponent<T>();
        behaviorList.Add(behavior);
        return behavior;
    }

    private ShapeBehavior AddBehavior(ShapeBehavior.ShapeBehaviorType type)
    {
        switch (type)
        {
            case ShapeBehavior.ShapeBehaviorType.Movement:
                return AddBehavior<MovementShapeBehavior>();
            case ShapeBehavior.ShapeBehaviorType.Rotation:
                return AddBehavior<RotationShapeBehavior>();
        }

        Debug.LogError("Forgot to support" + type);
        return null;
    }
}