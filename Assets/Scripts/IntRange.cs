﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct IntRange
{
    public int min, max;

    public int RandomValueInRange => Random.Range(min, max + 1);
}
