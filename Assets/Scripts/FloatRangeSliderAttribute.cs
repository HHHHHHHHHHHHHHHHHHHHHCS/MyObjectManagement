using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatRangeSliderAttribute : PropertyAttribute
{
    public float Min { get; set; }
    public float Max { get; set; }

    public FloatRangeSliderAttribute(float min, float max)
    {
        if (max < min)
        {
            Max = min;
            Min = max;
        }
        else
        {
            Min = min;
            Max = max;
        }
    }
}
