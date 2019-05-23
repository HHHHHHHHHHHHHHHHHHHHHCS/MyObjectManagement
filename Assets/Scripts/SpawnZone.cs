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

        [System.Serializable]
        public struct SatelliteConfiguration
        {
            public IntRange amount;

            [FloatRangeSlider(0.1f,1f)]
            public FloatRange relativeScale;

            public FloatRange orbitRadius;
            public FloatRange orbitFrequency;
        }

        [System.Serializable]
        public struct LifecycleConfiguration
        {
            [FloatRangeSlider(0f,2f)]
            public FloatRange growingDuration;

            [FloatRangeSlider(0f,100f)]
            public FloatRange adultDuration;

            [FloatRangeSlider(0f, 2f)]
            public FloatRange dyingDuration;

            public Vector3 RandomDurations
            {
                get
                {
                    return new Vector3(
                        growingDuration.RandomValueInRange,
                        adultDuration.RandomValueInRange,
                        dyingDuration.RandomValueInRange
                    );
                }
            }
        }

        public ShapeFactory[] factories;

        public SpawnMovementDirection spawnMovementDirection;
        public FloatRange spawnSpeed;
        public FloatRange speed;

        public FloatRange angularSpeed;
        public FloatRange scale;

        public ColorRangeHSV color;
        public bool uniformColor;

        public SpawnMovementDirection oscillationDirection;
        public FloatRange oscillationAmplitude;
        public FloatRange oscillationFrequency;

        public SatelliteConfiguration satellite;

        public LifecycleConfiguration lifecycle;

    }

    [SerializeField] private SpawnConfiguration spawnConfig;

    public abstract Vector3 SpawnPoint { get; }

    public virtual void SpawnShapes()
    {
        int factoryIndex = Random.Range(0, spawnConfig.factories.Length);
        Shape shape = spawnConfig.factories[factoryIndex].GetRandom();

        Transform t = shape.transform;
        t.localPosition = SpawnPoint;
        t.localRotation = Random.rotation;
        t.localScale = Vector3.one * spawnConfig.scale.RandomValueInRange;

        SetupColor(shape);

        float angularSpeed = spawnConfig.angularSpeed.RandomValueInRange;
        if (angularSpeed != 0f)
        {
            var rotation = shape.AddBehavior<RotationShapeBehavior>();
            rotation.AngularVelocity = Random.onUnitSphere * angularSpeed;
        }

        float speed = spawnConfig.speed.RandomValueInRange;
        if (speed != 0)
        {
            var movement = shape.AddBehavior<MovementShapeBehavior>();
            movement.Velocity = GetDirectionVector(spawnConfig.spawnMovementDirection, t) * speed;
        }


        float growingDuration = spawnConfig.lifecycle.growingDuration.RandomValueInRange;

        SetupOscillation(shape);

        Vector3 lifecycleDurations = spawnConfig.lifecycle.RandomDurations;
        int satelliteCount = spawnConfig.satellite.amount.RandomValueInRange;
        for (int i = 0; i < satelliteCount; i++)
        {
            CreateSatelliteFor(shape,lifecycleDurations);
        }

        SetupLifecycle(shape, lifecycleDurations);
    }

    private void SetupColor(Shape shape)
    {
        if (spawnConfig.uniformColor)
        {
            shape.SetColor(spawnConfig.color.RandomInRange);
        }
        else
        {
            for (int i = 0; i < shape.ColorCount; i++)
            {
                shape.SetColor(i, spawnConfig.color.RandomInRange);
            }
        }
    }

    private Vector3 GetDirectionVector(
        SpawnConfiguration.SpawnMovementDirection direction, Transform t)
    {
        switch (direction)
        {
            case SpawnConfiguration.SpawnMovementDirection.Forward:
                return transform.forward;
            case SpawnConfiguration.SpawnMovementDirection.Upward:
                return transform.up;
            case SpawnConfiguration.SpawnMovementDirection.Outward:
                return (t.position - transform.position).normalized;
            case SpawnConfiguration.SpawnMovementDirection.Random:
                return Random.onUnitSphere;
            default:
                return Vector3.zero;
        }
    }

    private void SetupOscillation(Shape shape)
    {
        float amplitude = spawnConfig.oscillationAmplitude.RandomValueInRange;
        float frequency = spawnConfig.oscillationFrequency.RandomValueInRange;
        if (amplitude == 0 || frequency == 0)
        {
            return;
        }

        var oscillation = shape.AddBehavior<OscillationShapeBehavior>();
        oscillation.Offset = GetDirectionVector(spawnConfig.oscillationDirection, shape.transform) * amplitude;
        oscillation.Frequency = frequency;
    }


    private void CreateSatelliteFor(Shape focalShape,Vector3 lifecycleDurations)
    {
        int factoryIndex = Random.Range(0, spawnConfig.factories.Length);
        Shape shape = spawnConfig.factories[factoryIndex].GetRandom();
        Transform t = shape.transform;
        t.localRotation = Random.rotation;
        t.localScale = focalShape.transform.localScale 
                       * spawnConfig.satellite.relativeScale.RandomValueInRange;
        SetupColor(shape);
        shape.AddBehavior<SatelliteShapeBehavior>().Initialize(shape,focalShape
            ,spawnConfig.satellite.orbitRadius.RandomValueInRange
            ,spawnConfig.satellite.orbitFrequency.RandomValueInRange);
        SetupLifecycle(shape, lifecycleDurations);
    }

    private void SetupLifecycle(Shape shape, Vector3 durations)
    {
        if (durations.x > 0)
        {
            if (durations.y > 0f || durations.z > 0f)
            {
                shape.AddBehavior<LifecycleShapeBehavior>()
                    .Initialize(shape,durations.x,durations.y,durations.z);
            }
            else
            {
                shape.AddBehavior<GrowingShapeBehavior>().Initialize(shape, durations.x);

            }
        }
        else if (durations.y > 0f)
        {
            shape.AddBehavior<LifecycleShapeBehavior>()
                .Initialize(shape,durations.x,durations.y,durations.z);
        }
        else if (durations.z > 0f)
        {
            shape.AddBehavior<DyingShapeBehavior>().Initialize(shape, durations.y);
        }
    }
}