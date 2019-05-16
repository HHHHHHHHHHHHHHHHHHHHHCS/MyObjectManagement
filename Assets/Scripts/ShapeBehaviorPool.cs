using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShapeBehaviorPool<T> where T : ShapeBehavior, new()
{
    private static readonly Stack<T> stack = new Stack<T>();

    public static T Get()
    {
        if (stack.Count > 0)
        {
            var temp = stack.Pop();
#if UNITY_EDITOR
            temp.IsReclaimed = false;
#endif
            return temp;
        }

#if UNITY_EDITOR
        return ScriptableObject.CreateInstance<T>();
#else
        return new T();
#endif
    }

    public static void Reclaim(T behavior)
    {
#if UNITY_EDITOR
        behavior.IsReclaimed = true;
#endif
        stack.Push(behavior);
    }
}