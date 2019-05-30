using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public static class RegisterLevelObjectMenuItem
{
    private const string menuItem = "GameObject/Register Level Object";

    [MenuItem(menuItem,true)]
    private static bool ValidateRegisterLevelObject()
    {
        if (Selection.objects.Length == 0)
        {
            return false;
        }

        foreach (var o in Selection.objects)
        {
            if (!(o is GameObject))
            {
                return false;
            }
        }
        return true;
    }

    [MenuItem(menuItem)]
    private static void RegisterLevelObject()
    {
        foreach (var o in Selection.objects)
        {
            Register(o as GameObject);
        }
    }

    private static void Register(GameObject o)
    {

        if (PrefabUtility.GetPrefabAssetType(o) != PrefabAssetType.NotAPrefab)
        {
            Debug.LogError(o.name + " is a prefab asset.", o);
            return;
        }

        var levelObject = o.GetComponent<GameLevelObject>();

        if (levelObject == null)
        {
            Debug.LogError(o.name + " isn't a game level object", o);
            return;
        }

        foreach (var rootObject in o.scene.GetRootGameObjects())
        {
            var gameLevel = rootObject.GetComponent<GameLevel>();
            if (gameLevel != null)
            {
                if (gameLevel.HasLevelObject(levelObject))
                {
                    Debug.LogError(o.name + " is already registered.", o);
                    return;
                }

                Undo.RecordObject(gameLevel, "Register Level Object.");
                gameLevel.RegisterLevelObject(levelObject);
                Debug.Log(
                    o.name + " registered to game level " +
                    gameLevel.name + " in scene " + o.scene.name + ".", o
                );
                return;
            }
        }
        Debug.LogError(o.name + " isn't part of a game level", o);
    }
}
