using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameLevel : PersistableObject
{
    public static GameLevel Current { get; private set; }

    public Vector3 SpawnPoint => spawanZone.SpawnPoint;

    [SerializeField]
    private SpawnZone spawanZone;

    [SerializeField]
    private PersistableObject[] persistableObjects;


    private void Start()
    {
        if (spawanZone == null)
        {
            spawanZone = GameObject.Find("SpawnZone").GetComponent<SpawnZone>();
        }
    }

    private void OnEnable()
    {
        Current = this;
        if (persistableObjects == null)
        {
            persistableObjects = new PersistableObject[0];
        }
    }

    public override void Save(GameDataWriter writer)
    {
        writer.Write(persistableObjects.Length);
        for (int i = 0; i < persistableObjects.Length; i++)
        {
            persistableObjects[i].Save(writer);
        }
    }

    public override void Load(GameDataReader reader)
    {
        int saveCount = reader.ReadInt();
        for (int i = 0; i < saveCount; i++)
        {
            persistableObjects[i].Load(reader);
        }
    }
}