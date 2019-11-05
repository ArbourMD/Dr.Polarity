using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EngineEventTrigger
{
    public enum TriggerType { OnEnter, OnStay, OnExit, External }
    public enum OnStayOptionType { Constant, Repeat, OnInput, OnInputDown, OnInputUp }
    public enum ValidationType { Layer, Tag, UnitValueAmount, Angle, Obstacle }
    public enum ActivationAmountType { Unlimited, Finite }
    public enum ValueOptionsType { Greater, Less, Equal }
    public enum ActivationType { Solo, Broadcast, Both }
    [SerializeField] protected string triggerName;
    public string TriggerName { get { return triggerName; } }
    [SerializeField] protected TriggerType triggerType;
    [SerializeField] protected int detectZoneInd;
    [SerializeField] protected OnStayOptionType onStayOption;
    [SerializeField] protected float repeatDelay;
    private float repeatTimer;
    [SerializeField] protected InputProperty input;
    [SerializeField] protected int validationMask;
    [SerializeField] protected LayerMask validationLayer;
    [SerializeField] protected TagProperty validationTag;
    [SerializeField] protected EngineValueDataManager engineValueDataManager;
    [SerializeField] protected EngineValueSelection selection;
    [SerializeField] protected ValueOptionsType valueOption;
    [SerializeField] protected float comparedValue;
    [SerializeField] protected float angle;
    [SerializeField] protected LayerMask obstacleLayer;
    [SerializeField] protected Vector3 eyesOffset;
    [SerializeField] protected Vector3 lookLocationOffset;

    [SerializeField] protected ActivationAmountType activationAmount;
    [SerializeField] protected int maxActivations;
    protected int curActivations;
    [SerializeField] protected ActivationType activationType;
    [SerializeField] protected EngineEvent[] engineEvents;
    public EngineEvent[] EngineEvents { get { return engineEvents; } }
    [SerializeField] protected EngineEventReceiver[] receivers;
    public EngineEventReceiver[] Receivers { get { return receivers; } }
    [SerializeField] protected int receiverIndMask;

    protected Coroutine valueRoutine;
    protected EngineEventTriggerManager manager;
    protected int triggerInd;

    public void Activate(EngineEventTriggerManager _manager, int _triggerIndex)
    {
        manager = _manager;
        triggerInd = _triggerIndex;

        if (triggerType == TriggerType.External)
            return;
        else if (triggerType == TriggerType.OnEnter)
            _manager.DetectZones[detectZoneInd].AddEnterTrigger(_manager.gameObject, OnEnter);
        else if (triggerType == TriggerType.OnStay)
            _manager.DetectZones[detectZoneInd].AddStayTrigger(_manager.gameObject, OnStay);
        else if (triggerType == TriggerType.OnExit)
            _manager.DetectZones[detectZoneInd].AddExitTrigger(_manager.gameObject, OnExit);
    }

    public void Deactivate(EngineEventTriggerManager _manager)
    {
        if (triggerType == TriggerType.External)
            return;
        else if (triggerType == TriggerType.OnEnter)
            _manager.DetectZones[detectZoneInd].RemoveEnterTrigger(_manager.gameObject, OnEnter);
        else if (triggerType == TriggerType.OnStay)
            _manager.DetectZones[detectZoneInd].RemoveStayTrigger(_manager.gameObject, OnStay);
        else if (triggerType == TriggerType.OnExit)
            _manager.DetectZones[detectZoneInd].RemoveStayTrigger(_manager.gameObject, OnExit);
    }

    protected void OnEnter(Collider _col)
    {
        if (!PassesValidation(_col))
            return;
        ActivateEvents(_col);
    }

    protected void OnStay(Collider _col)
    {
        if (!PassesValidation(_col))
            return;

        if (onStayOption == OnStayOptionType.Constant)
            ActivateEvents(_col);
        else if (onStayOption == OnStayOptionType.Repeat)
        {
            repeatTimer += Time.deltaTime;
            if (repeatTimer > repeatDelay)
            {
                ActivateEvents(_col);
                repeatTimer = 0;
            }
        }
        else if (onStayOption == OnStayOptionType.OnInput)
        {
            if (input.GetInput())
                ActivateEvents(_col);
        }
        else if (onStayOption == OnStayOptionType.OnInputDown)
        {
            if (input.GetInputDown())
                ActivateEvents(_col);
        }
        else if (onStayOption == OnStayOptionType.OnInputUp)
        {
            if (input.GetInputUp())
                ActivateEvents(_col);
        }
    }

    protected void OnExit(Collider _col)
    {
        if (!PassesValidation(_col))
            return;

        ActivateEvents(_col);
    }

    bool PassesValidation(Collider _col)
    {
        bool layerFilter = true;
        bool tagFilter = true;
        bool valueFilter = true;
        bool angleFilter = true;
        bool obstacleFilter = true;
        if (validationMask == (validationMask | (1 << (int)ValidationType.Layer)))
            layerFilter = _col.gameObject.IsInLayerMask(validationLayer);
        if (validationMask == (validationMask | (1 << (int)ValidationType.Tag)))
            tagFilter = _col.gameObject.tag == validationTag.stringValue;
        if (validationMask == (validationMask | (1 << (int)ValidationType.UnitValueAmount)))
        {
            var unit = _col.GetComponent<Unit>();
            if (unit)
            {
                var local = unit.GetLocalEngineValue(selection.valueData.ID);
                if (local != null)
                {
                    if (valueOption == ValueOptionsType.Equal)
                        valueFilter = (float)local.Value == comparedValue;
                    else if (valueOption == ValueOptionsType.Greater)
                        valueFilter = (float)local.Value > comparedValue;
                    else if (valueOption == ValueOptionsType.Less)
                        valueFilter = (float)local.Value < comparedValue;
                }
            }
        }
        if (validationMask == (validationMask | (1 << (int)ValidationType.Angle)))
        {
            var pos = _col.transform.position;
            var diff = pos - manager.transform.position;
            var dir = diff.normalized;

            var dirFlat = dir;
            dirFlat.y = 0;
            angleFilter = Vector3.Dot(dirFlat, manager.transform.forward) > Mathf.Cos(angle * 0.5f * Mathf.Deg2Rad);

        }
        if (validationMask == (validationMask | (1 << (int)ValidationType.Obstacle)))
        {
            var pos = _col.transform.position + lookLocationOffset;
            var origin = manager.transform.position + eyesOffset;
            obstacleFilter = !Physics.Linecast(origin, pos, obstacleLayer);
            Debug.DrawLine(origin, pos, Color.red);
        }   
        return layerFilter && tagFilter && valueFilter && angleFilter && obstacleFilter;
    }

    public void ActivateEvents(Collider _col)
    {
        //Only continue if we haven't surpassed max activations
        if (activationAmount == ActivationAmountType.Finite)
        {
            if (curActivations < maxActivations)
                curActivations++;
            else
                return;
        }

        //Activate Events
        if (activationType == ActivationType.Solo || activationType == ActivationType.Both)
        {
            for (int i = 0; i < engineEvents.Length; i++)
            {
                engineEvents[i].DoEvent(manager.gameObject, engineEvents, i, _col.gameObject);
            }
        }

        //Activate Receivers
        if (activationType == ActivationType.Broadcast || activationType == ActivationType.Both)
        {
            ActivateReceivers();
        }

    }

    public void ActivateReceivers()
    {
        for (int i = 0; i < receivers.Length; i++)
        {
            receivers[i].Activate();
        }
    }

    public string[] GetEventNames()
    {
        var names = new string[engineEvents.Length];
        for (int i = 0; i < engineEvents.Length; i++)
            names[i] = engineEvents[i].eventName;
        return names;
    }


}
