using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ShapeBehavior :
#if UNITY_EDITOR
    ScriptableObject
#endif
{

    public abstract ShapeBehaviorType BehaviorType { get; }

#if UNITY_EDITOR
    public bool IsReclaimed { get; set; }
#endif

    private void OnEnable()
    {
        if (IsReclaimed)
        {
            Recycle();
        }
    }

    public void GameUpdate()
    {
    }

    public abstract void GameUpdate(Shape shape);

    public abstract void Save(GameDataWriter writer);
    public abstract void Load(GameDataReader reader);

    public abstract void Recycle();
}