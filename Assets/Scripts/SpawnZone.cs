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
    }

    [SerializeField]
    private SpawnConfiguration spawnConfig;

    public abstract Vector3 SpawnPoint { get; }

    public virtual void ConfigureSpawn(Shape shape)
    {
        Transform t = shape.transform;
        t.localPosition = SpawnPoint;
        t.localRotation = Random.rotation;
        t.localScale = Vector3.one * Random.Range(0.1f, 1f);
        shape.SetColor(Random.ColorHSV(
            hueMin: 0, hueMax: 1
            , saturationMin: 0.5f, saturationMax: 1
            , valueMin: 0.25f, valueMax: 1
            , alphaMin: 1, alphaMax: 1));
        shape.AngularVelocity = Random.onUnitSphere * Random.Range(0, 90f);
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