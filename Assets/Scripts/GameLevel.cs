using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameLevel : PersistableObject
{
    public static GameLevel Current { get; private set; }

    public Vector3 SpawnPoint => spawanZone.SpawnPoint;

    private SpawnZone spawanZone;

    private void Start()
    {
        spawanZone = GameObject.Find("SpawnZone").GetComponent<SpawnZone>();
    }

    private void OnEnable()
    {
        Current = this;
    }

    public override void Save(GameDataWriter writer)
    {
        base.Save(writer);
    }

    public override void Load(GameDataReader reader)
    {
        base.Load(reader);
    }
}