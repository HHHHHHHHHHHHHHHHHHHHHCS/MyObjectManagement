using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class GrowingShapeBehavior : ShapeBehavior
{
    private Vector3 originalScale;
    private float duration,dyingAge;

    public override ShapeBehaviorType BehaviorType => ShapeBehaviorType.Growing;

    public void Initialize(Shape shape, float duration)
    {
        originalScale = shape.transform.localScale;
        this.duration = duration;
        //shape.transform.localScale = Vector3.zero;
        dyingAge = shape.Age;
    }

    public override bool GameUpdate(Shape shape)
    {
        float dyingDuration = shape.Age - dyingAge;
        if (dyingDuration < duration)
        {
            float s = 1f - shape.Age / duration;
            s = (3f - 2f * s) * s * s;
            shape.transform.localScale = s * originalScale;
            return true;
        }

        shape.transform.localScale = Vector3.zero;
        return true;
    }

    public override void Save(GameDataWriter writer)
    {
        writer.Write(originalScale);
        writer.Write(duration);
        writer.Write(dyingAge);
    }

    public override void Load(GameDataReader reader)
    {
        originalScale = reader.ReadVector3();
        duration = reader.ReadFloat();
        dyingAge = reader.ReadFloat();
    }

    public override void Recycle()
    {
        ShapeBehaviorPool<GrowingShapeBehavior>.Reclaim(this);
    }
}
