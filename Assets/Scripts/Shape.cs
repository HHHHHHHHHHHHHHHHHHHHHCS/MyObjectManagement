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
    }

    public override void Load(GameDataReader reader)
    {
        base.Load(reader);
        SetColor(reader.Version >= Game.version_3 ? reader.ReadColor() : Color.white);
    }
}