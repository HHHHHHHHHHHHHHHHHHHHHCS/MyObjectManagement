using System;
using System.Collections;
using System.Collections.Generic;
using Debug = UnityEngine.Debug;

public enum ShapeBehaviorType
{
    Movement,
    Rotation,
    Oscillation,
    Satellite
}

public static class ShapeBehaviorTypeMethods
{
    public static ShapeBehavior GetInstance(this ShapeBehaviorType type)
    {
        switch (type)
        {
            case ShapeBehaviorType.Movement:
                return ShapeBehaviorPool<MovementShapeBehavior>.Get(); 
            case ShapeBehaviorType.Rotation:
                return ShapeBehaviorPool<RotationShapeBehavior>.Get();
            case ShapeBehaviorType.Oscillation:
                return ShapeBehaviorPool<OscillationShapeBehavior>.Get();
            case ShapeBehaviorType.Satellite:
                return ShapeBehaviorPool<SatelliteShapeBehavior>.Get();
        }

        Debug.Log("Forgot to support:"+type);
        return null;
    }
}