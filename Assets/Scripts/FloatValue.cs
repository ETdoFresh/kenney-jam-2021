using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Create FloatValue", fileName = "FloatValue", order = 0)]
public class FloatValue : ScriptableObject
{
    public float value;
    
    public static implicit operator float(FloatValue floatValue)
    {
        return floatValue.value;
    }
}
