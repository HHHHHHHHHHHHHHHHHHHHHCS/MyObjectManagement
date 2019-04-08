using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public abstract class SpawnZone : PersistableObject
{
    [System.Serializable]
    public struct SpawnConfiguration
    {
        public enum SpawnMovementDirection
        {
            Forward,
            Upward,
            Outward,
            Random,
        }

        public SpawnMovementDirection spawnMovementDirection;
        public FloatRange spawnSpeed;
        public FloatRange angularSpeed;
        public FloatRange scale;
        public ColorRangeHSV color;
        public bool uniformColor;
    }

    [SerializeField] private SpawnConfiguration spawnConfig;

    public abstract Vector3 SpawnPoint { get; }

    public virtual void ConfigureSpawn(Shape shape)
    {
        Transform t = shape.transform;
        t.localPosition = SpawnPoint;
        t.localRotation = Random.rotation;
        t.localScale = Vector3.one * spawnConfig.scale.RandomValueInRange;
        if (spawnConfig.uniformColor)
        {
            shape.SetColor(spawnConfig.color.RandomInRange);
        }
        else
        {
            for (int i = 0; i < shape.ColorCount; i++)
            {
                shape.SetColor(i,spawnConfig.color.RandomInRange);
            }
        }
        shape.AngularVelocity = Random.onUnitSphere * spawnConfig.angularSpeed.RandomValueInRange;
        Vector3 direction;
        switch (spawnConfig.spawnMovementDirection)
        {
            case SpawnConfiguration.SpawnMovementDirection.Forward:
                direction = transform.forward;
                break;
            case SpawnConfiguration.SpawnMovementDirection.Upward:
                direction = transform.up;
                break;
            case SpawnConfiguration.SpawnMovementDirection.Outward:
                direction = (t.position - transform.position).normalized;
                break;
            case SpawnConfiguration.SpawnMovementDirection.Random:
                direction = Random.onUnitSphere;
                break;
            default:
                direction = Vector3.zero;
                break;
        }

        shape.Velocity = direction * spawnConfig.spawnSpeed.RandomValueInRange;
    }
}