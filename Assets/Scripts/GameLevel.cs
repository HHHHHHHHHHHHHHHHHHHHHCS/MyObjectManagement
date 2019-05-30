using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public partial class GameLevel : PersistableObject
{
    public static GameLevel Current { get; private set; }

    [SerializeField] private SpawnZone spawnZone;

    [SerializeField, FormerlySerializedAs("levelObjects")]
    private GameLevelObject[] levelObjects;

    [field: SerializeField] public int PopulationLimit { get; private set; }



    private void Start()
    {
        if (spawnZone == null)
        {
            spawnZone = GameObject.Find("SpawnZone")?.GetComponent<SpawnZone>();
        }
    }

    private void OnEnable()
    {
        Current = this;
        if (levelObjects == null)
        {
            levelObjects = new GameLevelObject[0];
        }
    }

    public void SpawnShape()
    {
        spawnZone.SpawnShapes();
    }

    public override void Save(GameDataWriter writer)
    {
        writer.Write(levelObjects.Length);
        for (int i = 0; i < levelObjects.Length; i++)
        {
            levelObjects[i].Save(writer);
        }
    }

    public override void Load(GameDataReader reader)
    {
        int saveCount = reader.ReadInt();
        for (int i = 0; i < saveCount; i++)
        {
            levelObjects[i].Load(reader);
        }
    }

}