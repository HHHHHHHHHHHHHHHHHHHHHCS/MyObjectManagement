using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shape : PersistableObject
{
    private static readonly int colorPropertyId = Shader.PropertyToID("_Color");
    private static  MaterialPropertyBlock shaderPropertyBlock;


    private MeshRenderer meshRenderer;
    private int shapeId = int.MinValue;

    public int MaterialId { get; private set; }
    public Vector3 AngularVelocity { get; set; }
    public Vector3 Velocity { get; set; }


    private Color color;

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

    private void Awake()
    {
        meshRenderer = GetComponent<MeshRenderer>();
    }

    public void GameUpdate()
    {
        transform.Rotate(AngularVelocity * Time.fixedDeltaTime);
        transform.localPosition += Velocity * Time.fixedDeltaTime;
    }

    public void SetMaterial(Material material, int materialId)
    {
        meshRenderer.material = material;
        MaterialId = materialId;
    }

    public void SetColor(Color col)
    {
        color = col;
        if (shaderPropertyBlock == null)
        {
            shaderPropertyBlock = new MaterialPropertyBlock();
        }
        shaderPropertyBlock.SetColor(colorPropertyId, color);
        meshRenderer.SetPropertyBlock(shaderPropertyBlock);
    }

    public override void Save(GameDataWriter writer)
    {
        base.Save(writer);
        writer.Write(color);
        writer.Write(AngularVelocity);
        writer.Write(Velocity);
    }

    public override void Load(GameDataReader reader)
    {
        base.Load(reader);
        SetColor(reader.Version >= Game.version_3 ? reader.ReadColor() : Color.white);
        AngularVelocity = reader.Version >= Game.version_7 ? reader.ReadVector3() : Vector3.zero;
        Velocity = reader.Version >= 4 ? reader.ReadVector3() : Vector3.zero;
    }
}