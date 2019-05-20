using System.Collections;
using System.Collections.Generic;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;

public class SatelliteShapeBehavior : ShapeBehavior
{
    private ShapeInstance focalShape;

    private float frequency;

    private Vector3 cosOffset, sinOffset;

    public override ShapeBehaviorType BehaviorType => ShapeBehaviorType.Satellite;

    public void Initialize(Shape shape, Shape focalShape, float radius, float frequency)
    {
        this.focalShape = focalShape;
        this.frequency = frequency;
        Vector3 orbitAxis = Random.onUnitSphere;
        do
        {
            cosOffset = Vector3.Cross(orbitAxis, Random.onUnitSphere).normalized;
        } while (cosOffset.sqrMagnitude < 0.1f);

        sinOffset = Vector3.Cross(cosOffset, orbitAxis);

        cosOffset *= radius;
        sinOffset *= radius;

        shape.AddBehavior<RotationShapeBehavior>().AngularVelocity =
            -360 * frequency * shape.transform.InverseTransformDirection(orbitAxis);
    }

    public override bool GameUpdate(Shape shape)
    {
        if (focalShape.IsValid)
        {
            float t = 2f * Mathf.PI * frequency * shape.Age;
            shape.transform.localPosition = focalShape.Shape.transform.localPosition
                                            + cosOffset * Mathf.Cos(t) + sinOffset * Mathf.Sin(t);
            return true;
        }

        return false;
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