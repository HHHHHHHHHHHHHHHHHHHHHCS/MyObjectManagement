#if UNITY_EDITOR

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public partial class GameLevel : PersistableObject
{

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

#endif