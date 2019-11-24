using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Engine;

[CreateAssetMenu(menuName = "Data/Value/EngineValue")]
public class EngineValueData : ScriptableObject
{
    public EngineValueUIType valueUIType;
    [SerializeField] private float value;
    [SerializeField] private float minValue;
    [SerializeField] private float maxValue;
    public float Value
    {
        get
        {
            return value;
        }
        set
        {
            if (value > MaxValue) this.value = MaxValue;
            else if (value < MinValue) this.value = MinValue;
            else this.value = value;
        }
    }
    public  float MinValue { get { return minValue; } }
    public  float MaxValue { get { return maxValue; } }

    [HideInInspector] public int id;
    public int ID { get { return id; } }
    public virtual void SetID(int _id) { id = _id; }

    public EngineValue CreateEngineValue()
    {
        var val = new EngineValue
        {
            MinValue = MinValue,
            MaxValue = MaxValue,
            Value = Value
        };
        return val;
    }
}
