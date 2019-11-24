using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Engine;
using MEC;

[System.Serializable]
public class EngineValue
{
    protected EngineValueData data;
    public virtual EngineValueData Data { get { return data; } }
    private float value;
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
    public virtual float MinValue { get; set; }
    public virtual float MaxValue { get; set; }
    protected float prevValue;

    protected int id;
    public int ID { get { return id; } }

    public bool IsReady { get { return !overheated && !IsEmpty; } }
    public bool IsEmpty { get { return Value <= MinValue; } }
    public bool IsFull { get { return Value >= MaxValue; } }

    protected bool overheated;
    public bool IsOverheated { get { return overheated; } }
    protected CoroutineHandle overheatedCoroutine;
    protected float overheatTimer;
    public float OverheatTimer { get { return overheatTimer; } }

    protected bool recharging;
    public bool IsRecharging { get { return recharging; } }
    protected CoroutineHandle rechargeCoroutine;

    //delegates
    //changed
    public delegate void OnValueChangeDelegate(float _value);
    public event OnValueChangeDelegate valueChanged;
    void OnValueChanged() { valueChanged?.Invoke(Value); }
    //minmax changed
    public delegate void OnMinMaxChangeDelegate(float _min, float _max);
    public event OnMinMaxChangeDelegate minMaxChanged;
    void OnMinMaxChanged() { minMaxChanged?.Invoke(MinValue, MaxValue); }
    //increased
    public delegate void OnValueIncreaseDelegate(float _value);
    public event OnValueIncreaseDelegate valueIncreased;
    void OnValueIncreased() { valueIncreased?.Invoke(Value); }
    //decrease
    public delegate void OnValueDecreaseDelegate(float _value);
    public event OnValueDecreaseDelegate valueDecreased;
    void OnValueDecreased() { valueDecreased?.Invoke(Value); }
    //empty
    public delegate void OnValueEmptyDelegate();
    public event OnValueEmptyDelegate valueEmpty;
    void OnValueEmpty() { valueEmpty?.Invoke(); }
    //full
    public delegate void OnValueFullDelegate();
    public event OnValueFullDelegate valueFull;
    void OnValueFull() { valueFull?.Invoke(); }
    //overheatFinished
    public delegate void OnOverHeatFinishedDelegate();
    public event OnOverHeatFinishedDelegate overheatFinished;
    void OnOverheatFinished() { overheatFinished?.Invoke(); }

    public virtual void InitializeValue(EngineValueData _data)
    {
        id = _data.ID;
        data = _data;
        Value = _data.Value;
        prevValue = Value;
        OnValueChanged();
    }

    public virtual void ValueDelta(float _amount)
    {
        Value += _amount;
        CheckEvents();
    }

    public virtual void ValueMaxDelta(float _amount)
    {
        MaxValue += _amount;

        OnMinMaxChanged();
    }

    public virtual void ValueMinDelta(float _amount)
    {
        MinValue += _amount;

        OnMinMaxChanged();
    }

    public virtual void ResetDefaultValues()
    {
        Value = Data.Value;
        MinValue = Data.MinValue;
        MaxValue = Data.MaxValue;
        CheckEvents();
    }

    public virtual void Recharge(float _speed)
    {
        if (rechargeCoroutine != null)
            Timing.KillCoroutines(rechargeCoroutine);
        rechargeCoroutine = Timing.RunCoroutine(StartRecharge(_speed));
    }

    IEnumerator<float> StartRecharge(float _speed)
    {
        recharging = true;
        while (Value < MaxValue)
        {
            yield return Timing.WaitForOneFrame;
            if (!overheated)
                ValueDelta(_speed * Time.deltaTime);
        }
        recharging = false;
    }

    public void OverHeat(float _overheatTime)
    {
        if (overheatedCoroutine != null)
            Timing.KillCoroutines(overheatedCoroutine);
        overheatedCoroutine = Timing.RunCoroutine(StartOverheat(_overheatTime));
    }

    IEnumerator<float> StartOverheat(float _overheatTime)
    {
        overheated = true;
        float perc = 0;
        while (perc < 1)
        {
            overheatTimer += Time.deltaTime;
            if (overheatTimer > _overheatTime)
                overheatTimer = _overheatTime;
            perc = overheatTimer / _overheatTime;

            yield return Timing.WaitForOneFrame;
        }
        overheated = false;
        OnOverheatFinished();
    }

    void CheckEvents()
    {
        if (Value != prevValue)
        {
            prevValue = Value;
            OnValueChanged();
        }
        if (Value > prevValue)
            OnValueIncreased();
        else if (Value < prevValue)
            OnValueDecreased();
        if (IsEmpty)
            OnValueEmpty();
        if (IsFull)
            OnValueFull();
    }
}
