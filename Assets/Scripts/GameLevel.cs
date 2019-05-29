using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class GameLevel : PersistableObject
{
    public static GameLevel Current { get; private set; }

    [SerializeField] private SpawnZone spawnZone;

    [SerializeField, FormerlySerializedAs("levelObjects")]
    private GameLevelObject[] levelObjects;

    [field: SerializeField] public int PopulationLimit { get; private set; }


    public bool HasMissingLevelObjects
    {
        get
        {
            if (levelObjects != null)
            {
                for (int i = 0; i < levelObjects.Length; i++)
                {
                    if (levelObjects[i] == null)
                    {
                        return true;
                    }
                }
            }

            return false;
        }
    }

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


    public void RemoveMissingLevelObjects()
    {
        if (Application.isPlaying)
        {
            Debug.LogError("Do not invoke in play mode!");
            return;
        }

        int holes = 0;
        for (int i = 0; i < levelObjects.Length - holes; i++)
        {
            if (levelObjects[i] == null)
            {
                holes += 1;
                Array.Copy(levelObjects, i + 1, levelObjects, i, levelObjects.Length - i - holes);
            }

            i -= 1;
        }

        Array.Resize(ref levelObjects, levelObjects.Length - holes);
    }

    public bool HasLevelObject(GameLevelObject o)
    {
        if (levelObjects != null)
        {
            foreach (var item in levelObjects)
            {
                if (item == o)
                {
                    return true;
                }
            }
        }

        return false;
    }

    public void RegisterLevelObject(GameLevelObject o)
    {
        if (Application.isPlaying)
        {
            Debug.LogError("Do not invoke in play mode!");
        }

        if (HasLevelObject(o))
        {
            return;
        }

        if (levelObjects == null)
        {
            levelObjects = new[] {o};
        }
        else
        {
            Array.Resize(ref levelObjects, levelObjects.Length + 1);
            levelObjects[levelObjects.Length - 1] = o;
        }
    }
}