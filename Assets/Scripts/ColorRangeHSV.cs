using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct ColorRangeHSV
{
    [FloatRangeSlider(0f, 1f)]
    public FloatRange hue, saturation, value;

    public Color RandomInRange => Random.ColorHSV(
        hue.min, hue.max,
        saturation.min, saturation.max,
        value.min, value.min,
        1f, 1f);
}