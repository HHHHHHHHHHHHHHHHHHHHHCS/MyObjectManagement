using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SatelliteShapeBehavior : ShapeBehavior
{
    public override ShapeBehaviorType BehaviorType => ShapeBehaviorType.Satellite;

    public void Initialize(Shape shape, Shape focalShape, float radius, float frequency)
    {

    }

    public override void GameUpdate(Shape shape)
    {
    }

    public override void Save(GameDataWriter writer)
    {
    }

    public override void Load(GameDataReader reader)
    {
    }

    public override void Recycle()
    {
    }
}
