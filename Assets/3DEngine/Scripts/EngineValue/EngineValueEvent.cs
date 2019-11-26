using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct EngineValueEvent
{
    public string eventName;
    public enum TriggerType { Changed, Increased, Decreased, Less, Greater, Equal, Empty, Full }
    public enum EventType { Damaged, Repaired, Die, Respawn, Lose, Win, ValueDelta, Custom }
    public TriggerType triggerType;
    public float compareValue;
    public int eventType;
    public float valueDelta;
    private float lastValue;
    private float minValue;
    private float maxValue;
    private EngineEntity owner;
    private EngineEventManager eventManager;

    public void Initialize(EngineEntity _owner, EngineValueData _data)
    {
        owner = _owner;
        if (owner.Data.engineEventManager)
            eventManager = Object.Instantiate(owner.Data.engineEventManager);

            maxValue = _data.MaxValue;
            minValue = _data.MinValue;
            lastValue = _data.Value;
    }

    public void SyncEvent(EngineValue _engineValue)
    {
        _engineValue.valueChanged += CheckEvent;
    }

    public void CancelEvent(EngineValue _engineValue)
    {
        _engineValue.valueChanged -= CheckEvent;
    }

    void CheckEvent(float _value)
    {
            DoTriggerFilter(_value);
    }

    public void DoTriggerFilter(float _curValue)
    {
        if (triggerType == TriggerType.Changed && _curValue != lastValue ||
            triggerType == TriggerType.Increased && _curValue > lastValue ||
            triggerType == TriggerType.Decreased && _curValue < lastValue ||
            triggerType == TriggerType.Less && _curValue < compareValue ||
            triggerType == TriggerType.Greater && _curValue > compareValue ||
            triggerType == TriggerType.Equal && _curValue == compareValue ||
            triggerType == TriggerType.Empty && _curValue <= minValue ||
            triggerType == TriggerType.Full && _curValue >= maxValue)
            DoEvents();

        lastValue = _curValue;
    }

    void DoEvents()
    {
        if(eventManager)
            eventManager.DoEvents(eventType, owner.gameObject, owner.gameObject);
    }

}


